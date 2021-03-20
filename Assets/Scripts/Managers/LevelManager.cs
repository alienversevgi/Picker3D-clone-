using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PoolManager poolManager;

    private const string LEVEL_PATH = "Level Data/";

    private List<LevelData> allLevelData;
    private LevelData currentLevelData;
    private int levelIndex;
    private int platformIndex;

    [Header("Current Level Objects")]
    private List<Platform> currentPlatforms;
    private List<CollectableObjectGroup> currentCollectableObjectGroup;
    private List<CollectableObject> currentCollectableObjects;
    private List<Helicopter> currentHelicopters;

    public void ResetLevel()
    {
        platformIndex = 0;

        currentHelicopters.ForEach(element => poolManager.HelicopterPools.Release(element));
        currentCollectableObjects.ForEach(element => poolManager.GetCollectableObjectPool(element.ObjectType).Release(element));
        currentPlatforms.ForEach(element => poolManager.PlatformPool.Release(element));
        currentCollectableObjectGroup.ForEach(element => poolManager.CollectableObjectGroupPool.Release(element));

        currentHelicopters.Clear();
        currentCollectableObjects.Clear();
        currentPlatforms.Clear();
        currentCollectableObjectGroup.Clear();
    }

    public void Initialize(int levelIndex, List<LevelData> levels)
    {
        allLevelData = levels;

        currentPlatforms = new List<Platform>();
        currentCollectableObjectGroup = new List<CollectableObjectGroup>();
        currentCollectableObjects = new List<CollectableObject>();
        currentHelicopters = new List<Helicopter>();
    }

    public void SetupLevel(int levelIndex)
    {
        this.levelIndex = levelIndex;
        currentLevelData = allLevelData[levelIndex];
        currentPlatforms = new List<Platform>();

        foreach (PlatformData platformData in currentLevelData.Platforms)
        {
            Platform platform = poolManager.PlatformPool.Allocate();
            platform.gameObject.SetActive(true);
            platform.Initialize(platformData);
            currentPlatforms.Add(platform);
        }

        for (int i = 0; i < currentLevelData.ObjectsData.Count; i++)
        {
            Platform currentPlatform = currentPlatforms[currentLevelData.ObjectsData[i].PlatformIndex];
            CollectableObjectGroup collectableObjectGroup = poolManager.CollectableObjectGroupPool.Allocate();

            collectableObjectGroup.gameObject.SetActive(true);
            collectableObjectGroup.Initialize(currentLevelData.ObjectsData[i].Position, currentPlatform.transform);
            currentPlatform.platformElements.Add(collectableObjectGroup);
            currentCollectableObjectGroup.Add(collectableObjectGroup);

            if (currentLevelData.ObjectsData[i].PresetData != null)
                ApplyPresetToCollectableObject(currentLevelData.ObjectsData[i], collectableObjectGroup);
        }

        for (int i = 0; i < currentLevelData.HelicopterData.Count; i++)
        {
            Platform currentPlatform = currentPlatforms[currentLevelData.HelicopterData[i].PlatformIndex];
            Helicopter helicopter = poolManager.HelicopterPools.Allocate();
            Pool<CollectableObject> collectableObjectPool = poolManager.GetCollectableObjectPool(currentLevelData.HelicopterData[i].ObjectType);
            List<CollectableObject> collectableObjects = new List<CollectableObject>();

            foreach (PlatformObjectData objectData in currentLevelData.HelicopterData[i].ObjectData)
            {
                CollectableObject collectableObject = collectableObjectPool.Allocate();
                collectableObject.Initialize(Vector3.zero, ObjectType.Ball, currentPlatform.transform);
                collectableObjects.Add(collectableObject);
            }

            currentPlatform.platformElements.AddRange(collectableObjects);
            currentCollectableObjects.AddRange(collectableObjects);
            helicopter.gameObject.SetActive(true);
            helicopter.Initialize(collectableObjects, currentLevelData.HelicopterData[i], currentPlatform.transform);
            currentHelicopters.Add(helicopter);
            currentPlatform.platformElements.Add(helicopter);
        }
    }

    private void ApplyPresetToCollectableObject(PlatformObjectData platformObjectData, CollectableObjectGroup collectableObjectGroup)
    {
        foreach (Vector3 point in platformObjectData.PresetData.Positions)
        {
            Pool<CollectableObject> collectableObjectPool = poolManager.GetCollectableObjectPool(platformObjectData.ObjectType);
            CollectableObject collectableObject = collectableObjectPool.Allocate();
            collectableObject.gameObject.SetActive(true);
            collectableObject.Initialize(point, platformObjectData.ObjectType, collectableObjectGroup.transform);
            currentPlatforms[platformIndex].platformElements.Add(collectableObjectGroup);
            currentPlatforms[platformIndex].platformElements.Add(collectableObject);
            currentCollectableObjects.Add(collectableObject);
            currentCollectableObjectGroup.Add(collectableObjectGroup);
        }
    }

    public void ActivateCurrentLevel()
    {
        currentPlatforms[platformIndex].platformElements.ForEach(platformElement => platformElement.Activate());
    }

    public bool GetPlatformTargetScoreSuccessful()
    {
        return currentPlatforms[platformIndex].CollectableObjectPocket.IsEnoughScoreReached;
    }

    public bool HasNextPlatform() => platformIndex < currentLevelData.Platforms.Count - 1;

    public void RunNextPlatform()
    {
        platformIndex++;
        ActivateCurrentLevel();
    }
}
