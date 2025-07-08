using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    private NavMeshAgent agent;

    [SerializeField] private Transform center;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask playerLayer;

    [Header("Timing Settings")]
    [SerializeField] private float initialDelay = 10f;
    [SerializeField] private float updateInterval = 10f;
    [SerializeField] private float trackingDuration = 5f;
    [SerializeField] private float attackTime = .5f;

    [Header("Script References")]
    [SerializeField] private DoorInteract doorInteract;
    [SerializeField] private SliderController healthBar;

    private float stateTimer = 0f;
    private enum TrackingState { Waiting, Cooldown, Tracking, Hunting }
    private TrackingState currentState;
    private bool setSpeed = false;
    private Transform currentTarget;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        currentState = TrackingState.Waiting;
        stateTimer = 0f;
    }

    private void Update() {
        nearDoor();
        checkPlayer();
        stateTimer += Time.deltaTime;

        switch (currentState) {
            case TrackingState.Waiting:
                if (stateTimer >= initialDelay) {
                    StartTracking();
                }
                break;

            case TrackingState.Cooldown:
                if (stateTimer >= updateInterval) {
                    StartTracking();
                }
                break;

            case TrackingState.Tracking:
                if (stateTimer >= trackingDuration) {
                    StopTracking();
                }
                break;
            case TrackingState.Hunting:
                beginHunt();
                break;
        }
    }

    private Transform GetClosestPlayer() {
        Collider[] players = Physics.OverlapBox(transform.position, new Vector3(50, 50, 50), transform.rotation, playerLayer);
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider playerCollider in players) {
            float distance = Vector3.Distance(transform.position, playerCollider.transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestPlayer = playerCollider.transform;
            }
        }

        return closestPlayer;
    }

    private void StartTracking() {
        currentTarget = GetClosestPlayer();
        if (currentTarget != null) {
            agent.SetDestination(currentTarget.position);
            currentState = TrackingState.Tracking;
            stateTimer = 0f;
        }
    }

    private void StopTracking() {
        agent.ResetPath();
        currentState = TrackingState.Cooldown;
        stateTimer = 0f;
    }

    private void beginHunt() {
        // Update target in case a closer player appears
        currentTarget = GetClosestPlayer();

        if (currentTarget != null) {
            agent.SetDestination(currentTarget.position);
            if (!setSpeed) {
                agent.speed *= 3f;
                setSpeed = true;
            }
        }

        currentState = TrackingState.Hunting;
    }

    private void nearDoor() {
        Collider[] nearDoors = Physics.OverlapBox(transform.position, new Vector3(.25f, .25f, .25f), transform.rotation, doorLayer);

        foreach (Collider collider in nearDoors) {
            Transform doorTrigger = collider.transform;
            if (doorTrigger.GetChild(0).gameObject.activeSelf) {
                doorInteract.toggleDoor(doorTrigger);
            }
        }
    }

    private void checkPlayer() {
        // Check for players in detection range
        Collider[] playerCheck = Physics.OverlapBox(transform.position, new Vector3(2, 1, 2), transform.rotation, playerLayer);

        // If we see players while tracking, switch to hunting
        if ((currentState == TrackingState.Tracking || currentState == TrackingState.Hunting) && playerCheck.Length > 0) {
            currentState = TrackingState.Hunting;
        }

        // Check for players in attack range
        Collider[] attackRange = Physics.OverlapBox(transform.position, new Vector3(.25f, 1, .25f), transform.rotation, playerLayer);
        if (currentState.Equals(TrackingState.Hunting) && attackRange.Length > 0) {

        }
    }

    // Visual feedback in Scene view
    private void OnDrawGizmos() {
        switch (currentState) {
            case TrackingState.Waiting: Gizmos.color = Color.yellow; break;
            case TrackingState.Cooldown: Gizmos.color = Color.green; break;
            case TrackingState.Tracking: Gizmos.color = Color.red; break;
            case TrackingState.Hunting: Gizmos.color = Color.magenta; break;
        }
        Gizmos.DrawWireSphere(transform.position, 1f);

        Vector3 size = new Vector3(0.25f, .25f, 0.25f);
        Gizmos.color = Color.cyan;
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = originalMatrix;

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(2, 1, 2));
    }
}