using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace DAL
{
    public class ToDoDbContext: IdentityDbContext<User, Role, Guid>
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
            : base(options)
        { }
        public DbSet<Item> Item { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .ApplyConfiguration(new ItemConfiguration());
            builder
                .ApplyConfiguration(new AuditLogConfiguration());
        }
    }
}
