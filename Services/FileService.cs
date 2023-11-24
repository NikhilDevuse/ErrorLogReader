using ErrorLogReader.Models;

namespace ErrorLogReader.Services
{
    public class FileService
    {
        public List<LogEntry> ReadLogFiles(string directory, List<string> fileNames)
        {
            List<LogEntry> logEntries = new List<LogEntry>();

            foreach (var fileName in fileNames)
            {
                var filePath = Path.Combine(directory, fileName);
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        if (IsErrorEntry(line))
                        {
                            logEntries.Add(new LogEntry { Text = line, Timestamp = DateTime.Now });
                        }
                    }
                }
            }

            return logEntries;
        }

        private bool IsErrorEntry(string line)
        {
            return line.Contains("ERROR", StringComparison.OrdinalIgnoreCase);
        }
    }
}
