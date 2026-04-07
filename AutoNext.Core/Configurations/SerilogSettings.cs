namespace AutoNext.Core.Configurations
{
    public class SerilogSettings
    {
        public MinimumLevelConfig MinimumLevel { get; set; } = new();
        public List<WriteToConfig> WriteTo { get; set; } = new();
        public Dictionary<string, string> Enrich { get; set; } = new();
        public Dictionary<string, object> Properties { get; set; } = new();
    }

}
