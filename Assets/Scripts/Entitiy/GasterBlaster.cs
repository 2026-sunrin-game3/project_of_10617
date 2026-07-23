using System.Collections;
using UnityEngine;

public class GasterBlaster : MonoBehaviour
{
    [SerializeField] float chargeTime = 0.6666667f;
    [SerializeField] float beamDuration = 1.5f;
    [SerializeField] float damageTickInterval = 1f;
    [SerializeField] Vector2 beamSize = new Vector2(22f, 1.2f);
    [SerializeField] SpriteRenderer beamVisual;
    [SerializeField] Transform muzzlePoint;

    Transform target;
    Vector2 fireDir = Vector2.right;
    float damage;
    EntityHealth attacker;
    LayerMask targetMask;
    SpriteRenderer sprite;
    bool fired;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Init(Transform playerTarget, float dmg, EntityHealth attackerHealth, LayerMask mask)
    {
        target = playerTarget;
        damage = dmg;
        attacker = attackerHealth;
        targetMask = mask;

        UpdateFacing();
        StartCoroutine(FireRoutine());
    }

    void Update()
    {
        if (!fired)
            UpdateFacing();
    }

    void UpdateFacing()
    {
        fireDir = target.position.x > transform.position.x ? Vector2.right : Vector2.left;

        sprite.flipX = fireDir.x < 0;
    }

    Vector2 MuzzlePosition()
    {
        if (muzzlePoint == null)
            return transform.position;

        Vector2 localOffset = muzzlePoint.localPosition;
        Vector2 facingOffset = new Vector2(sprite.flipX ? -localOffset.x : localOffset.x, localOffset.y);
        return (Vector2)transform.position + facingOffset;
    }

    IEnumerator FireRoutine()
    {
        yield return new WaitForSeconds(chargeTime);
        fired = true;

        float angle = Mathf.Atan2(fireDir.y, fireDir.x) * Mathf.Rad2Deg;
        Vector2 muzzlePos = MuzzlePosition();

        if (beamVisual != null)
        {
            Vector2 nativeSize = beamVisual.sprite.bounds.size;
            beamVisual.transform.position = muzzlePos;
            beamVisual.transform.localScale = new Vector3(beamSize.x / nativeSize.x, beamSize.y / nativeSize.y, 1);
            beamVisual.transform.rotation = Quaternion.Euler(0, 0, angle);
            beamVisual.enabled = true;
        }

        Vector2 center = muzzlePos + fireDir * beamSize.x * 0.5f;

        float elapsed = 0f;
        while (elapsed < beamDuration)
        {
            var hits = Physics2D.OverlapBoxAll(center, beamSize, angle, targetMask);
            foreach (var hit in hits)
            {
                EntityHealth hp = hit.GetComponent<EntityHealth>();
                if (hp != null)
                    hp.GetDamage(damage, attacker);
            }

            yield return new WaitForSeconds(damageTickInterval);
            elapsed += damageTickInterval;
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (muzzlePoint == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(muzzlePoint.position, 0.1f);
    }
}
