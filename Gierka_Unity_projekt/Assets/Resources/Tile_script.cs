using UnityEngine;
using System.Collections.Generic; // do używania listy
public class Tile : MonoBehaviour
{

    public Vector2Int gridPosition;
    public Sprite[] roomSprites; //sprity pokoi
    private SpriteRenderer sr;
    private BoxCollider2D roomCollider;
    private Color self_color;
    

    public List<enemy_script> RoomEnemies;

    //debug
    private int enemies_count;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            //Debug.LogError("Brak SpriteRenderer!");
        }
        roomCollider = GetComponent<BoxCollider2D>();
        roomCollider.size = sr.bounds.size;
    }

    void OnTriggerEnter2D(Collider2D other) //wejście playera
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player wszedł do pokoju"+gridPosition);
            MakeActive();
           
        }
    }

    public void AddEnemy(enemy_script enemy) { 
        RoomEnemies.Add(enemy);
    
    }

    void MakeActive() {
        enemies_count = 0;
        foreach (enemy_script enemy in RoomEnemies) {
            enemy.MakeActive();
            enemies_count++;
        }
    }
    //funkcja potrzebna by TileManager mógł przypisać kolor kafelkowi
    public void SetColor(Color meColor)
    {
        self_color = meColor;
        sr.color = self_color;  
    }

    public void SetGridPos(Vector2Int gridPos) { 
        gridPosition = gridPos;
    }

   
    public int ibn = 0;
    public void SetTile(int value)
    {
        if (value < 1)
        {
            sr.enabled = false;
            return;
        }

        sr.enabled = true;

        int index = value - 1;

        if (index >= 0 && index < roomSprites.Length)
            sr.sprite = roomSprites[index];
        else
            Debug.LogError("Brak sprite dla value: " + value);
    }

}
