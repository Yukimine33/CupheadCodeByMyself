using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustManager : MonoBehaviour
{
    [SerializeField]
    private PlayerCharacter _player;

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

    private List<GameObject> _dustPrefabList = new List<GameObject>();

    private enum DustType
    {
        WalkDust,
        DashDust,
        GroundDust
    }

    void Start()
    {
        _Init();
    }

    private void _Init()
    {
        _player.OnWalkDust += (pos) => { _CreateDust(pos, DustType.WalkDust); };
        _player.OnDashDust += (pos) => { _CreateDust(pos, DustType.DashDust); };
        _player.OnGroundDust += (pos) => { _CreateDust(pos, DustType.GroundDust); };

        _dustPrefabList.Add(_walkDustPrefab01);
        _dustPrefabList.Add(_walkDustPrefab02);
        _dustPrefabList.Add(_walkDustPrefab03);
    }

    private GameObject _GetDustInstance(DustType rType) //根据灰尘的种类获取对应的灰尘预制体
    {
        switch(rType)
        {
            case DustType.WalkDust:
                int index = Random.Range(0, 3);
                return Instantiate(_dustPrefabList[index]);
            case DustType.DashDust:
                return Instantiate(_dashDustPrefab);
            case DustType.GroundDust:
                return Instantiate(_groundDustPrefab);
        }

        return null;
    }

    private void _CreateDust(Vector2 pos, DustType rType)
    {
        var instance = _GetDustInstance(rType);
        instance.transform.position = new Vector3(pos.x, pos.y, -2);
    }
}
