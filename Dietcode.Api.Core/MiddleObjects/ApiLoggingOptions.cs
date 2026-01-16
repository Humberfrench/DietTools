namespace Dietcode.Api.Core.MiddleObjects
{
    public sealed class ApiLoggingOptions
    {
        public string Directory { get; set; } = "logs";
        public bool Enabled { get; set; } = true;
    }
}
