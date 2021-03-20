using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private PickerPool pickerPool;
    [SerializeField] private Camera camera;

    private Rigidbody rigidbody;
    private Vector3 direction;
    private bool isMoving;

    private Vector3 defaultPosition;
    private Vector3 cameraDefaultPosition;

    private void Awake()
    {
        isMoving = false;
        rigidbody = this.GetComponent<Rigidbody>();
        direction = Vector3.forward;
        defaultPosition = this.transform.position;
        cameraDefaultPosition = camera.transform.localPosition;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        camera.transform.position = new Vector3(0, 17, camera.transform.position.z);

        if (Input.GetMouseButton(0))
        {
            Vector3 position = Input.mousePosition;

            float distanceToScreen = camera.WorldToScreenPoint(this.transform.position).z;
            Vector3 mousePosition = camera.ScreenToWorldPoint(new Vector3(position.x, position.y, distanceToScreen));
            if (Math.Abs(mousePosition.x - transform.position.x) > 0.5f)
                direction.x = mousePosition.x > this.transform.position.x ? 1 : -1;
        }

        Vector3 newPosition = this.transform.position + direction * Time.deltaTime * speed;
        newPosition = new Vector3(Mathf.Clamp(newPosition.x, -5, 5), newPosition.y, newPosition.z);
        rigidbody.MovePosition(newPosition);

        direction.x = 0;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("CheckPoint"))
        {
            collider.gameObject.SetActive(false);
            GameEventManager.Instance.OnReachedToCheckPoint.Raise();
        }
    }

    public void StopMovingAndForcePoolObjects()
    {
        isMoving = false;
        pickerPool.GetPool().ForEach(collectableObject => collectableObject.Force(Vector3.forward, GameManager.OBJECT_FORCE_VALUE));
    }

    public void EnableMoving()
    {
        isMoving = true;
        pickerPool.ResetPool();
    }

    public void ResetPosition()
    {
        this.transform.position =  defaultPosition;
        camera.transform.localPosition = cameraDefaultPosition;
    }
}