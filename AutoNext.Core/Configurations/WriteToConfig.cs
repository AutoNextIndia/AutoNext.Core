namespace AutoNext.Core.Configurations
{
    public class WriteToConfig
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, object> Args { get; set; } = new();
    }
}
