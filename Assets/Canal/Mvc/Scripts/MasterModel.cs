using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MasterModel : MonoBehaviour
{
    [SerializeField]
    private List<Model> models;

    private Dictionary<string, Model> organizedModels;

    public T FindModel<T>(string id, bool searchOnFail = true) where T : Model
    {
        // initialize the model dictionary if it isn't already.
        organizedModels = organizedModels ?? models.ToDictionary<Model, string>(x => x.Id);

        Model model;
        if (organizedModels.TryGetValue(id, out model))
        {
            if (model is T) { return (T)model; }
        }

        if (searchOnFail)
        {
            T found = GetComponentsInChildren<T>().FirstOrDefault();
            if (found) { organizedModels[found.Id] = found; }
            return found;
        }

        return default(T);
    }
}
