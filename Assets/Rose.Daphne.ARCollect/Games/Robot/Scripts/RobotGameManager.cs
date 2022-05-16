using Microsoft.MixedReality.Toolkit.Utilities;
using Rose.Daphne.ARCollect.SpatialAwareness;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Rose.Daphne.ARCollect.Games
{
    public class RobotGameManager : MonoBehaviour
    {
        [SerializeField]
        private List<Collectable> robotParts = new List<Collectable>();

        [SerializeField]
        private float gameDuration = 60f;

        [SerializeField]
        private int maxSimultaneousParts = 3;

        [SerializeField]
        private bool debugPlay = false;

        private int collectedPartsCount = 0;

        private Vector3 lastPosition = default;


        private void Awake()
        {
            Random.InitState(Time.frameCount); // Initialize random
            robotParts.ForEach(go => go.gameObject.SetActive(false));
        }

        IEnumerator Start()
        {
            yield return new WaitUntil(() => SpatialAwarenessManager.Instance.FloorPlanes.Count > 0);
            Play();
        }

        private void Update()
        {
            if (debugPlay)
            {
                debugPlay = false;
                Play();
            }
        }

        public void Play()
        {
            ShowNextPart();
        }

        private void ShowNextPart()
        {
            Collectable robotPart = robotParts[collectedPartsCount];
#if UNITY_EDITOR
            robotPart.transform.position = GetPartPositionDebug();
#else
            robotPart.transform.position = GetPartPosition();
#endif
            robotPart.gameObject.SetActive(true);
            robotPart.OnCollectedNotification += RegisterPartCollected;
        }

        private void RegisterPartCollected(Collectable part)
        {
            part.OnCollectedNotification -= RegisterPartCollected;

            Debug.Log("Resister Collectable!");
            part.gameObject.SetActive(false);
            ++collectedPartsCount;
            Debug.Log("Parts: " + collectedPartsCount);

            if (collectedPartsCount < robotParts.Count)
            {
                // Keep showing parts
                ShowNextPart();
            }
            else
            {
                // End of the game
                Debug.Log("Win!");
            }
        }

        private Vector3 GetPartPosition()
        {
            // Get a random floor plane
            int randomIndex = Random.Range(0, SpatialAwarenessManager.Instance.FloorPlanes.Count - 1);
            ARPlane randomPlane = SpatialAwarenessManager.Instance.FloorPlanes[randomIndex];

            // Get a random point on the plane
            LayerMask mask = LayerMask.GetMask("NPC");
            Vector3 raycastOrigin;
            Vector3 randomPoint;
            do
            {
                Vector3 randomLocalPoint = new Vector3(Random.Range(-randomPlane.extents.x, randomPlane.extents.x), 0, Random.Range(-randomPlane.extents.y, randomPlane.extents.y));
                randomPoint = randomPlane.transform.TransformPoint(randomLocalPoint);

                // Check that the point is not colliding with the character
                raycastOrigin = randomPoint + randomPlane.transform.up * 1.5f;
            }
            while (Physics.Raycast(raycastOrigin, Vector3.down, 5f, mask));

            return randomPoint;
        }

        private Vector3 GetPartPositionDebug()
        {
            // Get a random floor plane
            Plane randomPlane = new Plane(SpatialAwarenessManager.Instance.DebugPlane.up, SpatialAwarenessManager.Instance.DebugPlane.position);
            Vector2 extents = new Vector2(SpatialAwarenessManager.Instance.DebugPlane.localScale.x * 0.5f, SpatialAwarenessManager.Instance.DebugPlane.localScale.z * 0.5f) * 10f;

            // Get a random point on the plane
            LayerMask mask = LayerMask.GetMask("NPC");
            Vector3 raycastOrigin;
            Vector3 randomPoint;
            int max = 200;
            int tracker = 0;
            do
            {
                Vector3 randomLocalPoint = new Vector3(Random.Range(-extents.x, extents.x), 0, Random.Range(-extents.y, extents.y));
                randomPoint = SpatialAwarenessManager.Instance.DebugPlane.TransformPoint(randomLocalPoint);

                // Check that the point is not colliding with the character
                raycastOrigin = randomPoint + SpatialAwarenessManager.Instance.DebugPlane.up * 1.5f;
                ++tracker;
                Debug.Log(tracker);
                Debug.DrawRay(raycastOrigin, Vector3.down, Color.magenta, 5f);
            }
            while (Physics.Raycast(raycastOrigin, Vector3.down, 5f, mask) && tracker < max);

            return randomPoint;
        }


    }
}

