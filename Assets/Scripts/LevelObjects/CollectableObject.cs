using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : MonoBehaviour, IResettable, IPlatformElement
{
    public ObjectType ObjectType;
    private Transform firstParent;
    private Rigidbody rigidbody;

    public void Reset()
    {
        rigidbody.isKinematic = true;
        this.transform.SetParent(firstParent);
        this.gameObject.SetActive(false);
    }

    public void Initialize(Vector3 position, ObjectType objectType, Transform parent)
    {
        this.ObjectType = objectType;
        rigidbody = this.GetComponent<Rigidbody>();
        firstParent = this.transform.parent;
        this.transform.SetParent(parent);
        this.transform.localPosition = position;
    }

    public void Force(Vector3 direction, float force)
    {
        rigidbody.AddForce(direction * 5000f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PickerController picker = collision.gameObject.GetComponent<PickerController>();
        if (picker != null)
        {
            rigidbody.velocity = Vector3.forward * 200 * Time.deltaTime;
        }
    }

    public void Activate()
    {
        rigidbody.isKinematic = false;
    }
}