using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rose.Daphne.ARCollect.Games
{
    /// <summary>
    /// Collectable object.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Collectable : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onCollected = null;

        internal delegate void CollectableEvent(Collectable collectable);
        /// <summary>
        /// Triggered when Player enter in collision with this <see cref="Collectable"/>.
        /// </summary>
        internal CollectableEvent OnCollectedNotification = null;

        private void OnEnable()
        {
            GetComponent<Collider>().enabled = true;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("Collected!");
                onCollected?.Invoke();
                OnCollectedNotification?.Invoke(this);

                // Prevent other collisions
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}

