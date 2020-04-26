using SwagOverflowWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SwagOverFlow.Logger
{
    public class FlogDetail
    {
        private readonly Stopwatch _sw = new Stopwatch();
        public FlogDetail()
        {
            Timestamp = DateTime.Now;
            AdditionalInfo = new Dictionary<string, object>();
        }
        public string Message { get; set; }
        public DateTime Timestamp { get; private set; }

        // WHERE
        public string Product { get; set; }
        public string Layer { get; set; }
        public string Location { get; set; }
        public string Hostname { get; set; }

        // WHO
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        // EVERYTHING ELSE
        public string CorrelationId { get; set; } // exception shielding from server to client
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long? ElapsedMilliseconds { get; set; }  // only for performance entries
        public Dictionary<string, object> AdditionalInfo { get; set; }  // catch-all for anything else
        public Exception Exception { get; set; }  // the exception for error logging

        public void StartTimer()
        {
            _sw.Start();
            StartTime = DateTime.Now;

            FlogDetail fdStart = JsonHelper.Clone<FlogDetail>(this);
            fdStart.Message = $"[Start] {this.Message}";
            Flogger.WritePerf(fdStart);
        }

        public void EndTimer()
        {
            _sw.Stop();
            EndTime = DateTime.Now;
            this.ElapsedMilliseconds = _sw.ElapsedMilliseconds;

            FlogDetail fdEnd = JsonHelper.Clone<FlogDetail>(this);
            fdEnd.Message = $"[End {_sw.Elapsed.ToString()}] {this.Message}";
            Flogger.WritePerf(fdEnd);
        }

        public void WriteDiagnostic()
        {
            Flogger.WriteDiagnostic(this);
        }
    }
}
