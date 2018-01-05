using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [SerializeField]
    private SimpleGameObjectPool _instancePool;

    [Header("Start Position")]
    [SerializeField]
    private Transform _upperStartPos;
    [SerializeField]
    private Transform _middleStartPos;
    [SerializeField]
    private Transform _lowerStartPos;

    public void GetCloudInstance(int num, GameObject go)
    {
        var startPos = _GetStartPos(num);
        var instanceToPool = _instancePool.GetInstance(go).GetComponent<Cloud>();

        Vector3 newStartPos = new Vector3(startPos.x, startPos.y, go.transform.position.z);
        instanceToPool.ResetPos(newStartPos, instanceToPool.CurrentType, this);
    }

    public void ReturnInstance(GameObject go)
    {
        _instancePool.ReturnInstance(go);
    }

    private Vector2 _GetStartPos(int num)
    {
        switch(num)
        {
            case 0:
            case 1:
            case 2:
                return _upperStartPos.position;
            case 3:
            case 4:
                return _middleStartPos.position;
            case 5:
            case 6:
            case 7:
                return _lowerStartPos.position;
            default:
                return new Vector2(0, 0);
        }
    }
}
