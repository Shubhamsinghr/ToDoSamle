using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public class Response
    {
    }
    public class ItemResponse
    {
        public AuditLog AuditLog { get; set; }
        public int Id { get; set; }
    }
}
