using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UHFPS.Runtime;
using System.Reactive.Subjects;

public class SwampController : MonoBehaviour, IPowerConsumer
{
    [field: SerializeField]
    public float ConsumeWattage { get; set; }
    public float targetYPosition;
    private bool powerState;
    private bool valveState;
    private float _yVelocity;
    public BehaviorSubject<bool> IsTurnedOn { get; set; } = new(false);

    public void OnPowerState(bool state)
    {
        powerState = state;
    }

    public void SetupValve()
    {
        valveState = true;
    }

    void Update()
    {
        if (powerState && valveState)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.SmoothDamp(transform.localPosition.y, targetYPosition, ref _yVelocity, 5f), transform.localPosition.z);
        }
    }
}
