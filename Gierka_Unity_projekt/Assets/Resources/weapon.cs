using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float damage;
    public float cooldown;

    protected float lastAttackTime;

    protected bool CanAttack()
    {
        if (Time.time < lastAttackTime + cooldown)
            return false;

        lastAttackTime = Time.time;
        return true;
    }

    public abstract void Attack(Transform origin, Vector2 direction);
}