using UnityEngine;

class PortalInstanceManager : MonoBehaviour
{
  public static PortalInstanceManager Instance { get; private set; }
  public GameObject[] portalInstances;

  private void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);    
  }
}