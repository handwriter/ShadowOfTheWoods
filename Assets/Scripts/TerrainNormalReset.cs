using UnityEngine;

public class TerrainNormalReset : MonoBehaviour
{
    public Terrain[] terrains;

    void Start()
    {
        foreach (var terrain in terrains)
        {
            if (terrain != null)
            {
                TerrainData terrainData = terrain.terrainData;
                terrainData.SetHeights(0, 0, terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution));
                terrain.Flush();
            }
        }
    }
}
