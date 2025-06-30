using System.Text;
using TinyGroxMods.HarmonyFramework;
using Verse;

namespace RWQoLPatch
{
    public class RWLogger: IModLogger
    {
        private readonly StringBuilder infoBuilder = new StringBuilder();
        private readonly StringBuilder warningBuilder = new StringBuilder();
        private readonly StringBuilder errorBuilder = new StringBuilder();
        public void LogWarning(string message)
        {
            warningBuilder.AppendLine(message);
        }

        public void LogMessage(string message)
        {
            infoBuilder.AppendLine(message);
        }

        public void LogError(string message)
        {
            errorBuilder.AppendLine(message);
        }

        public void Flush()
        {
            if (infoBuilder.Length > 0)
            {
                Log.Message("==== Info ====\n" + infoBuilder);
            }

            if (warningBuilder.Length > 0)
            {
                Log.Warning("==== Warning ====\n" + warningBuilder);
            }

            if (errorBuilder.Length > 0)
            {
                Log.Error("==== Error ====\n" + errorBuilder);
            }
            Reset();
        }

        private void Reset()
        {
            infoBuilder.Clear();
            warningBuilder.Clear();
            errorBuilder.Clear();
        }
    }
}