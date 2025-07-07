using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public void toggleDoor(Transform doorTrigger) {
        Transform door = doorTrigger.GetChild(0);
        bool doorIsActive = door.gameObject.activeSelf;
        door.gameObject.SetActive(!doorIsActive);
        doorTrigger.GetComponent<Collider>().isTrigger = doorIsActive;
    }
}
