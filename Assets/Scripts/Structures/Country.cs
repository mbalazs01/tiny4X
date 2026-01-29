using System;
using System.Collections.Generic;
using UnityEngine;

public class Country
{
    public CountryType countryType {get; private set;}
    public int goldPerTurn {get; private set; }
    public int gold { get; private set;} = 0;
    public int culturePerTurn {get; private set; }
    public int culture { get; private set;} = 0;
    public int sciencePerTurn {get; private set; }
    public int science { get; private set;} = 0;
    public bool isHuman {get; private set;} = false;
    public string countryName {get; private set;}
    public Color countryColor {get; private set;}
    public List<City> cities { get; private set; } = new List<City>();
    public List<Unit> units { get; private set; } = new List<Unit>(); 
    public int score { get; private set; } = 0;
    public Country (CountryType countryType, bool playerControlled)
    {
        isHuman = playerControlled;
        SetStats(countryType);
    }
    public Country (CountryType countryType)
    {
        SetStats(countryType);
    }
    public void AddCity(City city)
    {
        cities.Add(city);
    }
    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }
    public void RemoveCity(City city)
    {
        cities.Remove(city);
    }
    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
    }
    private void SetStats(CountryType countryType)
    {
        switch (countryType)
        {
            case CountryType.Hungary: 
                countryName = "Hungary";
                countryColor = Color.red;
                break;
            case CountryType.Germany:
                countryName = "Germany";
                countryColor = Color.black;
                break;
            case CountryType.Japan:
                countryName = "Japan";
                countryColor = Color.white;
                break;
            default:
                break;
        }
    }
    public void EndTurn()
    {
        Debug.Log("Trying to Do Turn End Country Logic");
        gold += goldPerTurn;
        science += sciencePerTurn;
        culture += culturePerTurn;
    } 
    internal void ChangeScoreBy(int scoreChange)
    {
        score += scoreChange;
    }
    internal void ChangeGoldIncomeBy(int incomeChange)
    {
        goldPerTurn += incomeChange;
    }
    internal void ChangeScienceIncomeBy(int incomeChange)
    {
        sciencePerTurn += incomeChange;
    }
    internal void ChangeCultureIncomeBy(int incomeChange)
    {
        culturePerTurn += incomeChange;
    }
}
