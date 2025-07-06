using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Camera playerCam;

    //[SerializeField] private Transform eyePos;
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform groundItems;

    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask itemLayer;

    private bool lookingAt;
    private bool holdingItem = false;

    private GameObject heldItem;

    private Ray ray;

    private void Update() {
        checkLookingDoor();
        checkLookingItem();
        drop();

        ray = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
    }

    private void checkLookingDoor() {
        lookingAt = Physics.Raycast(ray, out RaycastHit hit, 1f, doorLayer);

        if (!lookingAt) return;

        Transform doorTrigger = hit.collider.transform;
        Transform door = doorTrigger.GetChild(0);

        if (Input.GetKeyDown(KeyCode.F)) {
            bool doorIsActive = door.gameObject.activeSelf;
            door.gameObject.SetActive(!doorIsActive);
            doorTrigger.GetComponent<Collider>().isTrigger = doorIsActive;
        }
    }
    private void checkLookingItem() {
        Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, itemLayer)) {
            Debug.Log("Looking at item");

            if (Input.GetKeyDown(KeyCode.F)) {
                holdingItem = true;
                heldItem = hit.transform.gameObject;

                // Parent the item to the hand
                heldItem.transform.SetParent(handPos);
                heldItem.transform.localPosition = Vector3.zero;

                // Set a consistent local rotation for how it appears in front of the player
                heldItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 90f); // Adjust this as needed

                // Disable physics
                Rigidbody rb = heldItem.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
        }
    }

    private void drop() {
        if (holdingItem && Input.GetKey(KeyCode.G)) {
            heldItem.transform.parent = groundItems;
            heldItem.transform.GetComponent<Rigidbody>().isKinematic = false;
            heldItem.transform.GetComponent<Rigidbody>().detectCollisions = true;
        }
    }

    private float rotateItem() {
        return playerCam.transform.rotation.y;
    }
}
