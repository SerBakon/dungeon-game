using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform eyePos;

    [SerializeField] private LayerMask doorLayer;

    private bool lookingAt;

    private void Update() {
        checkLooking();
    }

    private void checkLooking() {
        lookingAt = Physics.Raycast(eyePos.position, eyePos.forward, out RaycastHit hit, 1f, doorLayer);

        if (!lookingAt) return;

        Transform doorTrigger = hit.collider.transform;
        Transform door = doorTrigger.GetChild(0);

        if (Input.GetKeyDown(KeyCode.F)) {
            bool doorIsActive = door.gameObject.activeSelf;
            door.gameObject.SetActive(!doorIsActive);
            doorTrigger.GetComponent<Collider>().isTrigger = doorIsActive;
        }
    }
}
