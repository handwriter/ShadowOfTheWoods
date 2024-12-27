using System.Collections;
using System.Collections.Generic;
using UHFPS.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HorrorGirlController : MonoBehaviour, IInteractStart
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _endGameSound;
    [SerializeField] private float _fadeSpeed;
    [SerializeField] private float _soundDelay;
    [SerializeField] private float _soundFadeTime;
    [SerializeField] private AudioSource[] _fadeAudioSources;
    
    private bool _startAudioFade;
    private float _time;
    private List<(AudioSource, float)> _sourcesData = new List<(AudioSource, float)>();

    public void InteractStart()
    {
        gameObject.layer = 0;
        StartCoroutine(EndGame());
    }

    private void Update()
    {
        if (_startAudioFade)
        {
            _time += Time.deltaTime;
            float percent = 1 - Mathf.Clamp01(_time / _soundFadeTime);
            if (_sourcesData.Count == 0 && _fadeAudioSources.Length != 0) SetupSourcesStartData();
            foreach (var source in _sourcesData)
            {
                source.Item1.volume = source.Item2 * percent;
            }
        }
    }

    private void SetupSourcesStartData()
    {
        foreach (var source in _fadeAudioSources)
        {
            _sourcesData.Add((source, source.volume));
        }
    }

    private IEnumerator EndGame()
    {
        _startAudioFade = true;
        yield return GameManager.Instance.StartBackgroundFade(false, 0, _fadeSpeed);
        yield return new WaitForSeconds(_soundDelay);
        _audioSource.PlayOneShot(_endGameSound);
        yield return new WaitForSeconds(_endGameSound.length);
        SceneManager.LoadScene(0);
    }
}
