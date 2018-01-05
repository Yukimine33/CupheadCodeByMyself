using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private const float NORMAL_BULLET_SPEED = 30f; //普通子弹速度
    private const float EXPLOSION_BULLET_SPEED = 18f; //大招子弹速度

    private float _speed; //子弹速度

    private Vector3 _moveDir; //子弹前进方向

    private AnimatorStateInfo _stateInfo;

    private bool _isMoving = false;

    private LayerMask _groundLayer;
    private LayerMask _enemyLayer;

    public event Action OnFinish;

    private float _damage = 1f;

    private GameObject _curHit;

    void Update ()
    {
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if (_stateInfo.normalizedTime >= 1f)
        {
            _isMoving = false;

            if (OnFinish != null)
            {
                OnFinish();
            }
        }

        if (_isMoving)
        {
            _Move();
        }
	}

    private void _Init()
    {
        _groundLayer = LayerMask.NameToLayer("Ground");
        _enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    public void StartMove(Vector2 shootDir, int bulletType)
    {
        _Init();

        switch(bulletType)
        {
            case 0:
                _speed = NORMAL_BULLET_SPEED;
                break;
            case 1:
                _speed = EXPLOSION_BULLET_SPEED;
                break;
        }

        _isMoving = true;
        _moveDir = new Vector3(shootDir.x, shootDir.y, 0).normalized;

        var origin = new Vector3(1, 0, 0).normalized;

        var rotate = Quaternion.FromToRotation(origin, _moveDir); //根据子弹前进方向对其旋转

        this.transform.rotation = rotate;
    }

    private void _Move()
    {
        this.transform.position += _moveDir * _speed * Time.deltaTime;
    }

    private void _ActiveEnemyMask(int state)
    {
        if(_curHit == null)
        { return; }

        bool isActive = false;

        if(state == 0)
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }

        _curHit.GetComponent<DamageManager>().ActiveMask(isActive);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D:" + collision.name);
        if (collision.gameObject.layer == _groundLayer.value || collision.gameObject.layer == _enemyLayer) //子弹触碰到地面时触发子弹击中动画
        {
            _isMoving = false;
            _animator.SetTrigger("Hit");
        }

        if (collision.gameObject.layer == _enemyLayer)
        {
            _curHit = collision.gameObject;
            collision.GetComponent<DamageManager>().GetDamage(_damage);
        }
    }
}
