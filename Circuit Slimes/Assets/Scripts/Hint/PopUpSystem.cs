using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpSystem: MonoBehaviour
{
    public GameObject popUpBox;
    public Animator animator;
    public Text popUpText;
    public Text title;

    public GameObject InputBlocker;

    public Game.GameController GameController;

    public bool GoToNextLevel = false;

    public bool Open = false;


    private void Start()
    {
        this.GameController = FindObjectOfType<Game.GameController>();
    }

    public void PopUp(string TutorialTitle, string TutorialText, bool goToNextLevel)
    {
        if (this.Open) return;

        this.Open = true;

        popUpBox.SetActive(true);
        popUpText.text = TutorialText;
        title.text = TutorialTitle;
        animator.SetTrigger("PopUp");

        InputBlocker.SetActive(true);

        GoToNextLevel = goToNextLevel;
    }


    public void closePopUp()
    {
        if (!this.Open) return;

        this.Open = false;

        animator.SetTrigger("close");
        InputBlocker.SetActive(false);

        if (this.GoToNextLevel)
        {
            this.GameController.NextLevel();
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            this.PopUp("hi there fellow", "hya buddy boi", false);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            this.PopUp("hi there fellow", "hya buddy boi", true);
        }
    }

}
