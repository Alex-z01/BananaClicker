using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public List<AudioClip> UI_Clips = new List<AudioClip>();
    public List<AudioClip> Banana_Clip = new List<AudioClip>();

    public void Mute()
    {
        AudioListener.volume = AudioListener.volume == 0 ? AudioListener.volume = 1f : AudioListener.volume = 0f;
    }
}
