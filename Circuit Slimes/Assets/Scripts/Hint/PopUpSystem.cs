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

    public void PopUp(string TutorialText, string TutorialTitle)
    {
        popUpBox.SetActive(true);
        popUpText.text = TutorialText;
        title.text = TutorialTitle;
        animator.SetTrigger("PopUp");
    }

}
