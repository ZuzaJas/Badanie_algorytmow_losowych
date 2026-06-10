using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
//GRID KOSTEK
public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public SaveSystem saveSystem;
    public character postac;
    public camera_script cam;
    public enemy_script enemy_prefab;

    //skrypty
    public DungeonGenerator GenDungeon;
    public EnemiesGenerator GenEnemies;

    //Wielkoœæ siatki
    public int width;
    public int height;

    private Tile[,] grid; //tworze array klasy Tile, ',' definiuje 2 wymiary i ma nazwe grid i private to tylko ten skypt mo¿e u¿ywaæ tej tablicy
    public int[,] pos;
    

    Vector2Int current;

    //pod zapis danych
    public int safe_rooms;
    public int danger_rooms;
    public List<Vector2Int> pos_details = new List<Vector2Int>(); //pozycje niebiespiecznych pokoi
    public int path_to_door;

    public Vector2 grid_with_rooms;
    public float tileWidth;
    public float tileHeight;

    //perlin gradient
    public int UnitPerFrame = 16;
    public float[,] gradients;
    public float[,] colors;
    public List<Vector2> enemies_spawn = new List<Vector2>();
    public List<Vector2> enemies_rooms = new List<Vector2>();

    Vector2 enemySize;
    public List<Vector2> enemy_pos_save = new List<Vector2>();


    void Start() //funkcje setup
    {
        SpriteRenderer sr = tilePrefab.GetComponent<SpriteRenderer>();

        tileWidth = sr.bounds.size.x;
        tileHeight = sr.bounds.size.y;
        cam.GetRoomHeight(tileWidth, tileHeight);
        enemySize= enemy_prefab.GetComponent<SpriteRenderer>().bounds.size;
        //Debug.Log("ENEMYYYYY"+enemySize.x);

        width = 5;
        height = 5;
        current = new Vector2Int(
        UnityEngine.Random.Range(0, width),
        UnityEngine.Random.Range(0, height)
        );
        
        //generacja lochów
        grid = new Tile[width, height]; //tworzymy now¹ tablice o rozmiarze width x hight o domyœlnej wartoœci null
        pos = new int[width, height];
        Grid_Iniciate();

        //perlin - rozmieszczenie przeciwników
        
        gradients = new float[Mathf.RoundToInt(tileWidth)+1, Mathf.RoundToInt(tileHeight)+1]; //ogó³em jeden pokój jest 
        colors = new float[Mathf.RoundToInt(tileWidth) * UnitPerFrame, Mathf.RoundToInt(tileHeight) * UnitPerFrame];
        SpawnEnemies();

        List<enemy_script> enemies_TempData = new List<enemy_script>();
        //pojawienie przeciwnika
        foreach (Vector2 enemy in enemies_spawn)
        {
            enemy_script newEnemy =
                Instantiate(enemy_prefab,
                            new Vector3(enemy.x, enemy.y, -1),
                            Quaternion.identity);
            newEnemy.SetManager(this); //zapobieganie null TileManager
            newEnemy.setRoomPos(enemies_rooms[0]);
            enemies_rooms.RemoveAt(0);
            enemies_TempData.Add(newEnemy);
        }

        //pojawianie siê pokoi
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                GameObject obj = Instantiate(
                    tilePrefab,
                    new Vector3(x * (tileWidth), y * (tileHeight), 0),
                    Quaternion.identity
                ); // tutaj z góry ustawiam jego pozycje
                SceneManager.MoveGameObjectToScene(
                    obj, gameObject.scene);

                Tile tile = obj.GetComponent<Tile>();

                int value = pos[x, y];   // numer pokoju

                tile.SetTile(value);
                tile.SetGridPos(new Vector2Int(x, y));
                for (int i=0; i<enemies_TempData.Count; i++)
                {
                    if (enemies_TempData[i].RoomPos == new Vector2(x, y)) {
                        enemies_TempData[i].SetRoom(tile);
                        tile.AddEnemy(enemies_TempData[i]);
                        enemies_TempData.RemoveAt(i);
                        i--;
                    }
                }
                    grid[x, y] = tile;
            }
        }
        saveSystem.SaveGame(); // na koñcu rozstawiania wszystkiego
    }

    //plan jest by najpierw rozstawiæ niebezpieczne pokoje, potem na podsatwie tego okreœliæ najdalsz¹ œcie¿kê i na koniec umieœciæ wyjœcie
    //potem na podstawie pozosta³ego s¹siadztwa safe room 
    // ===================================== DRUKNKARDS ====================================================================
    void Grid_Iniciate() {
        int x = current.x;
        int y = current.y;
        string logo = "START"+current.ToString();
        //Debug.Log(logo);
        pos[x, y] = 1;
        postac.SetStartPos(new Vector3(x * (tileWidth), y * (tileHeight), 0));

        //Generacja lochów i przeciwników za pomoc¹ skryptów przeznaczonych do nich
        GenerateDungeon(x, y);
        
    }

    void GenerateDungeon(int x, int y) {
        GenDungeon.ReadValues(width, height, current);
        GenDungeon.Drunkard(danger_rooms, pos_details, pos);
        GenDungeon.The_longest_path(path_to_door,  pos, x, y);
        GenDungeon.bezpieczne_pokoje(safe_rooms, pos_details, pos);
        GenDungeon.Test_print(pos);

    }

   
    void SpawnEnemies() {
        
        foreach (Vector2Int posi in pos_details) //przechodzimy przez ka¿dy niebiezpieczny pokój
        {
            int n = UnityEngine.Random.Range(2, 4); //iloœæ potworków na pokój
            GenEnemies.ReadValues(tileWidth, tileHeight, UnitPerFrame, enemySize);
            GenEnemies.Perlin(gradients, colors,Mathf.RoundToInt(tileWidth), Mathf.RoundToInt(tileHeight)); //zrobienie gradientu
            GenEnemies.CountNeighbours(colors,  enemy_pos_save, enemies_spawn, enemies_rooms ,posi, n); //zliczy gdzie jest najwiêkszê s¹siedztwo i na podstawie tego wyznaczy dostêpne pozycje 
            GenEnemies.ClearGradient( gradients,  colors); //wyczyœci mi wartoœci na elementy zerowe
        }
       
    }
    
}




