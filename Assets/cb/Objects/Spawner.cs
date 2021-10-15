using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class Spawner : MonoBehaviour
{
    float _nextSpawn;
    
    public GameObject ObjectToSpawn;
    public int QuantityToSpawn;
    public float SpawnTime = .25f;
    public int SpawnCount;

    void Start()
    {
        _nextSpawn = Time.time + SpawnTime;
    }

    void Update()
    {
        if (SpawnCount >= QuantityToSpawn && QuantityToSpawn >= 0)
            return;

        if (_nextSpawn <= Time.time)
        {
            SpawnCount++;
            _nextSpawn += SpawnTime;
            Instantiate(ObjectToSpawn, transform.position, transform.rotation);
        }
    }
}