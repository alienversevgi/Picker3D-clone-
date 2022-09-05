using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

namespace Game.Level
{
    public class CollectableObjectPocket : MonoBehaviour
    {
        #region Fields

        [SerializeField] private TextMeshPro countedObjectText;
        [SerializeField] private Transform platform;
        [SerializeField] private Transform leftBarrier;
        [SerializeField] private Transform rightBarrier;
        [SerializeField] private GameObject checkpoint;

        public bool IsEnoughScoreReached => currentScore >= endScore;

        private int currentScore;
        private int endScore;

        private Vector3 defaultPlatformPosition;
        private Vector3 defaultLeftBarrierRotation;
        private Vector3 defaultRightBarrierRotation;

        #endregion

        #region Unity Methods

        private void OnCollisionEnter(Collision collision)
        {
            CollectableObject collectableObject = collision.gameObject.GetComponent<CollectableObject>();

            if (collectableObject != null)
            {
                collectableObject.Reset();
                AddScore();
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(int endScore)
        {
            this.currentScore = 0;
            this.endScore = endScore;
            countedObjectText.text = $"{currentScore} / {endScore}";

            defaultLeftBarrierRotation = leftBarrier.localEulerAngles;
            defaultRightBarrierRotation = rightBarrier.localEulerAngles;

            defaultPlatformPosition = platform.localPosition;

            GameEventManager.Instance.OnReachedToCheckPoint.Register(() => StartCoroutine(WaitAndCallAction(1f, () => CheckScore())));
            GameEventManager.Instance.OnSuccesfulyPlatformCleared.Register(() => ShowPocketAnimation());
        }

        public void ResetSetup()
        {
            checkpoint.gameObject.SetActive(true);
            platform.gameObject.SetActive(false);
            leftBarrier.localEulerAngles = defaultLeftBarrierRotation;
            rightBarrier.localEulerAngles = defaultRightBarrierRotation;

            platform.localPosition = defaultPlatformPosition;
        }

        #endregion

        #region Private Methods

        private void AddScore()
        {
            currentScore++;
            countedObjectText.text = $"{currentScore} / {endScore}";
        }


        private IEnumerator WaitAndCallAction(float waitTime, Action action)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            action();
        }

        private void CheckScore()
        {
            if (currentScore >= endScore)
            {
                ShowPocketAnimation();
            }
        }

        private void ShowPocketAnimation()
        {
            platform.gameObject.SetActive(true);
            Tweener lastAnimation;
            Tweener platformAnimation = platform.DOMoveY(0, 1);
            platformAnimation.OnComplete(
                () =>
                {
                    leftBarrier.DOLocalRotate(new Vector3(0, 0, 50), 1, RotateMode.WorldAxisAdd);
                    lastAnimation = rightBarrier.DOLocalRotate(new Vector3(0, 0, -50), 1, RotateMode.WorldAxisAdd);
                }
            );
        }

        #endregion
    }
}