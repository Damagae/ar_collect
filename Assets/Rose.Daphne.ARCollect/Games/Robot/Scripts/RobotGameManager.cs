using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Rose.Daphne.ARCollect.SpatialAwareness;
using Rose.Daphne.ARCollect.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Random = UnityEngine.Random;

namespace Rose.Daphne.ARCollect.Games
{
    /// <summary>
    /// Manage the game cycle: introduction, game and repeat mechanism.
    /// </summary>
    public class RobotGameManager : MonoBehaviour
    {
        [SerializeField]
        private ObjectManipulator characterManipulator = null;

        [SerializeField]
        private Animator characterAnimator = null;

        [SerializeField]
        private List<Collectable> robotParts = new List<Collectable>();

        [SerializeField]
        [Tooltip("In seconds.")]
        private float gameDuration = 60f;

        [SerializeField]
        private Timer timer = null;

        [SerializeField]
        private AudioClip partCollectedSound = null;

        [SerializeField]
        private bool debugPlay = false;

        private int collectedPartsCount = 0;
        private AudioSource mainAudioSource = null;


        private void Awake()
        {
            mainAudioSource = CameraCache.Main.GetComponent<AudioSource>();
            UnityEngine.Random.InitState(Time.frameCount); // Initialize random
            robotParts.ForEach(go => go.gameObject.SetActive(false));
            timer.MaxTime = gameDuration;

            characterManipulator.OnManipulationStarted.AddListener((args) => characterAnimator.SetBool("moved", true));
            characterManipulator.OnManipulationEnded.AddListener((args) => characterAnimator.SetBool("moved", false));
        }

        private void Update()
        {
            if (debugPlay)
            {
                debugPlay = false;
                Play();
            }
        }

        public void StartIntroduction()
        {
            string[] introduction = new string[]
            {
                "Hello!",
                "Can you see me?",
                "I am sorry to ask you that, but I would really appreciate your help...",
                "Oh, what are those buttons up there? They can toggle the planes visibility.",
                "And that button in front of me? Oh, that is nothing...",
                "Please don't click on it!"
            };
            Debug.Log("UIManager:");
            Debug.Log(UIManager.Instance != null);
            UIManager.Instance.ShowDialog(introduction, callback: () => ListenToManipulation(ResumeIntroduction));
        }

        private void ListenToManipulation(Action callback)
        {
            characterManipulator.OnManipulationEnded.AddListener((args) => callback());
        }

        private void ResumeIntroduction()
        {
            characterManipulator.OnManipulationEnded.RemoveAllListeners();
            characterManipulator.OnManipulationStarted.AddListener((args) => characterAnimator.SetBool("moved", true));
            characterManipulator.OnManipulationEnded.AddListener((args) => characterAnimator.SetBool("moved", false));

            string[] introduction2 = new string[]
            {
                "You clicked on it! I said no!",
                "Well, I forgive you. And I still need your help.",
                "I am working on a robot and it was running around, but then it fell to pieces.",
                "They are everywhere on the floor and I can't find them.",
                "Would you help me gather it all? You just need to step on a piece to gather it.",
                "You accept? Thank you!",
                "But please hurry, I am hungry and I need to get lunch."
            };
            UIManager.Instance.ShowDialog(introduction2, finalLabel: "Play", callback: Play);
        }

        public void Play()
        {
            timer.OnTimerOver += () => EndGame(false);
            timer.StartTimer();
            collectedPartsCount = 0;
            UIManager.Instance.ShowMission();
            ShowNextPart();
            UIManager.Instance.UpdateParts(collectedPartsCount);
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
            part.OnCollectedNotification = null;

            Debug.Log("Resister Collectable!");
            part.gameObject.SetActive(false);
            ++collectedPartsCount;
            Debug.Log("Parts: " + collectedPartsCount);
            UIManager.Instance.UpdateParts(collectedPartsCount);

            mainAudioSource.PlayOneShot(partCollectedSound);

            if (collectedPartsCount < robotParts.Count)
            {
                // Keep showing parts
                ShowNextPart();
            }
            else
            {
                // End of the game
                Debug.Log("Win!");
                EndGame(true);
            }
        }

        private void EndGame(bool success)
        {

            foreach (var part in robotParts)
            {
                part.gameObject.SetActive(false);
                part.OnCollectedNotification = null;
            }

            if (success)
            {
                string[] outro = new string[]
                {
                    "Congratulations! You made it! Thank you!"
                };
                UIManager.Instance.ShowDialog(outro, callback: () => ListenToManipulation(PlayAgain));
            }
            else
            {
                string[] outro = new string[]
                {
                    "What a waste of time! You didn't make it!",
                    "Try again!",
                    "Yes I threw away all the pieces you already brought me!"
                };
                UIManager.Instance.ShowDialog(outro, finalLabel: "Play", callback: Play);
            }

            timer.OnTimerOver = null;
        }

        private void PlayAgain()
        {
            string[] play = new string[]
                {
                    "If you are bored, I can hide the pieces again..."
                };
            UIManager.Instance.ShowDialog(play, finalLabel: "Play", callback: Play);
        }

        private Vector3 GetPartPosition()
        {
            // Get a random floor plane
            int randomIndex = UnityEngine.Random.Range(0, SpatialAwarenessManager.Instance.FloorPlanes.Count - 1);
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

