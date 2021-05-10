using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudLandmarks : MonoBehaviour
{
    [SerializeField, AssetsOnly]
    private Image markerPrefab;

    private static List<Marker> markers;
    private struct Marker
    {
        public Landmark landmark;
        public Image marker;
        public Marker(Landmark landmark, Image marker) : this()
        {
            this.landmark = landmark;
            this.marker = marker;
            landmark.OnStateChangeEvent += OnLandmarkStateChange;
            landmark.OnDestroyEvent += OnLandmarkDestroyed;
        }
        private void OnLandmarkStateChange(bool state) => Hidden = state;
        private void OnLandmarkDestroyed() => markers.Remove(this);
        private bool Hidden
        {
            get => marker.isActiveAndEnabled;
            set
            {
                if (value == Hidden)
                    return;

                marker.enabled = value;
            }
        }
    }
    private void Awake()
    {
        Landmark.OnNewLandmarkEvent += AddLandmark;
    }
    private void OnDestroy()
    {
        Landmark.OnNewLandmarkEvent -= AddLandmark;
    }
    private void AddLandmark(Landmark landmark)
    {
        var marker = Instantiate(markerPrefab, transform);
        markers.Add(new Marker(landmark, marker));
    }
}
