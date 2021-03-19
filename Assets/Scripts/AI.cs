using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public int enemyLayer;
    public int ownerCastleLayer;
    public float speed;
    public Transform targetTransform;
    public GameManager gm;
    private NavMeshAgent _navMesh;
    public GameObject blood;

    private void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();
        _navMesh.speed = speed;
    }

    private void OnEnable()
    {
        Invoke(nameof(DisablenavMeshCollider), 0.1f);
        Invoke(nameof(EnableCollider), 0.7f);
    }

    private void Update()
    {
        _navMesh.SetDestination(targetTransform.position);
    }

    private void EnableCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    private void DisablenavMeshCollider()
    {
        _navMesh.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }
}
