using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Game;



public class MainMenu : MonoBehaviour
{
    #region /* Buttons */

    private Canvas MenuCanvas;

    private Button PlayButton;
    private Button LevelEditButton;
    private Button ReadMeButton;

    private Button QuitButton;

    #endregion


    #region /* Read Me Info */

    private Text ReadMeButtonText;

    private Transform ReadMeInfo;

    #endregion


    #region /* GameController */

    private GameController Controller { get; set; }

    #endregion


    private MainMenuCameraController MenuCamera;

    private AudioManager AudioManager;



    #region === Unity Events ===
    private void Awake()
    {
        this.Controller = GameController.CreateGameController();

        this.MenuCamera = GameObject.Find("MenuCamera").GetComponent<MainMenuCameraController>();

        this.AudioManager = FindObjectOfType<AudioManager>();
    }


    // Start is called before the first frame update
    void Start()
    {
        PlayButton = transform.Find("Play Button").GetComponent<Button>();
        LevelEditButton = transform.Find("LevelEdit Button").GetComponent<Button>();
        ReadMeButton = transform.Find("ReadMe Button").GetComponent<Button>();
        QuitButton = GameObject.Find("QuitButton").GetComponent<Button>();

        ReadMeButtonText = ReadMeButton.GetComponentInChildren<Text>();
        ReadMeInfo = transform.Find("ReadMe Info");

        //CALLBACK FUNCTIONS
        PlayButton.onClick.AddListener(() => this.Controller.ShowLevelMenu(GameController.LEVELS));
        LevelEditButton.onClick.AddListener(() => this.Controller.ShowLevelMenu(GameController.CREATOR));
        ReadMeButton.onClick.AddListener(() => ReadMeCallBack());
        QuitButton.onClick.AddListener(() => this.Controller.QuitGame());
    }

    #endregion


    #region === Sound Functions ===

    public void InfoSound()
    {
        AudioManager.Play("ReadInfo");
    }

    public void HoverSound() {
        AudioManager.Play("ButtonHover2");
    }

    public  void ClickSound()
    {
        AudioManager.PlayRandom("ButtonClick1", "ButtonClick2", "ButtonClick3");
    }

    void ReadMeCallBack()
    {

        if (!ReadMeInfo.gameObject.activeSelf)
        {
            ReadMeButtonText.text = "Close";
            ReadMeButtonText.color = new Color32(212, 81, 85, 255); //red

            ReadMeInfo.gameObject.SetActive(true);

            PlayButton.interactable = false;
            LevelEditButton.interactable = false;

            this.MenuCamera.GoToInfo();
        }
        else
        {
            ReadMeButtonText.text = "Read Me";
            ReadMeButtonText.color = new Color32(172, 255, 226, 255); //green

            ReadMeInfo.gameObject.SetActive(false);

            PlayButton.interactable = true;
            LevelEditButton.interactable = true;

            this.MenuCamera.GoToMenu();
        }
    }

    #endregion

}
