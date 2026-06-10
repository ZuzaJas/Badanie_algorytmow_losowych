using UnityEngine;
using UnityEngine.SceneManagement;
using static GraphScreen;

public class Heatmap_manager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform tileManager;
    public RectTransform canvasTransform;

    public float cellSize = 1f;
    public int[,] heatmap;
    float MaxRange;

    private void Awake()
    {
        if (SceneData.GraphType == 0) { 
        heatmap = SceneData.heatmap_data;
        MaxRange = SceneData.MaxValue_data;
        }
        else{ 
        heatmap = SceneData.EnemHeatmap_data;
        MaxRange = SceneData.MaxEnem_data;
        }
            GenerateHeatmap();
    }

    void GenerateHeatmap()
    {
        ClearHeatmap();
        int width = heatmap.GetLength(0);
        int height = heatmap.GetLength(1);

        

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int value = heatmap[x, y];

                float normalized = (MaxRange == 0) ? 0 : (float)value / MaxRange;

                Color color = GetHeatColor(normalized);

                GameObject tile = Instantiate(tilePrefab, transform);

                tile.transform.position = new Vector3(
                    x * cellSize,
                    y * cellSize,
                    0
                );

                tile.GetComponent<SpriteRenderer>().color = color;
            }
        }
    }
    void ClearHeatmap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    Color GetHeatColor(float t)
    {
        return Color.Lerp(Color.blue, Color.red, t);
    }

    public void GoBackButton() {
        ClearSceneData();
        SceneManager.LoadSceneAsync(3);
    }

    void ClearSceneData() {
        //overall
        SceneData.GraphType = 0;
        
        //clear dungeon
        SceneData.safeRooms_data = null;
        SceneData.dangerRooms_data = null;
        SceneData.PathsToDoor_data = null;
        SceneData.heatmap_data = null;
        SceneData.MaxValue_data = 0;


        //eng data
        SceneData.EnemHeatmap_data = null;
        SceneData.EnemiesNumber = null;
        SceneData.MaxEnem_data = 0;

}
}
