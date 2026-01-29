using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// Singleton
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TextMeshProUGUI tileInfoText;
    public TextMeshProUGUI cityInfoText;
    public TextMeshProUGUI unitInfoText;
    public TextMeshProUGUI victoryPointsText;
    public TextMeshProUGUI incomeText;
    public Image leftCountry;
    public Image selectedCountry;
    public Image rightCountry;
    public TextMeshProUGUI countryDescription;
    int carouselIndex = 0;

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
    public void UpdateTileInformation(HexTile tile)
    {
        if (tile == null)
        {
            MenuManager.Instance.HideTileInformation();
            MenuManager.Instance.HideUnitInformation();
            return;
        }

        MenuManager.Instance.ShowTileInformation();

        string owner = tile.city != null ? tile.city.ownerCountry.countryName : "None";

        if(tile.building == null) {tileInfoText.text = 
        $@"Selected Hex Info:
        Owner: {owner}
        Type: {tile.hexType}
        Gold: {tile.gold}
        Production: {tile.production}
        Food: {tile.food}
        Culture: {tile.culture}
        Science: {tile.science}";
        } else {
        tileInfoText.text = 
        $@"Selected Hex Info:
        Owner: {owner}
        Type: {tile.hexType}
        Building: {tile.building.BuildingDefinition.buildingType}
        Gold: {tile.gold}
        Production: {tile.production}
        Food: {tile.food}
        Culture: {tile.culture}
        Science: {tile.science}";
        }

        if (tile.unit != null)
        {
            UpdateUnitInformation(tile.unit);
        }
        else
        {
            MenuManager.Instance.HideUnitInformation();
        }

        if (tile.city != null)
        {
            UpdateCityInformation(tile.city);
        }
        else
        {
            MenuManager.Instance.HideCityInformation();
        }
    }
    public void UpdateUnitInformation(Unit unit)
    {
        if (unit == null)
        {
            MenuManager.Instance.HideUnitInformation();
            return;
        }

        MenuManager.Instance.ShowUnitInformation();

        string owner = unit.ownerCountry != null ? unit.ownerCountry.countryName : "None";

        unitInfoText.text =
        $@"Selected Unit Info:
        Owner: {owner}
        Type: {unit.UnitDefinition.UnitType}
        Moves: {unit.remainingMovement}
        HP: {unit.remainingHp}";
    }

    public void UpdateCityInformation(City city)
    {
        if (city == null)
        {
            MenuManager.Instance.HideCityInformation();
            return;
        }

        MenuManager.Instance.ShowCityInformation();

        string owner = city.ownerCountry != null ? city.ownerCountry.countryName : "None";
    
        if (owner == GameManager.Instance.playerCountry.countryName)
        {
            cityInfoText.text = 
            $@"Selected City Info:
            HP: {city.remainingHp}
            PRODUCTION: {city.productionPerTurn}
            PRODUCTION BUILDUP: {city.production}
            FOOD: {city.foodPerTurn}";

        } else
        {
            cityInfoText.text = 
            $@"Selected City Info:
            Owner: {owner}
            HP: {city.remainingHp}";
        }
    }
    public void UpdateVictoryPoints(List<Country> countries)
    {
        victoryPointsText.text = "";
        foreach (var country in countries)
        {
            if(country.score != 0){
            victoryPointsText.text += $"{country.countryName}: {country.score}\n";
            }
        }
    }
    public void UpdateIncome(Country country)
    {
        incomeText.text = $"Gold: {country.gold} + {country.goldPerTurn}\n" +
                          $"Science: {country.science} + {country.sciencePerTurn}\n" +
                          $"Culture: {country.culture} + {country.culturePerTurn} ";
    }
    CountryType GetCountryLeft()
    {
        int index = (carouselIndex - 1 + GameManager.Instance.countryTypes.Count) % GameManager.Instance.countryTypes.Count;
        return GameManager.Instance.countryTypes[index];
    }
    CountryType GetCountryRight()
    {
        int index = (carouselIndex + 1) %  GameManager.Instance.countryTypes.Count;
        return GameManager.Instance.countryTypes[index];
    }
    public CountryType GetCountrySelected()
    {
        return GameManager.Instance.countryTypes[carouselIndex];
    }

    public void SetUpCarosel()
    {
        leftCountry.color = GetCountryColor(GetCountryLeft());
        rightCountry.color = GetCountryColor(GetCountryRight());
        selectedCountry.color = GetCountryColor(GetCountrySelected());

        SetCountryDescription(GetCountrySelected());
    }
    public void RotateCaroselRight()
    {
        carouselIndex = (carouselIndex + 1) % GameManager.Instance.countryTypes.Count;
        SetUpCarosel();
    }
    public void RotateCaroselLeft()
    {
        carouselIndex = (carouselIndex - 1 + GameManager.Instance.countryTypes.Count) % GameManager.Instance.countryTypes.Count;
        SetUpCarosel();
    }
    private Color GetCountryColor(CountryType countryType)
    {
        switch (countryType)
        {
            case CountryType.Hungary: 
                return Color.red;
            case CountryType.Germany:
                return Color.black;
            case CountryType.Japan:
                return Color.white;
            default:
                return Color.grey;
        }
    }
    private void SetCountryDescription(CountryType countryType)
    {
        switch (countryType)
        {
            case CountryType.Hungary: 
                countryDescription.text = "Hungary";
                break;
            case CountryType.Germany:
                countryDescription.text = "Germany";
                break;
            case CountryType.Japan:
                countryDescription.text = "Japan";
                break;
            default:
                break;
        }
    }
}
