using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFuelConsumer
{
    float MaxFuelLiters { get; set; }
    float CurrentFuelLiters { get; set; }

    public void Refuel(float liters);
}
