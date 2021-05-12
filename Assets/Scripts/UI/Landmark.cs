using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A landmark is marked on the hud to the player.
/// </summary>
public class Landmark : MonoBehaviour
{
    public Sprite hudSprite;

    public static List<Landmark> landmarksPositions = new List<Landmark>();
    private bool _landmarkEnabled;
    #region Events
    public event Action OnDestroyEvent;
    public static event Action<Landmark> OnNewLandmarkEvent;
    #endregion
    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }
    private void Start()
    {
        LandmarkEnabled = true;
        OnNewLandmarkEvent?.Invoke(this);
    }

    public bool LandmarkEnabled
    {
        get => _landmarkEnabled;
        set
        {
            if (_landmarkEnabled == value)
                return;

            _landmarkEnabled = value;
            int _index = landmarksPositions.FindIndex((X) => X == transform);
            if (_landmarkEnabled && _index == -1)
                landmarksPositions.Add(this);
            else if (!_landmarkEnabled && _index != -1)
                landmarksPositions.RemoveAt(_index);
        }
    }
}
