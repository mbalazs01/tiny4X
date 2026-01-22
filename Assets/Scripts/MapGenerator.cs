using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

// Singleton
public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }
    public HexTile hexPrefab;
    private enum HexDirection {N,NE,SE,S,SW,NW }
    public HexTile[] landTiles;
    public HashSet<HexTile> allTiles { get; } = new();
    public bool generateWater;
    private List<UnityEngine.Vector2> occupiedPositions;
    //Max 500 Tiles!!
    public int tileCount;

    void Start()
    {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
    }

   public void GenerateMap()
    {
        landTiles = new HexTile[tileCount];   
        occupiedPositions = new List<UnityEngine.Vector2>();

        GenerateCells(UnityEngine.Vector2.zero, 0, GetRandomDirection());
        if (generateWater) {
            GenerateWaterTiles();
        }
    }

    private void GenerateWaterTiles()
    {
        UnityEngine.Vector2 bottomLeft = occupiedPositions[0];
        UnityEngine.Vector2 topRight = occupiedPositions[0];

        // Calculate Theoretical Edge
        foreach(UnityEngine.Vector2 position in occupiedPositions)
        {
            if (position.x < bottomLeft.x) bottomLeft.x = position.x;
            if (position.y < bottomLeft.y) bottomLeft.y = position.y;

            if (position.x > topRight.x) topRight.x = position.x;
            if (position.y > topRight.y) topRight.y = position.y;
        }

        // Find Edge on Grid

        // Check Column Parity
        if (Mathf.Abs(topRight.x) % 1.56f < 0.1f || 1.56f - (Mathf.Abs(topRight.x) % 1.56f) < 0.1f || topRight.x == 0f)
        {
            //Debug.Log("Top Hex Position Column EVEN");
            //Check if Y with the Column's offset is possible
            if (!(Mathf.Abs(topRight.y) % 0.9f < 0.1f || 0.9f - (Mathf.Abs(topRight.y) % 0.9f) < 0.1f || topRight.y == 0f))
            {
                Debug.Log("Top Hex Position Corrected-Case EVEN");
                topRight.y -= 0.45f;
            }
        } 
        else
        {
            //Debug.Log("Top Hex Position Column ODD");
            //Check if Y with the Column's offset is possible
            if (!(Mathf.Abs((Mathf.Abs(topRight.y) - 0.45f) % 0.9f) < 0.1f ||0.9f - Mathf.Abs((Mathf.Abs(topRight.y) - 0.45f) % 0.9f) < 0.1f))
            {
                Debug.Log("Top Hex Position Corrected-Case ODD");
                topRight.y -= 0.45f;
            }
        }

        // Check Column Parity
        if (Mathf.Abs(bottomLeft.x) % 1.56f < 0.1f || 1.56f - (Mathf.Abs(bottomLeft.x) % 1.56f) < 0.1f || bottomLeft.x == 0f)
        {
            //Debug.Log("Bottom Hex Position Column EVEN");
            //Check if Y with the Column's offset is possible
            if (!(Mathf.Abs(bottomLeft.y) % 0.9f < 0.1f || 0.9f - (Mathf.Abs(bottomLeft.y) % 0.9f) < 0.1f || bottomLeft.y == 0f))
            {
                Debug.Log("Bottom Hex Position Corrected-Case EVEN");
                bottomLeft.y -= 0.45f;
            }
        } 
        else
        {
            //Debug.Log("Bottom Hex Position Column ODD");
            //Check if Y with the Column's offset is possible
            if (!(Mathf.Abs((Mathf.Abs(bottomLeft.y) - 0.45f) % 0.9f) < 0.1f ||0.9f - Mathf.Abs((Mathf.Abs(bottomLeft.y) - 0.45f) % 0.9f) < 0.1f))
            {
                Debug.Log("Bottom Hex Position Corrected-Case ODD");
                bottomLeft.y += 0.45f;
            }
        }

        float offset = -0.45f;
        // If the Starting Column is Even
        if (Mathf.Abs(bottomLeft.x) % 1.56f < 0.1f || 1.56f - (Mathf.Abs(bottomLeft.x) % 1.56f) < 0.1f || bottomLeft.x == 0f)
                 offset = -offset;

        

        int increment = 0;
        for (float x = bottomLeft.x; x <= topRight.x; x += 0.78f)
        {
            x = Mathf.Round(x * 1000f) / 1000f;
            for (float y = bottomLeft.y + offset * (increment%2); y <= topRight.y + Mathf.Abs(offset); y += 0.9f)
            {
                y = Mathf.Round(y * 1000f) / 1000f;
                UnityEngine.Vector2 currentPosition = new UnityEngine.Vector2(x, y);

                if (!occupiedPositions.Contains(currentPosition))
                {
                    HexTile water = Instantiate(hexPrefab).SetType(HexTile.HexType.Water);
                    allTiles.Add(water);
                    water.transform.SetParent(transform, false);
                    water.transform.localPosition = currentPosition;
                    occupiedPositions.Add(currentPosition);
                }
            } 
            increment++;
        } 
        
        /* 
        //Debug
        UnityEngine.Vector2 newPosition = new UnityEngine.Vector2(bottomLeft.x, bottomLeft.y);
        HexTile tile = Instantiate<HexTile>(hexPrefab).SetType(HexTile.HexType.Debug);
        tile.transform.SetParent(transform, false);
        tile.transform.localPosition = newPosition;

        UnityEngine.Vector2 newPosition2 = new UnityEngine.Vector2(topRight.x, topRight.y);
        HexTile tile2 = Instantiate<HexTile>(hexPrefab).SetType(HexTile.HexType.Debug);
        tile2.transform.SetParent(transform, false);
        tile2.transform.localPosition = newPosition2;  */
    }

    void GenerateCells(UnityEngine.Vector2 currentPos, int i, HexDirection baseDirection)
    {
        if (i >= tileCount)
            return;

        // Determine Next Hex Position based on the Previous
        UnityEngine.Vector2 newPosition = GetNextPosition(currentPos.x, currentPos.y, baseDirection);
        newPosition.x = Mathf.Round(newPosition.x * 1000f) / 1000f;
        newPosition.y = Mathf.Round(newPosition.y * 1000f) / 1000f;

        // Make sure Hex doesn't already exist in position
        if (!occupiedPositions.Contains(newPosition))
        {
            occupiedPositions.Add(newPosition);

            HexTile tile = landTiles[i] = Instantiate<HexTile>(hexPrefab).SetType(HexTile.HexType.Land);
            allTiles.Add(tile);
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = newPosition;

            GenerateCells(newPosition, i + 1, GetRandomDirection());
            return;
        }

        // See if there's a different Direction available
        List<HexDirection> remainingDirections = new List<HexDirection>();
        foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
        {
            if (direction != baseDirection)
                remainingDirections.Add(direction);
        }

        Shuffle(remainingDirections);

        Debug.Log("Tried to generate at occupied position, trying different direction" + " index of:" + i);
        foreach (HexDirection direction in remainingDirections)
        {
            newPosition = GetNextPosition(currentPos.x, currentPos.y, direction);
            newPosition.x = Mathf.Round(newPosition.x * 1000f) / 1000f;
            newPosition.y = Mathf.Round(newPosition.y * 1000f) / 1000f;
            if (!occupiedPositions.Contains(newPosition))
            {
                GenerateCells(currentPos, i, direction);
                return;
            }
        }

        // If all 6 Positions are blocked, Backtrack from the previous Hex 
        for (int back = 2; back <= occupiedPositions.Count; back++)
        {
            UnityEngine.Vector2 previousPosition = occupiedPositions[occupiedPositions.Count - back];
            foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
            {
                newPosition = GetNextPosition(previousPosition.x, previousPosition.y, direction);
                newPosition.x = Mathf.Round(newPosition.x * 1000f) / 1000f;
                newPosition.y = Mathf.Round(newPosition.y * 1000f) / 1000f;

                Debug.Log("All 6 Positions were taken at" + currentPos.x + " " + currentPos.y + " " + i + baseDirection + ", backtracking to previous position");

                if (!occupiedPositions.Contains(newPosition))
                {
                    GenerateCells(previousPosition, i, direction);
                    return;
                }
            }
        }
    }
    private HexDirection GetRandomDirection()
    {
        int number = UnityEngine.Random.Range(0, 7); 
        switch (number)
        {
            case 1:
                return HexDirection.NE;
            case 2:
                return HexDirection.SE;
            case 3:
                return HexDirection.N;
            case 4:
                return HexDirection.SW;
            case 5:
                return HexDirection.NW;
            case 6:
                return HexDirection.S;
            default:
                return HexDirection.N;
        }
    }
    private UnityEngine.Vector2 GetNextPosition(float x, float y, HexDirection direction)
    {
        switch (direction)
        {
            // Direction is Clockwise - Starting from Top Right
            case HexDirection.NE:
                return new UnityEngine.Vector2(x -0.78f,    y +0.45f);
            case HexDirection.SE:
                return new UnityEngine.Vector2(x -0.78f,    y -0.45f);
            case HexDirection.N:
                return new UnityEngine.Vector2(x,           y +0.9f);
            case HexDirection.SW:
                return new UnityEngine.Vector2(x +0.78f,    y -0.45f);
            case HexDirection.NW:
                return new UnityEngine.Vector2(x +0.78f,    y +0.45f);
            case HexDirection.S:
                return new UnityEngine.Vector2(x,           y -0.9f);
            default:
                return UnityEngine.Vector2.zero;
        }
    }
    // Fisher-Yates shuffle
    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
