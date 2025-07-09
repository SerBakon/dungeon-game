using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public void toggleDoor(Transform doorTrigger) {
        Transform door = doorTrigger.GetChild(0);
        bool doorIsActive = door.gameObject.activeSelf;
        door.gameObject.SetActive(!doorIsActive);
        doorTrigger.GetComponent<Collider>().isTrigger = doorIsActive;
    }

    public void openDoor(Transform doorTrigger) {
        Transform door = doorTrigger.GetChild(0);
        door.gameObject.SetActive(false);
        doorTrigger.GetComponent<Collider>().isTrigger = true;
    }
}
