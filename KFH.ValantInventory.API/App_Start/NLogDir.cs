using NLog.LayoutRenderers;
using System;
using System.Text;

namespace KFH.ValantInventory.API
{
    /// <summary>
    /// Class used to determine the NLog directory
    /// </summary>
    [LayoutRenderer("NLogDir")]
    public class NLogDir : LayoutRenderer
    {

        // Default to Current Directory
        private static string _logDir = Environment.CurrentDirectory;

        public static string LogDir
        {
            get { return _logDir; }
            set { _logDir = value; }
        }

        protected override void Append(StringBuilder builder, NLog.LogEventInfo logEvent)
        {
            builder.Append(_logDir);
        }

    }
}