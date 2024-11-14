using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaterMonsterController : MonoBehaviour
{
    [Serializable]
    public class TransparentMeshMaterial
    {
        public SkinnedMeshRenderer MeshRenderer;
        public Material TransparentMaterial;
    }
    public TransparentMeshMaterial[] MeshMaterials;

    public void SetupMaterials()
    {
        foreach (TransparentMeshMaterial mat in MeshMaterials)
        {
            mat.MeshRenderer.material = mat.TransparentMaterial;
        }
    }

    public void SetAlphaValue(float value)
    {
        foreach (var mat in MeshMaterials)
        {
            Color color = mat.MeshRenderer.material.color;
            mat.MeshRenderer.material.color = new Color(color.r, color.g, color.b, value);
        }
    }
}
