using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    Player player;
    PlayerControls controls;
    Text _pauseText;
    Button _firstButton;
    public GameObject controlsInfo;
    public GameObject buttons;
    public string pauseText
    {
        get => _pauseText.text;
        set => _pauseText.text = value;
    }
    #region Input systems
    public void OnEnable()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.UI.Enable();
        if (player)
            player.enabled = false;
        if (_firstButton)
            EventSystem.current.SetSelectedGameObject(_firstButton.gameObject);
    }
    public void OnDisable()
    {
        controls.UI.Disable();
    }
    public void OnMenuExit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (controlsInfo.activeInHierarchy)
            {
                controlsInfo.SetActive(false);
                buttons.SetActive(true);
            }
            else
            {
                ExitMenu();
            }
        }
    }

    public void ExitMenu()
    {
        if (controlsInfo.activeInHierarchy)
        {
            controlsInfo.SetActive(false);
            buttons.SetActive(true);
        }
        if (player)
            player.enabled = true;
        player.OnInventoryMenuExit();
    }

    public void OnSelect(InputAction.CallbackContext context)
    {

    }

    #endregion

    public void OnControlMenu()
    {
        controlsInfo.SetActive(true);
        buttons.SetActive(false);
    }

    public void onGameExit()
    {
        WorldManager.Instance.SaveState();
        Application.Quit();
    }
    void Start()
    {
        player = Player.Instance;

        Player.hookInputAction(controls.UI.MenuExit, OnMenuExit);
        Player.hookInputAction(controls.UI.Cancel, OnMenuExit);

        _pauseText = GetComponentInChildren<Text>();
        _firstButton = GetComponentInChildren<Button>();
        gameObject.SetActive(false);
    }
}
