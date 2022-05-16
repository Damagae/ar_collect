using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rose.Daphne.ARCollect.Utility
{
    public class FollowTransform : MonoBehaviour
    {
        [SerializeField]
        private Transform target = null;

        [SerializeField]
        private bool followOnStart = true;

        [SerializeField]
        private bool relativeFollow = false;

        private bool isFollowing = true;
        private bool isInterrupted = false;

        private Vector3 relativePosition = Vector3.zero;
        private Vector3 relativeScale = Vector3.zero;
        private Vector3 relativeRotation = Vector3.zero;

        private void Awake()
        {
            isFollowing = followOnStart;
            UpdateRelativeTransformInformation();
        }

        private void Update()
        {
            if (!isInterrupted && isFollowing && target != null)
            {
                // Position
                Vector3 position = target.position;

                if (relativeFollow)
                {
                    position += relativePosition;
                }

                transform.position = position;

                // Rotation
                Vector3 rotation = target.rotation.eulerAngles;

                if (relativeFollow)
                {
                    rotation += relativeRotation;
                }

                transform.rotation = Quaternion.Euler(rotation);
            }
        }

        public void UpdateRelativeTransformInformation()
        {
            relativePosition = transform.position - target.position;
            relativeRotation = transform.rotation.eulerAngles - target.rotation.eulerAngles;
        }
    }
}

