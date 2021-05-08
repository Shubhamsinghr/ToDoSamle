using AutoMapper;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TODOSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        public AuthController(IMapper mapper, UserManager<User> userManager, IOptionsSnapshot<JwtSettings> jwtSettings,
            RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _roleManager = roleManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserModel model)
        {
            var user = _mapper.Map<UserModel, User>(model);

            var userCreateResult = await _userManager.CreateAsync(user, model.Password);

            if (userCreateResult.Succeeded)
            {
                var createdUser = _userManager.Users.SingleOrDefault(u => u.UserName == model.Email);
                return Ok(new Response(true, "User created successfully", createdUser));
            }
            return Problem(userCreateResult.Errors.First().Description, null, 500);
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(UserModel model)
        {
            try
            {
                LoginResponse response = new LoginResponse();
                var user = _userManager.Users.SingleOrDefault(u => u.UserName == model.Email);
                if (user is null)
                {
                    return Ok(new Response(false, "User not found", response));
                }
                var userSigninResult = await _userManager.CheckPasswordAsync(user, model.Password);
                if (userSigninResult)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    response.Token = GenerateJwt(user, roles);
                    response.UserId = user.Id.ToString();
                    response.IsSuperAdmin = (roles.Count > 0) ? await _userManager.IsInRoleAsync(user, Helper.EnumRoles.SA.ToString()) : false;
                    return Ok(new Response(true, "Logged in successfully", response));
                }
                return BadRequest("Email or password incorrect.");
            }
            catch (Exception ex)
            {
                return Ok(new Response(false, ex.Message, null));
            }
        }

        [Authorize("SA")]
        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var user = _userManager.Users;
            if (user != null)
            {
                return Ok(new Response(true, "", user));
            }
            return BadRequest();
        }

        [Authorize("SA")]
        [HttpGet("getRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = _roleManager.Roles;
            if (roles != null)
            {
                return Ok(new Response(true, "", roles));
            }
            return BadRequest();
        }

        [Authorize("SA")]
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.RoleName))
                {
                    return Ok(new Response(true, "Role name should be provided", null));
                }

                var newRole = new Role
                {
                    Name = model.RoleName
                };

                var roleResult = await _roleManager.CreateAsync(newRole);

                if (roleResult.Succeeded)
                {
                    return Ok(new Response(true, "Role added successfully", null));
                }

                return Problem(roleResult.Errors.First().Description, null, 500);
            }
            catch (Exception ex)
            {
                return Ok(new Response(false, ex.Message, null));
            }
           
        }

        [Authorize("SA")]
        [HttpPost("assignRole")]
        public async Task<IActionResult> AddUserToRole([FromBody] UserRole model)
        {
            try
            {
                var user = _userManager.Users.SingleOrDefault(u => u.Id.ToString() == model.UserId);

                var result = await _userManager.AddToRoleAsync(user, model.RoleName);

                if (result.Succeeded)
                {
                    return Ok(new Response(true, "Role assigned successfully", null));
                }
                return Ok(new Response(false, result.Errors.First().Description, null));
            }
            catch (Exception ex)
            {
                return Ok(new Response(false, ex.Message, null));
            }
        }

        private string GenerateJwt(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtSettings.ExpirationInDays));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
