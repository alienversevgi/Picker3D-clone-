﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using Game.Data;

namespace Game.Level
{
    public class Helicopter : MonoBehaviour, IResettable, IPlatformElement
    {
        #region Fields

        private List<CollectableObject> collectableObjects;
        private CustomObjectData customObjectData;
        private Transform firstParent;
        private List<Vector3> curvePoints;
        private float duration;

        #endregion

        #region Unity Methods

        public void Reset()
        {
            this.transform.SetParent(firstParent);
            this.gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        public void Initialize(List<CollectableObject> collectableObjects, CustomObjectData customObjectData, Transform parent)
        {
            firstParent = this.transform.parent;
            this.transform.position = customObjectData.Position;
            this.transform.SetParent(parent);
            this.customObjectData = customObjectData;
            this.collectableObjects = collectableObjects;
            this.curvePoints = customObjectData.CurvePoints;
            this.duration = customObjectData.Duration;

            collectableObjects.ForEach(element => element.Initialize(this.transform.localPosition, customObjectData.ObjectType, this.transform.parent));
        }

        public void Activate()
        {
            Path path = new Path(PathType.CubicBezier, curvePoints.ToArray(), 1);
            this.transform.DOPath(path, duration, PathMode.Full3D);
            StartCoroutine(OverTimeCoroutine());
        }

        #endregion

        #region Private Methods

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

        #endregion
    }
}