using UnityEngine;
using UnityEngine.Events;

namespace UHFPS.Runtime
{
    public class GLocText : MonoBehaviour
    {
        public GString GlocKey;
        public bool ObserveMany;
        public UnityEvent<string> OnUpdateText;

        private void Start()
        {
            LocalizationManager.OnLanguageUpdated += OnLanguageUpdated;
            OnLanguageUpdated();
        }

        private void OnLanguageUpdated()
        {
            OnUpdateText?.Invoke(LocalizationManager.GetLocaleText(GlocKey.GlocText));
        }
    }
}