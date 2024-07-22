using UnityEngine;

public class TerrainHeightComparer : MonoBehaviour
{
    public Terrain terrain1;
    public Terrain terrain2;

    void Start()
    {
        if (terrain1 == null || terrain2 == null)
        {
            Debug.LogError("Пожалуйста, назначьте оба террейна для сравнения.");
            return;
        }

        TerrainData data1 = terrain1.terrainData;
        TerrainData data2 = terrain2.terrainData;

        if (data1.heightmapResolution != data2.heightmapResolution)
        {
            Debug.LogError("Террейны имеют разные разрешения heightmap.");
            return;
        }

        int resolution = data1.heightmapResolution;
        float[,] heights1 = data1.GetHeights(0, 0, resolution, resolution);
        float[,] heights2 = data2.GetHeights(0, 0, resolution, resolution);

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                if (heights1[y, x] != heights2[y, x])
                {
                    Debug.Log($"Различие в высоте найдено на ({x}, {y}): {heights1[y, x]} vs {heights2[y, x]}");
                }
            }
        }

        Debug.Log("Сравнение высотных данных завершено.");
    }
}
