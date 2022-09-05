using Game.Data;
using Game.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        #region Fields

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

        #endregion

        #region Public Methods

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

                for (int i = 0; i < platformData.ObjectsData.Count; i++)
                {
                    CollectableObjectGroup collectableObjectGroup = poolManager.CollectableObjectGroupPool.Allocate();

                    collectableObjectGroup.gameObject.SetActive(true);
                    collectableObjectGroup.Initialize(platformData.ObjectsData[i].Position, platformData.ObjectsData[i].Rotation, platform.transform);
                    platform.platformElements.Add(collectableObjectGroup);
                    currentCollectableObjectGroup.Add(collectableObjectGroup);

                    if (platformData.ObjectsData[i].PresetData != null)
                        ApplyPresetToCollectableObject(platformData.ObjectsData[i], collectableObjectGroup);
                }

                for (int i = 0; i < platformData.HelicopterData.Count; i++)
                {
                    Helicopter helicopter = poolManager.HelicopterPools.Allocate();
                    Pool<CollectableObject> collectableObjectPool = poolManager.GetCollectableObjectPool(platformData.HelicopterData[i].ObjectType);
                    List<CollectableObject> collectableObjects = new List<CollectableObject>();

                    for (int j = 0; j < platformData.HelicopterData[i].SpawnObjectCount; j++)
                    {
                        CollectableObject collectableObject = collectableObjectPool.Allocate();
                        collectableObject.Initialize(Vector3.zero, platformData.HelicopterData[i].ObjectType, platform.transform);
                        collectableObjects.Add(collectableObject);
                    }

                    platform.platformElements.AddRange(collectableObjects);
                    currentCollectableObjects.AddRange(collectableObjects);
                    helicopter.gameObject.SetActive(true);
                    helicopter.Initialize(collectableObjects, platformData.HelicopterData[i], platform.transform);
                    currentHelicopters.Add(helicopter);
                    platform.platformElements.Add(helicopter);
                }
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

        #endregion

        #region Private Methods

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

        #endregion
    }
}