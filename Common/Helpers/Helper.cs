﻿using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;

namespace ServicePortal.Common.Helpers
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

        public static string GetDescriptionFromValue<TEnum>(int value) where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                return value.ToString();

            var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), value);
            return GetDescriptionEnum(enumValue);
        }

        #endregion

        public static string ConvertObjToString(object obj)
        {
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
