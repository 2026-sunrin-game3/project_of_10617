using System.Collections;
using UnityEngine;

public class GasterBlaster : MonoBehaviour
{
    [SerializeField] float chargeTime = 0.6666667f;
    [SerializeField] float beamDuration = 2f;
    [SerializeField] float damageTickInterval = 1f;
    [SerializeField] Vector2 beamSize = new Vector2(22f, 1.2f);
    [SerializeField] SpriteRenderer beamVisual;
    // Drag this child object (in the prefab, native/unflipped orientation)
    // so it sits exactly on the mouth opening in the Scene view - much more
    // reliable than guessing a numeric offset from screenshots.
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
        // Keep tracking the player for the whole charge-up instead of
        // locking the aim in at spawn time - otherwise, by the time it
        // actually fires ~0.7s later, the boss (still chasing) or the
        // player moving around leaves it aimed at empty air.
        if (!fired)
            UpdateFacing();
    }

    void UpdateFacing()
    {
        fireDir = target.position.x > transform.position.x ? Vector2.right : Vector2.left;

        // Every previous attempt assumed the skull art faces left by
        // default and flipped when firing right - and it was reportedly
        // still backwards each time, so the art's actual default must be
        // snout-right: flip only when firing left instead.
        sprite.flipX = fireDir.x < 0;
    }

    Vector2 MuzzlePosition()
    {
        if (muzzlePoint == null)
            return transform.position;

        // flipX only mirrors the X axis, so mirror the offset's X to match
        // rather than the Y (the mouth's height above/below center doesn't
        // change when flipped left/right).
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
            // Beam.png's pivot is left-center, so at scale 1 it already runs
            // from its position outward along its own +X; just place it at
            // the muzzle, rotate it onto fireDir, and stretch it to beamSize.
            Vector2 nativeSize = beamVisual.sprite.bounds.size;
            beamVisual.transform.position = muzzlePos;
            beamVisual.transform.localScale = new Vector3(beamSize.x / nativeSize.x, beamSize.y / nativeSize.y, 1);
            beamVisual.transform.rotation = Quaternion.Euler(0, 0, angle);
            beamVisual.enabled = true;
        }

        Vector2 center = muzzlePos + fireDir * beamSize.x * 0.5f;

        // Deal damage repeatedly for as long as the beam is out instead of a
        // single instant check - a one-shot check could miss the player
        // entirely if they weren't standing in the box on that exact frame,
        // which is why hits weren't registering before.
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
