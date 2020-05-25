using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{

    private AudioManager AudioManager;

    // Start is called before the first frame update
    void Start()
    {
        this.AudioManager = FindObjectOfType<AudioManager>();
    }

    public void HoverSound()
    {
        AudioManager.Play("ButtonHover2");
    }

    public void ClickSound()
    {
        AudioManager.PlayRandom("ButtonClick1", "ButtonClick2", "ButtonClick3");
    }

    public void GrabSound()
    {
        AudioManager.PlayRandom("ButtonGrab");
    }

    public void ReleaseSound()
    {
        AudioManager.PlayRandom("ButtonRelease");
    }
}
