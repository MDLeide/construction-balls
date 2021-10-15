using UnityEngine;

class PistonEngine : MonoBehaviour
{
    float _next;

    public PistonArm Arm;
    public float Cooldown;
    public float Offset = .5f;

    void Start()
    {
        _next = Time.time + Offset;
    }

    void Update()
    {
        if (Time.time >= _next)
        {
            Arm.Launch();
            _next = Time.time + Cooldown;
        }
    }
}