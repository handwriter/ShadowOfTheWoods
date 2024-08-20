using UnityEngine;

public class CandleBehaviour : MonoBehaviour
{
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 2.0f;
    public Color startColor = Color.red;
    public Color endColor = new Color(1.0f, 0.5f, 0.0f); // Orange color

    private Light candleLight;
    private float targetIntensity;
    private Color targetColor;
    private float timeSinceLastFlicker;

    void Start()
    {
        candleLight = GetComponent<Light>();
        candleLight.color = startColor;
        targetIntensity = Random.Range(minIntensity, maxIntensity);
        targetColor = startColor;
        timeSinceLastFlicker = Random.Range(0.0f, 1.0f);
    }

    void Update()
    {
        timeSinceLastFlicker += Time.deltaTime;

        // Check if it's time for a flicker
        if (timeSinceLastFlicker >= flickerSpeed)
        {
            // Randomly change intensity and color
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            targetColor = Color.Lerp(startColor, endColor, Random.value);

            // Reset the timer
            timeSinceLastFlicker = 0.0f;
        }

        // Smoothly lerp intensity and color
        candleLight.intensity = Mathf.Lerp(candleLight.intensity, targetIntensity, Time.deltaTime * 2.0f);
        candleLight.color = Color.Lerp(candleLight.color, targetColor, Time.deltaTime * 2.0f);
    }
}
