using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesControllersManager : MonoBehaviour
{
    [SerializeField] WavesFieldController firstWavesFieldController;
    [SerializeField] WavesFieldController secondWavesFieldController;
    private static WavesControllersManager Instance;

    private void Awake() {
        Instance = this;
    }

    public static WavesControllersManager GetInstance() {
        return Instance;
    }

    public void Initialize() {
        firstWavesFieldController.LocateWavesOnField();
        secondWavesFieldController.LocateWavesOnField();
    }
}
