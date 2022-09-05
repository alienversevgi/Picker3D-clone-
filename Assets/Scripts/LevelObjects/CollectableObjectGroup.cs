using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public class CollectableObjectGroup : MonoBehaviour, IResettable, IPlatformElement
    {
        #region Fields

        private Transform firstParent;

        #endregion

        #region Unity Methods

        public void Reset()
        {
            this.transform.SetParent(firstParent);
            this.gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        public void Initialize(Vector3 position, Vector3 rotation, Transform parent)
        {
            firstParent = this.transform.parent;
            this.transform.SetParent(parent);
            this.transform.localPosition = position;
            this.transform.eulerAngles = rotation;
        }

        public void Activate()
        {
        }

        #endregion
    }
}