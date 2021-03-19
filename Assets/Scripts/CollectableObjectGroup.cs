using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObjectGroup : MonoBehaviour, IResettable
{
    private List<CollectableObject> childObjects;
    private Transform firstParent;

    public void Reset()
    {
        this.transform.SetParent(firstParent);
        this.gameObject.SetActive(false);
    }

    public void Initialize(Vector3 position, Transform parent)
    {
        firstParent = this.transform.parent;
        this.transform.SetParent(parent);
        childObjects = new List<CollectableObject>();
        this.transform.localPosition = position;
    }
}
