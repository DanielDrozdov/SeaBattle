using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using System;

public class MobileNotificationsController : MonoBehaviour
{
    public static bool IsMessageCalledOnce;

    private void Awake() {
        if(Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer) {
            Destroy(this);
        }
        AndroidNotificationCenter.CancelAllNotifications();
    }

    private void CreateChannelAndSendNotification() {
            var channel = new AndroidNotificationChannel() {
                Id = "id_52572",
                Name = "Return to game",
                Importance = Importance.High,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            var notification = new AndroidNotification();
            notification.Title = "Корабли заправлены";
            notification.Text = "Пора вернутся к битвам!";
            notification.SmallIcon = "icon";
            notification.FireTime = DateTime.Now + TimeSpan.FromSeconds(30);

            AndroidNotificationCenter.SendNotification(notification, channel.Id);     
    }

    private void OnApplicationPause(bool pause) {
        if(pause && !IsMessageCalledOnce) {
            CreateChannelAndSendNotification();
            IsMessageCalledOnce = true;
        }
    }
}
