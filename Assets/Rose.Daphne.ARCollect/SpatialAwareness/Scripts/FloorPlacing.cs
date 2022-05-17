using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rose.Daphne.ARCollect.SpatialAwareness
{
    /// <summary>
    /// Place object on the floor when floor is detected.
    /// </summary>
    public class FloorPlacing : MonoBehaviour
    {
        [SerializeField]
        private float distanceFromCamera = 1.5f;

        [SerializeField]
        private UnityEvent onFloorDetected = null;

        void Start()
        {
            SpatialAwarenessManager.Instance.onFloorUpdated += UpdatePosition;
        }

        private void UpdatePosition()
        {
            Transform cameraTransform = CameraCache.Main.transform;
            Vector3 cameraForwardScaled = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1));
            Vector3 targetPosition = cameraTransform.position + cameraForwardScaled * distanceFromCamera;

            if (SpatialAwarenessManager.Instance.TryGetFloorProjection(targetPosition, out Vector3 projection))
            {
                transform.position = projection;
                Quaternion goalWorldRotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
                Quaternion goalLocalRotation = transform.parent == null ? goalWorldRotation : Quaternion.Inverse(transform.parent.rotation) * goalWorldRotation;
                Vector3 goalLocalRotationEuler = goalLocalRotation.eulerAngles;

                goalLocalRotationEuler.x = 0f;
                goalLocalRotationEuler.z = 0f;

                goalLocalRotation = Quaternion.Euler(goalLocalRotationEuler);
                transform.localRotation = goalLocalRotation;

                onFloorDetected?.Invoke();
                SpatialAwarenessManager.Instance.onFloorUpdated -= UpdatePosition;
            }
            else
            {
                Debug.Log("Failed to find point on the floor");
            }
           
        }
    }
}

