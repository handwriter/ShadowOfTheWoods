using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class TrimNames : MonoBehaviour
{
    [MenuItem("Tools/Utility/Trim Object Names")]
    public static void TrimObjectNames()
    {
        var regex = new Regex("\\(.*?\\)");
        
        var objects = FindObjectsOfType<GameObject>();
        foreach (var obj in objects)
        {
            if(string.IsNullOrEmpty(obj.name))
                continue;
            var matches = regex.Matches(obj.name);
            if(matches.Count < 1)
                continue;
            var lastMatch = matches.Cast<Match>().Last();
            if(lastMatch == null)
                continue;
            obj.name = obj.name.Remove(lastMatch.Index, lastMatch.Length).TrimEnd();
        }
    }
}
