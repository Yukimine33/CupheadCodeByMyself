using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSoil : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _frontRenderer;
    [SerializeField]
    private SpriteRenderer _behindRenderer;
    [SerializeField]
    private Animator _groundSoilFront;
    [SerializeField]
    private Animator _groundSoilBehind;

    private bool _isFadeAway = false;

    public event Action OnShow;

    private float _curAlpha;
    private float _minAlpha;

	void Start ()
    {
        _groundSoilBehind.gameObject.SetActive(false);
        _groundSoilBehind.speed = 0f;
        _curAlpha = 1;

        _frontRenderer.color = new Color(1, 1, 1, _curAlpha);
        _behindRenderer.color = new Color(1, 1, 1, _curAlpha);
    }
	
	void Update ()
    {
	    if(_isFadeAway)
        {
            _curAlpha = Mathf.Lerp(_curAlpha, _minAlpha, 0.1f);
            _frontRenderer.color = new Color(1, 1, 1, _curAlpha);
            _behindRenderer.color = new Color(1, 1, 1, _curAlpha);
        }

        if(_curAlpha == _minAlpha)
        {
            this.gameObject.SetActive(false);
            _isFadeAway = false;
        }
	}

    private void _StartBehindAnimation()
    {
        _groundSoilBehind.gameObject.SetActive(true);
        _groundSoilBehind.speed = 1.7f;

        if(OnShow != null)
        {
            OnShow();
        }
    }

    public void ActiveSoil(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }

    public void FadeAway()
    {
        _isFadeAway = true;
    }
}
