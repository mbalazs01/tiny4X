using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Unity.Mathematics;
using UnityEngine;

public class Unit
{
    public UnitDefinition UnitDefinition {get; private set; }
    public int remainingHp { get; private set; }
    public int remainingMovement { get; private set; }
    public HexTile currentTile { get; private set; }
    public City ownerCity { get; private set; }
    public Country ownerCountry { get; private set; }
    public Unit(UnitDefinition unitDefinition, HexTile tile)
    {
        UnitDefinition = unitDefinition;
        currentTile = tile;
        ownerCity = tile.city;
        ownerCountry = tile.city.ownerCountry;
        ownerCountry.AddUnit(this);

        remainingHp = UnitDefinition.MaxHP;
        remainingMovement = UnitDefinition.MaxMovement;

        currentTile.SetUnit(this);
        ownerCountry.ChangeScoreBy(unitDefinition.Score);
        ownerCountry.ChangeGoldIncomeBy(-unitDefinition.MaintenanceCost);
    }

    public void DestroyUnit()
    {
        if (currentTile != null)
        {
            currentTile.SetUnit(null);
            currentTile = null;
        }

        if (ownerCountry != null)
        {
            ownerCountry.RemoveUnit(this);
            ownerCountry.ChangeScoreBy(-UnitDefinition.Score);
            ownerCountry.ChangeGoldIncomeBy(UnitDefinition.MaintenanceCost);
            ownerCountry = null;
        }

        ownerCity = null;
    }
    public bool HasAbility(UnitAbility ability)
    {
        return UnitDefinition.Abilities.Contains(ability);
    }
    internal void Move(HexTile clickedHex)
    {
        if (!CanMoveTo(clickedHex))
            return;

        if (clickedHex.unit == null) {
            ExecuteMove(clickedHex);
        }
        else if (clickedHex.isCityCenter = false && clickedHex.unit.ownerCountry != ownerCountry)
        {
            HandleCombat(clickedHex.unit);
        } 
        else if (clickedHex.isCityCenter = true && clickedHex.city.ownerCountry != ownerCountry)
        {
            HandleCityCombat(clickedHex.city);
        }
    }

    private void HandleCityCombat(City targetCity)
    {
        int cityAttack = 1;
        if(targetCity.cityCenterTile.unit != null)
        {
            cityAttack += targetCity.cityCenterTile.unit.UnitDefinition.Defense;
        }

        TakeDamage(cityAttack);
        targetCity.Siege(this.UnitDefinition.Attack, this.ownerCountry);
    }

    private void HandleCombat(Unit targetUnit)
    {
        targetUnit.TakeDamage(UnitDefinition.Attack);
        TakeDamage(targetUnit.UnitDefinition.Attack);
    }

    public void TakeDamage(int attack)
    {
        int potentialDamage = attack - UnitDefinition.Defense;
        potentialDamage = Mathf.Max(potentialDamage, 0);

        remainingHp -= potentialDamage;

        if(remainingHp <= 0)
        {
            DestroyUnit();
        }
    }

    public void RegenHealth()
    {
        if(remainingHp != UnitDefinition.MaxHP)
        {
            remainingHp += 2;
            remainingHp = Mathf.Min(remainingHp, UnitDefinition.MaxHP);
        }
    }
    bool CanMoveTo(HexTile targetHex)
    {
        if(remainingMovement == 0)
            return false;

        if(!currentTile.IsTileAdjacent(targetHex))
            return false;

        if(targetHex.hexType == HexType.Water && !UnitDefinition.Abilities.Contains(UnitAbility.Embark))
            return false;

        return true;
    }
    void ExecuteMove(HexTile targetHex)
    {
        remainingMovement--;
        currentTile.RemoveUnit();
        targetHex.SetUnit(this);
        currentTile = targetHex;

        RevealSurroundingTiles();
    }

    public void EndTurn()
    {
        ResetMoves();
        RegenHealth();
    }

    public void ResetMoves()
    {
        remainingMovement = UnitDefinition.MaxMovement;
    }

    public void RevealSurroundingTiles()
    {
        List<HexTile> nearbyTiles = MapGenerator.Instance.allTiles
            .Where(tile =>
                Vector2.Distance(tile.transform.position, currentTile.transform.position) <= 3f && tile.isExplored[ownerCountry] == false)
            .ToList();

        foreach (HexTile tile in nearbyTiles)
        {
            tile.Reveal(ownerCountry);
        }
    }
}
