using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RandomPointManager : MonoBehaviour
{
    [Serializable]
    public class PointGroup
    {
        public string key;
        public int defaultPointIndex;
        public GameObject[] points;
        public GameObject defaultPoint => points[defaultPointIndex];
    }

    public PointGroup[] Groups;
    private Dictionary<string, PointGroup> _groups = new Dictionary<string, PointGroup>();

    private void Awake()
    {
        foreach (PointGroup group in Groups) _groups.Add(group.key, group);
    }

    public Vector3 GetRandomPoint(string key, bool useDefaultValue = false)
    {
        if (useDefaultValue)
        {
            return _groups[key].defaultPoint.transform.position;
        }
        return _groups[key].points[UnityEngine.Random.Range(0, _groups[key].points.Length)].transform.position;
    }
}
