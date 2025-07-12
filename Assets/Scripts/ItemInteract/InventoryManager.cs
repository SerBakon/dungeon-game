using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{

    [SerializeField] private GameObject select1;
    [SerializeField] private GameObject select2;
    [SerializeField] private GameObject select3;

    [SerializeField] private GameObject flashLight;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject object3;

    [SerializeField] private TextMeshProUGUI numItems;

    [SerializeField] private PlayerInteract playerInteract;

    public int numObject3;
    public bool holdingDonut;

    private bool selectingThird = false;
    void Start()
    {
        selectFirstSlot();
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
    }

    private void selectFirstSlot() {
        select1.SetActive(true);
        select2.SetActive(false);
        select3.SetActive(false);

        flashLight.SetActive(false);
        holdingDonut = false;
    }
    private void selectSecondSlot() {
        select1.SetActive(false);
        select2.SetActive(true);
        select3.SetActive(false);

        flashLight.SetActive(true);
        holdingDonut = false;
    }
    public void selectThirdSlot() {
        select1.SetActive(false);
        select2.SetActive(false);
        select3.SetActive(true);

        flashLight.SetActive(false);

        holdingDonut = true;

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
        }
    }

    public void cloneDonute() {
        var object3Copy = Instantiate(object3);
        playerInteract.pickUpItem(object3Copy);
        if(!holdingDonut) {
            object3Copy.SetActive(false);
        }
    }
}
