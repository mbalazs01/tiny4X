using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.Mathematics;
using UnityEngine;

public class Unit
{
    public enum UnitType {Settler, Scout, Warrior, Archer, CommercialShip, WarShip }
    public enum UnitAbility {Settle, Embark, Attack, RangedAttack, ExploitResource}
    public UnitType unitType { get; private set; }
    public int maxHp { get; private set; }
    public int remainingHp { get; private set; }
    public int attack { get; private set; }
    public int defense { get; private set; }
    public int maxMovement { get; private set; }
    public int remainingMovement { get; private set; }
    public int unitScore { get; private set; }
    public HexTile currentTile { get; private set; }
    public City ownerCity { get; private set; }
    public Country ownerCountry { get; private set; }
    private HashSet<UnitAbility> abilities = new HashSet<UnitAbility>();
    public Unit(UnitType type, HexTile tile, City city)
    {
        unitType = type;
        currentTile = tile;
        ownerCity = city;
        ownerCountry = city.ownerCountry;
        ownerCountry.AddUnit(this);

        SetStats(type);

        currentTile.SetUnit(this);
        ownerCountry.addScore(unitScore);
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
            ownerCountry.addScore(-unitScore);
            ownerCountry = null;
        }

        ownerCity = null;
    }
    public bool HasAbility(UnitAbility ability)
    {
        return abilities.Contains(ability);
    }
    private void SetStats(UnitType unitType)
    {
        maxHp = 1;
        attack = 0;
        defense = 0;
        maxMovement = 0;
        unitScore = 1;
        abilities.Clear();

        switch (unitType)
        {
            case UnitType.Settler: 
                unitScore = 0;
                maxMovement = remainingMovement = 2;
                abilities.Add(UnitAbility.Settle);
                break;
            case UnitType.Scout:
                maxHp = remainingHp = 5; 
                attack = 2;
                defense = 0;
                maxMovement = remainingMovement = 3;
                abilities.Add(UnitAbility.Attack);
                break;
            case UnitType.Warrior: 
                maxHp = remainingHp = 10; 
                attack = 5;
                defense = 2;
                maxMovement = remainingMovement = 2;
                abilities.Add(UnitAbility.Attack);
                break;
            case UnitType.Archer: 
                maxHp = remainingHp = 5; 
                attack = 4;
                defense = 1;
                maxMovement = remainingMovement = 2;
                abilities.Add(UnitAbility.RangedAttack);
                break;
            case UnitType.CommercialShip: 
                unitScore = 0;
                maxMovement = remainingMovement = 4;
                abilities.Add(UnitAbility.Embark);
                abilities.Add(UnitAbility.ExploitResource);
                break;
            case UnitType.WarShip: 
                maxHp = remainingHp = 10; 
                attack = 3;
                defense = 2;
                maxMovement = remainingMovement = 4;
                abilities.Add(UnitAbility.Embark);
                abilities.Add(UnitAbility.RangedAttack);
                break;
            default:
                break;
        }
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
            cityAttack += targetCity.cityCenterTile.unit.defense;
        }

        TakeDamage(cityAttack);
        targetCity.Siege(this.attack, this.ownerCountry);
    }

    private void HandleCombat(Unit targetUnit)
    {
        targetUnit.TakeDamage(attack);
        TakeDamage(targetUnit.attack);
    }

    public void TakeDamage(int attack)
    {
        int potentialDamage = attack - defense;
        potentialDamage = Mathf.Max(potentialDamage, 0);

        remainingHp -= potentialDamage;

        if(remainingHp <= 0)
        {
            DestroyUnit();
        }
    }

    public void RegenHealth()
    {
        if(remainingHp != maxHp)
        {
            remainingHp += 2;
            remainingHp = Mathf.Min(remainingHp, maxHp);
        }
    }
    bool CanMoveTo(HexTile targetHex)
    {
        if(remainingMovement == 0)
            return false;

        if(!currentTile.IsTileAdjacent(targetHex))
            return false;

        if(targetHex.hexType == HexTile.HexType.Water && !abilities.Contains(UnitAbility.Embark))
            return false;

        return true;
    }
    void ExecuteMove(HexTile targetHex)
    {
        remainingMovement--;
        currentTile.RemoveUnit();
        targetHex.SetUnit(this);
        currentTile = targetHex;
    }

    public void ResetMoves()
    {
        remainingMovement = maxMovement;
    }
}
