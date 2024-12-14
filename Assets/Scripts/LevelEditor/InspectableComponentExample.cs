using UnityEngine;

[Inspectable]
public class InspectableComponentExample : MonoBehaviour
{
    [InspectKey(0)]
    public string strValue;
    [InspectKey(1)]
    public int intValue;
    [InspectKey(2)]
    public float floatValue;
}
