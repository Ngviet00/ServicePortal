using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Serilog;

namespace ServicePortals.Infrastructure.Helpers
{
    public static class Helper
    {
        #region BCrypt

        public static string HashString(string? input)
        {
            return BCrypt.Net.BCrypt.HashPassword(input, workFactor: 12);
        }

        public static bool VerifyString(string hash, string currrentInput)
        {
            if (string.IsNullOrEmpty(hash))
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(currrentInput, hash);
        }

        #endregion

        #region Enum

        public static string GetDescriptionEnum(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();

            return attr?.Description ?? value.ToString();
        }

        //var list = EnumHelper.ToList<TypeLeaveEnum>();
        public static List<object> ToList<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new
                {
                    Key = Convert.ToInt32(e),
                    Value = e.ToString(),
                    Note = GetDescriptionEnum(e)
                })
                .Cast<object>()
                .ToList();
        }

        public static string? GetDescriptionFromValue<TEnum>(int? value) where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                return value?.ToString();

            var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), value);
            return GetDescriptionEnum(enumValue);
        }

        #endregion

        #region
        public static string ConvertObjToString(object obj)
        {
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Chuẩn hóa lại về dạng FormC
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string UnicodeToTCVN(string input)
        {
            string[] tcvnChars = (
                "184,181,182,183,185,168,190,187,188,189,198,169,202,199,200,201,203," +
                "208,204,206,207,209,170,213,210,211,212,214,221,215,216,220,222,227," +
                "223,225,226,228,171,232,229,230,231,233,172,237,234,235,236,238,243," +
                "239,241,242,244,173,248,245,246,247,249,253,250,251,252,254,174," +
                "184,181,182,183,185,161,190,187,188,189,198,162,202,199,200,201,203," +
                "208,204,206,207,209,163,213,210,211,212,214,221,215,216,220,222,227," +
                "223,225,226,228,164,232,229,230,231,233,165,237,234,235,236,238,243," +
                "239,241,242,244,166,248,245,246,247,249,253,250,251,252,254,167"
            ).Split(',');

            string[] unicodeCodes = (
                "225,224,7843,227,7841,259,7855,7857,7859,7861,7863,226,7845,7847,7849,7851,7853," +
                "233,232,7867,7869,7865,234,7871,7873,7875,7877,7879,237,236,7881,297,7883," +
                "243,242,7887,245,7885,244,7889,7891,7893,7895,7897,417,7899,7901,7903,7905,7907," +
                "250,249,7911,361,7909,432,7913,7915,7917,7919,7921,253,7923,7927,7929,7925,273," +
                "193,192,7842,195,7840,258,7854,7856,7858,7860,7862,194,7844,7846,7848,7850,7852," +
                "201,200,7866,7868,7864,202,7870,7872,7874,7876,7878,205,204,7880,296,7882," +
                "211,210,7886,213,7884,212,7888,7890,7892,7894,7896,416,7898,7900,7902,7904,7906," +
                "218,217,7910,360,7908,431,7912,7914,7916,7918,7920,221,7922,7926,7928,7924,272"
            ).Split(',');

            var unicodeToTCVN = new Dictionary<int, char>();

            for (int i = 0; i < unicodeCodes.Length; i++)
            {
                if (int.TryParse(unicodeCodes[i].Trim(), out int unicode)
                    && int.TryParse(tcvnChars[i].Trim(), out int tcvn))
                {
                    unicodeToTCVN[unicode] = (char)tcvn;
                }
            }

            var sb = new StringBuilder();

            foreach (char c in input)
            {
                int code = (int)c;
                if (unicodeToTCVN.TryGetValue(code, out char tcvnChar))
                {
                    sb.Append(tcvnChar);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
        #endregion

        #region Files
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

        #endregion

        public static string CalculateRoundedTime(double durationInHours)
        {
            if (durationInHours < 0)
            {
                throw new ArgumentException("Đầu vào không hợp lệ. Vui lòng cung cấp một số không âm.");
            }

            const double STANDARD_WORK_HOURS_PER_DAY = 8;
            double doubledDuration = durationInHours * 2;
            double ceiledDoubledDuration = Math.Ceiling(doubledDuration);
            double roundedHours = ceiledDoubledDuration / 2;
            double finalRoundedTime = roundedHours / STANDARD_WORK_HOURS_PER_DAY;

            return finalRoundedTime.ToString();
        }

        public static string GenerateFormCode(string prefix)
        {
            DateTimeOffset now = DateTimeOffset.Now;
            string timestampPart = now.ToString("yyMMddHHmmssfff");
            string orderCode = $"{prefix}#{timestampPart}";

            return orderCode;
        }
    }
}
