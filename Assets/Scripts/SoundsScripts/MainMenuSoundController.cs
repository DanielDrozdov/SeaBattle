using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSoundController : MonoBehaviour
{
    [SerializeField] private AudioSource secondaryAudioSource;
    [SerializeField] private AudioClip buttonClickClip;
    private static MainMenuSoundController Instance;

    private void Awake() {
        Instance = this;
    }

    public static MainMenuSoundController GetInstance() {
        return Instance;
    }

    public void PlayButtonClickSound() {
        secondaryAudioSource.clip = buttonClickClip;
        secondaryAudioSource.Play();
    }
}
