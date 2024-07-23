using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LeshiyEnemyController : MonoBehaviour
{
    public float MinRandomRunningDistance;
    public float MaxRandomRunningDistance;
    public float IdleTimeout;

    private Rigidbody _rgBody;
    private NavMeshAgent _navMeshAgent;

    private Vector3 _targetAgentPoint;
    private bool _isPointValid = false;

    private void Awake()
    {
        _rgBody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetupRandomTargetPoint()
    {
        int randomSignX = Random.Range(0, 2) == 0 ? -1 : 1;
        int randomSignZ = Random.Range(0, 2) == 0 ? -1 : 1;
        Vector2 targetPoint = new Vector3(transform.position.x + randomSignX * Random.Range(MinRandomRunningDistance, MaxRandomRunningDistance),
            transform.position.z + randomSignZ * Random.Range(MinRandomRunningDistance, MaxRandomRunningDistance));
        RaycastHit groundHit;
        if (Physics.Raycast(new Vector3(targetPoint.x, 100, targetPoint.y), transform.TransformDirection(Vector3.down), out groundHit, Mathf.Infinity))
        {
            Debug.DrawRay(new Vector3(targetPoint.x, 100, targetPoint.y), transform.TransformDirection(Vector3.down) * groundHit.distance, Color.yellow);

            _targetAgentPoint = groundHit.transform.position;
            Debug.Log("New point setup");
            GameObject newObj = new GameObject();
            newObj.transform.position = new Vector3(targetPoint.x, 100, targetPoint.y);
            _isPointValid = true;
        }
    }

    void Update()
    {
        if (!_isPointValid) SetupRandomTargetPoint();
        else
        {
            _navMeshAgent.destination = _targetAgentPoint;
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_targetAgentPoint.x, _targetAgentPoint.z)) <= 1)
            {
                _isPointValid = false;
            }
        }
        Debug.Log($"Magn: {_rgBody.velocity.magnitude}");
    }
}
