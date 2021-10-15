using System;
using Cashew.Utility.Extensions;
using TMPro;
using UnityEngine;

class Dispenser : MonoBehaviour
{
    float _resetTime;

    float _spawnTime;

    PickUp _selected;
    PickUp _spawned;

    public float Cooldown;
    public PickUp BlueBall;
    public PickUp RedBall;

    public Transform SpawnPoint;

    public float BallsSpawned;
    public float BallsPerHour => BallsSpawned / ((Time.time - _resetTime) / 60);

    public Interactable BlueButton;
    public Interactable RedButton;
    public Interactable ResetButton;

    public TMP_Text BallsSpawnedText;
    public TMP_Text BallsPerHourText;

    void Start()
    {
        SelectBlue();

        if (BlueButton != null)
            BlueButton.InteractedWith += (sender, args) => SelectBlue();

        if (RedButton != null)
            RedButton.InteractedWith += (sender, args) => SelectRed();

        if (ResetButton != null)
            ResetButton.InteractedWith += (sender, args) => Reset();
    }

    void Update()
    {
        if (_spawnTime <= Time.time && _spawned == null)
        {
            _spawned = Instantiate(_selected);
            _spawned.transform.position = SpawnPoint.position;

            _spawned.PickedUp += OnPickedUp;
            BallsSpawned++;
        }

        if (BallsSpawnedText != null)
            BallsSpawnedText.text = BallsSpawned.ToString("N0");
        if (BallsPerHourText != null)
            BallsPerHourText.text = BallsPerHour.ToString("N1");
    }

    void SelectBlue()
    {
        _selected = BlueBall;
        BallsSpawnedText?.SetColor(Game.Instance.Blue);
        BallsPerHourText?.SetColor(Game.Instance.Blue);
    }

    void SelectRed()
    {
        _selected = RedBall;
        BallsSpawnedText?.SetColor(Game.Instance.Red);
        BallsPerHourText?.SetColor(Game.Instance.Red);
    }



    void Reset()
    {
        BallsSpawned = 0;
        _resetTime = Time.time;
    }

    void OnPickedUp(object sender, EventArgs e)
    {
        _spawned.PickedUp -= OnPickedUp;
        _spawned = null;
        _spawnTime = Time.time + Cooldown;
    }
}