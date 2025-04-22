using ServicePortal.Domain.Enums;

namespace ServicePortal.Common.Helpers
{
    public static class FileHelper
    {
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public static void WriteLog(TypeErrorEnum typeError, string? message)
        {
            _lock.EnterWriteLock();

            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));

                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                using (StreamWriter writer = new StreamWriter($@"{logPath}\{DateTime.Now.ToString("dd")}.txt", true))
                {
                    if (typeError == TypeErrorEnum.INFO)
                    {
                        writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd__HH:mm:ss:fff")} ---> [INFO] {message}");
                    }
                    else
                    {
                        writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd__HH:mm:ss:fff")} ---> ==================== [ERROR] \n{message} \n =============");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message: {ex.Message}");
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public static void CleanupOldLogs()
        {
            try
            {
                string logRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logRootPath))
                    return;

                DateTime thresholdDate = DateTime.Now.AddMonths(-1);

                foreach (var yearDir in Directory.GetDirectories(logRootPath))
                {
                    foreach (var monthDir in Directory.GetDirectories(yearDir))
                    {
                        foreach (var logFile in Directory.GetFiles(monthDir, "*.txt"))
                        {
                            DateTime fileDate;
                            string fileName = Path.GetFileNameWithoutExtension(logFile);

                            if (DateTime.TryParseExact(fileName, "dd", null, System.Globalization.DateTimeStyles.None, out fileDate))
                            {
                                fileDate = new DateTime(int.Parse(Path.GetFileName(yearDir)), int.Parse(Path.GetFileName(monthDir)), fileDate.Day);
                            }
                            else
                            {
                                fileDate = File.GetLastWriteTime(logFile);
                            }

                            if (fileDate < thresholdDate)
                            {
                                File.Delete(logFile);
                                Console.WriteLine($"Deleted log: {logFile}");
                            }
                        }

                        if (Directory.GetFiles(monthDir).Length == 0 && Directory.GetDirectories(monthDir).Length == 0)
                        {
                            Directory.Delete(monthDir);
                        }
                    }

                    if (Directory.GetFiles(yearDir).Length == 0 && Directory.GetDirectories(yearDir).Length == 0)
                    {
                        Directory.Delete(yearDir);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup Log Error: {ex.Message}");
            }
        }
    }
}
