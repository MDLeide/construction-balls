using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;


class WorldGen : MonoBehaviour
{
    Random _random;

    [Header("Seed")]
    public int Seed;
    public bool GenerateSeed;

    [Header("Generators")]
    public LandGen LandGen;
    public ChunkGen ChunkGen;
    public PanelGen PanelGen;
    public SpringGen SpringGen;

    void Start()
    {
        if (GenerateSeed)
            Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        _random = new Random(Seed);

        SpringGen.Random = new Random(_random.Next(int.MinValue, int.MaxValue));
        ChunkGen.Random = new Random(_random.Next(int.MinValue, int.MaxValue));
    }
}

class HubGen : MonoBehaviour
{
    
}

class PanelGen : MonoBehaviour
{
    public Panel PanelPrototype;
    public float PanelSize;

    public void MakePanel(Vector3 chunkPosition, int x, int z)
    {
        var panelPosition =
            new Vector3(
                x * PanelSize,
                0,
                z * PanelSize);

        var panel = Instantiate(PanelPrototype);
        panel.transform.position =
            chunkPosition +
            panelPosition;
    }
}

class SpringGen : MonoBehaviour
{
    [HideInInspector]
    public Random Random;

    public PanelGen PanelGen;
    [Space]

    public Spring SpringPrototype;

    [Space]
    public float ChanceForBlue;
    public float ChanceForRed;
    public float ChanceForYellow;
    
    public void MakeSpring(Vector3 chunkPosition, int x, int z)
    {
        var springPosition =
            new Vector3(
                x * PanelGen.PanelSize,
                0,
                z * PanelGen.PanelSize);

        var spring = Instantiate(SpringPrototype);
        spring.transform.position =
            chunkPosition +
            springPosition;

        spring.Color = GetSpringColor();
    }
    
    BallColor GetSpringColor()
    {
        var blue = ChanceForBlue / (ChanceForBlue + ChanceForRed + ChanceForYellow);
        var red = ChanceForRed / (ChanceForBlue + ChanceForRed + ChanceForYellow);
        var yellow = (ChanceForBlue + ChanceForRed + ChanceForYellow);

        var val = Random.NextDouble();

        if (val <= blue)
            return BallColor.Blue;
        if (val <= blue + red)
            return BallColor.Red;

        return BallColor.Yellow;
    }
}

class ChunkGen : MonoBehaviour
{
    [HideInInspector]
    public Random Random;

    public PanelGen PanelGen;
    public SpringGen SpringGen;

    [Space]
    public int PanelsPerChunkSide;
    public float ChanceChunkHasSpring;

    public float ChunkSize => PanelsPerChunkSide * PanelGen.PanelSize;

    public void GenerateChunk(Vector3 worldPosition, int chunkX, int chunkZ)
    {
        var hasSpring = Random.NextDouble() <= ChanceChunkHasSpring;

        var springX = Random.Next(0, PanelsPerChunkSide);
        var springZ = Random.Next(0, PanelsPerChunkSide);

        var chunkPosition =
            worldPosition +
            new Vector3(
                chunkX * PanelsPerChunkSide * PanelGen.PanelSize,
                0,
                chunkZ * PanelsPerChunkSide * PanelGen.PanelSize);

        for (int x = 0; x < PanelsPerChunkSide; x++)
        for (int z = 0; z < PanelsPerChunkSide; z++)
        {
            if (hasSpring && x == springX && z == springZ)
                SpringGen.MakeSpring(chunkPosition, x, z);
            else
                PanelGen.MakePanel(chunkPosition, x, z);
        }
    }
}

class LandGen : MonoBehaviour
{
    [Header("Generators")]
    public ChunkGen ChunkGen;

    [Header("Placement Settings")]
    public Vector3 CenterPoint;
    public Vector3 Offset;
    
    public float WorldSize;
    
    [Button]
    public void Generate()
    {
        var chunks = (int) (WorldSize / ChunkGen.ChunkSize);

        var worldPosition =
            CenterPoint -
            new Vector3(
                chunks * ChunkGen.ChunkSize / 2,
                0,
                chunks * ChunkGen.ChunkSize / 2);

        for (int x = 0; x < chunks; x++)
        for (int z = 0; z < chunks; z++)
            ChunkGen.GenerateChunk(worldPosition, x, z);
    }
}