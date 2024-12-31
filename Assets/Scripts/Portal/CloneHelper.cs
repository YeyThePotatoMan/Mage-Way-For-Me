using UnityEngine;

/// <summary>
/// A helper class with static methods to help cloning GameObjects.
/// </summary>
public class CloneHelper : MonoBehaviour
{
  /// <summary>
  /// A helper function to clone g GameObject like Instantiate but also copy all fields with CloneReferenceAttribute.
  /// </summary>
  /// <param name="original">Original GameObject to clone.</param>  
  /// <returns>Cloned GameObject.</returns>
  public static GameObject Clone(GameObject original)
  {
    // Insatantiate the original GameObject.
    GameObject clone = Instantiate(original);

    // Iterate all components and fields of the clone.
    foreach (var component in clone.GetComponents<Component>())
    {
      var originalComponent = original.GetComponent(component.GetType());
      foreach (var field in component.GetType().GetFields())
      {
        if (field.GetCustomAttributes(typeof(CloneReferenceAttribute), true).Length > 0)
        {
          // Get the value from the original gameObject.
          object value = originalComponent
            .GetType()
            .GetField(field.Name)
            .GetValue(originalComponent);

          // Set the value to the clone gameObject.          
          field.SetValue(component, value);
        }
      }
    }

    return clone;
  }
}

/// <summary>
/// An attribute to mark a field to be copied when cloning a GameObject with CloneHelper.Clone.
/// </summary>
public class CloneReferenceAttribute : System.Attribute
{
  public CloneReferenceAttribute() { }
}