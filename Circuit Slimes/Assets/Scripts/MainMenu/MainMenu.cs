using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Game;



public class MainMenu : MonoBehaviour
{
    #region /* Buttons */

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



    #region === Unity Events ===
    private void Awake()
    {
        this.Controller = GameController.CreateGameController();
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

        PlayButton.onClick.AddListener(() => this.Controller.ChooseLevel(GameController.LEVELS));
        LevelEditButton.onClick.AddListener(() => this.Controller.ChooseLevel(GameController.CREATOR));

        ReadMeButton.onClick.AddListener(() => ReadMeCallBack());

        QuitButton.onClick.AddListener(() => this.Controller.QuitGame());
    }

    #endregion


    #region === CallBack Functions ===

    void ReadMeCallBack()
    {
        if (!ReadMeInfo.gameObject.activeSelf)
        {
            ReadMeButtonText.text = "Close";
            ReadMeButtonText.color = new Color32(212, 81, 85, 255); //red

            ReadMeInfo.gameObject.SetActive(true);

            PlayButton.enabled = false;
            LevelEditButton.enabled = false;
        }
        else
        {
            ReadMeButtonText.text = "Read Me";
            ReadMeButtonText.color = new Color32(6, 183, 128, 255); //green

            ReadMeInfo.gameObject.SetActive(false);

            PlayButton.enabled = true;
            LevelEditButton.enabled = true;
        }

        
    }

    #endregion

}
