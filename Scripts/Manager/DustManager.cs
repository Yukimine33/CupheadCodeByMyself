using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustManager : MonoBehaviour
{
    [SerializeField]
    private PlayerCharacter _player;
    [SerializeField]
    private SimpleGameObjectPool _instancePool;

    //三种行走灰尘效果预制体
    [Header("Walk Dust")]
    [SerializeField]
    private GameObject _walkDustPrefab01;
    [SerializeField]
    private GameObject _walkDustPrefab02;
    [SerializeField]
    private GameObject _walkDustPrefab03;

    [Header("Dash Dust")]
    [SerializeField]
    private GameObject _dashDustPrefab; //冲刺灰尘效果预制体

    [Header("Ground Dust")]
    [SerializeField]
    private GameObject _groundDustPrefab; //跳跃落地时的灰尘效果

    [Header("Explosion Dust")]
    [SerializeField]
    private GameObject _explosionDustBornEffectPrefab;
    [SerializeField]
    private GameObject _explosionDustPrefab; //放大招时的灰尘效果

    [Header("Hit Effect")]
    [SerializeField]
    private GameObject _hitEffectPrefab; //玩家被击中时的效果

    [Header("Death Dust")]
    [SerializeField]
    private GameObject _deathDustPrefab; //死亡时的灰尘效果

    [Header("Floating Soul")]
    [SerializeField]
    private GameObject _floatingSoulPrefab; //死亡时的灵魂漂浮效果

    private List<GameObject> _dustPrefabList = new List<GameObject>(); //存放走路灰尘的三个预制体

    private enum EffectType
    {
        WalkDust,
        DashDust,
        GroundDust,
        ExplosionDustBornEffect,
        ExplosionDust,
        PlayerHitEffect,
        DeathDust,
        SoulFloatingEffect,
    }

    void Start()
    {
        _Init();
    }

    private void _Init()
    {
        _player.OnWalkDust += (pos) => { _CreateDust(pos, EffectType.WalkDust); };
        _player.OnDashDust += (pos) => { _CreateDust(pos, EffectType.DashDust); };
        _player.OnGroundDust += (pos) => { _CreateDust(pos, EffectType.GroundDust); };
        _player.OnExplosionBornEffect += (pos) => { _CreateDust(pos, EffectType.ExplosionDustBornEffect); };
        _player.OnExplosionDust += _CreateExplosionDust;
        _player.OnHit += (pos) => { _CreateDust(pos, EffectType.PlayerHitEffect); };
        _player.OnDeath += (pos) => { _CreateDust(pos, EffectType.DeathDust); };
        _player.OnCreateSoul += _CreateFloatingSoul;

        _dustPrefabList.Add(_walkDustPrefab01);
        _dustPrefabList.Add(_walkDustPrefab02);
        _dustPrefabList.Add(_walkDustPrefab03);
    }

    private GameObject _GetDustInstance(EffectType rType) //根据灰尘的种类获取对应的灰尘预制体
    {
        switch(rType)
        {
            case EffectType.WalkDust:
                int index = Random.Range(0, 3);
                return _instancePool.GetInstance(_dustPrefabList[index]);
            case EffectType.DashDust:
                return _instancePool.GetInstance(_dashDustPrefab);
            case EffectType.GroundDust:
                return _instancePool.GetInstance(_groundDustPrefab);
            case EffectType.ExplosionDust:
                return _instancePool.GetInstance(_explosionDustPrefab);
            case EffectType.ExplosionDustBornEffect:
                return _instancePool.GetInstance(_explosionDustBornEffectPrefab);
            case EffectType.PlayerHitEffect:
                return _instancePool.GetInstance(_hitEffectPrefab);
            case EffectType.DeathDust:
                return _instancePool.GetInstance(_deathDustPrefab);
            case EffectType.SoulFloatingEffect:
                return _instancePool.GetInstance(_floatingSoulPrefab);
        }

        return null;
    }

    private void _CreateDust(Vector2 pos, EffectType rType)
    {
        var instance = _GetDustInstance(rType);
        instance.GetComponent<Dust>().OnFinish += () => { _ReturnInstanceToPool(instance); };

        if (rType == EffectType.ExplosionDustBornEffect)
        {
            instance.transform.position = new Vector3(pos.x, pos.y, -1);
        }
        else if(rType == EffectType.DeathDust)
        {
            instance.transform.position = new Vector3(pos.x, pos.y, -3);
        }
        else
        {
            instance.transform.position = new Vector3(pos.x, pos.y, -2);
        }
    }

    /// <summary>
    /// 生成大招灰尘效果
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="shootDir"></param>
    private void _CreateExplosionDust(Vector2 dustPos, Vector2 shootDir)
    {
        var dustInstance = _GetDustInstance(EffectType.ExplosionDust);
        dustInstance.GetComponent<Dust>().OnFinish += () => { _ReturnInstanceToPool(dustInstance); };

        var origin = new Vector3(1, 0, 0).normalized;
        var rotate = Quaternion.FromToRotation(origin, shootDir); //根据子弹前进方向对其旋转
        dustInstance.transform.rotation = rotate;
        dustInstance.transform.position = new Vector3(dustPos.x, dustPos.y, -1);
    }

    private void _CreateFloatingSoul(Vector2 bornPos)
    {
        var soulInstance = _GetDustInstance(EffectType.SoulFloatingEffect);
        soulInstance.transform.position = new Vector3(bornPos.x, bornPos.y, -2);
        soulInstance.GetComponent<FloatingSoul>().OnFinish += () => 
        {
            _ReturnInstanceToPool(soulInstance);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Cuphead");
        };
        soulInstance.GetComponent<FloatingSoul>().Init();
    }

    private void _ReturnInstanceToPool(GameObject instanceObject)
    {
        _instancePool.ReturnInstance(instanceObject);
    }
}
