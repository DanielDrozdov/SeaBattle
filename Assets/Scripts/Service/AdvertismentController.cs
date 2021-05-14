using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertismentController : MonoBehaviour
{
    private static AdvertismentController Instance;

    private void Awake() {
        Instance = this;
        Advertisement.Initialize("4128495", false);
        StartCoroutine(ShowBannerWhenReady());
    }

    public static AdvertismentController GetInstance() {
        return Instance;
    }

    private IEnumerator ShowBannerWhenReady() {
        while(!Advertisement.IsReady("bannerPlacement")) {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show("bannerPlacement");
    }
}
