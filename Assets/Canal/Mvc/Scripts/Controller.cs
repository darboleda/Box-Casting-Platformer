using UnityEngine;
using System.Collections.Generic;

public struct ControllerNotification
{
    private string notification;
    private View notifier;
    private IDictionary<string, object> args;

    public ControllerNotification(string notification, View notifier, IDictionary<string, object> args)
    {
        this.notification = notification;
        this.notifier = notifier;
        this.args = args;
    }

    public string Notification { get { return notification; } }
    public View Notifier { get { return notifier; } }
    public T GetParam<T>(string id)
    {
        object param;
        return (args.TryGetValue(id, out param) && param is T) ? (T)param : default(T);
    }
}

public abstract class Controller : MonoBehaviour
{
    public abstract void HandleNotification(ControllerNotification notification);
}
