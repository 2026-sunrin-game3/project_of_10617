using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[System.Serializable]
public struct AttackRange
{
    public Vector2 offset, size;

    public bool drawGizmos;

    public bool isKarmaAttack;
}

public class PlayerBattle : MonoBehaviour
{

    
    public EntityHealth health;
    public PlayerMovement movement;
    public EntityStat stat;
    public float atkCool;


    public AttackRange defaultAttack;
    
    [SerializeField] LayerMask enemyMask;
    [SerializeField] float dashPower,dashTime;
    [SerializeField] DamageIndicator indicator;
    public bool inDash;
    [SerializeField] Slider healthbar;
    [SerializeField] Image krFill;
    public const int MaxKarma = 40;
    public int karma;
    float karmaTickTimer;
    void Start()
    {
        health = GetComponent<EntityHealth>();
        stat = GetComponent<EntityStat>();
        movement = GetComponent<PlayerMovement>();

        health.OnDamage(OnHurt);
    }
    void OnHurt(EntityHealth.Context ctx){
        if(inDash)
            ctx.canceled = true;
        if(ctx.canceled)
            return;
        indicator.IndicateDamage(ctx.damage,transform.position+new Vector3(0,1),Color.red);
        if (ctx.karmaAmount > 0)
            karma = Mathf.Min(karma + Mathf.CeilToInt(ctx.karmaAmount), MaxKarma);
    }

    void Update(){
        // KARMA: each stack drains 1 HP every 2 seconds, so the combined tick
        // interval shrinks as karma stacks up (and stretches back out as it
        // depletes), giving a fast-then-tapering drain instead of a flat rate.
        if (karma > 0 && health.health > 1)
        {
            karmaTickTimer += Time.deltaTime;
            float tickInterval = 2f / karma;
            if (karmaTickTimer >= tickInterval)
            {
                karmaTickTimer -= tickInterval;
                health.health = Mathf.Max(1, health.health - 1);
                karma--;
            }
        }
        else
        {
            karmaTickTimer = 0;
        }

        healthbar.value = health.health / health.maxHealth;
        if (krFill != null)
            krFill.fillAmount = Mathf.Min(1, (health.health + karma) / health.maxHealth);
        if (atkCool > 0)
            atkCool -= Time.deltaTime*(1+stat.GetResultValue("atkSpeed")/100);
    }
    public void Dash(int direction){
        StartCoroutine(dash_(direction));
    }
    IEnumerator dash_(int direction) {
        inDash = true;  
        movement.SetVelocity(Vector2.right * direction * dashPower);
        yield return new WaitForSeconds(dashTime);
        movement.SetVelocity(Vector2.zero);
        inDash = false;
    } 
    
    public void Attack()
    {
        if (atkCool > 0)
            return;
        atkCool = 0.5f;

        var col = Physics2D.OverlapBoxAll((Vector2)transform.position + defaultAttack.offset,defaultAttack.size,0,enemyMask);

        foreach (var target in col)
        {
            EntityHealth hp = target.GetComponent<EntityHealth>();
            if (hp != null)
            {
                hp.GetDamage(stat.GetResultValue("attackDamage"), health);
            }
        }
    }
    public void Skill1(){
        StartCoroutine(Skill1_());
    }
    IEnumerator Skill1_(){
        var atkspeedBuf = new EntityStat.Buf{
            Key="atkSpeed",
            mathType = MathType.Add,
            Value = 90
        };
        stat.bufs.Add(atkspeedBuf);
        stat.Calc("atkSpeed");
        yield return new WaitForSeconds(5);

        stat.bufs.Remove(atkspeedBuf);
        stat.Calc("atkSpeed");
    }
    void Draw(AttackRange range)
    {
        if (!range.drawGizmos) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + range.offset, range.size);
    }

    void OnDrawGizmos(){
        Draw(defaultAttack);
    }
}