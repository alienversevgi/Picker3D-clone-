using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ObjectSpawnType
{
    Burst,
    OverTime
}

public class Helicopter : MonoBehaviour, IResettable, IPlatformElement
{
    [SerializeField] private Transform spawnPoint;

    private List<CollectableObject> collectableObjects;
    private CustomObjectData customObjectData;
    private Transform firstParent;
    private bool isMoving;

    public void Initialize(List<CollectableObject> collectableObjects, CustomObjectData customObjectData, Transform parent)
    {
        firstParent = this.transform.parent;
        this.transform.SetParent(parent);
        this.transform.localPosition = customObjectData.Position;
        this.customObjectData = customObjectData;
        this.collectableObjects = collectableObjects;

        collectableObjects.ForEach(element => element.Initialize(this.transform.localPosition, customObjectData.ObjectType, this.transform.parent));
    }

    private void Update()
    {
        if (isMoving && this.transform.localPosition.z < customObjectData.endPositionZ)
        {
            Vector3 newPosition =
                new Vector3(customObjectData.xAxisMovement.Evaluate(Time.time), customObjectData.yAxisMovement.Evaluate(Time.time), customObjectData.zAxisMovement.Evaluate(Time.time));
            this.transform.position +=  (customObjectData.MovementSpeed *  newPosition * Time.deltaTime);
        }
    }

    public void Activate()
    {
        isMoving = true;
        StartCoroutine(OverTimeCoroutine());
    }

    private IEnumerator OverTimeCoroutine()
    {
        foreach (CollectableObject collectableObject in collectableObjects)
        {
            yield return new WaitForSeconds(customObjectData.ObjectSpawnTime);
            collectableObject.gameObject.SetActive(true);
            collectableObject.Initialize(this.transform.localPosition, customObjectData.ObjectType, this.transform.parent);
            collectableObject.Force(Vector2.down, GameManager.OBJECT_FORCE_VALUE);
        }
    }

    public void Reset()
    {
        this.transform.SetParent(firstParent);
        this.gameObject.SetActive(false);
        isMoving = false;
    }
}