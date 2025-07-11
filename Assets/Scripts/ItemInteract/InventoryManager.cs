using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{

    [SerializeField] private GameObject select1;
    [SerializeField] private GameObject select2;
    [SerializeField] private GameObject select3;

    [SerializeField] private GameObject object1;
    [SerializeField] private GameObject object2;
    [SerializeField] private GameObject object3;

    [SerializeField] private TextMeshProUGUI numItems;

    [SerializeField] private PlayerInteract playerInteract;

    public int numObject3;
    private bool holdingDonut;
    void Start()
    {
        selectFirstSlot();
        numObject3 = 0;
        holdingDonut = false;
    }

    // Update is called once per frame
    void Update()
    {
        checkDonuts();
        numItems.text = numObject3.ToString();

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            selectFirstSlot();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            selectSecondSlot();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && numObject3 > 0) {
            selectThirdSlot();
        }
    }

    private void selectFirstSlot() {
        select1.SetActive(true);
        select2.SetActive(false);
        select3.SetActive(false);

        holdingDonut = false;
    }
    private void selectSecondSlot() {
        select1.SetActive(false);
        select2.SetActive(true);
        select3.SetActive(false);

        holdingDonut = false;
    }
    public void selectThirdSlot() {
        select1.SetActive(false);
        select2.SetActive(false);
        select3.SetActive(true);

        holdingDonut = true;

        object3.SetActive(true);

        cloneDonute();
    }

    private void checkDonuts() {
        if (numObject3 == 0) {
            object3.SetActive(false);
            selectFirstSlot();
            playerInteract.holdingItem = false;
        }
        if (numObject3 > 0 && holdingDonut) {
            object3.SetActive(true);
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
