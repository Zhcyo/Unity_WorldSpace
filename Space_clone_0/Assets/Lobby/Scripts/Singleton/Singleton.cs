using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T _instance;
    public static T Instance {
        get {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = string.Format("_{0}", typeof(T).Name);
                _instance = obj.AddComponent<T>();
            }
            return _instance;
        }
    
    }
    protected virtual void Awake()
    {
        _instance = this as T;
    }
}
