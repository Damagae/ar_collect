using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rose.Daphne.ARCollect.Utility
{
    public class LoaderAnimation : MonoBehaviour
    {
        [SerializeField]
        private float offset = 20f;

        [SerializeField]
        private float speed = 1f;

        private bool goingUp = true;
        private Vector2 originalPosition = default;

        private Vector2 top = default;
        private Vector2 bottom = default;

        private float epsilon = 5f;

        private void Start()
        {
            originalPosition = transform.localPosition;
            top = originalPosition + new Vector2(0, offset);
            bottom = originalPosition - new Vector2(0, offset);
        }

        private void Update()
        {
            if (goingUp)
            {
                if (transform.localPosition.y < top.y - epsilon)
                {
                    transform.localPosition = Vector2.Lerp(transform.localPosition, top, speed * Time.deltaTime);
                }
                else
                {
                    goingUp = false;
                }
            }
            else
            {
                if (transform.localPosition.y > bottom.y + epsilon)
                {
                    transform.localPosition = Vector2.Lerp(transform.localPosition, bottom, speed * Time.deltaTime);
                }
                else
                {
                    goingUp = true;
                }
            }
        }
    }
}

