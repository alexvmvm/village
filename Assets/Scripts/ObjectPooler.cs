using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectPooler : MonoBehaviour
{
    public int PooledAmount = 5;
    public GameObject GameObject;
    public bool WillGrow = true;
    public Transform Parent;

    private List<GameObject> _pool;

    GameObject InstantiateGameObject()
    {
        if (GameObject != null)
            return Instantiate(GameObject);
        else
            return new GameObject();
    }

	public List<GameObject> GetActiveObjects()
	{
        if (_pool == null)
            _pool = new List<GameObject>();
		return _pool.Where(p => p != null && p.activeSelf).ToList();
	}

    public bool ContainsObject(GameObject obj)
    {
        return _pool.Contains(obj);
    }

    public void DeactivateAll()
    {
        if (_pool == null) return;
        foreach (var item in _pool)
            item.SetActive(false);
    }

	public void RemoveAll()
	{
		if (_pool == null) return;
		foreach (var item in _pool)
			Destroy(item);
		_pool.Clear();
	}

    public void Init()
    {
        if(_pool == null)
            _pool = new List<GameObject>();
        for (var i = 0; i < PooledAmount; i++)
        {
            var obj = InstantiateGameObject();
            obj.SetActive(false);
            _pool.Add(obj);
            if (Parent)
                obj.transform.SetParent(Parent);
        }
    }

    public GameObject GetPooledObject()
    {
        if (_pool == null)
            Init();

        for (var i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].activeSelf)
            {
                _pool[i].SetActive(true);
                if (Parent)
                    _pool[i].transform.SetParent(Parent);
                return _pool[i];
            }
        }

        if(WillGrow)
        {
            var obj = InstantiateGameObject();
            _pool.Add(obj);
            if (Parent)
                obj.transform.SetParent(Parent);
            return obj;
        }

        return null;
    }
}
