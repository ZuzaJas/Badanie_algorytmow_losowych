using UnityEngine;

public class enemy_script : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TileManager Tmanager;

    public Vector2 RoomPos;
    public Tile ResidentRoom;
    public bool is_alive = true;
    public bool is_active = false;
    public Vector2 velocity;
    public float health = 100;
    private SpriteRenderer sr;
    private BoxCollider2D EnemyCollider;


    //warunki graniczne poruszania siê
    private float leftborder;
    private float rightborder;
    private float upborder;
    private float downborder;

    void Start()
    {
        RoomPos = Vector2.zero;
        //velocity = new Vector2(
        //  UnityEngine.Random.Range(-3.0f, 3.0f),
        //UnityEngine.Random.Range(-3.0f, 3.0f)
        //);
        velocity = new Vector2(-1f, 1f); //na razie by by³y w jedn¹ stronê dla testów
        sr = GetComponent<SpriteRenderer>();
        EnemyCollider = GetComponent<BoxCollider2D>();
        EnemyCollider.size = sr.bounds.size;

    }
    
    void Update()
    {
        if (is_active && is_alive) {
            Move();
        }
    }

    
    public void SetPosition(Vector3 pos) {
        transform.position = new Vector3(pos.x, pos.y, pos.z);
    }

    public void MakeActive() {
        if (is_alive)
        {
            is_active = true;
        }
    }
    public void setRoomPos(Vector2 room_data) { 
        RoomPos = room_data;
        
            leftborder = RoomPos.x * Tmanager.tileWidth - Tmanager.tileWidth * 0.5f; //null to roomData
            rightborder = leftborder + Tmanager.tileWidth;
            downborder = RoomPos.y * Tmanager.tileHeight - Tmanager.tileHeight * 0.5f;
            upborder = downborder + Tmanager.tileHeight;
    

}
    public void SetRoom(Tile CurRoom) { 
        ResidentRoom = CurRoom;
    }
    public void SetManager(TileManager Tmana) {
        
        Tmanager = Tmana;
        
    }
    private void Move()
    {
       
        float posX = transform.position.x;
        float posY = transform.position.y;
        if (posX + velocity.x < leftborder || posX + velocity.x > rightborder) { velocity.x *= -1f; }
        if (posY + velocity.y < downborder || posY + velocity.y > upborder) { velocity.y *= -1f; }
        transform.position += new Vector3(velocity.x*Time.deltaTime, velocity.y*Time.deltaTime, 0f);
    }

    public void TakeDamage(float ammount) { 
           health -= ammount;
        
        if (health <= 0f) { MakeDead(); }

    
    }


    private void MakeDead() {
        is_alive = false;
        //ca³e wy³¹czenie Sprite i zwolnienie pamiêci maybe
        sr.enabled = false;
    }
}
