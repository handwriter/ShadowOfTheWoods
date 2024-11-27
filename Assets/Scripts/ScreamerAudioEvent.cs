using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    public AudioClip[] soundClips; // ������ ������, ��������� ��� �������������

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource �� ������! �������� AudioSource � �������.");
        }
    }

    /// <summary>
    /// ������������� ���� �� ������� �� ������� soundClips.
    /// </summary>
    /// <param name="soundIndex">������ ����� � ������� soundClips.</param>
    public void PlaySoundByIndex(int soundIndex)
    {
        if (soundIndex >= 0 && soundIndex < soundClips.Length)
        {
            audioSource.PlayOneShot(soundClips[soundIndex]);
        }
        else
        {
            Debug.LogWarning($"������ ����� {soundIndex} ��� ��������� ������� soundClips.");
        }
    }

    /// <summary>
    /// �������� ��������� ������� � ����������.
    /// </summary>
    /// <param name="functionName">�������� �������, ������� ����� �������.</param>
    /// <param name="parameter">��������� ��������, ������������ � �������.</param>
    public void InvokeFunction(string functionName, string parameter)
    {
        var method = GetType().GetMethod(functionName);
        if (method != null)
        {
            method.Invoke(this, new object[] { parameter });
        }
        else
        {
            Debug.LogWarning($"������� � ������ {functionName} �� �������.");
        }
    }

    /// <summary>
    /// ������ ���������������� �������, ������� ����� ���� ������� �� ��������.
    /// </summary>
    /// <param name="message">���������, ������������ � �������.</param>
    public void CustomMessage(string message)
    {
        Debug.Log($"CustomMessage ������� � ����������: {message}");
    }
}
