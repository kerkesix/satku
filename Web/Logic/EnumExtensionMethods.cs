namespace Web.Logic
{
    using System;
    using System.Collections.Generic;

    public static class EnumExtensionMethods
    {
        // Real hack: fixed values. This is ok as long as we do not have lots ot these
        private static readonly Dictionary<string, string> Translations = new Dictionary<string, string>
            {
                { "QuitReason:Feet", "Jalat" },
                { "QuitReason:Head", "Pää" },
                { "QuitReason:Stomach", "Vatsa" },
                { "QuitReason:Ass", "Perse" },
                { "QuitReason:Other", "Muu" }
            };

        public static string Translate<T>(this T value) where T : struct, IConvertible
        {
            string enumName = typeof(T).Name;
            string valueName = Enum.GetName(typeof(T), value);
            string key = enumName + ":" + valueName;

            return Translations.ContainsKey(key) ? Translations[key] : valueName; 
        }
    }
}