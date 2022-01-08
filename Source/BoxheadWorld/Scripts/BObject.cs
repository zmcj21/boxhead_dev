using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum BObjectType
{
    Actor = 1,
    MapObject,
    MapCellObject,
}

public class ExtraHitInfo
{
    public Vector3 hitPoint;
    public GameObject attacker;

    public ExtraHitInfo()
    {
        hitPoint = Vector3.zero;
        attacker = null;
    }

    public ExtraHitInfo(Vector3 hitPoint, GameObject attacker)
    {
        this.hitPoint = hitPoint;
        this.attacker = attacker;
    }

    public static implicit operator bool(ExtraHitInfo exists)
    {
        if (exists != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public abstract class BObject : MonoBehaviour
{
    private BObjectType objectType;

    private float health;

    private float maxHealth;

    private bool isDead;

    private bool isInvincible;

    public BObjectType ObjectType
    {
        get => objectType;
        set => objectType = value;
    }

    public float Health
    {
        get => health;
        set => health = value;
    }

    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    public bool IsDead
    {
        get => isDead;
        set => isDead = value;
    }

    public bool Alive
    {
        get => !isDead;
        set => isDead = !value;
    }

    public bool IsInvincible
    {
        get => isInvincible;
        set => isInvincible = value;
    }

    public Vector2 PositionOnPlane
    {
        get => new Vector2(transform.position.x, transform.position.z);
        set => transform.position = new Vector3(value.x, transform.position.y, value.y);
    }

    public Vector3 PositionOnPlane3D
    {
        get => transform.position;
        set => transform.position = new Vector3(value.x, transform.position.y, value.z);
    }

    public void InitBObject(BObjectType objectType, float maxHealth)
    {
        ObjectType = objectType;
        Alive = true;
        MaxHealth = maxHealth;
        Health = MaxHealth;
        WhenImBorn(Health);
    }

    public void Respawn(float curHealth)
    {
        Alive = true;
        Health = curHealth;
        WhenImBorn(Health);
    }

    public void SetInvincible(float time)
    {
        IEnumerator Delay()
        {
            IsInvincible = true;
            WhenInvincibleChange(time);
            yield return new WaitForSeconds(time);
            IsInvincible = false;
            WhenInvincibleChange(0);
        }
        StartCoroutine(Delay());
    }

    public void Kill(ExtraHitInfo hitInfo = null)
    {
        if (IsDead) return;

        float damage = Health;
        Health = 0;
        IsDead = true;
        WhenImHurt(damage, hitInfo);
        WhenImDead(hitInfo);
    }

    public void BeHit(float damage, ExtraHitInfo hitInfo = null)
    {
        if (IsInvincible) return;
        if (IsDead) return;

        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            IsDead = true;
            WhenImHurt(damage, hitInfo);
            WhenImDead(hitInfo);
        }
        else
        {
            WhenImHurt(damage, hitInfo);
        }
    }

    public void Heal(float heal)
    {
        if (IsDead) return;

        Health += heal;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        WhenGetHeal(heal);
    }

    protected virtual void WhenInvincibleChange(float time) { }

    protected virtual void WhenImHurt(float damage, ExtraHitInfo hitInfo) { }

    protected virtual void WhenImDead(ExtraHitInfo hitInfo) { }

    protected virtual void WhenGetHeal(float heal) { }

    protected virtual void WhenImBorn(float curHealth) { }
}