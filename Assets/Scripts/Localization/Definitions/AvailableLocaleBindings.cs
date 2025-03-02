using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace HexagonTile.Localization.Definitions
{
    public class AvailableLocaleBindings
    {
        // Define the support locales in this dictionary.
        // If there isn't a locale in this, it will be defaulted to English
        // even if the locale data is exist in the localization table.
        public static Dictionary<SystemLanguage, Locale> SystemLanguageToUnityLocalizationLocaleEnum =
        new Dictionary<SystemLanguage, Locale>
        {
            { SystemLanguage.English, LocalizationSettings.AvailableLocales.GetLocale("en") },
            { SystemLanguage.Korean, LocalizationSettings.AvailableLocales.GetLocale("ko") },
        };

        public static Dictionary<Locale, SystemLanguage> UnityLocalizationLocaleEnumToSystemLanguage =
            SystemLanguageToUnityLocalizationLocaleEnum.ToDictionary(x => x.Value, x => x.Key);

        public static Dictionary<SystemLanguage, Locale> SysLangToL18nLocale =>
            SystemLanguageToUnityLocalizationLocaleEnum;
        
        public static Dictionary<Locale, SystemLanguage> L18nLocaleToSysLang =>
            UnityLocalizationLocaleEnumToSystemLanguage;
    }
}
