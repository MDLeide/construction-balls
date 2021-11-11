using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

class ResearchLab : MonoBehaviour
{
    int _stationIndex;

    [Header("Settings")]
    public float Range = 25;

    [Header("Buttons")]
    public Interactable Next;
    public Interactable Previous;

    [Header("Bonus Time")]
    public float Blue;
    public float Red;
    public float Yellow;
    public float Orange;
    public float Green;
    public float Purple;

    [Header("Graphics")]
    public TMP_Text StationNameText;
    public TMP_Text StationDistanceText;

    [ShowInInspector, ReadOnly]
    public List<StationDistance> StationsInRange { get; private set; } = new List<StationDistance>();

    [ShowInInspector, ReadOnly]
    public StationDistance SelectedStationDistance
    {
        get
        {
            if (_stationIndex < 0 || !StationsInRange.Any())
                return null;

            return StationsInRange[_stationIndex];
        }
    }
    
    [ShowInInspector, ReadOnly]
    public ResearchStation SelectedStation => SelectedStationDistance?.Station;

    void Start()
    {
        UpdateStationsInRange();
    }

    public void UpdateStationsInRange()
    {
        var current = SelectedStationDistance;

        StationsInRange = GetStationsInRange();

        _stationIndex = StationsInRange.IndexOf(current);
        if (_stationIndex < 0)
            _stationIndex = 0;

        UpdateSelectedStation();
    }

    public float GetSeconds(BallColor color)
    {
        switch (color)
        {
            case BallColor.Blue:
                return Blue;
            case BallColor.Red:
                return Red;
            case BallColor.Yellow:
                return Yellow;
            case BallColor.Green:
                return Green;
            case BallColor.Purple:
                return Purple;
            case BallColor.Orange:
                return Orange;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
    }

    List<StationDistance> GetStationsInRange()
    {
        var colliders = Physics.OverlapBox(transform.position, new Vector3(Range, Range, Range));
        var stations = new List<StationDistance>();
        
        foreach (var hit in colliders)
        {
            var station = hit.transform.GetComponentAnywhere<ResearchStation>();
            if (!stations.Any(p => p.Station == station))
            {
                var distance = (transform.position - station.transform.position).magnitude;
                if (distance <= station.LabRange)
                    stations.Add(new StationDistance(station, distance));
            }
        }

        return stations.OrderBy(p => p.Distance).ToList();
    }
    
    void UpdateSelectedStation()
    {
        if (SelectedStation == null)
        {
            StationNameText.text = "---";
            StationDistanceText.text = string.Empty;
        }
        else
        {
            StationNameText.text = SelectedStation.name;
            var distance = (transform.position - SelectedStation.transform.position).magnitude;
            StationDistanceText.text = Format.Distance(distance);
        }
    }

    public class StationDistance
    {
        public float Distance;
        public ResearchStation Station;

        public StationDistance(ResearchStation station, float distance)
        {
            Station = station;
            Distance = distance;
        }
    }
}