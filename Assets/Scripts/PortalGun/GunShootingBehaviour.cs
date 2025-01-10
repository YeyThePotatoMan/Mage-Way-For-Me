using System;
using UnityEngine;

/// <summary>
/// Behaviour of the shooting mechanism of the gun, including projectile instantiation and cooldown.
/// </summary>
public class GunShootingBehaviour : MonoBehaviour
{
    [Tooltip("Cooldown duration in seconds between each shot.")]
    public float cooldownDuration = 0.5f;
    [Tooltip("Length of the portal.")]
    public int portalLength = 16;
    [Tooltip("Width of the portal.")]
    public int portalWidth = 2;

    [SerializeField]
    [Tooltip("Prefab of the projectile to shoot.")]
    private GameObject _projectilePrefab;

    [SerializeField]
    [Tooltip("Array of portal prefabs to spawn when the projectile triggers.")]
    private GameObject[] _portalPrefabs;
    [SerializeField]
    private LayerMask _solidLayer;

    // Last time the gun shot a projectile
    private Grid _grid;
    private float _lastShotTime;
    private int _halfPortalLength => portalLength / 2;
    private float _cellMagnitude;
    private Vector3[] _2DDirections = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    // When set to true, the gun will shoot the seocnd set of portals.
    private bool _isSecondSet = false;

    private void Start()
    {
        _lastShotTime = Time.unscaledTime - cooldownDuration;

        if (PortalInstanceManager.Instance.portalInstances.Length == 0)
            PortalInstanceManager.Instance.portalInstances = new GameObject[_portalPrefabs.Length];

        _grid = FindAnyObjectByType<Grid>();
        _cellMagnitude = _grid.transform.localScale.x;
    }

    private void Update()
    {
        // If the player clicks and the cooldown is over, shoot a projectile
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && Time.unscaledTime - _lastShotTime >= cooldownDuration)
        {
            GameObject projectile = Instantiate(_projectilePrefab, transform.position, transform.rotation);
            PortalProjectileBehaviour portalProjectileBehaviour = projectile.GetComponent<PortalProjectileBehaviour>();

            portalProjectileBehaviour.onTrigger = CreateProjectileTriggerAction(
                (_isSecondSet ? 2 : 0) +                // Pick between sets, 1st set is [0, 1] and 2nd set is [2, 3]
                (Input.GetMouseButtonDown(0) ? 0 : 1)   // Pick which portal of the pair, 1st portal is even and 2nd portal is odd
            );

            _lastShotTime = Time.unscaledTime;
        }
        else if (Input.GetButtonDown("Fire3"))
        {
            _isSecondSet = !_isSecondSet;
        }
    }

    /// <summary>
    /// Create the action to be executed when the projectile triggers.
    /// </summary>
    /// <returns>Action to be executed when the projectile triggers.</returns>
    private Action<GameObject> CreateProjectileTriggerAction(int portalIndex)
    {
        return (GameObject projectile) =>
            {
                Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
                Vector3Int projectileCellPos = _grid.WorldToCell(projectile.transform.position);

                // Determining optimal direction and position for the portal.
                Vector3 optimalDirection = Vector3.zero;
                Vector3 optimalPos = Vector3.zero;
                int optimalCost = int.MaxValue;
                foreach (Vector3 dir in _2DDirections)
                {
                    RaycastHit2D hit = Physics2D.Raycast(
                        _grid.GetCellCenterWorld(projectileCellPos),
                        -dir,
                        _cellMagnitude,
                        _solidLayer);
                    if (hit.collider == null) continue;

                    Vector3 right = Vector3.Cross(dir, Vector3.forward);
                    Vector3 left = -right;
                    int cellsLeft = 0, cellsRight = 0;

                    // Counts how many cells are in the right and left of the projectile.
                    cellsLeft = _halfPortalLength - GetDistanceCast(projectileCellPos, left, dir, _halfPortalLength, portalWidth, _solidLayer);
                    cellsRight = _halfPortalLength - GetDistanceCast(projectileCellPos, right, dir, _halfPortalLength, portalWidth, _solidLayer);

                    // Debug.DrawLine(_grid.GetCellCenterWorld(projectileCellPos), _grid.GetCellCenterWorld(projectileCellPos + Vector3Int.RoundToInt(dir * _halfPortalLength)), Color.green, 2f);

                    // If there are no cells in the right and left of the projectile, the portal can be placed at the projectile's position.
                    if (cellsLeft == 0 && cellsRight == 0)
                    {
                        optimalCost = 0;
                        optimalPos = _grid.GetCellCenterWorld(projectileCellPos);
                        optimalDirection = dir;
                        break; // Immidently break because it is the most optimal choice.
                    }
                    // If there are no cells in either the right or left of the projectile 
                    // and if it is more optimal than the current optimal choice, 
                    // the portal can be placed at the projectile's position + 
                    // the amount of cells in the other direction if there are no cells in the way.
                    else if (cellsLeft == 0)
                    {
                        // Debug.DrawLine(_grid.GetCellCenterWorld(projectileCellPos + Vector3Int.RoundToInt(left * (_halfPortalLength - 1))), _grid.GetCellCenterWorld(projectileCellPos + Vector3Int.RoundToInt(left * (_halfPortalLength - 1)) + Vector3Int.RoundToInt(left * cellsRight)), Color.red, 2f);
                        if (GetDistanceCast(
                                projectileCellPos + Vector3Int.RoundToInt(left * (_halfPortalLength - 1)),
                                left,
                                dir,
                                cellsRight,
                                portalWidth,
                                _solidLayer) == cellsRight
                            && cellsRight < optimalCost)
                        {
                            optimalCost = cellsRight;
                            optimalPos = _grid.GetCellCenterWorld(projectileCellPos + Vector3Int.RoundToInt(left * cellsRight));
                            optimalDirection = dir;
                        }
                    }
                    else if (cellsRight == 0)
                    {
                        // Debug.DrawLine(_grid.GetCellCenterWorld(projectileCellPos + Vector3Int.RoundToInt(right * (_halfPortalLength - 1))), _grid.GetCellCenterWorld(projectileCellPos + Vector3Int.RoundToInt(right * (_halfPortalLength - 1)) + Vector3Int.RoundToInt(right * cellsLeft)), Color.red, 2f);                              
                        if (GetDistanceCast(
                                projectileCellPos + Vector3Int.RoundToInt(right * (_halfPortalLength - 1)),
                                right,
                                dir,
                                cellsLeft,
                                portalWidth,
                                _solidLayer) == cellsLeft
                            && cellsLeft < optimalCost)
                        {
                            optimalCost = cellsLeft;
                            optimalPos = _grid.GetCellCenterWorld(projectileCellPos + Vector3Int.RoundToInt(right * cellsLeft));
                            optimalDirection = dir;
                        }
                    }
                }

                // If the optimal choice exists, spawn the portal at the optimal position.
                if (optimalCost != int.MaxValue)
                {                    
                    int pairIndex = portalIndex + (portalIndex % 2 == 0 ? 1 : -1);

                    if (PortalInstanceManager.Instance.portalInstances[portalIndex] != null) Destroy(PortalInstanceManager.Instance.portalInstances[portalIndex]);

                    PortalInstanceManager.Instance.portalInstances[portalIndex] = Instantiate(_portalPrefabs[portalIndex], optimalPos,
                        Quaternion.Euler(0, 0, Mathf.Atan2(optimalDirection.y, optimalDirection.x) * Mathf.Rad2Deg + 180));

                    // If the pair portal exists, link the two portals
                    if (PortalInstanceManager.Instance.portalInstances[pairIndex] != null)
                    {
                        PortalTeleportController thisController = PortalInstanceManager.Instance.portalInstances[portalIndex].GetComponent<PortalTeleportController>();
                        PortalTeleportController otherController = PortalInstanceManager.Instance.portalInstances[pairIndex].GetComponent<PortalTeleportController>();
                        thisController.OtherPortal = otherController;
                        otherController.OtherPortal = thisController;
                    }
                }
                return;
            };
    }

    /// <summary>
    /// Gets the minimum distance to the nearest solid object in the direction of 'direction' and 'outwardsDirection'.
    /// Basically a 2D raycast with a width.
    /// </summary>
    /// <param name="cellPos">Cell position to start the cast from.</param>
    /// <param name="direction">Main direction to cast, the direction to get the minimum distance of.</param>
    /// <param name="outwardsDirection">Outwards direction to cast, the direction to move the origin position to for every width.</param>
    /// <param name="maxDistance">Maximum distance to cast in the direction of 'direction'.</param>
    /// <param name="width">Width of the cast in the direction of 'outwardsDirection'.</param>
    /// <returns>The minimum distance in the direction of 'direction' for every width in 'outwardsDirection'. 0 <= distance <= 'maxDistance'</returns>
    private int GetDistanceCast(Vector3Int cellPos, Vector3 direction, Vector3 outwardsDirection, int maxDistance, int width, LayerMask solidLayer)
    {
        int distance = maxDistance;
        int gridDistance;
        Vector3 raycastOrigin = _grid.GetCellCenterWorld(cellPos);

        // For every row in the outwards direction, cast a ray to find the minimum distance to the nearest solid object.
        for (int i = 0; i < width; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                raycastOrigin,
                direction,
                maxDistance * _cellMagnitude,
                solidLayer);
            gridDistance = Mathf.FloorToInt(hit.distance / _cellMagnitude);
            if (hit.collider != null && gridDistance < distance) distance = gridDistance;

            // Move origin position to the next cell, in the outwards direction.
            raycastOrigin += outwardsDirection * _cellMagnitude;
        }

        return distance;
    }
}
