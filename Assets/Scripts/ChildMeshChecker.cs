using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMeshSimplifier;

[ExecuteInEditMode]
public class ChildMeshChecker : MonoBehaviour
{
    public bool IsNeedUpdate = false;
    void Start()
    {
        
    }

    public void CheckChilds(Transform obj)
    {
        LODGroup group = null;
        LODGeneratorHelper helper = null;
        MeshRenderer renderer = null;
        if (obj.TryGetComponent(out renderer) && obj.TryGetComponent(out helper) && obj.TryGetComponent(out group))
        {
            renderer.enabled = false;
        }
        for (int i = 0;i<obj.childCount;i++)
        {
            CheckChilds(obj.GetChild(i));
        }
    }
    
    void Update()
    {
        if (IsNeedUpdate)
        {
            IsNeedUpdate = false;
            CheckChilds(transform);
        }
    }
}
