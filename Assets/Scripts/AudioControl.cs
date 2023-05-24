using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public List<AudioClip> UI_Clips = new List<AudioClip>();
    public List<AudioClip> Banana_Clip = new List<AudioClip>();
    public List<AudioClip> Music = new List<AudioClip>();

    public AudioSource music;
    public AudioSource sfx;

    public void MuteMusic()
    {
        music.mute = !music.mute;
    }

    public void MuteSFX()
    {
        sfx.mute = !sfx.mute;
    }
}
