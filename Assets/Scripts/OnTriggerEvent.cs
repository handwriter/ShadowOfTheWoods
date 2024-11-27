using UnityEngine;
using UnityEngine.Events;

public class ColliderEventTrigger : MonoBehaviour
{
    public UnityEvent OnColliderEnter; // ������� ��� ����� � ���������
    public UnityEvent OnColliderExit;   // ������� ��� ������ �� ����������

    private void OnTriggerEnter(Collider other)
    {
        if (OnColliderEnter != null)
        {
            OnColliderEnter.Invoke();
        }
        Debug.Log("Collider entered: " + other.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (OnColliderExit != null)
        {
            OnColliderExit.Invoke();
        }
        Debug.Log("Collider exited: " + other.gameObject.name);
    }
}
