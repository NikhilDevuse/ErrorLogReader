namespace ErrorLogReader.Models
{
    public class LogEntry
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public bool ContainsError()
        {
            return Text.Contains("ERROR", StringComparison.OrdinalIgnoreCase);
        }
    }
}
