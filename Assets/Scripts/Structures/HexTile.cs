using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public enum HexType {Land,Water,Debug }
    public Unit unit { get; private set;}
    public City city { get; private set;}
    public int gold { get; private set;}
    public int production { get; private set;}
    public int science { get; private set;}
    public int culture { get; private set;}
    public HexType hexType { get; private set;}
    public bool isOccupied => unit != null;
    public bool isCityCenter = false;
    public void SetUnit(Unit newUnit)
    {
        unit = newUnit;
        UpdateSprite();
    }
    public void RemoveUnit()
    {
        unit = null;
        UpdateSprite();
    }

    public bool IsTileAdjacent(HexTile otherTile)
    {
        UnityEngine.Vector2 thisTilePosition = transform.position;
        UnityEngine.Vector2 otherTilePosition = otherTile.transform.position;

        float distance = UnityEngine.Vector2.Distance(thisTilePosition, otherTilePosition);
        return Mathf.Abs(distance - 0.9f) <= 0.05f;
    }
    public void SetCity(City newCity)
    {
        city = newCity;
        UpdateSprite();
    }

    public HexTile SetType(HexType type)
    {
        hexType = type;
        UpdateSprite();
        return this;
    }
    public void UpdateSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        switch(hexType)
        {
            case HexType.Land: 
                production = 2;
                gold = 1;
                sr.color = Color.green;
                break;
            case HexType.Water: 
                production = 1;
                gold = 1;
                sr.color = Color.navyBlue;
                break;
            default:
                sr.color = Color.grey;
                break;
        }

        if (unit != null)
        {
            sr.color = Color.Lerp(sr.color, unit.ownerCountry.countryColor, 1f);
        }

        if (city != null)
        {
            sr.color = Color.Lerp(sr.color, city.ownerCountry.countryColor, 0.65f);
            if (isCityCenter)
            {
                sr.color = Color.gold;
            }
        }
    }
    public override string ToString()
    {
        string output = "";

        if (city != null)
        {
            output += "Hex contains a city named " + city.name + " controlled by "+ city.ownerCountry.countryName + "\n";
        }
        if (unit != null)
        {
            output += "Hex contains a unit of type " + unit.unitType + " controlled by "+ unit.ownerCity.ownerCountry.countryName + "\n"; 
        }

        return output += "Hex is of type " + hexType;
    }

    internal void ClearCity()
    {
        isCityCenter = true;
        UpdateSprite();
    }

    internal void SetAsCityCenter()
    {
        isCityCenter = true;
        UpdateSprite();
    }
}
