using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class GraphScreen : MonoBehaviour
{
    //dane pod przeciwników 
    List<int> EnemyNumber = new List<int>();

    //dane pod dungeon
    List<int> safeRooms = new List<int>();
    List<int> dangerRooms = new List<int>();
    List<int> PathsToDoor = new List<int>();
   

    //kamera
    public float dragSpeed = 0.01f;
    public float zoomSpeed = 5f;

    public float minZoom = 2f;
    public float maxZoom = 20f;

    private Vector3 lastMousePos;
    private Camera cam;

    public List<GameObject> Texts = new List<GameObject>();
    void Start()
    {
        cam = Camera.main;

        safeRooms = SceneData.safeRooms_data;
        dangerRooms = SceneData.dangerRooms_data;
        PathsToDoor = SceneData.PathsToDoor_data;
        EnemyNumber = SceneData.EnemiesNumber;
        

        GenerateGraphDungeon();
    }
    void Update()
    {
        HandleDrag();
        HandleZoom();
    }
    

    void GenerateGraphDungeon() {
        List<string> values = new List<string>();
        if (SceneData.GraphType == 0)
        {
            float AVG_safeRooms = safeRooms.Sum() / safeRooms.Count;
            float AVG_dangerRooms = dangerRooms.Sum() / dangerRooms.Count;
            float AVG_pathToDoor = PathsToDoor.Sum() / PathsToDoor.Count;

           
            values.Add("Srednia wartosc bezpiecznych pokoi na dungeon: "+AVG_safeRooms);
            values.Add("Srednia wartosc niebezpiecznych pokoi na dungeon: " + AVG_dangerRooms);
            values.Add("Srednia wartosc sciezki na dungeon: " + AVG_pathToDoor);

            
        }
        else { 
            float AVG_enemyCount = EnemyNumber.Sum() / EnemyNumber.Count;

            values.Add("Srednia wartosc przeciwnikow na dungeon: "+AVG_enemyCount);
            values.Add("_");
            values.Add("_");
        }

        for (int i = 0; i < Texts.Count; i++)
        {
            TMP_Text description = Texts[i].GetComponent<TMP_Text>();
            description.text = values[i];
        }
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;

            // wa¿ne: odwrócenie osi
            cam.transform.position -= new Vector3(delta.x, delta.y, 0) * dragSpeed;

            lastMousePos = Input.mousePosition;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
    public static class SceneData
    {
        public static int GraphType;
        //dung data
        public static List<int> safeRooms_data;
        public static List<int> dangerRooms_data;
        public static List<int> PathsToDoor_data;
        public static int[,] heatmap_data;
        //public static List<int[,]> GridValues;
        public static float MaxValue_data;

        //eng data
        public static int[,] EnemHeatmap_data;
        public static List<int> EnemiesNumber;
        public static float MaxEnem_data;

    }
}
