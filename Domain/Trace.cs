namespace KsxEventTracker.Domain 
{
    using Microsoft.Extensions.Logging;

    public static class Trace {

        public static ILogger Logger { get; set; }

        public static void TraceInformation(string format, params object[] args){
            if (Logger == null) {
                return;
            }

            var msg = string.Format(format, args);
            Logger.LogInformation(msg);
        }
        
        public static void TraceWarning(string format, params object[] args){
            if (Logger == null) {
                return;
            }

            var msg = string.Format(format, args);
            Logger.LogWarning(msg);
        }

        public static void TraceError(string format, params object[] args){
            if (Logger == null) {
                return;
            }

            var msg = string.Format(format, args);
            Logger.LogError(msg);
        }   
    }
}