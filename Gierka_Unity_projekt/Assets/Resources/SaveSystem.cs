using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{

    public TileManager tileManager;
    public StatsGame statsGame;


    public void SaveGame()
    {
        SaveDungeon();
        SaveEnemies(null);

    }
    void SaveDungeon() {
        RoomData data = new RoomData();

        data.safe_room = tileManager.safe_rooms;
        data.danger_room = tileManager.danger_rooms;
        data.path_to_door = tileManager.path_to_door;

        int[,] grid = tileManager.pos;

        data.grid_values = FlattenGrid(grid);
        data.grid_width = tileManager.width;
        data.grid_height = tileManager.height;

        string json = JsonUtility.ToJson(data, true);
        string seedResult = ReadSeed();
        string path = Application.persistentDataPath + "/dungData/" + seedResult + ".json";
        File.WriteAllText(path, json);
        Debug.Log("Game saved at: " + path);


    }

    public void SaveEnemies(string? input_seed)
    {
        //zapis przeciwników
        EnemyData dataEnemy = new EnemyData();

        

        
        string seedResult;
        if (input_seed == null) { //zapisywanie od TileManagera
            seedResult = ReadSeed();
            dataEnemy.enemy_positions = tileManager.enemy_pos_save;
            dataEnemy.enemy_count = tileManager.enemy_pos_save.Count;
            dataEnemy.RoomWidth = tileManager.tileWidth;
            dataEnemy.RoomHeight = tileManager.tileHeight;
        }
        else { //funkcja dla stats by tylko wygenerowaæ pozycje przeciwników
            seedResult=input_seed;
            dataEnemy.enemy_positions = statsGame.enemy_pos_save;
            dataEnemy.enemy_count = statsGame.enemy_pos_save.Count;
            dataEnemy.RoomWidth = statsGame.tileWidth;
            dataEnemy.RoomHeight = statsGame.tileHeight;
        }
        string jsonEnemy = JsonUtility.ToJson(dataEnemy, true);
        string pathEnemy = Application.persistentDataPath + "/enemiesData/" + seedResult + ".json";
        File.WriteAllText(pathEnemy, jsonEnemy);
        Debug.Log("Enemies saved at: " + pathEnemy);

    }
    int[] FlattenGrid(int[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int[] flat = new int[width * height];

        int index = 0;

        // od góry do do³u
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                flat[index] = grid[x, y];
                index++;
            }
        }

        return flat;
    }

    string ReadSeed()
    {
        string path = Application.persistentDataPath + "/seed.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "0");
        }

        string line = File.ReadAllText(path);

        int seed = int.Parse(line);
        Debug.Log("Seed przed: " + seed);

        seed++;
        int seedCount = (seed.ToString()).Count(c => c != ' ');
        string newLine = "";
        if (seedCount < 6)
        {
            for (int i = 0; i < 6 - seedCount; i++)
            {
                newLine += '0';
            }

        }
        newLine += seed.ToString();
        File.WriteAllText(path, newLine);
        return newLine;
    }
}

[System.Serializable]
public class RoomData
{
    public int safe_room;
    public int danger_room;
    public int path_to_door;

    
    public int grid_width;
    public int grid_height;

    public int[] grid_values;
}

[System.Serializable]
public class EnemyData
{
    public int enemy_count;
    public List<Vector2> enemy_positions;

    public float RoomWidth;
    public float RoomHeight;

    //mo¿e dodaæ coœ jak enemy room ¿eby by³o wiadomo w którym pokoju siê znajdowali
}
