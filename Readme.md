# Finding null references in Unity

![alt text](https://raw.githubusercontent.com/Moolt/Unity-Null-Reference-Detection/master/Documentation/screenshot.png "screenshot")

This handy Unity plug-in helps you to find `null references` in your scene objects to avoid runtime exceptions. It's lightweight and both easy to install and to use.

## Setup

If you want to get a quick overview you should check out the [demo project](https://github.com/Moolt/Unity-Null-Reference-Detection/archive/master.zip). 
You can also download the [package](https://github.com/Moolt/Unity-Null-Reference-Detection/raw/master/null-reference-detection.unitypackage) containing only the essential scripts.

After downloading and importing the package, you will find a new entry under `Tools > Find Null References`.

## How to use

In the screenshot above there's a cube with a script containing three references. Here's the example code:

```csharp
using NullReferenceDetection;
using UnityEngine;

public class CubeScript : MonoBehaviour {

    [SerializeField]
    //Serialized, private fields are visible in the inspector
    private SphereScript sphere;

    [ValueRequired]
    //Will be shown as red text if null
    public CapsuleScript capsule;

    [ValueOptional]
    //Won't be shown even if null
    public CylinderScript cylinder;
}
```

All fields that appear in the `Unity Inspector` will also be checked for `null` by the plug-in. That includes `public` fields and `serialized private` fields.

In addition you can use two custom attributes to further specify how your fields should be handled:
  * `[ValueRequired]` will print a red error message if null
  * `[ValueOptional]`won't show in the console even if null

__Using these custom attributes is purely optional__.

If you now execute the plug-in via `Tools > Find Null References` the console will show two error messages, like in the screenshot above. The `capsule` reference is marked as `required`, therefore it's show as red. `sphere` is shown with the default severity and `cylinder` is not shown at all as the `[ValueOptional]` attribute allows the field to be a `null` reference.

## Good to know

Clicking on an error message will highlight the target object in the `Hierarchy`.

## The plug-in does not...
 *  check list items for null references.
 *  check for properties of default unity components (such as transform, box collider etc.)
 *  consider objects in other scenes but the currently opened scene.
 *  consider inactive objects.

