using System.Collections;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private GameObject openDoorText;
    [SerializeField] private GameObject closeDoorText;

    //[SerializeField] private Transform eyePos;
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform groundItems;

    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask itemLayer;

    private bool lookingAt;
    public bool holdingItem = false;
    private bool justToggled = false;

    public GameObject heldItem;

    private Ray ray;

    [SerializeField] private DoorInteract DoorInteract;
    [SerializeField] private SliderController healthBar;
    [SerializeField] private MouseLook camControl;
    [SerializeField] private ProgressBarController progressBar;
    [SerializeField] private InventoryManager inventoryManager;

    private void Update() {
        checkLookingDoor();
        checkLookingItem();
        drop();

        ray = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if(healthBar.HP <= 0) {
            death();
        }
    }

    private void checkLookingDoor() {
        lookingAt = Physics.Raycast(ray, out RaycastHit hit, 1f, doorLayer);

        if (!lookingAt) {
            openDoorText.SetActive(false);
            closeDoorText.SetActive(false);
            return;
        }
        Transform doorTrigger = hit.collider.transform;
        var doorMesh = doorTrigger.GetChild(0).gameObject;
        if (lookingAt && doorMesh.activeSelf) {
            closeDoorText.SetActive(false);
            openDoorText.SetActive(true);
        } else {
            closeDoorText.SetActive(true);
            openDoorText.SetActive(false);
        }
        if (Input.GetKey(KeyCode.F)) {
            if (doorMesh.activeSelf && !justToggled) {
                progressBar.gameObject.SetActive(true);
                if (progressBar.increaseProgress()) {
                    DoorInteract.toggleDoor(doorTrigger);
                    justToggled = true;
                    StartCoroutine(ResetToggleFlag());
                }
            }
            else {
                if (!justToggled) {
                    DoorInteract.toggleDoor(doorTrigger);
                    progressBar.gameObject.SetActive(false);
                    justToggled = true;
                    StartCoroutine(ResetToggleFlag());
                }

            }

        }
        else {
            progressBar.progress = 0;
            progressBar.gameObject.SetActive(false);
        }
    }
    private void checkLookingItem() {
        Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, itemLayer)) {
            //Debug.Log("Looking at item");

            if (Input.GetKeyDown(KeyCode.F)) {
                //pickUpItem(hit.transform.gameObject);
                hit.transform.gameObject.SetActive(false);
                inventoryManager.numObject3++;
                if(inventoryManager.numObject3 == 0) {
                    inventoryManager.cloneDonute();
                }
            }
        }
    }

    public void pickUpItem(GameObject heldItem) {
        holdingItem = true;
        this.heldItem = heldItem;

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

    private void drop() {
        if (holdingItem && Input.GetKeyDown(KeyCode.G) && inventoryManager.numObject3 > 0) {
            heldItem.SetActive(true);
            heldItem.transform.parent = groundItems;
            heldItem.transform.GetComponent<Rigidbody>().isKinematic = false;
            heldItem.transform.GetComponent<Rigidbody>().detectCollisions = true;
            inventoryManager.numObject3--;
            if (inventoryManager.numObject3 > 0) {
                inventoryManager.selectThirdSlot();
            }
        }
        
    }

    private void death() {
        transform.gameObject.GetComponent<PlayerMovement>().enabled = false;
        transform.gameObject.GetComponent<PlayerInteract>().enabled = false;
        camControl.enabled = false;
    }
    private IEnumerator ResetToggleFlag() {
        yield return new WaitForSeconds(0.5f); // Adjust time as needed
        justToggled = false;
    }
}
