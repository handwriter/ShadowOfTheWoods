using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMesh : MonoBehaviour
{
    [SerializeField] private NavMeshAgent NavMeshAgent;
    [SerializeField] private GameObject targetPoint;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        NavMeshAgent.destination = targetPoint.transform.position;
    }
}
