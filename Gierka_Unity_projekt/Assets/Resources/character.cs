using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class character : MonoBehaviour
{
    public float moveSpeed = 10f;
    
    private SpriteRenderer sr;
    private BoxCollider2D CharCollider;

    //weapons
    public SwordWeapon swordWeapon1;
    public List<Weapon> weapons;
    private Weapon chosen_weapon;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        CharCollider = GetComponent<BoxCollider2D>();
        CharCollider.size = sr.bounds.size;
        
        //obs³uga broni
        weapons.Add(swordWeapon1);
        chosen_weapon = weapons[0]; //tem dla testów: do dodania opcja wyboru broni

    }
        

    // Update is called once per frame
    void Update()
    {
        // przesuwanie kamer¹ WASD
        float moveX = Input.GetAxis("Horizontal");
        if (moveX < 0)
        {
            sr.flipX = true;
        }
        else { 
            sr.flipX = false;
        }
            float moveY = Input.GetAxis("Vertical");

        transform.position += new Vector3(moveX, moveY, 0) * moveSpeed * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadSceneAsync(0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector2 dir = (mousePos - transform.position).normalized;

            chosen_weapon.Attack(transform, dir);
        }

    }

    public void SetStartPos(Vector3 FirstTile) {
        transform.position = new Vector3(FirstTile.x, FirstTile.y, FirstTile.z);
    }


    
}
