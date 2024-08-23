using System.Collections;
using System.Collections.Generic;
using UHFPS.Runtime;
using UnityEngine;

public class BoatController : MonoBehaviour, IFuelConsumer, IInteractStart
{
    public float MaxFuel;
    public float MaxFuelLiters { get => MaxFuel; set => MaxFuel = value; }
    public float CurrentFuelLiters { get; set; }

    public bool isKeyActive = false;
    public Transform TapPoint;
    public Transform Player;

    public void SetupKey()
    {
        isKeyActive = true;
    }

    public void InteractStart()
    {
        if (isKeyActive && MaxFuel == CurrentFuelLiters)
        {
            StartCoroutine(TapPlayer());
        }
    }

    public IEnumerator TapPlayer()
    {
        yield return GameManager.Instance.StartBackgroundFade(false);
        Player.position = TapPoint.position;
        yield return GameManager.Instance.StartBackgroundFade(true, waitTime: 1);
    }

    public void Refuel(float liters)
    {
        CurrentFuelLiters += liters;
    }
}
