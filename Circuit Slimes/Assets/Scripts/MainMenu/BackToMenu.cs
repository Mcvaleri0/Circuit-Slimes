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

    // Start is called before the first frame update
    void Start()
    {
        BackButton = transform.GetComponent<Button>();
        BackButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("MainMenu"));

    }
}
