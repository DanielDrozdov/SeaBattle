﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManagerController : MonoBehaviour
{
    public void OnClickButton() {
        SceneManager.LoadScene("MainMenu");
    }
}
