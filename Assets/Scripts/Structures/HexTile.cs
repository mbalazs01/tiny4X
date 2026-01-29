using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexTile : MonoBehaviour
{
    public Unit unit { get; private set;}
    public City city { get; private set;}
    public Building building { get; private set;}
    public int gold { get; private set;}
    public int food {get; private set;}
    public int production { get; private set;}
    public int science { get; private set;}
    public int culture { get; private set;}
    public int score {get; private set;}
    public HexType hexType { get; private set;}
    public bool isOccupied => unit != null;
    public bool isCityCenter = false;
    public Dictionary<Country, bool> isExplored = new();
    public void SetStartingVisibility()
    {
        foreach (Country c in GameManager.Instance.countries)
        {
            isExplored.Add(c, false);
        }
    }
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

    public void HandleCountryTransfer(Country oldOwnerCountry, Country newOwnerCountry)
    {
            oldOwnerCountry.ChangeScoreBy(-score);
            newOwnerCountry.ChangeScoreBy(score);

            oldOwnerCountry.ChangeGoldIncomeBy(-gold);
            newOwnerCountry.ChangeGoldIncomeBy(gold);

            oldOwnerCountry.ChangeCultureIncomeBy(-culture);
            newOwnerCountry.ChangeCultureIncomeBy(culture);
            
            oldOwnerCountry.ChangeScienceIncomeBy(-science);
            newOwnerCountry.ChangeScienceIncomeBy(science);
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
        Debug.Log("Changing Income of " + city.ownerCountry.countryName);
        city.ownerCountry.ChangeScoreBy(score);
        city.ownerCountry.ChangeGoldIncomeBy(gold);
        city.ownerCountry.ChangeScienceIncomeBy(science);
        city.ownerCountry.ChangeCultureIncomeBy(culture);

        city.ChangeProductionBy(production);
        city.ChangeFoodProductionBy(food);

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
                food = 2;
                production = 2;
                gold = 1;
                score = 1;
                sr.color = Color.green;
                break;
            case HexType.Water:
                food = 1; 
                production = 1;
                gold = 1;
                score = 1;
                sr.color = Color.navyBlue;
                break;
            default:
                sr.color = Color.grey;
                break;
        }

        if(GameManager.Instance.playerCountry == null || !isExplored.ContainsKey(GameManager.Instance.playerCountry) || isExplored[GameManager.Instance.playerCountry] == false)
        {
            sr.color = Color.grey;
            return;
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
    internal void ClearCity()
    {
        city.ownerCountry.ChangeScoreBy(-score);
        city.ownerCountry.ChangeGoldIncomeBy(-gold);
        city.ownerCountry.ChangeScienceIncomeBy(-science);
        city.ownerCountry.ChangeCultureIncomeBy(-culture);

        city.ChangeProductionBy(-production);
        city.ChangeFoodProductionBy(-food);

        city = null;
        UpdateSprite();
    }

    internal void SetAsCityCenter()
    {
        isCityCenter = true;
        UpdateSprite();
    }

    internal void ClearBuilding()
    {
        building = null;
    }

    internal void ChangeProductionBy(int extraProduction)
    {
        production += extraProduction;
    }

    internal void ChangeFoodBy(int extraFood)
    {
        food += extraFood;
    }

    internal void ChangeGoldBy(int extraGold)
    {
        gold += extraGold;
    }

    internal void ChangeScienceBy(int extraScience)
    {
        science += extraScience;
    }

    internal void ChangeCultureBy(int extraCulture)
    {
        culture += extraCulture;
    }

    internal void ChangeScoreBy(int extraScore)
    {
        score += extraScore;
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
            output += "Hex contains a unit of type " + unit.UnitDefinition.UnitType + " controlled by "+ unit.ownerCity.ownerCountry.countryName + "\n"; 
        }

        return output += "Hex is of type " + hexType;
    }
    internal void BuildingConstructed(Building building)
    {
        ChangeProductionBy(building.BuildingDefinition.ExtraProduction);
        ChangeFoodBy(building.BuildingDefinition.ExtraFood);
        city.ChangeProductionBy(building.BuildingDefinition.ExtraProduction);
        city.ChangeFoodProductionBy(building.BuildingDefinition.ExtraFood);

        ChangeGoldBy(building.BuildingDefinition.ExtraGold - building.BuildingDefinition.MaintenanceCost);
        ChangeScienceBy(building.BuildingDefinition.ExtraScience);
        ChangeCultureBy(building.BuildingDefinition.ExtraCulture);
        ChangeScoreBy(building.BuildingDefinition.Score);
        ChangeScoreBy(-building.BuildingDefinition.Score);
        Debug.Log("Changing Income of " + city.ownerCountry.countryName);
        city.ownerCountry.ChangeGoldIncomeBy(building.BuildingDefinition.ExtraGold - building.BuildingDefinition.MaintenanceCost);
        city.ownerCountry.ChangeScienceIncomeBy(building.BuildingDefinition.ExtraScience);
        city.ownerCountry.ChangeCultureIncomeBy(building.BuildingDefinition.ExtraCulture);
        city.ownerCountry.ChangeScoreBy(building.BuildingDefinition.Score);

        this.building = building;
    }
    internal void BuildingDemolished(Building building)
    {
        ChangeProductionBy(-building.BuildingDefinition.ExtraProduction);
        ChangeFoodBy(-building.BuildingDefinition.ExtraFood);
        city.ChangeProductionBy(-building.BuildingDefinition.ExtraProduction);
        city.ChangeFoodProductionBy(-building.BuildingDefinition.ExtraFood);

        ChangeGoldBy(-(building.BuildingDefinition.ExtraGold - building.BuildingDefinition.MaintenanceCost));
        ChangeScienceBy(-building.BuildingDefinition.ExtraScience);
        ChangeCultureBy(-building.BuildingDefinition.ExtraCulture);
        ChangeScoreBy(-building.BuildingDefinition.Score);
        city.ownerCountry.ChangeGoldIncomeBy(-(building.BuildingDefinition.ExtraGold - building.BuildingDefinition.MaintenanceCost));
        city.ownerCountry.ChangeScienceIncomeBy(-building.BuildingDefinition.ExtraScience);
        city.ownerCountry.ChangeCultureIncomeBy(-building.BuildingDefinition.ExtraCulture);
        city.ownerCountry.ChangeScoreBy(-building.BuildingDefinition.Score);

        ClearBuilding();
    }

    internal void Reveal(Country c)
    {
        isExplored[c] = true;
        UpdateSprite();
    }
}
