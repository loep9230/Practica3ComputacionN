using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace config_domain
{
    public class Variables
    {
        [Key]
        public required string name { get; set; }
        public required string value { get; set; }
        public string description { get; set; }
        public bool is_sensitive { get; set; } = false;
        [Key]
        public required string name_entorno { get; set; }
        public  DateTime created_at { get; set; } = DateTime.UtcNow;
        public  DateTime updated_at { get; set; } = DateTime.UtcNow;
    }
}
