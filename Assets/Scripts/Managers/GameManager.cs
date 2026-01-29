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
    public List<CountryType> countryTypes;
    public List<Country> countries = new List<Country>();
    public Country playerCountry {get; private set; }

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

        countryTypes = Enum.GetValues(typeof(CountryType))
                       .Cast<CountryType>()
                       .ToList();    
    }

    public void StartGame(CountryType playerCountryType)
    {
        MapGenerator.Instance.GenerateMap();
        SetupCountries(playerCountryType);
        foreach(HexTile tile in MapGenerator.Instance.allTiles)
        {
            tile.SetStartingVisibility();
        }
        playerCountry.units[0].RevealSurroundingTiles();
    }
    private void SetupCountries(CountryType playerCountryType)
    {
        generateAllCountries(playerCountryType);

        UIManager.Instance.UpdateIncome(playerCountry);
        UIManager.Instance.UpdateVictoryPoints(countries);
    }
    void generateAllCountries(CountryType playerCountryType)
    {
        playerCountry = generateCountry(playerCountryType);
        CameraManager.Instance.MoveCameraToHex(playerCountry.cities[0].cityCenterTile);

        if(GameRuleNoIdenticalCountriesAtGeneration)
        {
            List<CountryType> availableTypes = new List<CountryType>(countryTypes);
            availableTypes.Remove(playerCountryType);
            
            Shuffle(availableTypes);                 

            for (int i = 1; i < PlayerCount; i++)
            {
                CountryType countryType = availableTypes[i - 1]; 
                generateCountry(countryType);
            }
        } 
        else
        {
            CountryType countryType = countryTypes[UnityEngine.Random.Range(0, countryTypes.Count)];
            generateCountry(countryType);
        }
    }

    Country generateCountry(CountryType countryType)
    {
        HexTile StartingTile = FindStartingTile();
        Country country = new Country(countryType);
        City City = new City(StartingTile, country);
        Unit Scout = new Unit(UnitDatabase.Scout, StartingTile);

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
            Debug.Log(country.countryName + " Ends Turn");
            country.EndTurn();
            foreach(Unit unit in country.units)
            {
                unit.EndTurn();
            }

            foreach(City city in country.cities)
            {
                city.EndTurn();
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
