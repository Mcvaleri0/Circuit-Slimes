using UnityEngine.Audio;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class Sound {

	public string name;

	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume = .75f;
	[Range(0f, 1f)]
	public float volumeVariance = .1f;

	[Range(.1f, 3f)]
	public float pitch = 1f;
	[Range(0f, 1f)]
	public float pitchVariance = .1f;

	public bool loop = false;

	public AudioMixerGroup mixerGroup;

	[HideInInspector]
	public AudioSource source;

    //used for fades
    private float CurrentVolume;


    public IEnumerator FadeOut()
    {
        this.CurrentVolume = this.volume;

        for (float ft = this.CurrentVolume; ft >= 0; ft -= 0.1f)
        {
            this.CurrentVolume = ft;
            this.source.volume = this.CurrentVolume;
            yield return new WaitForSeconds(.1f);
        }

        //reset
        this.source.Stop();
        this.source.volume = this.volume;
        yield break;
    }

    public IEnumerator FadeIn()
    {
        this.CurrentVolume = 0.0f;
        this.source.volume = this.CurrentVolume;
        this.source.Play();

        for (float ft = this.CurrentVolume; ft <= this.volume; ft += 0.1f)
        {
            this.CurrentVolume = ft;
            this.source.volume = this.CurrentVolume;
            yield return new WaitForSeconds(.1f);
        }

        this.source.volume = this.volume;
        yield break;
    }
}
