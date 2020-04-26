using Serilog;
using Serilog.Events;
using System;
using System.Data.SqlClient;

namespace SwagOverFlow.Logger
{
    /// <summary>
    /// based on https://app.pluralsight.com/library/courses/dotnet-logging-using-serilog-opinionated-approach/table-of-contents
    /// </summary>
    public static class Flogger
    {
        private static readonly ILogger _perfLogger;
        private static readonly ILogger _usageLogger;
        private static readonly ILogger _errorLogger;
        private static readonly ILogger _diagnosticLogger;

        static Flogger()
        {
            //https://github.com/serilog/serilog/wiki/Formatting-Output
            _perfLogger = new LoggerConfiguration()
                //.WriteTo.File(path: "perf.txt")
                .WriteTo.Debug(outputTemplate : "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                //.WriteTo.File(path: "usage.txt")
                .WriteTo.Debug()
                .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                //.WriteTo.File(path: "error.txt")
                .WriteTo.Debug()
                .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                //.WriteTo.File(path: "diagnostic.txt")
                .WriteTo.Debug()
                .CreateLogger();
        }

        public static void WritePerf(FlogDetail infoToLog)
        {
            _perfLogger.Write(LogEventLevel.Information, "{@FlogDetail}", infoToLog);
        }
        public static void WriteUsage(FlogDetail infoToLog)
        {
            _usageLogger.Write(LogEventLevel.Information, "{@FlogDetail}", infoToLog);
        }
        public static void WriteError(FlogDetail infoToLog)
        {
            _errorLogger.Write(LogEventLevel.Information, "{@FlogDetail}", infoToLog);
        }
        public static void WriteDiagnostic(FlogDetail infoToLog)
        {
            //var writeDiagnostics = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableDiagnostics"]);
            //if (!writeDiagnostics)
            //    return;

            _diagnosticLogger.Write(LogEventLevel.Information, "{@FlogDetail}", infoToLog);
        }

        private static string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetMessageFromException(ex.InnerException);

            return ex.Message;
        }
    }
}
