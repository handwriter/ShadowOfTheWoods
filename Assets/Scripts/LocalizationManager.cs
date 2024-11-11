using System;
using System.Collections;
using System.Collections.Generic;
using UHFPS.Runtime;
using UHFPS.Scriptable;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    [Serializable]
    public class LanguageData
    {
        public string Key;
        public GameLocalizationAsset LocaleAsset;
    }

    public LanguageData[] Locales;
    private Dictionary<string, Dictionary<string, string>> _processedLocales = new Dictionary<string, Dictionary<string, string>>();
    private static LocalizationManager _inst;
    public static LocalizationManager Instance
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<LocalizationManager>();
            return _inst;
        }
    }
    public static Action OnLanguageUpdated;
    private string _currentLanguage;
    public string DefaultLanguage;
    private int _prevLng;
    private void Awake()
    {
        _prevLng = -1;
        _inst = this;
        foreach (LanguageData data in Locales)
        {
            _processedLocales[data.Key] = ProcessLanguageData(data.LocaleAsset);
        }
        SetLanguage(DefaultLanguage);
    }

    public static void SetLanguage(string key)
    {
        Instance._currentLanguage = key;
        OnLanguageUpdated?.Invoke();
    }

    public static string GetLocaleText(string key)
    {
        if (key == null) return "";
        if (Instance._processedLocales[Instance._currentLanguage].ContainsKey(key)) return Instance._processedLocales[Instance._currentLanguage][key];
        return "";
    }

    private Dictionary<string, string> ProcessLanguageData(GameLocalizationAsset localizationAsset)
    {
        Dictionary<string, string> glocDict = new();

        foreach (var section in localizationAsset.Localizations)
        {
            if (string.IsNullOrEmpty(section.Section))
                continue;

            string sectionName = section.Section.Replace(" ", "");
            foreach (var loc in section.Localizations)
            {
                if (string.IsNullOrEmpty(loc.Key) || string.IsNullOrEmpty(loc.Text))
                    continue;

                string keyName = loc.Key.Replace(" ", "");
                string key = sectionName + "." + keyName;

                if (glocDict.ContainsKey(key))
                {
                    Debug.LogError($"[GameLocalization] Key with the same name has already been added. Key: {key}");
                    continue;
                }

                glocDict.Add(sectionName + "." + keyName, loc.Text);
            }
        }

        return glocDict;
    }

    private void Update()
    {
        int lng = (int)OptionsManager.Instance.GetOption("General.language").Value.Option.GetOptionValue();
        if (_prevLng != lng)
        {
            SetLanguage(lng == 0 ? "ru" : "en");
        }
        _prevLng = lng;
    }
}
