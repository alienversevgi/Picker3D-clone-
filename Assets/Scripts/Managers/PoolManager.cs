using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject collectableObjectGroupPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject pyramidPrefab;

    public Pool<Platform> PlatformPool { get; private set; }
    public Pool<CollectableObjectGroup> CollectableObjectPool { get; private set; }
    public Pool<CollectableObject> BallPools { get; private set; }
    public Pool<CollectableObject> CubePools { get; private set; }
    public Pool<CollectableObject> PyramidPools { get; private set; }

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        PlatformPool = new Pool<Platform>(new PrefabFactory<Platform>(platformPrefab, "Platforms"), 10);
        CollectableObjectPool = new Pool<CollectableObjectGroup>(new PrefabFactory<CollectableObjectGroup>(collectableObjectGroupPrefab, "Collectable Object Group"), 10);

        BallPools = new Pool<CollectableObject>(new PrefabFactory<CollectableObject>(ballPrefab, "Balls"), 10);
        CubePools = new Pool<CollectableObject>(new PrefabFactory<CollectableObject>(cubePrefab, "Cubes"), 10);
        PyramidPools = new Pool<CollectableObject>(new PrefabFactory<CollectableObject>(pyramidPrefab, "Pyramid"), 10);
    }

    public Pool<CollectableObject> GetCollectableObjectPool(ObjectType objectType)
    {
        switch (objectType)
        {
            case ObjectType.Ball: return BallPools;
            case ObjectType.Cube: return CubePools;
            case ObjectType.Pyramid: return PyramidPools;

            default:
                throw new System.InvalidOperationException();
        }
    }
}
