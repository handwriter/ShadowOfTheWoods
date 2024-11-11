using System.Collections;
using UnityEngine;
using ThunderWire.Attributes;
using TMPro;
using UHFPS.Tools;

namespace UHFPS.Runtime
{
    [InspectorHeader("Objective Notification")]
    public class ObjectiveNotification : MonoBehaviour
    {
        public Animator Animator;
        public TMP_Text Title;

        [Header("Animation")]
        public string ShowTrigger = "Show";
        public string HideTrigger = "Hide";
        public string HideState = "Hide";

        private bool isShowed;
        private string _titleKey;

        public void ShowNotification(string title, float duration)
        {
            if (isShowed)
                return;

            _titleKey = title;
            Animator.SetTrigger(ShowTrigger);
            StartCoroutine(OnShowNotification(duration));
            isShowed = true;
        }

        IEnumerator OnShowNotification(float duration)
        {
            yield return new WaitForSeconds(duration);
            Animator.SetTrigger(HideTrigger);
            yield return new WaitForAnimatorStateExit(Animator, HideState);
            isShowed = false;
        }

        private void Update()
        {
            Title.text = LocalizationManager.GetLocaleText(_titleKey);
            if (Title.text.IsEmpty()) Title.text = _titleKey;
        }
    }
}