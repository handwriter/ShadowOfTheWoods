using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustStormController : MonoBehaviour
{
    private bool[] _idolCheckStates = new bool[] { false, false, false }; 
    public void IdolCheckedOut(int idolIndex)
    {
        _idolCheckStates[idolIndex] = true;
    }

    private void Update()
    {
        if (_idolCheckStates[0] && _idolCheckStates[1] && _idolCheckStates[2])
        {
            GetComponent<ParticleSystem>().Stop();
        }
    }
}
