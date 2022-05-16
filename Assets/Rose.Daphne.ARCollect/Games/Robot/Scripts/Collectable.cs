using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rose.Daphne.ARCollect.Games
{
    [RequireComponent(typeof(Collider))]
    public class Collectable : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onCollected = null;

        internal delegate void CollectableEvent(Collectable collectable);
        internal CollectableEvent OnCollectedNotification = null;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("Collected!");
                onCollected?.Invoke();
                OnCollectedNotification?.Invoke(this);

                GetComponent<Collider>().enabled = false;
            }
        }
    }
}

