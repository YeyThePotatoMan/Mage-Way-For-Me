using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserInterfaceHelper
{
  public static bool IsPointerOverUIElement()
  {
    PointerEventData eventData = new(EventSystem.current) { position = Input.mousePosition };

    List<RaycastResult> raycastResults = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventData, raycastResults);

    for (int i = 0; i < raycastResults.Count; i++)
    {
      RaycastResult curRaysastResult = raycastResults[i];

      if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
        return true;
    }

    return false;
  }
}
