using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ReplaceObjWithPrefabs : MonoBehaviour
{
    public Transform[] ObjectsToReplace;
    public GameObject PrefToReplace;
    public bool IsNeedUpdate = false;
    public Transform RootObj;
    void Update()
    {
        if (IsNeedUpdate)
        {
            foreach (Transform t in ObjectsToReplace)
            {
                
                GameObject newObj = PrefabUtility.InstantiatePrefab(PrefToReplace) as GameObject;
                newObj.transform.parent = RootObj;
                newObj.transform.position = t.transform.position;
                newObj.transform.rotation = t.transform.rotation;
                newObj.transform.localScale = t.transform.localScale;
                DestroyImmediate(t.gameObject);
            }
            ObjectsToReplace = new Transform[0];
            IsNeedUpdate = false;
        }
    }
}
