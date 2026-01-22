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
    public Country ownerCountry {get; private set;}
    public HashSet<HexTile> OwnedTiles { get; } = new();
    public City(HexTile tile, Country country)
    {
        cityCenterTile = tile;
        ownerCountry = country;

        remainingHp = maxHp;
        
        ownerCountry.AddCity(this);

        ClaimStartingTiles(tile);

        ownerCountry.UpdateIncome();
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
        ownerCountry.addScore(1);
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
        }

        /*
        foreach (Vector2 offset in HexNeighborOffsets)
        {
            Vector2 neighborPos = (Vector2)cityCenter.transform.position + offset;

            HexTile tile = MapGenerator.Instance.allTiles.FirstOrDefault(t => Vector2.Distance((Vector2)t.transform.position, neighborPos) < 0.01f);
            if (tile != null)
            {
                ClaimTile(tile);
            }

        }
        */

    }

    public void Siege(int attack, Country attackingCountry)
    {
        int cityDefense = 1;

        if(cityCenterTile.unit != null)
        {
            cityDefense += cityCenterTile.unit.defense;
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
            oldOwnerCountry.addScore(-1);
            newOwnerCountry.addScore(1);
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

    public void RegenHealth() {
        if(remainingHp != maxHp)
        {
            remainingHp += 1;
            remainingHp = Mathf.Min(remainingHp, maxHp);
        }
    }
}
