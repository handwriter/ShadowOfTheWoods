using UnityEngine;

public class ScreamerAudioEvent : MonoBehaviour
{
    public AudioClip SoundClip; // AudioClip в инспекторе
    public AudioSource audioSource;

    private void Start()
    {
        // Важно: назначить AudioSource в инспекторе.
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }
    // Например, для события OnStateEnter
    void OnAnimationEvent(AnimationEvent e)
    {
        if (e.stringParameter == "PlaySound") // Убедитесь что имя события в анимации совпадает
        {
            if (SoundClip != null)
            {
                audioSource.PlayOneShot(SoundClip);
            }
            else
            {
                Debug.LogWarning("Аудиоклип не задан!");
            }
        }
    }
}