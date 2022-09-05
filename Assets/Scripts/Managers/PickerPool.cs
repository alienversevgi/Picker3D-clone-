using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public class PickerPool : MonoBehaviour
    {
        #region Fields

        public List<CollectableObject> GetPool() => poolOfCollectableObject;
        private List<CollectableObject> poolOfCollectableObject;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            poolOfCollectableObject = new List<CollectableObject>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            CollectableObject collectableObject = collider.GetComponent<CollectableObject>();

            if (collectableObject != null)
            {
                poolOfCollectableObject.Add(collectableObject);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            CollectableObject collectableObject = collider.GetComponent<CollectableObject>();

            if (collectableObject != null)
            {
                poolOfCollectableObject.Remove(collectableObject);
            }
        }

        #endregion

        #region Public Methods

        public void ResetPool()
        {
            poolOfCollectableObject.Clear();
        }

        #endregion
    }
}