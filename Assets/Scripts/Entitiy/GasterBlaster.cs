using System.Collections;
using UnityEngine;

public class GasterBlaster : MonoBehaviour
{
    [SerializeField] float chargeTime = 0.6f;
    [SerializeField] float fireTime = 0.3f;
    [SerializeField] Vector2 beamSize = new Vector2(22f, 1.2f);
    [SerializeField] SpriteRenderer beamVisual;
    // Where the mouth actually opens, in the skull art's native (unflipped,
    // snout-left) local space, relative to the sprite's center pivot - so
    // the beam starts from inside the jaw instead of the skull's center.
    [SerializeField] Vector2 muzzleOffset = new Vector2(-1.15f, -0.57f);

    Vector2 fireDir;
    float damage;
    EntityHealth attacker;
    LayerMask targetMask;
    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Init(Vector2 direction, float dmg, EntityHealth attackerHealth, LayerMask mask)
    {
        fireDir = direction.normalized;
        damage = dmg;
        attacker = attackerHealth;
        targetMask = mask;

        // The skull art faces left (snout toward -X) by default. A 180-degree
        // rotation would mirror it AND flip it upside down (a Z rotation is a
        // point reflection, not a mirror), so use flipX to mirror left/right
        // only, and keep the beam's own hitbox oriented off fireDir directly
        // rather than off the transform, which never actually rotates.
        sprite.flipX = fireDir.x > 0;

        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        yield return new WaitForSeconds(chargeTime);

        float angle = Mathf.Atan2(fireDir.y, fireDir.x) * Mathf.Rad2Deg;

        // flipX only mirrors the X axis, so mirror the offset's X to match
        // rather than the Y (the mouth's height above/below center doesn't
        // change when flipped left/right).
        Vector2 facingOffset = new Vector2(sprite.flipX ? -muzzleOffset.x : muzzleOffset.x, muzzleOffset.y);
        Vector2 muzzlePos = (Vector2)transform.position + facingOffset;

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
        var hits = Physics2D.OverlapBoxAll(center, beamSize, angle, targetMask);
        foreach (var hit in hits)
        {
            EntityHealth hp = hit.GetComponent<EntityHealth>();
            if (hp != null)
                hp.GetDamage(damage, attacker);
        }

        yield return new WaitForSeconds(fireTime);
        Destroy(gameObject);
    }
}
