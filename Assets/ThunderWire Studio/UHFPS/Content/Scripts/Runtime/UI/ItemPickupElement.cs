using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using static UHFPS.Runtime.Inventory;
using UHFPS.Tools;

namespace UHFPS.Runtime
{
    public class ItemPickupElement : MonoBehaviour
    {
        public TMP_Text PickupText;
        public UnityEngine.UI.Image PickupIcon;
        public string LootText;

        [Header("Fit Settings")]
        public bool FitIcon = true;
        public float FitSize = 50f;

        [Header("Animation")]
        public Animator Animator;
        public string ShowAnimation = "Show";
        public string HideAnimation = "Hide";

        private string _itemName;
        private int _shortcutId;

        private void Start()
        {
        }

        public void ShowItemPickup(string text, Sprite icon, float time, int shortcutId = -1)
        {
            _itemName = text;
            _shortcutId = shortcutId;
            PickupIcon.sprite = icon;

            Vector2 slotSize = Vector2.one * FitSize;
            Vector2 iconSize = icon.rect.size;

            Vector2 scaleRatio = slotSize / iconSize;
            float scaleFactor = Mathf.Min(scaleRatio.x, scaleRatio.y);
            PickupIcon.rectTransform.sizeDelta = iconSize * scaleFactor;

            StartCoroutine(OnShowPickupElement(time));
        }

        IEnumerator OnShowPickupElement(float time)
        {
            Animator.SetTrigger(ShowAnimation);
            yield return new WaitForAnimatorClip(Animator, ShowAnimation);

            yield return new WaitForSeconds(time);

            Animator.SetTrigger(HideAnimation);
            yield return new WaitForAnimatorClip(Animator, HideAnimation);

            yield return new WaitForEndOfFrame();
            Destroy(gameObject);
        }

        private void Update()
        {
            string itemName = LocalizationManager.GetLocaleText(_itemName);
            if (itemName.IsEmpty()) itemName = _itemName;
            string pickupText = LocalizationManager.GetLocaleText(LootText) + " " + itemName;
            if (_shortcutId >= 0) pickupText += $"\nPress {_shortcutId + 1} to Equip";
            PickupText.text = pickupText;
        }
    }
}