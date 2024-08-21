using System;
using System.Collections;
using System.Collections.Generic;
using UHFPS.Input;
using UnityEngine;

public class FastTeleportManager : MonoBehaviour
{
    public Transform[] PointsToTeleport;

    public void Teleport(int index)
    {
        transform.position = PointsToTeleport[index].position;
    }
}
