using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : MonoBehaviour, IResettable
{
    private Transform firstParent;
    private Rigidbody rigidbody;

    public void Reset()
    {
        this.transform.SetParent(firstParent);
        this.gameObject.SetActive(false);
    }

    public void Initialize(Vector3 position, Transform parent)
    {
        rigidbody = this.GetComponent<Rigidbody>();
        firstParent = this.transform.parent;
        this.transform.SetParent(parent);
        this.transform.localPosition = position;
    }

    public void Force()
    {
        rigidbody.AddForce(this.transform.forward * GameManager.OBJECT_FORCE_VALUE);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PickerController picker = collision.gameObject.GetComponent<PickerController>();
        if (picker != null)
        {
            rigidbody.velocity = Vector3.forward * 200 * Time.deltaTime;
        }
    }
}