using UnityEngine;

[RequireComponent(typeof(ItemSpawner))]
class ItemSpawnerController : MonoBehaviour
{
    float _next;

    public ItemSpawner ItemSpawner;

    public float CraftDelay = 1f;
    public int ObjectsToCraft = 50;
    public int ObjectsCrafted;

    void Update()
    {
        if (_next <= Time.time && ObjectsCrafted < ObjectsToCraft)
        {
            ItemSpawner.Spawn();
            _next = Time.time + CraftDelay;
            ObjectsCrafted++;
        }
    }

    void Reset()
    {
        ItemSpawner = GetComponent<ItemSpawner>();
    }
}