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

        tileInfoText.text = 
        $@"Selected Hex Info:
        Owner: {owner}
        Type: {tile.hexType}
        Gold: {tile.gold}
        Production: {tile.production}
        Culture: {tile.culture}
        Science: {tile.science}";

        if (tile.unit != null)
        {
            UpdateUnitInformation(tile.unit);
        }
        else
        {
            MenuManager.Instance.HideUnitInformation();
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
        Type: {unit.unitType}
        Moves: {unit.remainingMovement}
        HP: {unit.remainingHp}";
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
        incomeText.text = $"Gold: {country.gold}\n" +
                          $"Science: {country.science}\n" +
                          $"Culture: {country.culture}";
    }
    Country.CountryType GetCountryLeft()
    {
        int index = (carouselIndex - 1 + GameManager.Instance.countryTypes.Count) % GameManager.Instance.countryTypes.Count;
        return GameManager.Instance.countryTypes[index];
    }
    Country.CountryType GetCountryRight()
    {
        int index = (carouselIndex + 1) %  GameManager.Instance.countryTypes.Count;
        return GameManager.Instance.countryTypes[index];
    }
    public Country.CountryType GetCountrySelected()
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
    private Color GetCountryColor(Country.CountryType countryType)
    {
        switch (countryType)
        {
            case Country.CountryType.Hungary: 
                return Color.red;
            case Country.CountryType.Germany:
                return Color.black;
            case Country.CountryType.Japan:
                return Color.white;
            default:
                return Color.grey;
        }
    }
    private void SetCountryDescription(Country.CountryType countryType)
    {
        switch (countryType)
        {
            case Country.CountryType.Hungary: 
                countryDescription.text = "Hungary";
                break;
            case Country.CountryType.Germany:
                countryDescription.text = "Germany";
                break;
            case Country.CountryType.Japan:
                countryDescription.text = "Japan";
                break;
            default:
                break;
        }
    }
}
