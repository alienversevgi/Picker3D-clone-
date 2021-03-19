using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, IResettable
{
    public CollectableObjectPocket CollectableObjectPocket;

    private List<CollectableObjectGroup> childObjects;
    private Transform firstParent;

    public void Reset()
    {
        this.gameObject.SetActive(false);
        this.transform.SetParent(firstParent);
    }

    public void Initialize(PlatformData data, Action callback)
    {
        firstParent = this.transform.parent;
        this.transform.parent = null;
        childObjects = new List<CollectableObjectGroup>();
        this.transform.position = data.Position;
        CollectableObjectPocket.Initialize(data.EndScore);
    }

    public void SetParentCollectableObjectGroup(CollectableObjectGroup collectableObjectGroup)
    {
        collectableObjectGroup.transform.SetParent(this.transform);
        childObjects.Add(collectableObjectGroup);
    }
}