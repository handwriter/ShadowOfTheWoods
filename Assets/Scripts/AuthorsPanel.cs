using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AuthorsPanel : MonoBehaviour
{
    public ScrollRect ScrollRect;
    public float AutoScrollSpeed;
    public float DelayToAutoScroll;
    private float _timeFromActive = 0;

    private void OnEnable()
    {
        _timeFromActive = 0;
    }

    void Update()
    {
        _timeFromActive += Time.deltaTime;
        if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.delta.magnitude > 0) _timeFromActive = 0;
        if (_timeFromActive > DelayToAutoScroll)
        {
            ScrollRect.verticalNormalizedPosition = Mathf.Clamp01(ScrollRect.verticalNormalizedPosition - AutoScrollSpeed * Time.deltaTime);
        }
    }
}
