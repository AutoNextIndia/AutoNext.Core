namespace AutoNext.Core.Configurations
{
    public class MinimumLevelConfig
    {
        public string Default { get; set; } = "Information";
        public Dictionary<string, string> Override { get; set; } = new();
    }
}
