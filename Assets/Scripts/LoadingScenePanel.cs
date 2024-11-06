using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoadingScenePanel : MonoBehaviour
{
    public RawImage BackTexturedImage;
    public Texture[] BackTextures;
    public float ChangeImageTime;
    public float SmoothTime;
    private float _alphaVel;
    private float _targetAlpha;
    private float _timeout;
    private int _currentImage;
    private AsyncOperation _loadOperation;

    void Start()
    {
        _targetAlpha = 1;
    }

    public void StartLoadingScene()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        _loadOperation = SceneManager.LoadSceneAsync(1);
        while (_loadOperation.progress !=  0.9f)
        {
            yield return null;
        }
    }

    private IEnumerator ImgTransition()
    {
        _targetAlpha = 0;
        yield return new WaitForSeconds(SmoothTime);
        BackTexturedImage.texture = BackTextures[_currentImage];
        _targetAlpha = 1;
    }

    void Update()
    {
        _timeout += Time.deltaTime;
        if (((int)(_timeout / ChangeImageTime)) != _currentImage)
        {
            _currentImage += 1;
            if (_currentImage >= BackTextures.Length)
            {
                _currentImage = 0;
                _timeout = 0;
            }
            StartCoroutine(ImgTransition());
        }
        BackTexturedImage.color = new Color(BackTexturedImage.color.r, BackTexturedImage.color.g, BackTexturedImage.color.b, Mathf.SmoothDamp(BackTexturedImage.color.a, _targetAlpha, ref _alphaVel, SmoothTime));
    }
}
