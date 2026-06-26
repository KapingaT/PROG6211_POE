using System;
using System.Collections.Generic;
using System.Text;

namespace POE_FINAL
{
   
        public class ActivityLog
        {
            private List<string> logs;
            private int maxEntries = 100;

            public ActivityLog()
            {
                logs = new List<string>();
            }

            public void AddLog(string action)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                logs.Add($"[{timestamp}] {action}");

                if (logs.Count > maxEntries)
                {
                    logs.RemoveRange(0, logs.Count - maxEntries);
                }
            }

            public string GetRecentLogs(int count = 10)
            {
                if (logs.Count == 0)
                    return "No activities have been logged yet.";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(" Activity Log (Last " + Math.Min(count, logs.Count) + " actions):");
                sb.AppendLine("----------------------------------------");

                int startIndex = Math.Max(0, logs.Count - count);
                int logNumber = 1;

                for (int i = startIndex; i < logs.Count; i++)
                {
                    sb.AppendLine($"{logNumber}. {logs[i]}");
                    logNumber++;
                }

                if (logs.Count > count)
                {
                    sb.AppendLine($"\n {logs.Count - count} more entries. Type 'show full log' to see all.");
                }

                return sb.ToString();
            }

            public string GetFullLog()
            {
                if (logs.Count == 0)
                    return "No activities have been logged yet.";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(" Complete Activity Log:");
                sb.AppendLine("----------------------------------------");

                for (int i = 0; i < logs.Count; i++)
                {
                    sb.AppendLine($"{i + 1}. {logs[i]}");
                }

                return sb.ToString();
            }
        }
    }
