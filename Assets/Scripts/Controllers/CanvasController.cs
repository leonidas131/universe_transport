
using UnityEngine;
using TMPro;
using System.Globalization;
using System.Collections;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject pauseText;
    [SerializeField] private GameObject creatingUniverse;
    [SerializeField] private Toggle pauseToggle;
    [SerializeField] private Toggle playToggle;
    [SerializeField] private Toggle x2Toggle;
    [SerializeField] private Toggle x4Toggle;


    [Header("date time text")]
    [SerializeField]
    private TMP_Text yearText;
    [SerializeField]
    private TMP_Text monthText;
    [SerializeField]
    private TMP_Text dayText;
    [Header("Menus")]
    [SerializeField] private GameObject solarRouteMenu;
    [SerializeField] private SingleRouteMenu singleRouteMenu;

    [SerializeField] private GameObject constructionMenu;
    [SerializeField] private GameObject destructionMenu;

    [Header("Right menu")]
    [SerializeField] private GameObject rightBottomMenu;
    private Button[] rightMenuItems;
    [Header("Left menu")]
    [SerializeField] private GameObject leftBottomMenu;
    private Button[] leftMenuItems;

    private bool isPaused;
    private SolarClusterStruct[] solarClusters;

    [Header("constructions")]
    [SerializeField] private Transform constructionBttnListTransform;
    [SerializeField] private ConstructionBttn constructionBttnPrefab;
    [SerializeField] private GameObject shipYardStationPrefab;
    [SerializeField] private GameObject cargoStationPrefab;

    private void OnEnable()
    {
        UIEventHandler.PauseTextClicked.AddListener(PauseTextClicked);
        UIEventHandler.YearChanged.AddListener(OnYearChanged);
        UIEventHandler.MonthChanged.AddListener(OnMonthChanged);
        UIEventHandler.DayChanged.AddListener(OnDayChanged);
        PlayerManagerEventHandler.MapChangeEvent.AddListener(MapChanged);
        UIEventHandler.CreatingUniverse.AddListener(CreatingUniverse);
        UIEventHandler.SingleRouteItemClickedEvent.AddListener(SingleRouteMenu);
        UIEventHandler.RouteMenuOpenEvent.AddListener(SolarRouteMenuOpened);

        //GameControl buttons clicked
        pauseToggle.onValueChanged.AddListener(PauseButtonClicked);
        playToggle.onValueChanged.AddListener(PlayButtonClicked);
        x2Toggle.onValueChanged.AddListener(ToggleX2ButtonClicked);
        x4Toggle.onValueChanged.AddListener(ToggleX4ButtonClicked);
    }
    void Start()
    {
        rightMenuItems = rightBottomMenu.GetComponentsInChildren<Button>();
        leftMenuItems = leftBottomMenu.GetComponentsInChildren<Button>();
        RightMenuOpener(false);
        AddConstructionBttn(shipYardStationPrefab);
        AddConstructionBttn(cargoStationPrefab);
    }
    public void AddConstructionBttn(GameObject _prefab)
    {
        ConstructionBttn bttn = Instantiate(constructionBttnPrefab, constructionBttnListTransform);
        bttn.prefab = _prefab;
        bttn.gameObject.GetComponentInChildren<TMP_Text>().text = _prefab.GetComponent<IStation>().StationType.ToString();
        bttn.gameObject.GetComponent<Image>().sprite = _prefab.GetComponent<IConstructable>().BttnTexture;
    }
    private void OnDisable()
    {
        UIEventHandler.PauseTextClicked.RemoveListener(PauseTextClicked);
        UIEventHandler.YearChanged.RemoveListener(OnYearChanged);
        UIEventHandler.MonthChanged.RemoveListener(OnMonthChanged);
        UIEventHandler.DayChanged.RemoveListener(OnDayChanged);
        PlayerManagerEventHandler.MapChangeEvent.RemoveListener(MapChanged);
        UIEventHandler.CreatingUniverse.RemoveListener(CreatingUniverse);
        UIEventHandler.SingleRouteItemClickedEvent.RemoveListener(SingleRouteMenu);
        UIEventHandler.RouteMenuOpenEvent.RemoveListener(SolarRouteMenuOpened);

        pauseToggle.onValueChanged.RemoveListener(PauseButtonClicked);
        playToggle.onValueChanged.RemoveListener(PlayButtonClicked);
        x2Toggle.onValueChanged.RemoveListener(ToggleX2ButtonClicked);
        x4Toggle.onValueChanged.RemoveListener(ToggleX4ButtonClicked);
    }
    private void MapChanged(bool isOpened)
    {
        RightMenuOpener(isOpened);
    }
    private void RightMenuOpener(bool isOpened)
    {
        foreach (var item in rightMenuItems)
        {
            item.gameObject.SetActive(isOpened);
        }
    }
    public void SolarRouteMenuOpened()
    {
        if (solarRouteMenu.activeSelf) return;
        solarRouteMenu.SetActive(true);
        solarRouteMenu.GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
    }
    public void SolarRouteMenuClosed()
    {
        solarRouteMenu.GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
        solarRouteMenu.SetActive(false);
    }


    public void SingleRouteMenu(Route route, bool isActive)
    {
        singleRouteMenu.gameObject.SetActive(isActive);
        singleRouteMenu.UpdateDisplay(route, isActive);
        route.ShowRoute(isActive);
    }

    public void SingleRouteMenuClosed()
    {
        solarRouteMenu.GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
        UIEventHandler.RouteCreatingBegunEvent?.Invoke();
    }
    public void ConstructionMenuOpened()
    {
        if (constructionMenu.activeSelf) return;
        constructionMenu.SetActive(true);
        constructionMenu.GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();

        destructionMenu.SetActive(false);
    }
    public void ConstructionMenuClosed()
    {
        constructionMenu.GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
        constructionMenu.SetActive(false);
    }
    public void DestructionMenuOpened()
    {
        if (destructionMenu.activeSelf) return;
        destructionMenu.SetActive(true);
        if (constructionMenu.activeSelf)
        {
            UIEventHandler.ConstructionEndedEvent?.Invoke();
            constructionMenu.SetActive(false);
        }
    }
    public void DestructionMenuClosed()
    {
        destructionMenu.SetActive(false);
    }

    private void CreatingUniverse(bool isCreating)
    {
        creatingUniverse.SetActive(isCreating);
    }

    private IEnumerator WaitAndPrint(float waitTime, string text)
    {
        pauseText.SetActive(true);
        pauseText.GetComponent<TMP_Text>().text = text;
        yield return new WaitForSecondsRealtime(waitTime);
        if (!isPaused) pauseText.SetActive(false);
    }
    #region game buttons 
    public void PauseButtonClicked(bool isChanged)
    {
        UIEventHandler.PauseButtonClicked?.Invoke(Vector2.up);
        isPaused = true;
        StopAllCoroutines();
        StartCoroutine(WaitAndPrint(2f, "Game Paused..."));
    }
    public void PlayButtonClicked(bool isChanged)
    {
        UIEventHandler.PauseButtonClicked?.Invoke(Vector2.down);
        isPaused = false;
        StopAllCoroutines();
        StartCoroutine(WaitAndPrint(2f, "Game Speed x" + 1));
    }
    public void ToggleX2ButtonClicked(bool isChanged)
    {
        UIEventHandler.PauseButtonClicked?.Invoke(Vector2.left);
        isPaused = false;
        StopAllCoroutines();
        StartCoroutine(WaitAndPrint(2f, "Game Speed x" + 2));
    }
    public void ToggleX4ButtonClicked(bool isChanged)
    {
        UIEventHandler.PauseButtonClicked?.Invoke(Vector2.right);
        isPaused = false;
        StopAllCoroutines();
        StartCoroutine(WaitAndPrint(2f, "Game Speed x" + 4));
    }
    private void PauseTextClicked(float time)
    {
        switch (time)
        {
            case 0:
                isPaused = true;
                StopAllCoroutines();
                StartCoroutine(WaitAndPrint(2f, "Game Paused..."));
                pauseToggle.isOn = true;
                break;
            case 1:
                isPaused = false;
                StopAllCoroutines();
                StartCoroutine(WaitAndPrint(2f, "Game Speed x" + (int)time));
                playToggle.isOn = true;
                break;
            case 2:
                isPaused = false;
                StopAllCoroutines();
                StartCoroutine(WaitAndPrint(2f, "Game Speed x" + (int)time));
                x2Toggle.isOn = true;
                break;
            case 4:
                isPaused = false;
                StopAllCoroutines();
                StartCoroutine(WaitAndPrint(2f, "Game Speed x" + (int)time));
                x4Toggle.isOn = true;
                break;
            default:
                StopAllCoroutines();
                StartCoroutine(WaitAndPrint(2f, "Game Paused..."));
                pauseToggle.isOn = true;
                break;
        }
    }
    #endregion

    #region game date change
    private void OnYearChanged(int year)
    {
        yearText.text = year.ToString();
    }
    private void OnMonthChanged(int month)
    {
        monthText.text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
    }
    private void OnDayChanged(int day)
    {
        dayText.text = day.ToString();
    }
    #endregion
}
