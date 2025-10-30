namespace config_api.Dtos
{
    public class ActualizarVariableDto
    {
        public required string value { get; set; }
        public string description { get; set; }
        public bool is_sensitive { get; set; } = false;
    }
}
