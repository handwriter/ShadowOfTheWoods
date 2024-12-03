using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TransparentMaterialsManager : MonoBehaviour
{
    [Serializable]
    public class MaterialSwapData
    {
        public Material OpaqueMaterial;
        public Material TransparentMaterial;
        public SkinnedMeshRenderer SkinnedMeshRenderer;
    }

    [SerializeField] private MaterialSwapData[] MaterialsData;

    public void SetTransparentState(bool value)
    {
        //foreach (MaterialSwapData data in MaterialsData)
        //{
        //    if (data.SkinnedMeshRenderer) data.SkinnedMeshRenderer.material = value ? data.TransparentMaterial : data.OpaqueMaterial;
        //}
    }

    public void SetAlpha(float value)
    {
        //foreach (MaterialSwapData data in MaterialsData)
        //{
        //    Color cl = data.SkinnedMeshRenderer.material.color;
        //    data.SkinnedMeshRenderer.material.color = new Color(cl.r, cl.g, cl.b, value);
        //}
    }
}
