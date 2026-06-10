using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LoadSystem : MonoBehaviour
{
    public List<string> seedNames = new List<string>();
    public TMP_Dropdown SeedBox;
    public string SelectedSeed;
    RoomData DungData;
    EnemyData EnemData;

    public StatsGame Stats;
    public GameObject tilePrefab;
    public EnemiesGenerator GenEnemies;
    public enemy_script enemyprefab;
    public List<Vector2> enemy_pos_save = new List<Vector2>();
    public SaveSystem saveSystem;

    string DungeonPath;
    string EnemiesPath;

    public List<string> GraphOptions = new List<string>();
    float tileHeight;
    float tileWidth;


    private void Awake()
    {
        DungeonPath =
        Application.persistentDataPath +
        "/dungData/";

        EnemiesPath =
            Application.persistentDataPath +
            "/enemiesData/";

        GraphOptions.Add("Enemies"); //heatmapa przeciwników
        GraphOptions.Add("Dungeon"); 
    }
    public void LoadAllSeeds(int option) { //odczyt dzia³a
        
        seedNames.Clear();
        
       
        if (Directory.Exists(DungeonPath))
        {
            string[] files = Directory.GetFiles(DungeonPath, "*.json"); //awaryjny filtr, który mi odczytuje tylko json
            
            foreach (string file in files)
            {
                string name =
                    Path.GetFileNameWithoutExtension(file);

                seedNames.Add(name);
            }
          
            if (option == 1) { //je¿eli dla Load Menu
                SeedBox.ClearOptions();
                SeedBox.AddOptions(seedNames);
            }
            if (option == 2) { //je¿eli dla Stats
                Stats.LoadSeeds(seedNames);
               
            }
            
        }
    }

     

    public void ButtonLoad() //dzia³a podstawowe znalezienie pliku bez odczytu
    {
        int index = SeedBox.value;
        SelectedSeed = seedNames[index];
        string option = GraphOptions[0];

        string pathDung = DungeonPath + SelectedSeed + ".json";
        string pathEnem = EnemiesPath + SelectedSeed + ".json";

        Debug.Log("Loading: " + pathDung);

            //==ADD if statement for graph selection==
            //overall I seperately have to load 
         //for dungeon file read  
         if (File.Exists(pathDung))
         {
            if (option == "Dungeon")
            {
                string json = File.ReadAllText(pathDung);
                DungData = JsonUtility.FromJson<RoomData>(json);
                Debug.Log(DungData.safe_room);
                foreach (int values in DungData.grid_values)
                {
                    Debug.Log(values); //funkcja na ponowne zwijanie na wektor - poprawka na pozniej
                }
                LoadDungeon();
            }
            else if (option == "Enemies") {
                if (!File.Exists(pathEnem))
                {
           
                
                    float[,] gradients;
                    float[,] colors;
                    

                    Debug.Log("Enemies generating...");
                    string json = File.ReadAllText(pathDung);
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

                    saveSystem.SaveEnemies(SelectedSeed);
                    Debug.Log("ENEMIES SAVED!");
                    

                    
                }
                //the loading part
                string jsonEnem = File.ReadAllText(pathEnem);
                EnemData = JsonUtility.FromJson<EnemyData>(jsonEnem);
                Debug.Log("ENEMIES LOADED!");
                LoadEnemies();
            } 
         }
    }



    int[,] Dungeon_Setup(int gridWidth, int gridHeight, int[] grid_values)
    { //dzia³a
        int[,] finalValues = new int[gridWidth, gridHeight];
        int idx = 0;

        // Debug.Log("ROZBITA TABELA");
        for (int y = 0; y < gridHeight; y++)
        {
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

    void SetPosDetails(int[,] GridValues, List<Vector2Int> pos_details)
    {
        for (int y = 0; y < GridValues.GetLength(0); y++)
        {
            //string tempString = "";
            for (int x = 0; x < GridValues.GetLength(1); x++)
            {
                if (GridValues[x, y] == 1)
                {
                    pos_details.Add(new Vector2Int(x, y));
                }
            }
            // Debug.Log(tempString);
        }


    }

    void LoadDungeon() {
        

    }

    void LoadEnemies() { 
    
    
    }

    
    

}
