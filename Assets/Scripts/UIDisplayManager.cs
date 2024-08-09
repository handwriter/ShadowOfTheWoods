using System.Collections;
using System.Collections.Generic;
using UHFPS.Runtime;
using UnityEngine;

public class UIDisplayManager : MonoBehaviour
{
    public CanvasGroup[] CanvasDoChangeAlpha;
    void Update()
    {
        foreach (CanvasGroup group in CanvasDoChangeAlpha)
        {
            group.alpha = OptionsManager.Instance.GetOption("General.show_ui").Value.Option.GetOptionValue().ToString() == "1" ? 1 : 0;
        }
    }
}
