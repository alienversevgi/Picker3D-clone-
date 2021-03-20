using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickerPool : MonoBehaviour
{
    private List<CollectableObject> poolOfCollectableObject;
    public List<CollectableObject> GetPool() => poolOfCollectableObject;

    private void Awake()
    {
        poolOfCollectableObject = new List<CollectableObject>();
    }

    public void ResetPool()
    {
        poolOfCollectableObject.Clear();
    }

    private void OnTriggerEnter(Collider collider)
    {
        CollectableObject collectableObject = collider.GetComponent<CollectableObject>();

        if (collectableObject != null )
        {
            poolOfCollectableObject.Add(collectableObject);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        CollectableObject collectableObject = collider.GetComponent<CollectableObject>();

        if (collectableObject != null )
        {
            poolOfCollectableObject.Remove(collectableObject);
        }
    }
}
