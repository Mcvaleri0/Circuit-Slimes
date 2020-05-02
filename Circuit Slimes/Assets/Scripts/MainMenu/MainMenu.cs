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
        this.Controller = GameObject.Find("GameController").GetComponent<GameController>();

        GameObject.DontDestroyOnLoad(this.Controller);
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayButton = transform.Find("Play Button").GetComponent<Button>();
        LevelEditButton = transform.Find("LevelEdit Button").GetComponent<Button>();
        ReadMeButton = transform.Find("ReadMe Button").GetComponent<Button>();

        ReadMeButtonText = ReadMeButton.GetComponentInChildren<Text>();

        ReadMeInfo = transform.Find("ReadMe Info");

        PlayButton.onClick.AddListener(() => this.Controller.LoadScene(GameController.LEVELS));
        LevelEditButton.onClick.AddListener(() => this.Controller.LoadScene(GameController.CREATOR));

        ReadMeButton.onClick.AddListener(() => ReadMeCallBack());
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
