using Serilog;
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

        public static void DeleteOldFiles(string path, DateTime now, int dayDelete)
        {
            if (!Directory.Exists(path))
            {
                Log.Information($"Path {path} not found for deletion!");
                return;
            }

            int batchSize = 200;

            var fileBatch = Directory.EnumerateFiles(path).Take(batchSize);

            while (fileBatch.Any())
            {
                foreach (var file in fileBatch)
                {
                    DateTime creationTime = File.GetCreationTime(file); 
                    TimeSpan fileAge = now - creationTime;

                    if (fileAge.TotalDays > dayDelete)
                    {
                        try
                        {
                            File.Delete(file);
                            Log.Information($"Deleted: {file}");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error deleting file {file}, error: {ex.Message}");
                        }
                    }
                }

                fileBatch = Directory.EnumerateFiles(path).Skip(batchSize).Take(batchSize);
            }

            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                DeleteOldFiles(directory, now, dayDelete);
            }

            if (!Directory.EnumerateFileSystemEntries(path).Any())
            {
                try
                {
                    Directory.Delete(path);
                    Log.Information($"Deleted empty directory: {path}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Error deleting directory {path}, error: {ex.Message}");
                }
            }
        }
    }
}
