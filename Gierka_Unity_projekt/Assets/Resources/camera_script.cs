using UnityEngine;
using UnityEngine.SceneManagement;


public class camera_script : MonoBehaviour
{

    public float moveSpeed = 10f;
    public float zoomSpeed = 10f;

    public Transform target; //mój player
    private Camera cam;
    public TileManager Tmanager;
    

    private float roomWidth;
    private float roomHeight;


    void Start()
    {
        roomWidth = 1f;
        roomHeight = 1f;

        currentRoom = GetRoomIndex(target.position);
        targetPos = GetRoomCenter(currentRoom);

        transform.position = targetPos;

        
        
    }
    Vector2 currentRoom;
    Vector3 targetPos;

    void LateUpdate()
    {
        Vector2 room = GetRoomIndex(target.position);

        if (room != currentRoom)
        {
            currentRoom = room;
            targetPos = GetRoomCenter(room);
            
        }
        
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * 5f
        );
    }

    public void GetRoomHeight(float mRoomH, float mRoomW)
    {
        roomWidth = mRoomH;
        roomHeight = mRoomW;
    }
    bool testik = false;
    Vector2 GetRoomIndex(Vector3 playerPos)
    {
        int x = Mathf.FloorToInt((playerPos.x + (roomWidth / 2f)) / roomWidth);
        int y = Mathf.FloorToInt((playerPos.y + (roomHeight / 2f)) / roomHeight);

        //Debug.Log("RoomWidth ="+ playerPos.x / roomWidth);
        //Debug.Log("RoomHeight =" + playerPos.y / roomHeight);
        return new Vector2(x, y);
    }
    Vector3 GetRoomCenter(Vector2 roomIndex)
    {
        float x = roomIndex.x * roomWidth;
        float y = roomIndex.y * roomHeight;

        return new Vector3(x, y, -10f);
    }
    public void GoBackButton()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
