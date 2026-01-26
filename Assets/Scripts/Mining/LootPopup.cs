using UnityEngine;
using TMPro;
using System.Linq;

public class LootPopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float riseSpeed = 1.5f;
    public float lifetime = 1.0f;
    public float fadeDuration = 0.8f;
    public AnimationCurve riseCurve;
    public AnimationCurve fadeCurve;

    private Vector3 startPos;
    private float timer;
    private CanvasGroup canvasGroup;
    private RectTransform popupRect; 
    private Vector2 startAnchoredPos;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        popupRect = GetComponent<RectTransform>();
    }

    public void Initialize(string message, Color color)
    {
        text.color = color;
        text.text = message;

        //startPos = transform.position;
        startAnchoredPos = popupRect.anchoredPosition;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifetime;

        // Rise with curve
        float riseAmount = riseCurve.Evaluate(t);
        //transform.position = startPos + Vector3.up * riseAmount;
        popupRect.anchoredPosition = startAnchoredPos + Vector2.up * riseAmount;


        // Fade with curve
        float fade = fadeCurve.Evaluate(t);
        canvasGroup.alpha = fade;
        //Debug.Log($"t={t}, rise={riseAmount}, fade={fade}");


        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
