using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BackToMenu : MonoBehaviour
{
    private Button BackButton;
    private Text BackButtonText;
    private Rect BackButtonRect;

    private enum State
    {
        Normal,
        Confirmation
    }

    State ButtonState = State.Normal;

    // Start is called before the first frame update
    void Start()
    {
        BackButton = transform.GetComponent<Button>();
        BackButtonText = BackButton.GetComponentInChildren<Text>();
        BackButtonRect = BackButton.GetComponent<RectTransform>().rect;

        BackButton.onClick.AddListener(() => this.CallBack());

    }

    void CallBack()
    {
        switch (ButtonState)
        {
            case State.Normal:

                BackButtonText.text = "Click Again to Confirm";
                BackButtonText.color = Color.white;

                var colors = BackButton.colors;
                colors.selectedColor = new Color32(212, 81, 85, 255);
                BackButton.colors = colors;

                ButtonState = State.Confirmation;

                break;

            case State.Confirmation:

                SceneManager.LoadSceneAsync("MainMenu");

                break;
        }
    }

    public void Reset()
    {
        BackButtonText.text = "Back To Menu";
        BackButtonText.color = new Color32(101,101,101,255);

        var colors = BackButton.colors;
        colors.selectedColor = Color.white;
        BackButton.colors = colors;

        ButtonState = State.Normal;
    }
}
