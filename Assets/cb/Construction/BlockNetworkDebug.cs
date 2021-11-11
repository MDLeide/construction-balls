using System.Collections.Generic;
using System.Linq;
using Cashew;
using Cashew.Utility.Extensions;
using UnityEngine;

class BlockNetworkDebug : MonoBehaviour
{
    static List<Color> Colors = new List<Color>();

    public Color Color;
    public BlockNetwork BlockNetwork;

    void Start()
    {
        if (!Colors.Any())
            InitializeList();

        Color = Colors.Pick();
    }


    void Update()
    {
        if (BlockNetwork?.Network == null)
            return;

        for (int x = 0; x < BlockNetwork.Network.GetLength(0); x++)
        for (int y = 0; y < BlockNetwork.Network.GetLength(1); y++)
        for (int z = 0; z < BlockNetwork.Network.GetLength(2); z++)
        {
            var networkBlock = BlockNetwork.Network[x, y, z];
            if (networkBlock == null)
                continue;

            DrawHelper.DrawBox(
                networkBlock.transform.position + Vector3.up * Game.HalfUnitDistance,
                Game.HalfUnitCube,
                Color);
        }
    }

    void OnDestroy()
    {
        Colors.Add(Color);
    }

    static void InitializeList()
    {
        Colors = new List<Color>()
        {
            Color.white,
            Color.blue,
            Color.red,
            Color.yellow,
            Color.magenta,
            Color.cyan,
            Color.gray,
            Color.green
        };
    }
}