using System;

namespace SwagOverFlow.Utils
{
    public static class ExceptionExtensions
    {
        public static String DeepMessage(this Exception ex)
        {
            String message = "";
            while (ex != null)
            {
                message += ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + Environment.NewLine;
                ex = ex.InnerException;
            }

            return message;
        }
    }
}
