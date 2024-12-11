using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomovoyController : MonoBehaviour
{
    public int MaxAttackCount;
    public int AttackCount;
    public bool IsInLightZone = false;
    public bool PlayerIsUnavalilable = false;
    public TransparentMaterialsManager TransparentMaterialsManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FlashlightZone"))
        {
            IsInLightZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("FlashlightZone"))
        {
            IsInLightZone = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("FlashlightZone"))
        {
            IsInLightZone = true;
        }
    }
}
