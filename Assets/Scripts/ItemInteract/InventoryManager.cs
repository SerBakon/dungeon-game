using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{

    [SerializeField] private GameObject select1;
    [SerializeField] private GameObject select2;
    [SerializeField] private GameObject select3;
    [SerializeField] public GameObject EatDonutText;

    [SerializeField] private GameObject flashLight;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject object3;

    [SerializeField] private AudioClip gunshot;
    [SerializeField] private AudioSource gunAudio;
    [SerializeField] private GameObject gunLight;

    [SerializeField] private TextMeshProUGUI numItems;

    [SerializeField] private PlayerInteract playerInteract;

    [SerializeField] private Transform gunBarrel;
    [SerializeField] private Transform center;
    [SerializeField] private LayerMask enemy;
    [SerializeField] private LineRenderer bullet;

    public int numObject3;
    public bool holdingDonut;

    private bool holdingGun = true;
    private bool selectingThird = false;
    void Start()
    {
        selectFirstSlot();
        gunLight.gameObject.SetActive(false);
        numObject3 = 3;
        holdingDonut = false;
    }

    // Update is called once per frame
    void Update()
    {
        checkDonuts();
        numItems.text = numObject3.ToString();

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            selectFirstSlot();
            selectingThird = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            selectSecondSlot();
            selectingThird = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && numObject3 > 0) {
            selectThirdSlot();
            selectingThird = true;
        }
        if (holdingGun && Input.GetKeyDown(KeyCode.Mouse0)) {
            shootGun();
        }
    }

    private void selectFirstSlot() {
        select1.SetActive(true);
        select2.SetActive(false);
        select3.SetActive(false);

        gun.SetActive(true);
        flashLight.SetActive(false);
        holdingDonut = false;
        holdingGun = true;
    }
    private void selectSecondSlot() {
        select1.SetActive(false);
        select2.SetActive(true);
        select3.SetActive(false);

        gun.SetActive(false);

        flashLight.SetActive(true);
        holdingDonut = false;
        holdingGun = false;
    }
    public void selectThirdSlot() {
        select1.SetActive(false);
        select2.SetActive(false);
        select3.SetActive(true);

        flashLight.SetActive(false);
        gun.SetActive(false);

        holdingDonut = true;
        holdingGun = false;

        object3.SetActive(true);

        //cloneDonute();
    }

    private void checkDonuts() {
        if (numObject3 == 0 && selectingThird) {
            object3.SetActive(false);
            selectFirstSlot();
            playerInteract.holdingItem = false;
        }
        if (numObject3 > 0 && holdingDonut) {
            object3.SetActive(true);
        }
        if (!holdingDonut) {
            object3.SetActive(false);
            EatDonutText.SetActive(false);
        } else {
            EatDonutText.SetActive(true);
        }
    }

    public void cloneDonute() {
        var object3Copy = Instantiate(object3);
        playerInteract.pickUpItem(object3Copy);
        if(!holdingDonut) {
            object3Copy.SetActive(false);
        }
    }

    private void shootGun() {
        RaycastHit hit;
        // Get the center of the screen in world coordinates (at a reasonable distance)
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0);
        Vector3 worldCrosshairPos = playerInteract.playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));

        // Calculate direction from gun barrel to crosshair world position
        Vector3 direction = (worldCrosshairPos - gunBarrel.transform.position).normalized;
        Ray bulletRay = playerInteract.playerCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        bool hitTarget = Physics.Raycast(bulletRay , out hit, 100f, enemy);
        //gunAudio.clip = gunshot;
        gunAudio.PlayOneShot(gunshot);
        StartCoroutine(DrawTempRay(direction));
        if (hitTarget) {
            //Debug.Log("hit enemy");
            hit.transform.gameObject.GetComponent<EnemyController>().takeDamage(4);
        }
    }
    

    IEnumerator DrawTempRay(Vector3 direction) {
        bullet.gameObject.SetActive(true);
        bullet.SetPosition(0, gunBarrel.transform.position);
        bullet.SetPosition(1, gunBarrel.transform.position + direction * 10f);
        bullet.startWidth = 0.01f;
        bullet.endWidth = 0.01f;
        //bullet.material = new Material(Shader.Find("Unlit/Color")) { color = Color.red };
        gunLight.gameObject.SetActive(true);
        yield return new WaitForSeconds(.3f);
        gunLight.gameObject.SetActive(false);
        bullet.gameObject.SetActive(false);
    }
}
