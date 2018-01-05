using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoBullet : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Collider2D _collider;

    private const float BULLET_SPEED = 10f; //子弹速度

    private const float BULLET_STAY_TIME = 2.5f; //子弹显示时间

    private bool _isMoving = true;

    private float _timer = 0f;

    private Vector3 _moveDir = new Vector3(-1, 0, 0); //子弹前进方向

    private LayerMask _playerLayer;

    public event Action OnFinish;

    void Update()
    {
        
        if (_isMoving)
        {
            if (_timer >= BULLET_STAY_TIME)
            {
                _isMoving = false;

                if (OnFinish != null)
                {
                    OnFinish();
                }
            }
            else
            {
                _timer += Time.deltaTime;
            }

            _Move();
        }
    }

    public void Init()
    {
        _playerLayer = LayerMask.NameToLayer("Player");
        _timer = 0f;
        _isMoving = true;
        _collider.enabled = true;
    }

    private void _Init()
    {
        _playerLayer = LayerMask.NameToLayer("Player");
    }

    private void _Move()
    {
        this.transform.position += _moveDir * BULLET_SPEED * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _playerLayer.value) //子弹触碰到玩家时触发子弹击中动画
        {
            _isMoving = false;
            _animator.SetTrigger("Hit");
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>().GetHit();
            _collider.enabled = false;
        }
    }

    public void BulletHitFinish()
    {
        if (OnFinish != null)
        {
            OnFinish();
        }
    }
}
