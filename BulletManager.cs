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

    private float[] _shootOffsets;

    private int _curOffsetIndex = 0;

	void Start()
    {
        _Init();
	}

    private void _Init()
    {
        _player.OnShoot += _OnShoot; //将射击事件传递进来
        _player.OnWalkShoot += _OnWalkShoot;
        _shootOffsets = new float[] { 0.2f, 0f, -0.2f };
    }

    private GameObject _GetBulletInstance() //实例化子弹
    {
        return Instantiate(_bulletPrefab);
    }

    private GameObject _GetBornInstance() //实例化子弹生成特效
    {
        return Instantiate(_bornPrefab);
    }

    private void _OnShoot(Vector2 bornPos, Vector2 shootDic) //站立射击
    {
        var born = _GetBornInstance();
        born.transform.position = new Vector3(bornPos.x, bornPos.y, -1);

        var bullet = _GetBulletInstance();
        bullet.transform.position = _GetOffsetPos(bornPos);
        bullet.GetComponent<Bullet>().StartMove(shootDic);
    }

    private void _OnWalkShoot(Vector2 bornPos, Vector2 shootDic, Transform parent) //行走射击
    {
        var born = _GetBornInstance();
        born.transform.position = new Vector3(bornPos.x, bornPos.y, -1);
        born.transform.SetParent(parent);

        var bullet = _GetBulletInstance();
        bullet.transform.position = _GetOffsetPos(bornPos);
        bullet.GetComponent<Bullet>().StartMove(shootDic);
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
