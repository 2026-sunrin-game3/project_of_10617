using UnityEngine;

// Added to the player at runtime by Boss when it first casts gravity
// manipulation. Launches the player in a fixed direction (on top of, not
// instead of, their normal control), rings them with a blue outline while
// active, and stops the launch on wall contact.
public class PlayerGravityStatus : MonoBehaviour
{
    static readonly Vector2[] OutlineOffsets =
    {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right,
        new Vector2(1, 1).normalized, new Vector2(1, -1).normalized,
        new Vector2(-1, 1).normalized, new Vector2(-1, -1).normalized,
    };

    [SerializeField] Color outlineColor = new Color(0.2f, 0.5f, 1f, 1f);
    [SerializeField] float outlineThickness = 0.12f;

    // Added at runtime (never present in the scene file), so there is no
    // Inspector to configure this in - resolve the ground/wall layer by name
    // instead of relying on a serialized mask nobody can ever set.
    LayerMask wallMask;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    SpriteRenderer[] outlineRenderers;

    public bool IsActive { get; private set; }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        wallMask = LayerMask.GetMask("ground");

        outlineRenderers = new SpriteRenderer[OutlineOffsets.Length];
        for (int i = 0; i < OutlineOffsets.Length; i++)
        {
            GameObject outlineObj = new GameObject("GravityOutline_" + i);
            outlineObj.transform.SetParent(transform, false);
            outlineObj.transform.localPosition = OutlineOffsets[i] * outlineThickness;

            SpriteRenderer renderer = outlineObj.AddComponent<SpriteRenderer>();
            renderer.color = outlineColor;
            renderer.sortingLayerID = sprite.sortingLayerID;
            renderer.sortingOrder = sprite.sortingOrder - 1;
            renderer.enabled = false;
            outlineRenderers[i] = renderer;
        }
    }

    void Update()
    {
        if (!IsActive)
            return;

        foreach (var renderer in outlineRenderers)
        {
            renderer.sprite = sprite.sprite;
            renderer.flipX = sprite.flipX;
        }
    }

    public void ApplyGravityLaunch(Vector2 direction, float speed)
    {
        IsActive = true;
        SetOutlineEnabled(true);
        rigid.linearVelocity = direction.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsActive)
            return;

        if (((1 << collision.gameObject.layer) & wallMask.value) != 0)
            EndGravityLaunch();
    }

    void EndGravityLaunch()
    {
        IsActive = false;
        SetOutlineEnabled(false);
        rigid.linearVelocity = Vector2.zero;
    }

    void SetOutlineEnabled(bool value)
    {
        foreach (var renderer in outlineRenderers)
            renderer.enabled = value;
    }
}
