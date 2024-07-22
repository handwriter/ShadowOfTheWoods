using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodes_System : MonoBehaviour
{
    [Tooltip ("List of GameObjects with the 'Node' script. The sequence establishes the path")]
    public Node[] nodes;
    public enum moveNode { Lerp = 0, MoveTowards = 1, Slerp = 2 }
    [Tooltip("How the movingObject moves. MoveTowards is set to default.")]
    public moveNode movementNodes = moveNode.MoveTowards;
    [Tooltip ("The object to move")]
    public Transform movingObject;
    [Tooltip("The speed in which the object moves")]
    public float speedMovement = 1;
    [Range (0.0F, 0.5F)]
    [Tooltip("The radius of curvature. The greater this value, the more the object rotates near the node")]
    public float curvature = 0;
    [Tooltip("The greater this value, the more the object slows down near the node")]
    [Range(1.0F, 4.0F)]
    public float damping = 2;

    int currentNodeIndex;
    float currentDistance0;
    float currentDistance1;
    float currentDistance2;
    Vector3 currentPos;
    Vector3 reference;

    // Start is called before the first frame update
    void Start()
    {
        if (nodes.Length > 1) {
            currentNodeIndex = 0;
            currentDistance1 = Vector3.Distance(movingObject.position, nodes[currentNodeIndex].transform.position);
            currentPos = nodes[0].transform.position;
            currentDistance0 = Vector3.Distance(movingObject.position, currentPos);
            if (currentDistance0 < 0.05F) {
                currentNodeIndex = 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (nodes.Length > 1) {
            if (movementNodes == moveNode.Lerp) {
                movingObject.position = Vector3.Lerp(movingObject.position, currentPos, Time.deltaTime * speedMovement * (currentDistance0 / damping));
            } else if (movementNodes == moveNode.MoveTowards) {
                movingObject.position = Vector3.MoveTowards(movingObject.position, currentPos, Time.deltaTime * speedMovement * (currentDistance0 / damping));
            } else if (movementNodes == moveNode.Slerp) {
                movingObject.position = Vector3.Slerp(movingObject.position, currentPos, Time.deltaTime * speedMovement * (currentDistance0 / damping));
            }
            currentDistance0 = Vector3.Distance(movingObject.position, currentPos);
            currentDistance1 = Vector3.Distance(currentPos, nodes[currentNodeIndex % nodes.Length].transform.position);
            currentDistance2 = Vector3.Distance(nodes[currentNodeIndex % nodes.Length].transform.position, nodes[(currentNodeIndex + 1) % nodes.Length].transform.position);
            currentPos = Vector3.SmoothDamp(currentPos, nodes[currentNodeIndex % nodes.Length].transform.position, ref reference, 1 / (speedMovement + 1 + currentDistance0), speedMovement / (currentDistance0 + 1));
            if (currentDistance1 <= ((currentDistance2 * curvature) + 0.05F)) {
                if (currentNodeIndex < nodes.Length - 1) {
                    currentNodeIndex += 1;
                } else {
                    currentNodeIndex = 0;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.cyan;
        if (nodes.Length > 2) {
            for (int i = 0; i < nodes.Length; i++) {
                if (nodes[i % nodes.Length] != null && nodes[(i + 1) % nodes.Length] != null) {
                    Gizmos.DrawLine(nodes[i % nodes.Length].transform.position, nodes[(i + 1) % nodes.Length].transform.position);
                }
            }
        } else if (nodes.Length == 2) {
            for (int i = 0; i < nodes.Length - 1; i++) {
                if (nodes[i] != null && nodes[i + 1] != null) {
                    Gizmos.DrawLine(nodes[i].transform.position, nodes[i + 1].transform.position);
                }
            }
        }
    }
}
