using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoBoss : MonoBehaviour
{
    [SerializeField]
    private Collider2D _collider;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private GroundSoil _groundSoil;
    [SerializeField]
    private DamageManager _damageManager;
    [SerializeField]
    private GameObject _hitMask;
    [SerializeField]
    private SpriteRenderer _spriteRender;
    [SerializeField]
    private SpriteMask _spriteMask;
    [SerializeField]
    private Transform _centerPoint;

    private float[] _fireSpeed = new float[3];

    private AnimatorStateInfo _stateInfo;

    private bool _isInAttackState = false;
    private bool _isTimerWorking = false; //是否进入计时状态
    private bool _isDefeated = false;

    private int _bulletCount = 0;
    private float _curSpeed = 0f;

    private float _timer = 0f;
    private float _intervalTime = 1.95f;

    private float _health = 100f;

    private enum BossState
    {
        Idle,
        Fire,
        Defeat
    }

    private enum AttackStage
    {
        StageOne = 0,
        StageTwo = 1,
        StageThree = 2
    }

    private AttackStage _curAttackStage = AttackStage.StageOne;

    public event Action<int> OnShoot;
    public event Action<Vector2> OnDefeat;

	void Start ()
    {
        this.gameObject.SetActive(false);
        _animator.speed = 0f;
        _groundSoil.OnShow += _EnableBoss;

        _fireSpeed[0] = 1.2f;
        _fireSpeed[1] = 1.35f;
        _fireSpeed[2] = 1.5f;

        _curSpeed = _fireSpeed[0];

        _damageManager.OnGetDamage += _GetDamage;
        _damageManager.OnActiveMask += _ActiveMask;
    }
	
	void Update ()
    {
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if(_stateInfo.IsName("Defeat") && _stateInfo.normalizedTime >= 1f)
        {
            this.gameObject.SetActive(false);
            _groundSoil.FadeAway();
        }

        if(_isDefeated)
        {
            _animator.SetBool("IsDefeated", true);
        }

        _spriteMask.sprite = _spriteRender.sprite;
        
        //检查间隔时间
        if (_isTimerWorking)
        {
            _ChangeLocalPos(-0.2f);
            _animator.speed = 1;
            _timer += Time.deltaTime;
        }

        if(_timer < _intervalTime)
        {
            return;
        }

        if(!_isInAttackState)
        {
            _isTimerWorking = false;

            _animator.speed = _curSpeed;

            _animator.SetTrigger("Attack");

            _isInAttackState = true;
        }
    }

    private void _ExitAnimation(AttackStage stage)
    {
        if(_curAttackStage == stage && !_isTimerWorking)
        {
            //_animator.Play("Attack", 0, 0);
            _animator.SetTrigger("ExitAttack");
            _isInAttackState = false;

            if (_bulletCount == 4)
            {
                _timer = 0f;
                _bulletCount = 0;
                _OnNextSpeed((int)_curAttackStage);
                _isTimerWorking = true;
            }
        }
    }

    /// <summary>
    /// 切换动画速度
    /// </summary>
    /// <param name="stage"></param>
    private void _OnNextSpeed(int stage)
    {
        if (stage != 2)
        {
            stage = stage + 1;
            _curAttackStage = (AttackStage)stage;
        }
        else
        {
            stage = 0;
            _curAttackStage = AttackStage.StageOne;
        }

        _curSpeed = _fireSpeed[stage];
    }

    private void _ShootBullet()
    {
        _bulletCount += 1;

        if (_bulletCount < 4)
        {
            if(OnShoot != null)
            {
                OnShoot(0);
            }
        }
        else
        {
            if (OnShoot != null)
            {
                OnShoot(1);
            }
        }
    }

    private void _EnableBoss()
    {
        this.gameObject.SetActive(true);
        _animator.speed = 1.2f;
    }

    private void _StartTimer()
    {
        _isTimerWorking = true;
    }

    private void _ChangeLocalPos(float posZ)
    {
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, posZ);
    }

    private void _GetDamage(float damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            _isDefeated = true;
            _collider.enabled = false;
            _animator.speed = 1f;
            _ChangeLocalPos(-0.2f);
        }
    }

    private void _ActiveMask(bool isActive)
    {
        _hitMask.SetActive(isActive);
    }

    private void _CreateDefeatDust()
    {
        var pos = _GetDustPos();
        if (OnDefeat != null)
        {
            OnDefeat(pos);
        }

        _animator.speed = 1f;
    }

    private Vector2 _GetDustPos()
    {
        var tempPos = UnityEngine.Random.insideUnitCircle * 5;
        tempPos = new Vector2(_centerPoint.position.x + tempPos.x, _centerPoint.position.y + tempPos.y);
        return tempPos;
    }
}
