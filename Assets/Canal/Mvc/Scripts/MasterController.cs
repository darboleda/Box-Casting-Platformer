using UnityEngine;
using System.Collections.Generic;

public class MasterController : MonoBehaviour
{
    [SerializeField]
    private List<Controller> controllers;

    public void ReceiveNotification(string notification, View notifier, Dictionary<string, object> args)
    {
        ReceiveNotification(new ControllerNotification(notification, notifier, args));
    }

    public void ReceiveNotification(ControllerNotification notification)
    {
        foreach (var controller in controllers)
        {
            controller.HandleNotification(notification);
        }
    }
}
