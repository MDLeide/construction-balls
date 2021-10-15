using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class HubStats : MonoBehaviour
{
    float _updateBallStats;
    Dictionary<BallColor, BallsPerMinute> _ballStats;

    [Header("Config")]

    public float UpdatePeriod = 1f;
    public int PeriodsToCount = 60;
    
    [Header("Stats")]

    public float TotalBallsPerMinute;
    public float BlueBallsPerMinute;
    public float RedBallsPerMinute;
    public float YellowBallsPerMinute;

    public BallInventory Inventory;
    
    void Start()
    {
        _ballStats = new Dictionary<BallColor, BallsPerMinute>
        {
            {BallColor.Blue, new BallsPerMinute(PeriodsToCount, UpdatePeriod)},
            {BallColor.Red, new BallsPerMinute(PeriodsToCount, UpdatePeriod)},
            {BallColor.Yellow, new BallsPerMinute(PeriodsToCount, UpdatePeriod)}
        };

        Inventory.BallReceived += BallReceived;
        _updateBallStats = Time.time;
    }

    void BallReceived(object sender, BallInventoryChangedEventArgs e)
    {
        _ballStats[e.Color].Count++;
    }

    void Update()
    {
        if (_updateBallStats <= Time.time)
        {
            _updateBallStats += UpdatePeriod;
            foreach (var kvp in _ballStats)
                kvp.Value.EndPeriod();

            TotalBallsPerMinute = _ballStats.Sum(p => p.Value.Average);
            BlueBallsPerMinute = _ballStats[BallColor.Blue].Average;
            RedBallsPerMinute = _ballStats[BallColor.Red].Average;
            YellowBallsPerMinute = _ballStats[BallColor.Yellow].Average;
        }
    }

    class BallsPerMinute
    {
        public BallsPerMinute(int periodsToCount, float period)
        {
            PeriodsToCount = periodsToCount;
            Period = period;
            PeriodAverages = new float[periodsToCount];
        }
        
        public readonly float[] PeriodAverages;
        public readonly int PeriodsToCount;
        public readonly float Period;

        public int PeriodsCounted;
        public int Count;
        public float Average;

        public void EndPeriod()
        {
            if (PeriodsCounted < PeriodsToCount)
            {
                PeriodAverages[PeriodsCounted] = (Count / Period) * (60 / Period);
                PeriodsCounted++;
            }
            else
            {
                for (int i = 0; i < PeriodsToCount - 1; i++)
                    PeriodAverages[i] = PeriodAverages[i + 1];
                PeriodAverages[PeriodsToCount - 1] = (Count / Period) * (60 / Period);
            }

            var total = 0f;
            for (int i = 0; i < PeriodsCounted; i++)
                total += PeriodAverages[i];

            Average = total / PeriodsCounted;
            Count = 0;
        }
    }
}