using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Rose.Daphne.ARCollect.SpatialAwareness
{
    /// <summary>
    /// Manage environement detection.
    /// </summary>
    public class SpatialAwarenessManager : MonoBehaviour
    {
        [SerializeField]
        private ARPlaneManager planeManager = null;

        [SerializeField]
        private Material floorMaterial = null;

        [SerializeField]
        private bool showFloor = true;

        [SerializeField]
        private bool showSurfaces = false;

        [SerializeField]
        private Transform debugPlane = null;
        internal Transform DebugPlane => debugPlane;

        [SerializeField]
        private bool debug = false;

        internal static SpatialAwarenessManager Instance { private set; get; } = null;

        private List<ARPlane> floorPlanes = new List<ARPlane>();
        private List<ARPlane> surfacePlanes = new List<ARPlane>();
        internal List<ARPlane> FloorPlanes => floorPlanes;

        internal bool IsFloorAvailable => true;// floorPlanes.Count > 0;

        internal delegate void SpatialAwarenessEvent();
        internal SpatialAwarenessEvent onFloorUpdated = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this);
                return;
            }

            planeManager.planesChanged += UpdateFloor;

#if !UNITY_EDITOR
            debugPlane.gameObject.SetActive(false);
#endif
        }

        private void Update()
        {
            if (debug)
            {
                onFloorUpdated?.Invoke();
                debug = false;
            }
        }

        private void UpdateFloor(ARPlanesChangedEventArgs eventData)
        {
            foreach (ARPlane newPlane in eventData.added)
            {
                if (IsPlaneFloor(newPlane))
                {
                    floorPlanes.Add(newPlane);
                    if (newPlane.TryGetComponent(out Renderer renderer))
                    {
                        renderer.material = floorMaterial;
                    }

                    if (!showFloor)
                    {
                        newPlane.gameObject.SetActive(false);
                    }
                }
                else
                {
                    surfacePlanes.Add(newPlane);

                    if (!showSurfaces)
                    {
                        newPlane.gameObject.SetActive(false);
                    }
                }
            }

            floorPlanes.RemoveAll(p => eventData.removed.Exists(r => r.trackableId == p.trackableId));
            surfacePlanes.RemoveAll(p => eventData.removed.Exists(r => r.trackableId == p.trackableId));

            eventData.removed.ForEach(p => Destroy(p.gameObject));

            onFloorUpdated?.Invoke();
        }

        private bool IsPlaneFloor(ARPlane plane)
        {
            Vector3 cameraPosition = CameraCache.Main.transform.position;

            if (cameraPosition.y - plane.center.y > 1f)
            {
                return true;
            }

            return false;
        }

        internal bool TryGetFloorProjection(Vector3 point, out Vector3 projection)
        {
            projection = default;
            LayerMask mask = LayerMask.GetMask("Spatial Awareness");
            Debug.DrawRay(point, Vector3.down * 10f, Color.blue);
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hitInfo, 5f, mask))
            {
                projection = hitInfo.point;
                return true;
            }

            return false;
        }

        internal Vector3 GetFloorProjection(Vector3 point)
        {
            LayerMask mask = LayerMask.GetMask("Spatial Awareness");
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hitInfo, 5f, mask))
            {
                return hitInfo.point;
            }
            else
            {
                float smallestDistance = float.PositiveInfinity;
                ARPlane nearestPlane = null;
                foreach (ARPlane plane in floorPlanes)
                {
                    Bounds planeBounds = GetBounds(plane);
                    Vector3 closestPoint = planeBounds.ClosestPoint(point);
                    float distance = Vector3.Distance(point, closestPoint);
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        nearestPlane = plane;
                    }
                }

                return Vector3.ProjectOnPlane(point, nearestPlane.normal);
            }
        }

        private Bounds GetBounds(ARPlane plane)
        {
            return new Bounds(plane.center, plane.size);
        }

        public void ShowFloor(bool state)
        {
            floorPlanes.ForEach(p => p.gameObject.SetActive(state));
            showFloor = state;
        }

        public void ShowSurfaces(bool state)
        {
            surfacePlanes.ForEach(p => p.gameObject.SetActive(state));
            showSurfaces = state;
        }
    }
}

