using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _checkPoint;

    [Header("Dust Born Position")]
    [SerializeField]
    private Transform _walkDustBornPoint;
    [SerializeField]
    private Transform _dashDustBornPoint;
    [SerializeField]
    private Transform _groundDustBornPoint;

    [Header("Bullet Born Position")]
    [SerializeField]
    private Transform _standBulletPos;
    [SerializeField]
    private Transform _standUpBulletPos;
    [SerializeField]
    private Transform _downBulletPos;
    [SerializeField]
    private Transform _walkRightBulletPos;
    [SerializeField]
    private Transform _walkRightUpBulletPos;
    [SerializeField]
    private Transform _jumpRightBulletPos;
    [SerializeField]
    private Transform _jumpDownBulletPos;

    [Header("Colliders")]
    [SerializeField]
    private GameObject _standCollider;
    [SerializeField]
    private GameObject _downCollider;
    [SerializeField]
    private GameObject _walkCollider;
    [SerializeField]
    private GameObject _jumpCollider;
    [SerializeField]
    private GameObject _dashCollider;

    public event Action<Vector2, Vector2> OnShoot;
    public event Action<Vector2, Vector2, Transform> OnWalkShoot;
    public event Action<Vector2> OnWalkDust;
    public event Action<Vector2> OnDashDust;
    public event Action<Vector2> OnGroundDust;

    private float _moveSpeed = 6f;
    private float _dashSpeed = 10f;
    private float _jumpSpeed = 17f;

    private float checkDistance = 0.03f;
    private float _timeCount = 0f;
    private float _previousPosY = 0f;

    private int _faceDic = 1;

    private bool _isFacingRight = true;
    private bool _isGrounded = true;
    private bool _isJump = false;
    private bool _isDown = false;
    private bool _isWalking = false;
    private bool _isShoot = false;
    private bool _isDash = false;
    private bool _isAirDash = false;

    private bool _isAlreadyAirDash = false;
    private bool _isAlreadyLand = true;

    private LayerMask _groundLayer;
    private LayerMask _enemyLayer;

    private AnimatorStateInfo _stateInfo;

    private Transform _bulletBornPos;

    private Vector2 _shootDic;

    private List<GameObject> _colliderList = new List<GameObject>();

    private enum PlayerState
    {
        Idle = 0,
        Down = 1,
        Walk = 2,
        Jump = 3,
        Dash = 4,
    }

    void Start()
    {
        _Init();
    }

    void Update()
    {
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        //move
        if(Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.D))
        {
            _Move(1);
        }
        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.A))
        {
            _Move(-1);
        }
        else if(Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
        {
            _StopMove();
        }

        _CheckIsGrounded();

        if(_stateInfo.IsName("Jump"))
        {
            _isAlreadyLand = false;
        }

        if (!_isGrounded && !_isJump)
        {
            _animator.SetBool("IsJump", true);
            _isJump = true;
        }
        else if(_isGrounded)
        {
            _animator.SetBool("IsJump", false);
            _isJump = false;

            if (!_isAlreadyLand)
            {
                GroundDust();
                _isAlreadyLand = true;
            }
        }

        //jump
        if (Input.GetKeyDown(KeyCode.K) && _isGrounded && !_isDash)
        {
            _Jump();
        }
        
        //down
        if(Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S))
        {
            _Down(true);
        }
        else if(Input.GetKeyUp(KeyCode.S))
        {
            _Down(false);
        }


        //shoot
        if (Input.GetKeyDown(KeyCode.J) || Input.GetKey(KeyCode.J))
        {
            if (!_isDash)
            { _Shoot(); }
        }
        else if(Input.GetKeyUp(KeyCode.J))
        {
            _StopShoot();
        }

        //dash
        if(Input.GetKeyDown(KeyCode.Space) && (!_isDash || !_isAirDash))
        {
            _Dash();
        }

        if(_isDash && (_stateInfo.IsName("DashGround") || _stateInfo.IsName("DashAir")))
        {
            _rigidbody.velocity = new Vector2(_faceDic * _dashSpeed, 0);

            if(_stateInfo.normalizedTime > 0.9)
            {
                _rigidbody.velocity = new Vector2(0, 0);
                _rigidbody.gravityScale = 6;
                _isDash = false;

                _animator.SetTrigger("QuitDash");
                
            }
        }

        //if(Input.GetKeyDown(KeyCode.L) && !_isDash)
        //{
        //    _Explosion();
        //}

        _CheckCollider();
    }

    private void _Init()
    {
        _groundLayer = 1 << LayerMask.NameToLayer("Ground");
        _enemyLayer = 1 << LayerMask.NameToLayer("Enemy");

        _bulletBornPos = _standBulletPos;
        _shootDic = new Vector2(1, 0);

        _colliderList.Add(_standCollider);
        _colliderList.Add(_downCollider);
        _colliderList.Add(_walkCollider);
        _colliderList.Add(_jumpCollider);
        _colliderList.Add(_dashCollider);

        for (int i = 0; i < _colliderList.Count; i++)
        {
            _colliderList[i].SetActive(false);
        }

        _previousPosY = this.transform.position.y;
    }

    /// <summary>
    /// 反转角色
    /// </summary>
    private void _Reverse()
    {
        if (_isDash)
            return;

        _isFacingRight = !_isFacingRight;
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="dic"></param>
    private void _Move(int dic) //dic代表面向方向，左为-1，右为1
    {
        if (!_isDown)
        {
            _rigidbody.velocity = new Vector2(dic * _moveSpeed, _rigidbody.velocity.y);
            _animator.SetBool("IsWalk", true);
            _isWalking = true;
        }

        if (!_isDash)
        {
            _faceDic = dic;
        }

        if (dic > 0 && !_isFacingRight)
        {
            _Reverse();
        }
        else if (dic < 0 && _isFacingRight)
        {
            _Reverse();
        }
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    private void _StopMove()
    {
        if (!_isDash)
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        }
        else
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y);
        }

        _animator.SetBool("IsWalk", false);
        _isWalking = false;
        _timeCount = 0f;
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    private void _Jump()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpSpeed);
    }

    /// <summary>
    /// 检测角色是否接触地面
    /// </summary>
    private void _CheckIsGrounded()
    {
        Vector2 check = _checkPoint.position;
        RaycastHit2D hit = Physics2D.Raycast(check, Vector2.down, checkDistance, _groundLayer.value);

        if (hit.collider != null)
        {
            _isGrounded = true;
            _isAlreadyAirDash = false;
        }
        else
        {
            _isGrounded = false;
            _isAirDash = false;
        }
    }


    /// <summary>
    /// 下蹲
    /// </summary>
    private void _Down(bool isDown)
    {
        _animator.SetBool("IsDown", isDown);
        _isDown = isDown;

        if (_isJump)
        {
            return;
        }
        
        if(isDown)
        {
            _StopMove();
        }
    }

    /// <summary>
    /// 冲刺
    /// </summary>
    private void _Dash()
    {
        if(_isAlreadyAirDash)
        {
            return;
        }

        if(!_isGrounded)
        {
            _rigidbody.gravityScale = 0;
            _animator.SetTrigger("AirDash");
            _isAirDash = true;

            _isAlreadyAirDash = true;
        }
        else
        {
            _animator.SetTrigger("GroundDash");
        }

        _isDash = true;
    }

    /// <summary>
    /// 射击
    /// </summary>
    private void _Shoot()
    {
        _shootDic = new Vector2(_faceDic, 0);
        _bulletBornPos = _standBulletPos;

        if (_isWalking && !_isJump)
        {
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.W))
            {
                _animator.SetFloat("WalkState", 1f);
                _shootDic = new Vector2(_faceDic, 1).normalized;
                _bulletBornPos = _walkRightUpBulletPos;
            }
            else
            {
                _animator.SetFloat("WalkState", 0.333f);
                _bulletBornPos = _walkRightBulletPos;
            }

            _timeCount += Time.deltaTime;

            if (_timeCount >= 0.0165 * 9)
            {
                _timeCount = 0f;
                WalkShoot();
            }
        }
        else if(!_isWalking && !_isJump)
        {
            _animator.SetBool("IsShoot", true);

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.W))
            {
                _animator.SetFloat("IdleState", 1f);
                _shootDic = new Vector2(0, 1).normalized;
                _bulletBornPos = _standUpBulletPos;
            }
            else
            {
                _animator.SetFloat("IdleState", 0f);
                _bulletBornPos = _standBulletPos;
            }
        }

        if(_isDown)
        {
            _bulletBornPos = _downBulletPos;
            _animator.SetBool("IsShoot", true);
        }

        if(_isJump)
        {
            _bulletBornPos = _jumpRightBulletPos;

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S))
            {
                _shootDic = new Vector2(0, -1).normalized;
                _bulletBornPos = _jumpDownBulletPos;
            }

            _timeCount += Time.deltaTime;

            if(_timeCount >= 0.0165 * 10)
            {
                _timeCount = 0f;
                ShootBullet();
            }
        }

        _isShoot = true;
    }

    private void _StopShoot()
    {
        _animator.SetFloat("WalkState", 0);
        _animator.SetBool("IsShoot", false);
        _isShoot = false;
    }

    /// <summary>
    /// 大招
    /// </summary>
    //private void _Explosion()
    //{
    //    _rigidbody.velocity = new Vector2(0, 0);
    //}

#region Bullet
    public void ShootBullet()
    {
        Vector2 bornPos = _bulletBornPos.position;

        if (OnShoot != null)
        {
            OnShoot(bornPos, _shootDic);
        }
    }

    public void WalkShoot()
    {
        Vector2 bornPos = _bulletBornPos.position;

        if (OnWalkShoot != null)
        {
            OnWalkShoot(bornPos, _shootDic, _bulletBornPos);
        }
    }
#endregion

#region Collider
    private void _CheckCollider()
    {
        if(_isDash || _isAirDash)
        {
            _ActiveCollider((int)PlayerState.Dash);
        }
        else if(!_isGrounded)
        {
            _ActiveCollider((int)PlayerState.Jump);
        }
        else if(_isWalking)
        {
            _ActiveCollider((int)PlayerState.Walk);
        }
        else if(_isDown)
        {
            _ActiveCollider((int)PlayerState.Down);
        }
        else
        {
            _ActiveCollider((int)PlayerState.Idle);
        }
    }

    private void _ActiveCollider(int index)
    {
        for(int i = 0; i < _colliderList.Count; i++)
        {
            if(i == index)
            {
                _colliderList[i].SetActive(true);
            }
            else
            {
                _colliderList[i].SetActive(false);
            }
        }
    }
    #endregion

#region Dust
    public void WalkDust()
    {
        Vector2 dustPos = _walkDustBornPoint.position;

        if(OnWalkDust != null)
        {
            OnWalkDust(dustPos);
        }
    }

    public void DashDust()
    {
        Vector2 dustPos = _dashDustBornPoint.position;

        if(OnDashDust != null)
        {
            OnDashDust(dustPos);
        }
    }

    public void GroundDust()
    {
        Vector2 dustPos = _dashDustBornPoint.position;

        if (OnGroundDust != null)
        {
            OnGroundDust(dustPos);
        }
    }
#endregion
}
