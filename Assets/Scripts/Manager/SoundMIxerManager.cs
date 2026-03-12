using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    public void setMasterVolume(float level) {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level)* 20f);
    }
    public void setSoundFXVolume(float level) {
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(level)* 20f);
    }
    public void setMusicVolume(float level) {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level)* 20f);
    }

}
