using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightGameSoundsController : MonoBehaviour {

    [SerializeField] private AudioSource hitShotAudioSource;
    [SerializeField] private AudioSource missShotAudioSource;
    [SerializeField] private AudioSource[] destroyedShipSources;
    [SerializeField] private AudioClip shipDestroySoundClip;
    [SerializeField] private AudioClip missShotSoundClip;
    [SerializeField] private AudioClip hitShotSoundClip;
    private static FightGameSoundsController Instance;

    private void Awake() {
        Instance = this;
    }

    public static FightGameSoundsController GetInstance() {
        return Instance;
    }

    public void PlayDestroyShipSound() {
        for(int i = 0;i < destroyedShipSources.Length;i++) {
            if(!destroyedShipSources[i].isPlaying) {
                destroyedShipSources[i].clip = shipDestroySoundClip;
                destroyedShipSources[i].Play();
            }
        }
    }

    public void PlayMissShotSound() {
        missShotAudioSource.clip = missShotSoundClip;
        missShotAudioSource.Play();
    }

    public void PlayHitShotSound() {
        hitShotAudioSource.clip = hitShotSoundClip;
        hitShotAudioSource.Play();
    }
}
