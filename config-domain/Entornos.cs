using System.ComponentModel.DataAnnotations;

namespace config_domain
{
    public class Entornos
    {
        [Key]
        public required string name { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime updated_at { get; set; } = DateTime.UtcNow;

    }
}
