using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] Text text;
    
    [SerializeField] float time, floatingScale;

    public static DamageIndicator Instance = null;

    void Start()
    {
        Instance = this;
    }
    
    void Update()
    {
        time += Time.deltaTime;

        transform.Translate(Vector2.up * floatingScale * Time.deltaTime);

        if (time > 0.65f)
        {
            Destroy(gameObject);
        }
    }

    public void IndicateDamage(float damage, Vector2 pos, Color color)
    {
        DamageIndicator indicator = Instantiate(this, pos, Quaternion.identity);
        indicator.text.text = Mathf.Round(damage).ToString();
        indicator.text.color = color;
    }
}
