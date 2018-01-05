using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGameObjectPool : MonoBehaviour
{
    [SerializeField]
    private Transform _poolInstanceTranform;

    private Dictionary<string, Queue<GameObject>> _instancePool = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<GameObject, string> _tagPool = new Dictionary<GameObject, string>();

	public void ClearPool()
    {
        _instancePool.Clear();
        _tagPool.Clear();
    }

    /// <summary>
    /// 获取对应预制体的实例化物体
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public GameObject GetInstance(GameObject prefab)
    {
        string tag = prefab.name;
        tag = tag.Replace("(Clone)", "");

        GameObject poolInstance = _GetFromPool(tag);

        if(poolInstance == null)
        {
            poolInstance = Instantiate(prefab);
            _MarkOut(poolInstance, tag);
            poolInstance.transform.SetParent(_poolInstanceTranform);
        }

        return poolInstance;
    }

    /// <summary>
    /// 将物体返回给池内
    /// </summary>
    /// <param name="instance"></param>
    public void ReturnInstance(GameObject instance)
    {
        if(instance == null)
        {
            return;
        }

        instance.transform.SetParent(_poolInstanceTranform);
        instance.SetActive(false);

        if(_tagPool.ContainsKey(instance))
        {
            string tag = _tagPool[instance];
            _RemoveMarkOut(instance);

            if(!_instancePool.ContainsKey(tag))
            {
                _instancePool[tag] = new Queue<GameObject>();
            }

            _instancePool[tag].Enqueue(instance);
        }
    }

    /// <summary>
    /// 池中获取对应tag的物体
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    private GameObject _GetFromPool(string tag)
    {
        if (_instancePool.ContainsKey(tag) && _instancePool[tag].Count > 0)
        {
            GameObject obj = _instancePool[tag].Dequeue();
            obj.SetActive(true);
            _MarkOut(obj, tag);
            return obj;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 将出池物体加上tag
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="tag"></param>
    private void _MarkOut(GameObject instance, string tag)
    {
        _tagPool.Add(instance, tag);
    }

    /// <summary>
    /// 将回池物体去除tag
    /// </summary>
    /// <param name="instance"></param>
    private void _RemoveMarkOut(GameObject instance)
    {
        if(_tagPool.ContainsKey(instance))
        {
            _tagPool.Remove(instance);
        }
    }
}
