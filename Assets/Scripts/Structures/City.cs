using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class City
{
    public HexTile cityCenterTile { get; private set; }
    public int maxHp { get; private set; } = 1;
    public int remainingHp { get; private set; }
    public string name { get; private set; }
    public int productionPerTurn {get; private set; } = 0;
    public int production {get; private set;} = 0;
    public int foodPerTurn {get; private set;} = 0;
    private List<ConstructionDefinition> ConstructionQueue = new List<ConstructionDefinition>();
    public Country ownerCountry {get; private set;}
    public HashSet<HexTile> OwnedTiles { get; } = new();
    public HexTile farmTile { get; private set; }
    public City(HexTile tile, Country country)
    {
        cityCenterTile = tile;
        ownerCountry = country;

        remainingHp = maxHp;
        
        ownerCountry.AddCity(this);

        ClaimStartingTiles(tile);

        ownerCountry.EndTurn();
    }
    public void ChangeName(string newName)
    {
        name = newName;
    }

    public void ClaimTile(HexTile targetTile)
    {
        if (targetTile == null || targetTile.city != null)
            return;

        targetTile.SetCity(this);
        OwnedTiles.Add(targetTile);
    }

    public void LoseTile(HexTile targetTile)
    {
        OwnedTiles.Remove(targetTile);
        targetTile.ClearCity();
    }
    void ClaimStartingTiles(HexTile cityCenter)
    {
        ClaimTile(cityCenter);
        cityCenter.SetAsCityCenter();

        int tileCount = 7;
        int searchRadius = 1;

        while (OwnedTiles.Count < tileCount)
        {
            List<HexTile> candidates = MapGenerator.Instance.allTiles
                .Where(tile =>
                    tile.city == null &&
                    Vector2.Distance(tile.transform.position, cityCenter.transform.position)
                        <= searchRadius * 1.0f)
                .ToList();

            if (candidates.Count == 0)
            {
                searchRadius++;
                continue;
            }

            HexTile chosen = candidates
                .OrderByDescending(tile => tile.production + tile.gold)
                .First();
            ClaimTile(chosen);
            if(farmTile == null) farmTile = chosen; // DELETE LATER DEBUG ONLY
        }
    }

    public void Siege(int attack, Country attackingCountry)
    {
        int cityDefense = 1;

        if(cityCenterTile.unit != null)
        {
            cityDefense += cityCenterTile.unit.UnitDefinition.Defense;
        }

        int potentialDamage = attack - cityDefense;
        potentialDamage = Mathf.Max(potentialDamage, 0);

        remainingHp -= potentialDamage;
        remainingHp = Mathf.Max(remainingHp, 0);

        if(remainingHp == 0)
        {
            Debug.Log("Transfering City to " + attackingCountry);
            TransferCity(attackingCountry);
        } else
        {
            Debug.Log("Repulsed Attack");
        }
    }
    private void TransferCity(Country newOwnerCountry)
    {
        Country oldOwnerCountry = ownerCountry;
        ownerCountry = newOwnerCountry;
        remainingHp = maxHp / 2;

        if(cityCenterTile.unit != null)
        {
            cityCenterTile.unit.DestroyUnit();
        }

        foreach(HexTile tile in OwnedTiles)
        {
            tile.HandleCountryTransfer(oldOwnerCountry, newOwnerCountry);

            tile.UpdateSprite();
        }

        if(oldOwnerCountry.cities != null)
        {
            foreach(Unit unit in oldOwnerCountry.units)
            {
                unit.DestroyUnit();
            }
            GameManager.Instance.removeCountry(oldOwnerCountry);
        }
    }
    public void EndTurn()
    {
        RegenHealth();
        production += productionPerTurn;

        //UNIT TEST: ConstructionQueue.Add(new ConstructionDefinition(UnitDatabase.Archer));
        //BUILDING TEST: ConstructionQueue.Add(new ConstructionDefinition(BuildingDatabase.Farm));

        if(ConstructionQueue != null && ConstructionQueue.Count != 0) {
            if (production > ConstructionQueue[0].ProductionCost)
            {
                production -= ConstructionQueue[0].ProductionCost;
                ConstructionQueue[0].Build(cityCenterTile);
                //BUILDING TEST ConstructionQueue[0].Build(farmTile);
                ConstructionQueue.Remove(ConstructionQueue[0]);
            }
        }
    }
    public void RegenHealth() {
        if(remainingHp != maxHp)
        {
            remainingHp += 1;
            remainingHp = Mathf.Min(remainingHp, maxHp);
        }
    }

    public void ChangeProductionBy(int production)
    {
        productionPerTurn += production;
    }

    internal void ChangeFoodProductionBy(int food)
    {
        foodPerTurn += food;
    }
}