using NullReferenceDetection;
using UnityEngine;

public class CubeScript : MonoBehaviour {

    [SerializeField]
    private SphereScript sphere;

    [ValueRequired]
    public CapsuleScript capsule;

    [ValueOptional]
    public CylinderScript cylinder;

}
