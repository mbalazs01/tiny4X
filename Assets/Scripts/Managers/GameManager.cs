using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// Singleton
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public HexTile selectedTile {get; private set; }
    public List<Country.CountryType> countryTypes;
    private List<Country> countries = new List<Country>();
    private Country playerCountry;

    // Gamerule Related Variables
    public bool GameRuleNoIdenticalCountriesAtGeneration {get; private set;} = true;
    public int PlayerCount {get; private set; } = 3;

    void Start()
    {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        countryTypes = Enum.GetValues(typeof(Country.CountryType))
                       .Cast<Country.CountryType>()
                       .ToList();    
    }

    public void StartGame(Country.CountryType playerCountryType)
    {
        MapGenerator.Instance.GenerateMap();
        SetupCountries(playerCountryType);
    }
    private void SetupCountries(Country.CountryType playerCountryType)
    {
        generateAllCountries(playerCountryType);

        UIManager.Instance.UpdateIncome(playerCountry);
        UIManager.Instance.UpdateVictoryPoints(countries);
    }
    void generateAllCountries(Country.CountryType playerCountryType)
    {
        playerCountry = generateCountry(playerCountryType);
        CameraManager.Instance.MoveCameraToHex(playerCountry.cities[0].cityCenterTile);

        if(GameRuleNoIdenticalCountriesAtGeneration)
        {
            List<Country.CountryType> availableTypes = new List<Country.CountryType>(countryTypes);
            availableTypes.Remove(playerCountryType);
            
            Shuffle(availableTypes);                 

            for (int i = 1; i < PlayerCount; i++)
            {
                Country.CountryType countryType = availableTypes[i - 1]; 
                generateCountry(countryType);
            }
        } 
        else
        {
            Country.CountryType countryType = countryTypes[UnityEngine.Random.Range(0, countryTypes.Count)];
            generateCountry(countryType);
        }
    }

    Country generateCountry(Country.CountryType countryType)
    {
        HexTile StartingTile = FindStartingTile();
        Country country = new Country(countryType);
        City City = new City(StartingTile, country);
        Unit Warrior = new Unit(Unit.UnitType.Warrior, StartingTile, City);

        countries.Add(country);

        return country;
    }
    HexTile FindStartingTile()
    {
        List<HexTile> suitableHexes = MapGenerator.Instance.landTiles
                .Where(h => h.city == null)
                .ToList();
        
        return suitableHexes[UnityEngine.Random.Range(0, suitableHexes.Count)];
    }
    void Update()
    {
        HandleClick();
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HexTile clickedHex = ClickManager.GetClickedHex();

            SelectTile(clickedHex);
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            HexTile clickedHex = ClickManager.GetClickedHex();
            if (clickedHex != null && selectedTile != null && selectedTile.unit != null)
            {
                if(clickedHex != selectedTile && selectedTile.unit.ownerCountry == playerCountry)
                {
                    selectedTile.unit.Move(clickedHex);
                    SelectTile(clickedHex);
                }
            } 
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void SelectTile(HexTile hexTile)
    {
        selectedTile = hexTile;
        UIManager.Instance.UpdateTileInformation(selectedTile);
    }
    public void EndTurn()
    {
        foreach(Country country in countries)
        {
            country.UpdateIncome();
            foreach(Unit unit in country.units)
            {
                unit.ResetMoves();
                unit.RegenHealth();
            }

            foreach(City city in country.cities)
            {
                city.RegenHealth();
            }
        }

        UIManager.Instance.UpdateIncome(playerCountry);   
        UIManager.Instance.UpdateVictoryPoints(countries);
    }
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

    internal void removeCountry(Country oldOwnerCountry)
    {
        countries.Remove(oldOwnerCountry);

        UIManager.Instance.UpdateVictoryPoints(countries);

        if(countries.Count == 1)
        {
            MenuManager.Instance.OnVictory();
        }
    }
}
