using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject helicopterPrefab;
    [SerializeField] private GameObject collectableObjectGroupPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject cubePrefab;

    public Pool<Platform> PlatformPool { get; private set; }
    public Pool<Helicopter> HelicopterPools { get; private set; }
    public Pool<CollectableObjectGroup> CollectableObjectGroupPool { get; private set; }

    public Pool<CollectableObject> BallPools { get; private set; }
    public Pool<CollectableObject> CubePools { get; private set; }

    [Header("Current Level Objects")]
    private List<Platform> currentPlatforms;
    private List<CollectableObjectGroup> currentCollectableObjectGroup;
    private List<CollectableObject> currentCollectableObjects;
    private List<Helicopter> currentHelicopters;

    public void Initialize()
    {
        PlatformPool = new Pool<Platform>(new PrefabFactory<Platform>(platformPrefab, "Platforms"), 10);
        HelicopterPools = new Pool<Helicopter>(new PrefabFactory<Helicopter>(helicopterPrefab, "Helicopters"), 2);

        CollectableObjectGroupPool = new Pool<CollectableObjectGroup>(new PrefabFactory<CollectableObjectGroup>(collectableObjectGroupPrefab, "Collectable Object Group"), 10);

        BallPools = new Pool<CollectableObject>(new PrefabFactory<CollectableObject>(ballPrefab, "Balls"), 10);
        CubePools = new Pool<CollectableObject>(new PrefabFactory<CollectableObject>(cubePrefab, "Cubes"), 10);
    }

    public Pool<CollectableObject> GetCollectableObjectPool(ObjectType objectType)
    {
        switch (objectType)
        {
            case ObjectType.Ball: return BallPools;
            case ObjectType.Cube: return CubePools;

            default:
                throw new System.InvalidOperationException();
        }
    }
}
