using UnityEngine;
using UnityEngine.UI;

public class MicIconAnimator : MonoBehaviour {
    [SerializeField] private float pulseSpeed = 2f;       
    [SerializeField] private float pulseScale = -0.3f;   
    [SerializeField] private float fadeAmount = 0.8f;    

    private Vector3 baseScale;
    private Image img;

    void Start() {
        baseScale = transform.localScale;
        img = GetComponent<Image>();
    }

    void Update() {
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        transform.localScale = baseScale * (1f + pulse * pulseScale);

        // Fade animation
        Color c = img.color;
        c.a = 1f - pulse * fadeAmount;
        img.color = c;
    }
}