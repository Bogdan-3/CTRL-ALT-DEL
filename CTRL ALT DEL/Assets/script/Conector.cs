using UnityEngine;

public class Conector : MonoBehaviour
{
    public DoorRequirement door;
    void OnTriggerEnter(Collider other)
    {
        door.pressure_plate_pressed++;
    }
    void OnTriggerExit(Collider other)
    {
        door.pressure_plate_pressed--;
    }
}
