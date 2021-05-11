using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ChangeLanguageDropDownController : MonoBehaviour {

    private TMP_Dropdown dropdown;

    private void Start() {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    public void ChangeLanguage() {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[dropdown.value];
    }

    //private IEnumerator SetLanguageCoroutine() {
    //    while(LocalizationSettings.InitializationOperation == null) {

    //    }
    //}
}
