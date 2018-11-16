using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour {

    public GameObject publicObjectReference;

    [HideInInspector]
    public GameObject hiddenObjectReference;

    private GameObject privateObjectReference;

    [SerializeField]
    private GameObject serializedPrivateObjectReference;

    [SerializeField]
    [HideInInspector]
    private GameObject serializedButHiddenPrivateObjectReference;
    
}
