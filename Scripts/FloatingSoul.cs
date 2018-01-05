using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSoul : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    public event Action OnFinish;

    private float _timer = 0f;
    private float _floatingTime = 5f;
    private float _floatingSpeed = 2f;

    private Vector3 _moveDirection = new Vector3(0, 1, 0);

    private bool _isMoving = false;

    void Update()
    {
        if (_isMoving)
        {
            _timer += Time.deltaTime;

            if (_timer >= _floatingTime)
            {
                _isMoving = false;
                if (OnFinish != null)
                {
                    OnFinish();
                }
            }
            else
            {
                _Floating();
            }
        }
    }

    public void Init()
    {
        _isMoving = true;
    }

    private void _Floating()
    {
        this.transform.position += _floatingSpeed * _moveDirection * Time.deltaTime;
    }
}
