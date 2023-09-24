using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public static MenuManager instance = null;
    public GameObject back;
    public Animator fadeUI;
    public string fadeUI_trigger = "Fade";
    public float transition_time = 0.25f;

    public int whichPlayer = 0;
    public TMP_Text playerText = null;
    
    List<GameObject> menus_previous = new List<GameObject>();
    List<GameObject> menus_opened = new List<GameObject>();

    public Button[] p1_buttons = new Button[8];
    public Button[] p2_buttons = new Button[8];
    public Button[] p1_keys = new Button[12];
    public Button[] p2_keys = new Button[12];

    public GameObject bindingPanel = null;
    public TMP_Text bindText = null;
    public EventSystem eventSystem = null;

    private bool bindingKey = false;
    private bool bindingAxis = false;
    private bool bindingButton = false;

    private int actionBinding = -1;
    private int playerBinding = -1;

    private bool waiting = false;

    IEnumerator transition(GameObject open, GameObject close)
    {
        close.SetActive(false);
        fadeUI.SetTrigger(fadeUI_trigger);
        yield return new WaitForSeconds(transition_time);
        open.SetActive(true);
    }

    public void BackMenu()
    {
        StartCoroutine(transition(menus_previous[menus_previous.Count - 1], menus_opened[menus_opened.Count - 1]));
        //crash? assuming we aren't given back our pointer ref in time
        menus_opened.RemoveAt(menus_opened.Count - 1);
        menus_previous.RemoveAt(menus_previous.Count - 1);
        if (menus_previous.Count == 0)
            back.SetActive(false);

    }

    public void OnPlay()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Sample Scene");
    }
    public void SetPrevious(GameObject previous)
    {
        menus_previous.Insert(menus_previous.Count, previous);
    }

    public void OpenMenu(GameObject menu)
    {
        StartCoroutine(transition(menu, menus_previous[menus_previous.Count - 1]));
        
        menus_opened.Insert(menus_opened.Count, menu);
        
        back.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        UpdateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (InputManager.instance.CheckForPlayerInput(whichPlayer))
            {
                //TurnOff(false);
                //GameManager.instance.ResumeGameplay();
            }
        }
        if (bindingKey || bindingButton) //code for remapping keys and buttons
        {
            if (waiting)
            {
                if (Input.anyKey) return;
                if (InputManager.instance.DetectButtonPress() > -1) return;
                waiting = false;
            }
            else
            {
                if (bindingKey)
                {
                    foreach (KeyCode key in KeyCode.GetValues(typeof(KeyCode)))
                    {
                        if (!key.ToString().Contains("Joystick"))
                        {
                            if (Input.GetKeyDown(key))// key is pressed
                            {
                                if (bindingAxis)
                                    InputManager.instance.BindPlayerAxisKey(playerBinding, actionBinding, key);
                                else
                                    InputManager.instance.BindPlayerKey(playerBinding, actionBinding, key);
                                bindingPanel.SetActive(false);
                                bindingKey = false;
                                bindingButton = false;
                                eventSystem.gameObject.SetActive(true);
                                UpdateButtons();
                            }
                            else if (bindingButton)
                            {
                                int button = InputManager.instance.DetectButtonPress();
                                if (button > -1) //button pressed
                                {
                                    InputManager.instance.BindPlayerButton(playerBinding, actionBinding, (byte)button);
                                    bindingPanel.SetActive(false);

                                    bindingKey = false;
                                    bindingButton = false;
                                    eventSystem.gameObject.SetActive(true);
                                    UpdateButtons();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    void UpdateButtons()
    {
        print(InputManager.instance);
        //controller buttons
        for (int b = 0; b < 8; b++)
        {
            p1_buttons[b].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetButtonName(0, b);
            p2_buttons[b].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetButtonName(1, b);
        }

        //key buttons
        for (int k = 0; k < 8; k++)
        {
            p1_keys[k].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetKeyName(0, k);
            p2_keys[k].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetKeyName(1, k);
        }

        //key axes
        for (int a = 0; a < 4; a++)
        {
            p1_keys[a+8].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetKeyAxisName(0, a);
            p2_keys[a+8].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetKeyAxisName(1, a);
        }
    }

    public void BindP1Key(int actionID)
    {

        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for player 1 " + InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = true;
        bindingAxis = false;
        bindingButton = false;
        playerBinding = 0;
        actionBinding = actionID;

        waiting = true;
    }
    public void BindP1AxisKeys(int actionID)
    {

        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for player 1 " + InputManager.axisNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = true;
        bindingAxis = true;
        bindingButton = false;
        playerBinding = 0;
        actionBinding = actionID;

        waiting = true;
    }
    public void BindP1Button(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a button for player 1 "+InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = false;
        bindingAxis = false;
        bindingButton = true;
        playerBinding = 0;
        actionBinding = actionID;

        waiting = true;
    }

    public void BindP2Key(int actionID)
    {

        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for player 2 " + InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = true;
        bindingAxis = false;
        bindingButton = false;
        playerBinding = 1;
        actionBinding = actionID;

        waiting = true;
    }
    public void BindP2AxisKeys(int actionID)
    {

        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for player 2 " + InputManager.axisNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = true;
        bindingAxis = true;
        bindingButton = false;
        playerBinding = 1;
        actionBinding = actionID;

        waiting = true;
    }
    public void BindP2Button(int actionID)
    {

        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a button for player 2 " + InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = false;
        bindingAxis = false;
        bindingButton = true;
        playerBinding = 1;
        actionBinding = actionID;

        waiting = true;
    }
}

