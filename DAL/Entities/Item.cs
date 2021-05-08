using System;
using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifedOn { get; set; }
        public string ModifedBy { get; set; }
        public string CreatedBy { get; set; }

    }
}
