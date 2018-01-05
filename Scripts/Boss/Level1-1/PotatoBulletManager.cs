using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoBulletManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _normalBulletPrefab; //普通子弹预制体
    [SerializeField]
    private GameObject _parryBulletPrefab; //可消除子弹预制体
    [SerializeField]
    private GameObject _bornDustPrefab; //子弹生成特效预制体
    [SerializeField]
    private GameObject _defeatDustPrefab; //被击败特效预制体
    [SerializeField]
    private PotatoBoss _potato;
    [SerializeField]
    private SimpleGameObjectPool _instancePool;

    [Header("Positions")]
    [SerializeField]
    private Transform _bulletPos;
    [SerializeField]
    private Transform _bulletDustPos;

    public enum BulletType
    {
        Normal = 0,
        Parry = 1
    }

    void Start()
    {
        _Init();
    }

    private void _Init()
    {
        _potato.OnShoot += _OnShoot;
        _potato.OnDefeat += _CreateDust;
    }

    private void _OnShoot(int bulletType) //射击
    {
        var ran = Random.Range(0, 2);
        var born = _instancePool.GetInstance(_bornDustPrefab);
        born.GetComponent<BulletBornEffect>().OnFinish += () => { _ReturnInstance(born); };
        born.transform.position = _bulletDustPos.position;
        born.transform.SetParent(_instancePool.transform);
        if(ran == 1)
        {
            var scale = born.transform.localScale;
            scale.y *= -1;
            born.transform.localScale = scale;
        }

        var bullet = _GetCurrentInstance(bulletType);
        bullet.transform.position = _bulletPos.position;
        bullet.GetComponent<PotatoBullet>().OnFinish += () => { _ReturnInstance(bullet); };
        bullet.GetComponent<PotatoBullet>().Init();
    }

    private void _CreateDust(Vector2 pos)
    {
        var instance = _GetOtherInstance(_defeatDustPrefab);
        instance.GetComponent<Dust>().OnFinish += () => { _ReturnInstance(instance); };
        instance.transform.position = new Vector3(pos.x, pos.y, -2);
    }

    /// <summary>
    /// 根据子弹类型获取对应物体
    /// </summary>
    /// <param name="bulletType"></param>
    /// <returns></returns>
    private GameObject _GetCurrentInstance(int bulletType)
    {
        switch (bulletType)
        {
            case 0:
                return _instancePool.GetInstance(_normalBulletPrefab);
            case 1:
                return _instancePool.GetInstance(_parryBulletPrefab);
            default:
                return new GameObject();
        }
    }

    private GameObject _GetOtherInstance(GameObject instanceObject)
    {
        return _instancePool.GetInstance(instanceObject);
    }

    private void _ReturnInstance(GameObject instanceObject)
    {
        _instancePool.ReturnInstance(instanceObject);
    }
}
