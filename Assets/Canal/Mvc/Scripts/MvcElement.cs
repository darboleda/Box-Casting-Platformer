using UnityEngine;
using System.Collections;

public class MvcElement : MonoBehaviour
{
    [SerializeField]
    private string id;
    public string Id { get { return id; } }

    private MvcContainer container;

}
