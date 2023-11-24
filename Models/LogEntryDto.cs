namespace ErrorLogReader.Models
{
    public class LogEntryDto
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public string ModuleName { get; set; }
    }

}
