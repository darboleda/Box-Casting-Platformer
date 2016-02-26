using UnityEngine;
using System.Collections;

public class MvcContainer : MonoBehaviour
{
    [SerializeField]
    private MasterModel model;

    [SerializeField]
    private MasterView view;

    [SerializeField]
    private MasterController controller;

    public MasterModel Model { get { return model; } }
    public MasterView View { get { return view; } }
    public MasterController Controller { get { return controller; } }
}
