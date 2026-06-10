using UnityEngine;
using System.Collections;

public class SwordWeapon : Weapon
{
    private BoxCollider2D hitbox;
    private SpriteRenderer sr;

    public float distance = 1f;
    public float attackTime = 3f;



    void Awake()
    {
        transform.SetParent(null);
        hitbox = GetComponent<BoxCollider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.enabled = false;
        sr.sortingOrder = 10;
        hitbox.enabled = false;
        hitbox.isTrigger = true;
        damage = 25f;
        cooldown = 1f;

    }
    private void Start()
    {
        Debug.Log("WeaponAlive");    
    }

    public override void Attack(Transform origin, Vector2 direction)
    {
        Vector3 spawnPos = origin.position + (Vector3)direction.normalized * distance;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.position = spawnPos;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        sr.enabled = true;

        StopAllCoroutines();
        StartCoroutine(EnableHitboxNextFrame());
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemy_script enemy = collision.GetComponent<enemy_script>();

            if (enemy != null && enemy.is_alive)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy hit for " + damage);
            }
        }
    }
    void ResetAttack()
    {
        hitbox.enabled = false;
        sr.enabled = false;
    }
    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(attackTime);

        ResetAttack();
    }
    IEnumerator EnableHitboxNextFrame()
    {
        yield return new WaitForFixedUpdate(); // 🔥 MAGIA

        hitbox.enabled = true;

        yield return new WaitForSeconds(attackTime);

        ResetAttack();
    }
}