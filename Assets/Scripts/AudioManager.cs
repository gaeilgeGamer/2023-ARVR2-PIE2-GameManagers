using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] soundEffects;
    public AudioSource backgroundMusicSource;

    private AudioSource soundEffectSource;

    private void Start()
    {
        // Get the AudioSource component for playing sound effects
        soundEffectSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(string clipName)
    {
        // Find the AudioClip with the specified name
        AudioClip clip = FindClipByName(clipName, soundEffects);

        // If a matching AudioClip was found, play it
        if (clip != null)
        {
            soundEffectSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Could not find sound effect clip with name: " + clipName);
        }
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        backgroundMusicSource.clip = clip;
        backgroundMusicSource.Play();
    }

    private AudioClip FindClipByName(string clipName, AudioClip[] clips)
    {
        foreach (AudioClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }

        return null;
    }
public float GetSoundEffectDuration(string soundEffectName)
{
    foreach (AudioClip clip in soundEffects)
    {
        if (clip.name == soundEffectName)
        {
            return clip.length;
        }
    }

    Debug.LogWarning($"Sound effect '{soundEffectName}' not found!");
    return 0f;
}
}
