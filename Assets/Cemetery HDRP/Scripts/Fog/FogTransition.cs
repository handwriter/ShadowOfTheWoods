using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogTransition : MonoBehaviour
{
    [Tooltip ("The color of the fog at the end of the transition")]
    public Color colorFog = Color.black;
    [Tooltip("The distance in which fog starts to fade in at the end of the transition (it works only in Linear Mode)")]
    public float distanceStart = 20;
    [Tooltip("The distance in which fog ends to fade in at the end of the transition (it works only in Linear Mode)")]
    public float distanceEnd = 60;
    [Tooltip ("The speed of the transition")]
    public float speedTransition = 2;

    private bool inside;
    private float t0;
    private float t1;
    private float t2;
    private bool f0;
    private bool f1;
    private bool f2;

    void Start()
    {
        inside = false;
        t0 = 0;
        t1 = 0;
        t2 = 0;
        f0 = false;
        f1 = false;
        f2 = false;
    }

    void Update()
    {
        if (inside == true) {
            t0 += Time.deltaTime * speedTransition;
            t1 += Time.deltaTime * speedTransition;
            t2 += Time.deltaTime * speedTransition;
            if (RenderSettings.fogColor != colorFog) {
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, colorFog, t0);
            } else {
                f0 = true;
            }
            if (RenderSettings.fogStartDistance != distanceStart) {
                RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, distanceStart, t1);
            } else {
                f1 = true;
            }
            if (RenderSettings.fogEndDistance != distanceEnd) {
                RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, distanceEnd, t2);
            } else {
                f2 = true;
            }
            if (f0 == true && f1 == true && f2 == true) {
                inside = false;
                f0 = false;
                f1 = false;
                f2 = false;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (inside == false) {
            if (other.tag == "Player") {
                inside = true;
                t0 = 0;
                t1 = 0;
                t2 = 0;
            }
        }
    }
}
