using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CollectableObjectPocket : MonoBehaviour
{
    [SerializeField] private TextMesh countedObjectText;
    [SerializeField] private Transform platform;
    [SerializeField] private Transform leftBarrier;
    [SerializeField] private Transform rightBarrier;
    [SerializeField] private GameObject checkpoint;

    private int currentScore;
    private int endScore;

    public bool IsEnoughScoreReached => currentScore >= endScore;

    private Vector3 defaultPlatformPosition;
    private Vector3 defaultLeftBarrierRotation;
    private Vector3 defaultRightBarrierRotation;

    public void ResetSetup()
    {
        checkpoint.gameObject.SetActive(true);
        platform.gameObject.SetActive(false);
        leftBarrier.localEulerAngles = defaultLeftBarrierRotation;
        rightBarrier.localEulerAngles = defaultRightBarrierRotation;

        platform.localPosition = defaultPlatformPosition;
    }

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

    private void AddScore()
    {
        currentScore++;
        countedObjectText.text = $"{currentScore} / {endScore}";
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollectableObject collectableObject = collision.gameObject.GetComponent<CollectableObject>();

        if (collectableObject != null)
        {
            collectableObject.Reset();
            AddScore();
        }
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

    public void ShowPocketAnimation()
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
}