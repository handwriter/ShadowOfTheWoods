using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCanvas : MonoBehaviour
{
    Canvas canv;

    void Start()
    {
        canv = GetComponent<Canvas>();
    }

    void Update()
    {
        if (Input.GetKeyDown("q")) {
            canv.enabled = !canv.enabled;
        }
    }
}
