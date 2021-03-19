using System.Collections.Generic;
using UnityEngine;


public class PrefabFactory<T> : IFactory<T> where T : MonoBehaviour
{
    GameObject root;
    GameObject prefab;
    string name;
    int index = 0;

    public PrefabFactory(GameObject prefab) : this(prefab, prefab.name) { }

    public PrefabFactory(GameObject prefab, string name)
    {
        this.prefab = prefab;
        this.name = name;
        root = new GameObject();
        root.name = $"{name} Pool";
    }

    public T Create()
    {
        GameObject tempGameObject = GameObject.Instantiate(prefab) as GameObject;
        tempGameObject.name = $"{name}_{index.ToString()}";
        tempGameObject.transform.SetParent(root.transform);
        T objectOfType = tempGameObject.GetComponent<T>();
        index++;
        return objectOfType;
    }
}
