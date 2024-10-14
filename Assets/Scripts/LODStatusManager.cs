using System.Collections;
using System.Collections.Generic;
using UHFPS.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UHFPS.Runtime;
public class LODStatusManager : MonoBehaviour
{
    private LODGroup[] _lodGroups;
    private Dictionary<LODGroup, LOD[]> _lodStartParameters = new Dictionary<LODGroup, LOD[]>();
    public static float _lodMultiplyer = 1;
    private float _previousLodMultiplyer = 0;
    public TerrainData[] _treeDatas;
    public static System.Action<float> MultiplyerParameterUpdated;

    private void Start()
    {
        SaveLodParameters();
    }

    public void SaveLodParameters()
    {
        _lodGroups = FindObjectsOfType<LODGroup>();
        _lodStartParameters.Clear();
        foreach (LODGroup group in _lodGroups)
        {
            _lodStartParameters[group] = group.GetLODs();
        }
    }

    public void UpdateLodParameter()
    {
        List<LOD> newLodInfo = new List<LOD>();
        foreach (LODGroup group in _lodGroups)
        {
            newLodInfo.Clear();
            foreach (LOD lod in _lodStartParameters[group])
            {
                newLodInfo.Add(new LOD(Mathf.Clamp01(lod.screenRelativeTransitionHeight / _lodMultiplyer), lod.renderers));
            }
            group.SetLODs(newLodInfo.ToArray());
        }
    }

    void Update()
    {
        if (_previousLodMultiplyer != (float)((OptionsManager.OptionObject)OptionsManager.Instance.GetOption("Graphics.draw_distance")).Option.GetOptionValue())
        {
            _lodMultiplyer = (float)((OptionsManager.OptionObject)OptionsManager.Instance.GetOption("Graphics.draw_distance")).Option.GetOptionValue();
            UpdateLodParameter();
        }
        _previousLodMultiplyer = _lodMultiplyer;
    }
}
