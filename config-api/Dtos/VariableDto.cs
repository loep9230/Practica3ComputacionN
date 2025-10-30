namespace config_api.Dtos
{
    public class VariableDto
    {
        public required string name { get; set; }
        public required string value { get; set; }
        public string description { get; set; }
        public bool is_sensitive { get; set; } = false;
    }
}
