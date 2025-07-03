using UnityEngine;

public class MoveCam : MonoBehaviour
{
    public Transform camerapos;
    void Update()
    {
        transform.position = camerapos.position;
    }
}
