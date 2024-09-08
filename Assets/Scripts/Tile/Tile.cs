using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Tile[] adjacentTiles = new Tile[4];
    public int[] costs = new int[4];

    public bool cantRotate = false;

    public bool isRotate = false;
    float rotationAngle = 180f;

    public Vector2 objectPosition;

    public event Action<float> isRotateChanged;
    
    public void Rotate()
    {
        isRotate ^= true;
        objectPosition = -objectPosition;
        isRotateChanged?.Invoke(isRotate?(360f-rotationAngle):rotationAngle);
        UpdateCost();
    }

    private void Start()
    {
        UpdateCost();
    }

    public void UpdateCost()
    {
        for (int i = 0; i < adjacentTiles.Length; i++)
        {
            if (adjacentTiles[i] == null) continue;

            costs[i] = Calculate(adjacentTiles[i], i < 2);
            adjacentTiles[i].costs[3 - i] = adjacentTiles[i].Calculate(this, 3 - i < 2);
        }
    }

    public int Calculate(Tile adjTile, bool isUpper)
    {
        if (isRotate == adjTile.isRotate)
            return 2;

        if (isRotate != isUpper)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}
