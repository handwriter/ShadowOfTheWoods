using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwampBlockManager : MonoBehaviour
{
    public bool IsInSwamp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Swamp"))
        {
            IsInSwamp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Swamp"))
        {
            IsInSwamp = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Swamp"))
        {
            IsInSwamp = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Swamp"))
        {
            IsInSwamp = false;
        }
    }
}
