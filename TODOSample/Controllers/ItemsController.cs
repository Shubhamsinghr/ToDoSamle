using AutoMapper;
using DAL;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TODOSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("User")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IAuditLogRepository _auditLogRepository;
        public ItemsController(IItemService itemService, IAuditLogRepository auditLogRepository)
        {
            this._itemService = itemService;
            _auditLogRepository = auditLogRepository;
        }
        [HttpGet("getItems")]
        public async Task<ActionResult<IEnumerable<ItemModel>>> GetItems()
        {
            return Ok(await _itemService.GetItems());
        }

        [HttpPost("upsertItem")]
        public async Task<ActionResult> UpsertItem([FromBody] ItemModel model)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ItemResponse res = await _itemService.UpsertItem(model, userId);
            var jobId = BackgroundJob.Enqueue(() => _auditLogRepository.UpdateLog(res.AuditLog));
            return Ok(new Response(true, "", res.Id));
        }
    }
}
