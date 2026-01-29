using UnityEngine;
// Singleton
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    public GameObject MainMenuUI;
    public GameObject CountrySelectUI;
    public GameObject GameplayUI;
    public GameObject IncomeInformation;
    public GameObject VictoryInformation;
    public GameObject UnitInformation;
    public GameObject TileInformation;
    public GameObject CityInformation;
    public GameObject victoryText;

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
    public void OnCreateGamePress()
    {
        MainMenuUI.SetActive(false);
        CountrySelectUI.SetActive(true);

        UIManager.Instance.SetUpCarosel();
    }
    public void OnOptionsPress()
    {
        
    }
    public void OnQuitPress()
    {
        Application.Quit();
    }
    public void OnReturnPress()
    {
        CountrySelectUI.SetActive(false);
        MainMenuUI.SetActive(true);
    }
    public void OnSelectCountryPress()
    {
        CountrySelectUI.SetActive(false);
        MainMenuUI.SetActive(false);

        GameManager.Instance.StartGame(UIManager.Instance.GetCountrySelected());

        GameplayUI.SetActive(true);
    }

    public void OnEndTurnPress()
    {
        GameManager.Instance.EndTurn();
    }
    public void ShowUnitInformation()
    {
        UnitInformation.SetActive(true);
    }
    public void ShowTileInformation()
    {
        TileInformation.SetActive(true);
    }

    public void HideUnitInformation()
    {
        UnitInformation.SetActive(false);
    }

    public void HideTileInformation()
    {
        TileInformation.SetActive(false);
    }

    public void HideCityInformation()
    {
        CityInformation.SetActive(false);
    }

    public void ShowCityInformation()
    {
        CityInformation.SetActive(true);
    }
    public void OnLeftCountryPress()
    {
        UIManager.Instance.RotateCaroselLeft();
    }
    public void OnRightCountryPress()
    {
        UIManager.Instance.RotateCaroselRight();
    }
    public void OnVictory()
    {
        victoryText.SetActive(true);
    }
}
