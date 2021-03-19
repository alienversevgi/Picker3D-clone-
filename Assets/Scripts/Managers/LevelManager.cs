using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PoolManager poolManager;

    private const string LEVEL_PATH = "Level Data/Level-";
    private int levelIndex;
    private LevelData currentLevel;
    private List<Platform> platforms;
    private int platformIndex;

    public void Initialize(int levelIndex)
    {
        this.levelIndex = levelIndex;
        platformIndex = 0;
        levelIndex = 1;

        SetupLevel(levelIndex);
    }

    public void SetupLevel(int levelIndex)
    {
        this.levelIndex = levelIndex;
        currentLevel = Resources.Load<LevelData>(LEVEL_PATH + levelIndex);
        platforms = new List<Platform>();

        foreach (PlatformData platformData in currentLevel.Platforms)
        {
            Platform platform = poolManager.PlatformPool.Allocate();
            platform.gameObject.SetActive(true);
            platform.Initialize(platformData, null);
            platforms.Add(platform);
        }

        for (int i = 0; i < currentLevel.ObjectsData.Count; i++)
        {
            Platform currentPlatform = platforms[currentLevel.ObjectsData[i].PlatformIndex];
            CollectableObjectGroup collectableObjectGroup = poolManager.CollectableObjectPool.Allocate();

            collectableObjectGroup.gameObject.SetActive(true);
            collectableObjectGroup.Initialize(currentLevel.ObjectsData[i].Position, currentPlatform.transform);

            ApplyPresetToCollectableObject(currentLevel.ObjectsData[i], collectableObjectGroup);
        }
    }

    public bool GetPlatformTargetScoreSuccessful()
    {
        return platforms[platformIndex].CollectableObjectPocket.IsEnoughScoreReached;
    }

    public bool HasNextPlatform() => platformIndex < currentLevel.Platforms.Count - 1;

    public void IncreasePlatformIndex()
    {
        platformIndex++;
    }

    private void ApplyPresetToCollectableObject(PlatformObjectData platformObjectData, CollectableObjectGroup collectableObjectGroup)
    {
        foreach (Vector3 point in platformObjectData.PresetData.Positions)
        {
            Pool<CollectableObject> collectableObjectPool = poolManager.GetCollectableObjectPool(platformObjectData.ObjectType);
            CollectableObject collectableObject = poolManager.BallPools.Allocate();
            collectableObject.gameObject.SetActive(true);
            collectableObject.Initialize(point, collectableObjectGroup.transform);
        }
    }
}
