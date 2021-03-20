using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, IResettable, IPlatformElement
{
    public CollectableObjectPocket CollectableObjectPocket;

    public List<IPlatformElement> platformElements;

    private List<CollectableObjectGroup> childObjects;
    private Transform firstParent;

    public void Reset()
    {
        platformElements.Clear();
        this.gameObject.SetActive(false);
        this.transform.SetParent(firstParent);
        CollectableObjectPocket.ResetSetup();
    }

    public void Initialize(PlatformData data)
    {
        platformElements = new List<IPlatformElement>();
        CollectableObjectPocket.Initialize(data.EndScore);
        firstParent = this.transform.parent;
        this.transform.parent = null;
        childObjects = new List<CollectableObjectGroup>();
        this.transform.position = data.Position;
    }

    public void SetParentCollectableObjectGroup(CollectableObjectGroup collectableObjectGroup)
    {
        collectableObjectGroup.transform.SetParent(this.transform);
        childObjects.Add(collectableObjectGroup);
    }

    public void Activate()
    {
      
    }
}