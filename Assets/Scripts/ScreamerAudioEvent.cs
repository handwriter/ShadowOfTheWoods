using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    public AudioClip[] soundClips; // Массив звуков, доступных для использования

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource не найден! Добавьте AudioSource к объекту.");
        }
    }

    /// <summary>
    /// Воспроизводит звук по индексу из массива soundClips.
    /// </summary>
    /// <param name="soundIndex">Индекс звука в массиве soundClips.</param>
    public void PlaySoundByIndex(int soundIndex)
    {
        if (soundIndex >= 0 && soundIndex < soundClips.Length)
        {
            audioSource.PlayOneShot(soundClips[soundIndex]);
        }
        else
        {
            Debug.LogWarning($"Индекс звука {soundIndex} вне диапазона массива soundClips.");
        }
    }

    /// <summary>
    /// Вызывает указанную функцию с параметром.
    /// </summary>
    /// <param name="functionName">Название функции, которую нужно вызвать.</param>
    /// <param name="parameter">Строковый параметр, передаваемый в функцию.</param>
    public void InvokeFunction(string functionName, string parameter)
    {
        var method = GetType().GetMethod(functionName);
        if (method != null)
        {
            method.Invoke(this, new object[] { parameter });
        }
        else
        {
            Debug.LogWarning($"Функция с именем {functionName} не найдена.");
        }
    }

    /// <summary>
    /// Пример пользовательской функции, которая может быть вызвана из анимации.
    /// </summary>
    /// <param name="message">Сообщение, передаваемое в функцию.</param>
    public void CustomMessage(string message)
    {
        Debug.Log($"CustomMessage вызвана с параметром: {message}");
    }
}
