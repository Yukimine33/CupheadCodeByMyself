using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private float _speed = 30f; //子弹速度

    private Vector3 _moveDir; //子弹前进方向

    private AnimatorStateInfo _stateInfo;

    private bool _isMoving = false;

    private LayerMask _groundLayer;
    private LayerMask _enemyLayer;

    void Update ()
    {
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if (_stateInfo.normalizedTime >= 1f)
        {
            _Destroy(); //动画播完后销毁
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

    public void StartMove(Vector2 shootDir)
    {
        _Init();

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

    private void _Destroy()
    {
        Destroy(this.gameObject);
        _isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _groundLayer.value) //子弹触碰到地面时触发子弹击中动画
        {
            _isMoving = false;
            _animator.SetTrigger("Hit");
        }
    }
}
