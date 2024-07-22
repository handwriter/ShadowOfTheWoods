using UnityEngine;

public class TerrainHeightRestorer : MonoBehaviour
{
    public Terrain targetTerrain;
    public float[,] heightsToRestore;

    void Start()
    {
        if (targetTerrain == null || heightsToRestore == null)
        {
            Debug.LogError("Пожалуйста, назначьте террейн и высоты для восстановления.");
            return;
        }

        TerrainData terrainData = targetTerrain.terrainData;
        int resolution = terrainData.heightmapResolution;

        if (heightsToRestore.GetLength(0) != resolution || heightsToRestore.GetLength(1) != resolution)
        {
            Debug.LogError("Размеры высотных данных не соответствуют разрешению heightmap террейна.");
            return;
        }

        terrainData.SetHeights(0, 0, heightsToRestore);
        targetTerrain.Flush();

        Debug.Log("Высотные данные восстановлены.");
    }
}
