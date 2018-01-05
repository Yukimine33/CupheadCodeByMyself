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
    [SerializeField]
    private SpriteRenderer _spriteRender;

    [SerializeField]
    private Transform _parryEffectPos;

    [Header("Dust Born Position")]
    [SerializeField]
    private Transform _walkDustBornPoint;
    [SerializeField]
    private Transform _dashDustBornPoint;
    [SerializeField]
    private Transform _groundDustBornPoint;
    [SerializeField]
    private Transform _explosionDustBornEffectGroundPoint;
    [SerializeField]
    private Transform _explosionDustBornEffectAirPoint;
    [SerializeField]
    private Transform _explosionDustBornPointRight;
    [SerializeField]
    private Transform _explosionDustBornPointUp;
    [SerializeField]
    private Transform _explosionDustBornPointDown;
    [SerializeField]
    private Transform _explosionDustBornPointRightUp;
    [SerializeField]
    private Transform _explosionDustBornPointRightDown;
    [SerializeField]
    private Transform _hitEffectBornPoint;
    [SerializeField]
    private Transform _deathDustBornPoint;
    [SerializeField]
    private Transform _soulBornPoint;

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
    [SerializeField]
    private Transform _explosionBulletPos;

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
    [SerializeField]
    private GameObject _parryCollider;
    [SerializeField]
    private GameObject _hitCollider;

    public event Action<Vector2, Vector2, Transform> OnShoot;
    public event Action<Vector2, Vector2> OnExplosionShoot;
    public event Action<Vector2> OnWalkDust;
    public event Action<Vector2> OnDashDust;
    public event Action<Vector2> OnGroundDust;
    public event Action<Vector2> OnExplosionBornEffect;
    public event Action<Vector2, Vector2> OnExplosionDust;
    public event Action<Vector2> OnHit;
    public event Action<Vector2> OnDeath;
    public event Action<Vector2> OnCreateSoul;

    public event Action<bool> OnParry;
    public event Action<Vector2, Vector2> OnEnableParryEffect;
    public event Action<Vector2> OnParryEffect;

    private float _moveSpeed = 6f;
    private float _dashSpeed = 10f;
    private float _jumpSpeed = 20f;
    private float _parryForce = 15f;
    private float _floatingSpeed = 5f;

    private float checkDistance = 0.03f;
    private float _timeCount = 0f;
    private float _previousPosY = 0f;

    private float _curAnimatorSpeed;

    private int _faceDic = 1;

    private bool _isReady = false;
    private bool _isFacingRight = true;
    private bool _isGrounded = true;
    private bool _isJump = false;
    private bool _isDown = false;
    private bool _isWalking = false;
    private bool _isShoot = false;
    private bool _isDash = false;
    private bool _isAirDash = false;
    private bool _isExplosion = false;
    private bool _isParry = false; //控制是否进入parry动画
    private bool _isInParryState = false; //是否真的在parry
    private bool _isHit = false;
    private bool _isInvincible = false; //是否进入无敌状态
    private bool _isFadeAway = false; //是否进入闪烁状态
    private bool _isInFade = false; //是否在半透明状态
    private bool _isDead = false; //玩家是否已经死亡

    private bool _isAlreadyAirDash = false;
    private bool _isAlreadyLand = true;
    private bool _isAlreadyParry = false;

    private bool _isOnPause = false;
    private float _pauseTime = 0f;

    private const float GRAVITY_SCALE = 6f;
    private const float HIT_INVINCIBLE_TIME = 3f;

    private float _invincibleTimer = 0f;
    private float _fadeAwayTimer = 0f;
    private float _fadeAwayTime = 0.2f;
    private float _normalTime = 0.4f;
    private float _maxAlpha = 1f;
    private float _fadeAlpha = 0.5f;

    private LayerMask _groundLayer;
    private LayerMask _enemyLayer;

    private AnimatorStateInfo _stateInfo;

    private Transform _bulletBornPos;
    private Transform _explosionDustBornEffectPos;
    private Transform _explosionDustPos;

    private Vector2 _shootDic;

    private List<GameObject> _colliderList = new List<GameObject>();

    //暂时数据，最好用数据表的形式存储并读取
    private int _playerHP = 3;

    public enum PlayerState
    {
        Idle = 0,
        Down = 1,
        Walk = 2,
        Jump = 3,
        Dash = 4,
        Parry = 5,
        Invincible = 6,
    }

    public PlayerState _curState = PlayerState.Idle;

    void Start()
    {
        _Init();
    }

    void Update()
    {
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        _CheckColliderAndState();

        if(_isInvincible)
        {
            _EnterInvincibleState();
        }

        if(_isFadeAway && !_isDead)
        {
            _FadeAway();
        }

        if (_isHit)
        {
            _animator.SetBool("IsWalk", false);
            _animator.SetBool("IsDown", false);
            QuitDash();

            float tempSpeed = _GetTempSpeed();
            _rigidbody.velocity = new Vector2(tempSpeed, 0);
        }

        if (_isDead)
        {
            _EnterDeathState();
            return;
        }

        if (!_isReady || _isHit)
        {
            return;
        }

        if(_curState == PlayerState.Idle && (Input.GetKeyDown(KeyCode.C) || Input.GetKey(KeyCode.C)))
        {
            _Aim();
        }

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
            if (_isParry)
            {
                _animator.SetTrigger("ExitParry");
                _isParry = false;
            }

            if(_isAlreadyParry)
            {
                _isAlreadyParry = false;
            }

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

        //parry
        if (Input.GetKeyDown(KeyCode.K) && _isJump && !_isAlreadyParry)
        {
            _Parry();
        }

        //down
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S))
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
            if (!_isDash && !_isExplosion && !_isParry)
            { _Shoot(); }
        }
        else if(Input.GetKeyUp(KeyCode.J))
        {
            _StopShoot();
        }

        if (Input.GetKeyDown(KeyCode.L) && !_isDash && !_isExplosion)
        {
            _Explosion();
        }

        if (_isExplosion)
        {
            _rigidbody.velocity = new Vector2(0, 0);
            _rigidbody.gravityScale = 0;
        }

        //dash
        if (Input.GetKeyDown(KeyCode.Space) && !_isDash && !_isAirDash && !_isExplosion)
        {
            _Dash();
        }

        if(_isOnPause)
        {
            if(_pauseTime >= 0.2f)
            {
                _pauseTime = 0f;
                _ExitPause();
            }
            else
            {
                _pauseTime += Time.deltaTime;
            }
        }
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
        _colliderList.Add(_parryCollider);
        _colliderList.Add(_hitCollider);
        
        for (int i = 0; i < _colliderList.Count; i++)
        {
            _colliderList[i].SetActive(false);
        }

        _previousPosY = this.transform.position.y;

        _animator.SetTrigger("Ready");
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
    /// 瞄准
    /// </summary>
    private void _Aim()
    {

    }

#region Move
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="dic"></param>
    private void _Move(int dic) //dic代表面向方向，左为-1，右为1
    {
        if(_isInParryState)
        {
            return;
        }

        if (!_isDown && !_isDash && !_isHit)
        {
            _rigidbody.velocity = new Vector2(dic * _moveSpeed, _rigidbody.velocity.y);
            _animator.SetBool("IsWalk", true);
            _isWalking = true;
        }

        if (!_isDash)
        {
            _faceDic = dic;
        }

        if(_isExplosion)
        {
            return;
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
#endregion

#region Parry
    /// <summary>
    /// 跳跃消除
    /// </summary>
    private void _Parry()
    {
        _isParry = true;
        _isAlreadyParry = true;

        if (OnParry != null)
        {
            OnParry(true);
        }

        _animator.SetTrigger("Parry");
    }

    public void EnterPause(Vector2 parryHitPos)
    {
        _isInParryState = true;

        _rigidbody.velocity = new Vector2(0, 0);
        _rigidbody.gravityScale = 0;

        _curAnimatorSpeed = _animator.speed;
        _animator.speed = 0f;
        _isOnPause = true;

        if(OnEnableParryEffect != null)
        {
            OnEnableParryEffect(_parryEffectPos.position, parryHitPos);
        }
    }

    private void _ExitPause()
    {
        _isInParryState = false;

        _animator.speed = _curAnimatorSpeed;
        _isOnPause = false;
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _parryForce);
        _rigidbody.gravityScale = GRAVITY_SCALE;
        _isAlreadyParry = false;

        if (OnParry != null)
        {
            OnParry(false);
        }
    }

    private void _ExitParry()
    {
        _animator.SetTrigger("ExitParry");
        _isParry = false;

        if (OnParry != null)
        {
            OnParry(false);
        }
    }
    #endregion

#region Jump & Down
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
#endregion

#region Dash
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

        _rigidbody.velocity = new Vector2(_faceDic * _dashSpeed, 0);

        _isDash = true;
        _moveSpeed = _dashSpeed;
    }

    public void QuitDash()
    {
        _rigidbody.velocity = new Vector2(0, 0);
        _rigidbody.gravityScale = GRAVITY_SCALE;
        _isDash = false;

        _animator.SetTrigger("QuitDash");
        _moveSpeed = 6f;
    }
#endregion

#region Shoot
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
                _WalkShoot();
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
#endregion

#region Explosion
    /// <summary>
    /// 大招
    /// </summary>
    private void _Explosion()
    {
        _isExplosion = true;

        _rigidbody.velocity = new Vector2(0, 0);
        _rigidbody.gravityScale = 0;

        if (_isGrounded)
        {
            _explosionDustBornEffectPos = _explosionDustBornEffectGroundPoint;
            _animator.SetTrigger("GroundExplosion");
        }
        else
        {
            _explosionDustBornEffectPos = _explosionDustBornEffectAirPoint;
            _animator.SetTrigger("AirExplosion");
        }

        if(Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.W))
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.D))
            {
                _explosionDustPos = _explosionDustBornPointRightUp;
                _shootDic = new Vector2(_faceDic, 1);
                _animator.SetFloat("ExplosionState", 0.5f);
            }
            else
            {
                _explosionDustPos = _explosionDustBornPointUp;
                _shootDic = new Vector2(0, 1);
                _animator.SetFloat("ExplosionState", 0.25f);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.D))
            {
                _explosionDustPos = _explosionDustBornPointRightDown;
                _shootDic = new Vector2(_faceDic, -1);
                _animator.SetFloat("ExplosionState", 0.75f);
            }
            else
            {
                _explosionDustPos = _explosionDustBornPointDown;
                _shootDic = new Vector2(0, -1);
                _animator.SetFloat("ExplosionState", 1f);
            }
        }
        else
        {
            _explosionDustPos = _explosionDustBornPointRight;
            _shootDic = new Vector2(_faceDic, 0);
            _animator.SetFloat("ExplosionState", 0);
        }
    }

    public void ExplosionForce()
    {
        this.transform.position -= new Vector3(_faceDic, 0, 0) * 0.15f;
    }

    public void ExitExplosion()
    {
        _animator.SetTrigger("ExitExplosion");

        _isExplosion = false;

        _rigidbody.gravityScale = GRAVITY_SCALE;

        _animator.SetFloat("ExplosionState", 0);
    }
#endregion

#region Bullet
    public void ShootBullet()
    {
        Vector2 bornPos = _bulletBornPos.position;

        if (OnShoot != null)
        {
            OnShoot(bornPos, _shootDic, null);
        }
    }

    private void _WalkShoot()
    {
        Vector2 bornPos = _bulletBornPos.position;

        if (OnShoot != null)
        {
            OnShoot(bornPos, _shootDic, _bulletBornPos);
        }
    }

    public void ExplosionShoot()
    {
        Vector2 bornPos = _explosionBulletPos.position;

        if (OnExplosionShoot != null)
        {
            OnExplosionShoot(bornPos, _shootDic);
        }
    }
#endregion

#region Collider
    private void _CheckColliderAndState()
    {
        if(_isInvincible)
        {
            _ActiveCollider((int)PlayerState.Invincible);
            _curState = PlayerState.Invincible;
        }
        else if(_isDash || _isAirDash)
        {
            _ActiveCollider((int)PlayerState.Dash);
            _curState = PlayerState.Dash;
        }
        else if(_isParry)
        {
            _ActiveCollider((int)PlayerState.Parry);
            _curState = PlayerState.Parry;
        }
        else if(_isJump)
        {
            _ActiveCollider((int)PlayerState.Jump);
            _curState = PlayerState.Jump;
        }
        else if(_isWalking)
        {
            _ActiveCollider((int)PlayerState.Walk);
            _curState = PlayerState.Walk;
        }
        else if(_isDown)
        {
            _ActiveCollider((int)PlayerState.Down);
            _curState = PlayerState.Down;
        }
        else
        {
            _ActiveCollider((int)PlayerState.Idle);
            _curState = PlayerState.Idle;
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

    public void ExplosionDustBornEffect()
    {
        Vector2 bornEffectPos = _explosionDustBornEffectPos.position;

        if (OnExplosionBornEffect != null)
        {
            OnExplosionBornEffect(bornEffectPos);
        }
    }

    public void ExplosionDust()
    {
        Vector2 dustPos = _explosionDustPos.position;

        if (OnExplosionDust != null)
        {
            OnExplosionDust(dustPos, _shootDic);
        }
    }

    private void _HitDust()
    {
        if (OnHit != null)
        {
            OnHit(_hitEffectBornPoint.position);
        }
    }

    private void _DeathDust()
    {
        if(OnDeath != null)
        {
            OnDeath(_deathDustBornPoint.position);
        }
    }
#endregion

    public void GetHit()
    {
        if(_isParry || _isHit)
        {
            return;
        }

        _animator.SetBool("IsExitHit", false);

        if (!_isJump)
        {
            _animator.SetBool("IsGroundHit", true);
        }
        else
        {
            _animator.SetBool("IsAirHit", true);
        }

        _isHit = true;
        _isInvincible = true;

        _playerHP -= 1;
    }

    private float _GetTempSpeed()
    {
        if(_isFacingRight)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

    public void ExitHitState()
    {
        if (_playerHP <= 0)
        {
            //_animator.Play("Dead", 0, 0);
            _isDead = true;
        }
        else
        {
            _animator.SetBool("IsExitHit", true);
            _animator.SetBool("IsGroundHit", false);
            _animator.SetBool("IsAirHit", false);
            _isFadeAway = true;
        }

        _isHit = false;
        _spriteRender.color = new Color(1, 1, 1, _fadeAlpha);
    }

    /// <summary>
    /// 进入无敌状态
    /// </summary>
    private void _EnterInvincibleState()
    {
        _invincibleTimer += Time.deltaTime;
        if(_invincibleTimer >= HIT_INVINCIBLE_TIME)
        {
            _isInvincible = false;
            _isFadeAway = false;
            _spriteRender.color = new Color(1, 1, 1, _maxAlpha);
            _invincibleTimer = 0;
        }
    }

    private void _FadeAway()
    {
        _fadeAwayTimer += Time.deltaTime;
        if(!_isInFade && _fadeAwayTimer >= _normalTime)
        {
            _spriteRender.color = new Color(1, 1, 1, _fadeAlpha);
            _isInFade = true;
            _fadeAwayTimer = 0f;
        }
        else if(_isInFade && _fadeAwayTimer >= _fadeAwayTime)
        {
            _spriteRender.color = new Color(1, 1, 1, _maxAlpha);
            _isInFade = false;
            _fadeAwayTimer = 0f;
        }
    }

    public void _ExitReadyState()
    {
        _isReady = true;
    }

    private void _EnterDeathState()
    {
        _animator.SetBool("IsDead", true);
    }

    private void _EnterSoulFloating()
    {
        if(OnCreateSoul != null)
        {
            OnCreateSoul(_soulBornPoint.position);
        }
    }


    private void _DisablePlayer()
    {
        this.gameObject.SetActive(false);
    }
}
