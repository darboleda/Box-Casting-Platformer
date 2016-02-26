using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MasterView : MonoBehaviour
{
    [SerializeField]
    private List<View> views;

    private Dictionary<string, View> organizedViews;

    public T FindView<T>(string id, bool searchOnFail = true) where T : View
    {
        // initialize the view dictionary if it isn't already.
        organizedViews = organizedViews ?? views.ToDictionary<View, string>(x => x.Id);

        View view;
        if (organizedViews.TryGetValue(id, out view))
        {
            if (view is T) { return (T)view; }
        }

        if (searchOnFail)
        {
            T found = GetComponentsInChildren<T>().FirstOrDefault();
            if (found) { organizedViews[found.Id] = found; }
            return found;
        }

        return default(T);
    }

}
