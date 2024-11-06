using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

namespace UHFPS.Runtime
{
    public class LoadGameSlot : MonoBehaviour
    {
        public TMP_Text IndexText;
        public RawImage Thumbnail;
        public TMP_Text SaveTypeText;
        public TMP_Text SceneNameText;
        public TMP_Text TimeSavedText;
        public TMP_Text PlaytimeText;

        [SerializeField] private string _autoSaveKey;
        [SerializeField] private string _manualSaveKey;

        public void Initialize(int index, SavedGameInfo info)
        {
            IndexText.text = index.ToString();
            Thumbnail.texture = info.Thumbnail;
            SaveTypeText.text = info.IsAutosave ? LocalizationManager.GetLocaleText(_autoSaveKey) : LocalizationManager.GetLocaleText(_manualSaveKey);
            SceneNameText.text = info.Scene;
            TimeSavedText.text = info.TimeSaved.ToString("dd/MM/yyyy HH:mm:ss");
            PlaytimeText.text = info.TimePlayed.ToString(@"hh\:mm\:ss");
        }
    }
}