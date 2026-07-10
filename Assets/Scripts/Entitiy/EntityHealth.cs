using UnityEngine;
using System;
using System.Collections.Generic;

public class EntityHealth : MonoBehaviour
{  
    EntityStat stat;

    public float health, maxHealth;

    public bool isDeath;

    public struct Context
    {
        public float damage;
        
        public EntityHealth attacker;
        
        public bool canceled;
    }

    List<Action<Context>> onDamageEv = new();

    List<Action<Context>> onGiveDamageEv = new();

    List<Action<Context>> onDeathEv = new();

    void Start()
    {
        stat = GetComponent<EntityStat>();
        ResetHealth();
        
    }

    public void ResetHealth()
    {
        health = maxHealth;
    }
    
    public void OnDamage(Action<Context> action)
    {
        onDamageEv.Add(action);
    }

    public void OnGiveDamage(Action<Context> action)
    {
        onGiveDamageEv.Add(action);
    }

    public void OnDeath(Action<Context> action)
    {
        onDeathEv.Add(action);
    }


    public void GetDamage(float damage, EntityHealth attacker = null)
    {
        if (isDeath)
        {
            return;
        }

        Context ctx = new Context();
        ctx.damage = damage;
        ctx.attacker = attacker;

        float critPer = 0, critMul = 0, inc = 0;

        foreach (var c in onDamageEv)
        {
            c.Invoke(ctx);
        }

        if (attacker != null)
        {
            critPer = attacker.stat.GetResultValue("critPer");
            critMul = attacker.stat.GetResultValue("critMul");
            inc = attacker.stat.GetResultValue("increaseDamage");
            foreach (var c in attacker.onDamageEv)
            {
                c.Invoke(ctx);
            }
        }

        if (ctx.canceled)
        {
            return;
        }

        float dmg = ctx.damage * (1 + stat.GetResultValue("hurtDamage")/100); 

        if (UnityEngine.Random.Range(0, 100) <= critPer)
            dmg += 1 + critMul/100;

        health -= damage;

        if (health <= 0)
        {
            isDeath = true;

            
            foreach (var c in onDeathEv)
            {
                c.Invoke(ctx);
            }
        }
    }
}
