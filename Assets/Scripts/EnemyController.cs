using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    private NavMeshAgent agent;
    [SerializeField] private Transform player;

    [Header("Timing Settings")]
    [SerializeField] private float initialDelay = 10f; // Delay before first movement
    [SerializeField] private float updateInterval = 10f; // Time between tracking updates
    [SerializeField] private float trackingDuration = 10f; // How long to track each time

    private float stateTimer = 0f;
    private enum TrackingState { Waiting, Cooldown, Tracking }
    private TrackingState currentState = TrackingState.Waiting;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        currentState = TrackingState.Waiting; // Start in initial delay state
        stateTimer = 0f;
    }

    private void Update() {
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
        }
    }

    private void StartTracking() {
        agent.SetDestination(player.position);
        currentState = TrackingState.Tracking;
        stateTimer = 0f;
        Debug.Log("Started tracking player at: " + Time.time);
    }

    private void StopTracking() {
        agent.ResetPath();
        currentState = TrackingState.Cooldown;
        stateTimer = 0f;
        Debug.Log("Stopped tracking at: " + Time.time);
    }

    // Visual feedback in Scene view
    private void OnDrawGizmos() {
        switch (currentState) {
            case TrackingState.Waiting:
                Gizmos.color = Color.yellow;
                break;
            case TrackingState.Cooldown:
                Gizmos.color = Color.green;
                break;
            case TrackingState.Tracking:
                Gizmos.color = Color.red;
                break;
        }
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}