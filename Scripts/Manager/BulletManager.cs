using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _bulletPrefab; //子弹预制体
    [SerializeField]
    private GameObject _bornPrefab; //子弹生成特效预制体
    [SerializeField]
    private PlayerCharacter _player;
    [SerializeField]
    private SimpleGameObjectPool _instancePool;

    [Header("Explosion Bullet")]
    [SerializeField]
    private GameObject _explosionBulletPrefab; //大招子弹预制体

    private float[] _shootOffsets;

    private int _curOffsetIndex = 0;

    public enum BulletType
    {
        Normal = 0,
        Explosion = 1
    }

    void Start()
    {
        _Init();
	}

    private void _Init()
    {
        _player.OnShoot += _OnShoot; //将射击事件传递进来
        _player.OnExplosionShoot += _OnShoot;

        _shootOffsets = new float[] { 0.2f, 0f, -0.2f };
    }

    private void _OnShoot(Vector2 bornPos, Vector2 shootDic, Transform parent) //射击
    {
        var born = _instancePool.GetInstance(_bornPrefab);
        born.GetComponent<BulletBornEffect>().OnFinish += () => { _ReturnInstance(born); };
        born.transform.position = new Vector3(bornPos.x, bornPos.y, -1);
        born.transform.SetParent(parent);

        var bullet = _instancePool.GetInstance(_bulletPrefab);
        bullet.transform.position = _GetOffsetPos(bornPos);
        bullet.GetComponent<Bullet>().OnFinish += () => { _ReturnInstance(bullet); };
        bullet.GetComponent<Bullet>().StartMove(shootDic, (int)BulletType.Normal);
    }

    private void _OnShoot(Vector2 bornPos, Vector2 shootDic) //大招射击
    {
        var bullet = _instancePool.GetInstance(_explosionBulletPrefab);
        bullet.transform.position = new Vector3(bornPos.x, bornPos.y, -2.5f);
        bullet.GetComponent<Bullet>().OnFinish += () => { _ReturnInstance(bullet); };
        bullet.GetComponent<Bullet>().StartMove(shootDic, (int)BulletType.Explosion);
    }

    private void _ReturnInstance(GameObject instanceObject)
    {
        _instancePool.ReturnInstance(instanceObject);
    }

    private Vector3 _GetOffsetPos(Vector2 pos) //获取子弹位置的偏移量
    {
        var offsetY = pos.y + _shootOffsets[_curOffsetIndex];

        if(_curOffsetIndex < 2)
        {
            _curOffsetIndex += 1;
        }
        else
        {
            _curOffsetIndex = 0;
        }

        return new Vector3(pos.x, offsetY, -1);
    }
}
