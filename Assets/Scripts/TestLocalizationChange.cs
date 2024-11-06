using System.Collections;
using System.Collections.Generic;
using UHFPS.Runtime;
using UHFPS.Scriptable;
using UnityEngine;

public class TestLocalizationChange : MonoBehaviour
{
    public GameLocalizationAsset russ;
    void Start()
    {
        GameLocalization.Instance.LocalizationAsset = russ;
        Debug.Log(GameLocalization.Instance.textUpdaters);
        GameLocalization.Instance.Disposables.Dispose();
        Debug.Log(GameLocalization.Instance.Disposables.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
