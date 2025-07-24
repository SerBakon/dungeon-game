using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] public Camera playerCam;
    [SerializeField] private GameObject openDoorText;
    [SerializeField] private GameObject closeDoorText;
    [SerializeField] private GameObject pickUpItemText;
    [SerializeField] private GameObject escapeText;
    [SerializeField] private TextMeshProUGUI checkDonutText;

    [SerializeField] private GameObject aliveHUD;
    [SerializeField] private GameObject deadHUD;
    [SerializeField] private GameObject winHUD;

    //[SerializeField] private Transform eyePos;
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform groundItems;
    [SerializeField] private Transform player;

    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask escapeLayer;

    private bool lookingAt;
    private bool escaping;
    public bool holdingItem = false;
    private bool justToggled = false;

    public GameObject heldItem;

    private Ray ray;

    [SerializeField] private DoorInteract DoorInteract;
    [SerializeField] private SliderController healthBar;
    [SerializeField] private MouseLook camControl;
    [SerializeField] private ProgressBarController progressBar;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private EnemyController enemyController;

    private void Start() {
        checkDonutText.color = new Color(1f, 1f, 1f, 0f);
    }
    private void Update() {
        checkLookingDoor();
        checkLookingItem();
        checkLookingEscape();
        drop();
        eatDonut();

        ray = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if(healthBar.HP <= 0) {
            death();
        }

        //if(Input.GetKeyDown(KeyCode.R)) {
        //    death();
        //}
    }

    private void checkLookingEscape() {
        //if (Physics.Raycast(ray, 1.5f, escapeLayer)) Debug.Log("escape box");
        escaping = Physics.Raycast(ray, 1.5f, escapeLayer);
        if (escaping) {
            escapeText.SetActive(true);
            //Debug.Log("looking at escape block");
            if (Input.GetKeyDown(KeyCode.F)) {
                StartCoroutine(SmoothLookAt(dungeonGenerator.escapeBlockPos + Vector3Int.up, 5.0f));
                StartCoroutine(escapeSequence(1.0f));
                escaped();
            }
        } else {
            escapeText.SetActive(false);
        }
    }
    public IEnumerator SmoothLookAt(Vector3Int targetPosition, float rotationSpeed) {
        Vector3 target = targetPosition;
        Quaternion targetRotation = Quaternion.LookRotation(target - playerCam.transform.position);

        while (Quaternion.Angle(playerCam.transform.rotation, targetRotation) > 0.1f) {
            playerCam.transform.rotation = Quaternion.Slerp(
                playerCam.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Ensure perfect alignment at the end
        playerCam.transform.LookAt(target);
    }

    private IEnumerator escapeSequence(float speed) {
        Debug.Log("running escape sequence");
        Vector3 targetPosition = dungeonGenerator.escapeBlockPos + Vector3Int.up;

        // Disable camera controls
        camControl.GetComponent<MouseLook>().enabled = false;
        playerCam.transform.parent = null;

        while (Vector3.Distance(playerCam.transform.position, targetPosition) > 0.5f) {
            playerCam.transform.position = Vector3.MoveTowards(
                playerCam.transform.position,
                targetPosition,
                speed * Time.deltaTime
            );
            yield return null; // Wait for next frame
        }
        Debug.Log("send win text");
        winHUD.SetActive(true);
        aliveHUD.SetActive(false);
    }



    private void checkLookingDoor() {
        lookingAt = Physics.Raycast(ray, out RaycastHit hit, 1f, doorLayer);
        if (!lookingAt) {
            openDoorText.SetActive(false);
            closeDoorText.SetActive(false);
            
            progressBar.progress = 0;
            return;
        } 
        else
        {
            inventoryManager.EatDonutText.SetActive(false);
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
            pickUpItemText.SetActive(true);
            inventoryManager.EatDonutText.SetActive(false);
            if (Input.GetKeyDown(KeyCode.F)) {
                //pickUpItem(hit.transform.gameObject);
                //hit.transform.gameObject.SetActive(false);
                Destroy(hit.transform.gameObject);
                inventoryManager.numObject3++;
                if(inventoryManager.numObject3 == 0) {
                    inventoryManager.cloneDonute();
                }
            }
        } else {
            pickUpItemText.SetActive(false);
        }
            
    }

    private void eatDonut()
    {
        if (inventoryManager.holdingDonut && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(healthBar.HP == healthBar.maxHP) {
                //StopCoroutine(fadeText(checkDonutText));
                StartCoroutine(fadeText(checkDonutText));
            } else {
                inventoryManager.numObject3--;
                healthBar.HP += 10;
            }
        }
    }

    private IEnumerator fadeText(TextMeshProUGUI text) {
        // Get the initial color
        Color originalColor = new Color(1, 1, 1, 1);

        // Duration of the fade in seconds
        float fadeDuration = 1f; // You can adjust this as needed
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            // Calculate the new alpha value based on elapsed time
            float newAlpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration);

            // Apply the new color with updated alpha
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the alpha is exactly 0 at the end
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Optional: disable the text object completely after fading
        // text.gameObject.SetActive(false);
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
        if (inventoryManager.holdingDonut && Input.GetKeyDown(KeyCode.G) && inventoryManager.numObject3 > 0) {
            inventoryManager.cloneDonute() ;
            heldItem.SetActive(true);
            heldItem.transform.parent = groundItems;
            Rigidbody rb = heldItem.transform.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.AddForce(transform.forward);
            inventoryManager.numObject3--;
            if (inventoryManager.numObject3 > 0) {
                inventoryManager.selectThirdSlot();
            }
        }
        
    }

    private void death() {
        // stop movement
        transform.gameObject.GetComponent<PlayerMovement>().enabled = false;
        transform.gameObject.GetComponent<PlayerInteract>().enabled = false;
        playerCam.GetComponent<AudioListener>().enabled = false;

        //set hud
        aliveHUD.SetActive(false);
        deadHUD.SetActive(true);
        winHUD.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;

        //stop all inventory and camera control
        inventoryManager.gameObject.SetActive(false);
        camControl.enabled = false;
        playerCam.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void alive() {
        // reenabling movement/interactions
        transform.gameObject.GetComponent<PlayerMovement>().enabled = true;
        transform.gameObject.GetComponent<PlayerInteract>().enabled = true;
        playerCam.GetComponent<AudioListener>().enabled = true;
        aliveHUD.SetActive(true);
        deadHUD.SetActive(false);
        winHUD.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        inventoryManager.gameObject.SetActive(true);

        //camera
        StopAllCoroutines();
        camControl.enabled = true;
        playerCam.transform.GetChild(0).gameObject.SetActive(false);
        playerCam.transform.parent = player;
        playerCam.transform.localPosition = new Vector3(0, 0.2f, 0);

        //health
        healthBar.HP = healthBar.maxHP;
    }

    public void escaped() {
        transform.gameObject.GetComponent<PlayerMovement>().enabled = false;
        transform.gameObject.GetComponent<PlayerInteract>().enabled = false;
        playerCam.GetComponent<AudioListener>().enabled = false;
        aliveHUD.SetActive(false);
        winHUD.SetActive(true);
        deadHUD.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        inventoryManager.gameObject.SetActive(false);
        camControl.enabled = false;
        playerCam.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void findEscape() {
        StartCoroutine(SmoothLookAt(dungeonGenerator.escapeBlockPos + Vector3Int.up, 5.0f));
        StartCoroutine(escapeSequence(10.0f));
    }
    private IEnumerator ResetToggleFlag() {
        yield return new WaitForSeconds(0.5f); // Adjust time as needed
        justToggled = false;
    }
}
