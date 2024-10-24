using UnityEngine;
using System.Collections;

public class MusicTrigger : MonoBehaviour
{
    // Ссылка на компонент AudioSource
    public AudioSource audioSource;

    // Скорость изменения громкости при fade in/out
    public float fadeDuration = 1.0f;
    public float targetVolume = 1.0f;

    private Coroutine currentCoroutine;

    // Метод вызывается, когда другой объект входит в триггер
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Останавливаем текущую корутину, если она существует
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            // Запускаем fade in
            currentCoroutine = StartCoroutine(FadeIn());
        }
    }

    // Метод вызывается, когда другой объект выходит из триггера
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Останавливаем текущую корутину, если она существует
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            // Запускаем fade out
            currentCoroutine = StartCoroutine(FadeOut());
        }
    }

    // Корутин для плавного увеличения громкости
    private IEnumerator FadeIn()
    {
        // Устанавливаем громкость в 0 перед воспроизведением музыки
        audioSource.volume = 0f;
        audioSource.Play();

        float startVolume = 0f;

        for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    // Корутин для плавного уменьшения громкости
    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;

        for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
