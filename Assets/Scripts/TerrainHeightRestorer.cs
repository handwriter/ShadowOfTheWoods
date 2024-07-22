using UnityEngine;

public class TerrainHeightRestorer : MonoBehaviour
{
    public Terrain targetTerrain;
    public float[,] heightsToRestore;

    void Start()
    {
        if (targetTerrain == null || heightsToRestore == null)
        {
            Debug.LogError("����������, ��������� ������� � ������ ��� ��������������.");
            return;
        }

        TerrainData terrainData = targetTerrain.terrainData;
        int resolution = terrainData.heightmapResolution;

        if (heightsToRestore.GetLength(0) != resolution || heightsToRestore.GetLength(1) != resolution)
        {
            Debug.LogError("������� �������� ������ �� ������������� ���������� heightmap ��������.");
            return;
        }

        terrainData.SetHeights(0, 0, heightsToRestore);
        targetTerrain.Flush();

        Debug.Log("�������� ������ �������������.");
    }
}
