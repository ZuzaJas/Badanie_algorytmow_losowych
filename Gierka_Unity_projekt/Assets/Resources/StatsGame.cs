using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GraphScreen;

public class StatsGame : MonoBehaviour
{
    public List<string> MseedNames = new List<string>();
    public LoadSystem Loadsys;
    public GameObject seedItemPrefab;
    public Transform contentParent;
    public List<SeedItem> seedItems;
    public List<RoomData> DungData = new List<RoomData>();
    public List<EnemyData> EnemData = new List<EnemyData>();



    //gdyby trzeba by³o wygenerowaæ przeciwników na nowo
    public EnemiesGenerator GenEnemies; //gdyby byl przypadek dungeon jest, nie ma przeciwników 
    public GameObject tilePrefab;
    public enemy_script enemyprefab;
    public SaveSystem saveSystem;
    public List<Vector2> enemy_pos_save = new List<Vector2>();
    public float tileWidth;
    public float tileHeight;

    //paths
    string DungeonPath; 
    string EnemiesPath;


    private void Awake()
    {
        DungeonPath =
        Application.persistentDataPath +
        "/dungData/";

        EnemiesPath =
            Application.persistentDataPath +
            "/enemiesData/";
        
    }

    void Start()
    {
        Loadsys.LoadAllSeeds(2);
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("FORCE REBUILD TEST");
            BuildSeedList();
        }
    }

    public void LoadSeeds(List<string> seedNames) { 
        MseedNames = seedNames;
        Debug.Log("SEED COUNT: " + MseedNames.Count);
        Debug.Log("LoadSeed");
        contentParent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
        BuildSeedList();
    }

    public void BuildSeedList()
    {
        // usuñ stare UI
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
           
        }
        
        foreach (string seed in MseedNames)
        {
            
            GameObject obj = Instantiate(seedItemPrefab, contentParent);
            obj.transform.localScale = Vector3.one;
            
            SeedItem item = obj.GetComponent<SeedItem>();
            
            item.Setup(seed);
            item.toggle.onValueChanged.AddListener((value) =>
            {
                OnSeedToggled(seed, value);
            });
            seedItems.Add(item);
        }
        Debug.Log("[StatsGame] BuildSeedList END");
    }

    public List<string> selectedSeeds = new List<string>();

    void OnSeedToggled(string seed, bool isOn)
    {
        if (isOn)
        {
            if (!selectedSeeds.Contains(seed))
                selectedSeeds.Add(seed);
        }
        else
        {
            selectedSeeds.Remove(seed);
        }
    }

    public void ConfirmButton()
    {
        int ChartType = 1; //idx 0 - Dung, 1 - Enem
        DungData.Clear();
        EnemData.Clear();
        foreach (string seed in selectedSeeds)
        {
            //Debug.Log(seed);
            ReadFileContent(seed, ChartType);
        }
        SceneData.GraphType = ChartType;
        if (ChartType == 0) //je¿eli graf dungeon
        {
            List<int> safeRooms = new List<int>();
            List<int> dangerRooms = new List<int>();
            List<int> PathsToDoor = new List<int>();
            List<Vector2> Sizes = new List<Vector2>();
            List<int[,]> Grids = new List<int[,]>();
            Vector2 baseSize = new Vector2(DungData[0].grid_width, DungData[0].grid_height);
            for (int i = 0; i < DungData.Count; i++)
            {
                if (DungData[i].grid_width == baseSize.x && DungData[i].grid_height == baseSize.y)
                {

                    safeRooms.Add(DungData[i].safe_room);
                    dangerRooms.Add(DungData[i].danger_room);
                    PathsToDoor.Add(DungData[i].path_to_door);
                    Sizes.Add(new Vector2(DungData[i].grid_width, DungData[i].grid_height));
                    int[,] finalValues = new int[DungData[i].grid_width, DungData[i].grid_height];
                    int idx = 0;


                    for (int y = 0; y < DungData[i].grid_height; y++)
                    {
                        for (int x = 0; x < DungData[i].grid_width; x++)
                        {
                            finalValues[x, y] = DungData[i].grid_values[idx];
                            idx++;
                        }
                    }
                    Grids.Add(finalValues);
                }
            }



            int[,] Heatmap_grid = new int[Grids[0].GetLength(0), Grids[0].GetLength(1)];
            foreach (int[,] values in Grids)
            {
                for (int x = 0; x < values.GetLength(0); x++)
                {
                    for (int y = 0; y < values.GetLength(1); y++)
                    {
                        if (values[x, y] != 0)
                        {
                            Heatmap_grid[x, y] += 1;

                        }
                    }
                }
            }

            //for heatmap setup
            List<int> temp_HeatmapGrid = new List<int>();
            for (int y = 0; y < Heatmap_grid.GetLength(1); y++)
            {
                for (int x = 0; x < Heatmap_grid.GetLength(0); x++)
                {
                    temp_HeatmapGrid.Add(Heatmap_grid[x, y]);
                }
            }
            float RedRange = temp_HeatmapGrid.Max();
            SceneData.safeRooms_data = safeRooms;
            SceneData.dangerRooms_data = dangerRooms;
            SceneData.PathsToDoor_data = PathsToDoor;
            SceneData.heatmap_data = Heatmap_grid;
            SceneData.MaxValue_data = RedRange;
            
            Debug.Log("SafeRoom_data: " + SceneData.safeRooms_data.Count);

            SceneManager.LoadSceneAsync(4);


        }
        else
        {
            List<Vector2> Grids = new List<Vector2>();
            List<int> EnemiesNumber = new List<int>();
            //co potrzebuje na enemies:
            //- convert na size pokoju czyli 10x18
            //nastêpnie to samo na heatmapie pokoju
            Vector2 baseSize = new Vector2(EnemData[0].RoomWidth, EnemData[0].RoomHeight);
            for (int i = 0; i < EnemData.Count; i++)
            {
                if (EnemData[i].RoomWidth == baseSize.x && EnemData[i].RoomHeight == baseSize.y)
                {
                    List<Vector2> finalValues = new List<Vector2>();
                    int idx = 0;
                    EnemiesNumber.Add(EnemData[i].enemy_count);
                    for (int x = 0; x < EnemData[i].enemy_positions.Count; x++)
                    {
                         Vector2 values = new Vector2(Mathf.RoundToInt(EnemData[i].enemy_positions[idx].x) % baseSize.x,
                             (Mathf.RoundToInt(EnemData[i].enemy_positions[idx].y) % baseSize.y)); //na kordy siatki
                        if (values.x >= baseSize.x) values.x = baseSize.x - 1;
                        if (values.y >= baseSize.y) values.y = baseSize.y - 1;
                        finalValues.Add(values);
                         idx++;
                        
                    }
                    foreach (Vector2 elem in finalValues) { 
                        Grids.Add(elem);
                    }
                    
                }
            }

            int[,] Heatmap_grid = new int[(int)EnemData[0].RoomWidth, (int)EnemData[0].RoomHeight];
            foreach (Vector2 elem in Grids) {
                Heatmap_grid[Mathf.Abs((int)elem.x), Mathf.Abs((int)elem.y)] += 1;
            }

            List<int> temp_HeatmapGrid = new List<int>();
            for (int y = 0; y < Heatmap_grid.GetLength(1); y++)
            {
                for (int x = 0; x < Heatmap_grid.GetLength(0); x++)
                {
                    temp_HeatmapGrid.Add(Heatmap_grid[x, y]);
                }
            }
            SceneData.EnemHeatmap_data = Heatmap_grid;
            SceneData.EnemiesNumber = EnemiesNumber;
            SceneData.MaxEnem_data = temp_HeatmapGrid.Max();

            SceneManager.LoadSceneAsync(4);

        }
    }

    public void SelectAllButton() {
        foreach (SeedItem item in seedItems) {
            item.ChangeTogg(true);
        }
    
    }

    public void ClearButton()
    {
        foreach (SeedItem item in seedItems)
        {
            item.ChangeTogg(false);
        }
    }

    void ReadFileContent(string seed, int type) {
        string path;
        
        if (type == 0) { //for dungeon file read
            path = DungeonPath + seed + ".json";
            if (File.Exists(path))
            {
                //dzia³a
                string json = File.ReadAllText(path);
                RoomData data = JsonUtility.FromJson<RoomData>(json);
                DungData.Add(data);
                Debug.Log(data.safe_room);
                foreach (int values in data.grid_values) {
                    Debug.Log(values); //funkcja na ponowne zwijanie na wektor - poprawka na pozniej
                }
            }
            else
            {
                Debug.Log("nie znaleziono pliku duneon" + seed);
            }
        }
        else{ //for enemies
            path = EnemiesPath + seed + ".json";
            if (!File.Exists(path))
            {
                
            
                float[,] gradients;
                float[,] colors;
                string path2 = DungeonPath + seed + ".json";
                if (File.Exists(path2)) {
                    Debug.Log("Enemies generating...");
                    string json = File.ReadAllText(path2);
                    RoomData data_room = JsonUtility.FromJson<RoomData>(json);


                    int[,] GridValues = Dungeon_Setup(data_room.grid_width, data_room.grid_height, data_room.grid_values);
                    List<Vector2Int> pos_details = new List<Vector2Int>();
                    SetPosDetails(GridValues, pos_details); //debug robiony: debug da³ pozycje pokoi - nie jest issue zerowania
                    
                    int UnitPerFrame = 16;
                    
                    
                    SpriteRenderer sr = tilePrefab.GetComponent<SpriteRenderer>();
                    tileWidth = sr.bounds.size.x; 
                    tileHeight = sr.bounds.size.y; //tile size dobrze odczytywane
                    gradients = new float[Mathf.RoundToInt(tileWidth) + 1, Mathf.RoundToInt(tileHeight) + 1]; //ogó³em jeden pokój jest 
                    colors = new float[Mathf.RoundToInt(tileWidth) * UnitPerFrame, Mathf.RoundToInt(tileHeight) * UnitPerFrame];
                    Vector2 enemySize = enemyprefab.GetComponent<SpriteRenderer>().bounds.size;
                    //Debug.Log("Enemy size"+enemySize); //(0.72, 0.72)
                    List<Vector2> enemies_spawn = new List<Vector2>();
                    List<Vector2> enemies_rooms = new List<Vector2>();
                    foreach (Vector2Int posi in pos_details) //przechodzimy przez ka¿dy niebiezpieczny pokój
                    {
                        int n = UnityEngine.Random.Range(2, 4); //iloœæ potworków na pokój
                        GenEnemies.ReadValues(tileWidth, tileHeight, UnitPerFrame, enemySize);
                        GenEnemies.Perlin(gradients, colors, Mathf.RoundToInt(tileWidth), Mathf.RoundToInt(tileHeight)); //zrobienie gradientu
                        GenEnemies.CountNeighbours(colors, enemy_pos_save, enemies_spawn, enemies_rooms, posi, n); //zliczy gdzie jest najwiêkszê s¹siedztwo i na podstawie tego wyznaczy dostêpne pozycje 
                        //enemy_pos_save powinien mieæ dane
                        GenEnemies.ClearGradient(gradients, colors); //wyczyœci mi wartoœci na elementy zerowe

                    }
                    
                    
                    saveSystem.SaveEnemies(seed);
                    Debug.Log("ENEMIES SAVED!");
                }
                else { 
                    Debug.Log("nie znaleziono pliku dungeon" + seed);
                }

            }
            string jsonEnem = File.ReadAllText(path);
            EnemyData data = JsonUtility.FromJson<EnemyData>(jsonEnem);
            EnemData.Add(data);
            Debug.Log(data);
        }
        
        
    }
    // TO DELETE -> by generowaæ przeciników potrzbuje: float tileWidth, float tileHeight, int UnitPerFrame, Vector2 enemySize
    // potrzebuje prefab przeciwnika i st¹d biorê size. 
    // tile prefab do tile size(tile width and size)
    // co jakby wzi¹æ
    int[,] Dungeon_Setup(int gridWidth, int gridHeight, int[] grid_values) { //dzia³a
        int[,] finalValues = new int[gridWidth, gridHeight];
        int idx = 0;
        
       // Debug.Log("ROZBITA TABELA");
        for (int y = 0; y < gridHeight; y++) {
            //string tempString = "";
            for (int x = 0; x < gridWidth; x++)
            {
                finalValues[x, y] = grid_values[idx];
                //tempString += grid_values[idx].ToString() + ", ";
                idx++;
            }
           // Debug.Log(tempString);
        }


        return finalValues;
    }

    void SetPosDetails(int[,] GridValues, List<Vector2Int> pos_details) {
        for (int y = 0; y < GridValues.GetLength(0); y++)
        {
            //string tempString = "";
            for (int x = 0; x < GridValues.GetLength(1); x++)
            {
                if (GridValues[x, y] == 1) { 
                    pos_details.Add(new Vector2Int(x, y));
                }
            }
            // Debug.Log(tempString);
        }


    }

    public void GoBackButton() {
        ClearSceneData();
        SceneManager.LoadSceneAsync(0);
    }

    void ClearSceneData() {
        SceneData.GraphType = 0;
        //dung data
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
