using UnityEngine;
using System.Collections;

public class MusicTrigger : MonoBehaviour
{
    // ������ �� ��������� AudioSource
    public AudioSource audioSource;

    // �������� ��������� ��������� ��� fade in/out
    public float fadeDuration = 1.0f;
    public float targetVolume = 1.0f;

    private Coroutine currentCoroutine;

    // ����� ����������, ����� ������ ������ ������ � �������
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ������������� ������� ��������, ���� ��� ����������
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            // ��������� fade in
            currentCoroutine = StartCoroutine(FadeIn());
        }
    }

    // ����� ����������, ����� ������ ������ ������� �� ��������
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ������������� ������� ��������, ���� ��� ����������
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            // ��������� fade out
            currentCoroutine = StartCoroutine(FadeOut());
        }
    }

    // ������� ��� �������� ���������� ���������
    private IEnumerator FadeIn()
    {
        // ������������� ��������� � 0 ����� ���������������� ������
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

    // ������� ��� �������� ���������� ���������
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
