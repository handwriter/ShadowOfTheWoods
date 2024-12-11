using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UHFPS.Runtime;
using Unity.VisualScripting;
using UnityEngine;

public class LeshyController : MonoBehaviour
{
    [SerializeField] private float _pointCloseDistance; 
    [HideInInspector] public GameObject TargetPoint;
    [HideInInspector]public bool IsLostSafeZone = false;
    [SerializeField] private GameObject[] _stuckPoints;
    public bool IsInLightZone = false;

    public GameObject GetTargetPoint()
    {
        if (!TargetPoint) GenerateNewTargetPoint();
        return TargetPoint;
    }

    public void GenerateNewTargetPoint()
    {
        TargetPoint = new GameObject();
        TargetPoint.transform.position = PlayerManager.Instance.CalculateRandomPointInSafeArea();
    }

    public Vector3 GetRandomStuckPoint()
    {
        GameObject target = _stuckPoints[Random.Range(0, _stuckPoints.Length)];
        return PlayerManager.Instance.CalculateRayPoint(target.transform.position, Vector3.down);
    }

    private void Update()
    {
        if (!TargetPoint) GenerateNewTargetPoint();
        if (!PlayerManager.Instance.IsPointInSafeZone(TargetPoint.transform))
        {
            Destroy(TargetPoint.gameObject);
            IsLostSafeZone = true;
            return;
        }
        if (Vector3.Distance(transform.position, TargetPoint.transform.position) <= _pointCloseDistance)
        {
            Destroy(TargetPoint.gameObject);
            IsLostSafeZone = true;
            return;
        }
    }

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
