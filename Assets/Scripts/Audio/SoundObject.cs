using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundObject : MonoBehaviour
{
    public AudioClip[] Sounds;

    private AudioSource _audioSource => GetComponent<AudioSource>();

    public void PlaySound(AudioClip sound, float volume = 1f)
    {
        _audioSource.PlayOneShot(sound, volume);
    }
    public void StopSound(AudioClip sound, float volume = 1f)
    {
        _audioSource.Stop();
    }
}
