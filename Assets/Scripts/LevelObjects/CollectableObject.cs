using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public class CollectableObject : MonoBehaviour, IResettable, IPlatformElement
    {
        #region Fields

        public ObjectType ObjectType;
        private Transform firstParent;
        private Rigidbody rigidbody;

        #endregion

        #region Unity Methods

        public void Reset()
        {
            rigidbody.isKinematic = true;
            this.transform.SetParent(firstParent);
            this.gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            PickerController picker = collision.gameObject.GetComponent<PickerController>();
            if (picker != null)
            {
                rigidbody.velocity = Vector3.forward * 200 * Time.deltaTime;
            }
        }

        #endregion

        #region Public Methods

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

        public void Activate()
        {
            rigidbody.isKinematic = false;
        }

        #endregion
    }
}