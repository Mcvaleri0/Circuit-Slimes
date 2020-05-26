using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using Game;



public class BackToMenu : MonoBehaviour
{
    #region /* Button Attributes */

    private Button BackButton;
    private Text BackButtonText;
    private Rect BackButtonRect;

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
        BackButton = transform.GetComponent<Button>();
        
        BackButton.onClick.AddListener(() => {
            //this.Controller.AnalyticsController.LevelQuit();
            this.Controller.LoadScene(GameController.MAIN_MENU);
        });
    }

    #endregion
}
