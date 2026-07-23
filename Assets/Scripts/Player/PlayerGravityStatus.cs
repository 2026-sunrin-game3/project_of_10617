using UnityEngine;

// Added to the player at runtime by Boss when it first casts gravity
// manipulation. Overrides normal movement with a launch in a fixed
// direction, tints an outline blue while active, and stops on wall contact.
public class PlayerGravityStatus : MonoBehaviour
{
    [SerializeField] Color outlineColor = new Color(0.2f, 0.5f, 1f, 1f);
    [SerializeField] float outlineScale = 1.15f;
    [SerializeField] LayerMask wallMask;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    PlayerMovement movement;
    SpriteRenderer outlineRenderer;

    public bool IsActive { get; private set; }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        movement = GetComponent<PlayerMovement>();

        GameObject outlineObj = new GameObject("GravityOutline");
        outlineObj.transform.SetParent(transform, false);
        outlineObj.transform.localScale = Vector3.one * outlineScale;

        outlineRenderer = outlineObj.AddComponent<SpriteRenderer>();
        outlineRenderer.color = outlineColor;
        outlineRenderer.sortingLayerID = sprite.sortingLayerID;
        outlineRenderer.sortingOrder = sprite.sortingOrder - 1;
        outlineRenderer.enabled = false;
    }

    void Update()
    {
        if (!IsActive)
            return;

        outlineRenderer.sprite = sprite.sprite;
        outlineRenderer.flipX = sprite.flipX;
    }

    public void ApplyGravityLaunch(Vector2 direction, float speed)
    {
        IsActive = true;
        outlineRenderer.enabled = true;
        movement.enabled = false;
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
        outlineRenderer.enabled = false;
        rigid.linearVelocity = Vector2.zero;
        movement.enabled = true;
    }
}
