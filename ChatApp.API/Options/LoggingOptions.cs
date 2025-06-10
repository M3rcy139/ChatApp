using Serilog.Events;

namespace ChatApp.API.Options;

public class LoggingOptions
{
    public string ApplicationName { get; set; }

    public LogEventLevel DefaultLogLevel { get; set; }
    public LogEventLevel AspNetCoreLogLevel { get; set; }
    public LogEventLevel ConsoleLogLevel { get; set; }
    public LogEventLevel FileLogLevel { get; set; }

    public string FilePath { get; set; } 
    public int RetainedFileCountLimit { get; set; }
}