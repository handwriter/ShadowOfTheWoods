using UnityEngine;

public class ScreamerAudioEvent : MonoBehaviour
{
    public AudioClip SoundClip; // AudioClip � ����������
    public AudioSource audioSource;

    private void Start()
    {
        // �����: ��������� AudioSource � ����������.
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }
    // ��������, ��� ������� OnStateEnter
    void OnAnimationEvent(AnimationEvent e)
    {
        if (e.stringParameter == "PlaySound") // ��������� ��� ��� ������� � �������� ���������
        {
            if (SoundClip != null)
            {
                audioSource.PlayOneShot(SoundClip);
            }
            else
            {
                Debug.LogWarning("��������� �� �����!");
            }
        }
    }
}