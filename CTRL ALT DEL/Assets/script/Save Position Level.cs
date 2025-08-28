using UnityEngine;

public class SavePositionLevel : MonoBehaviour
{
    public ShortcutManager Position;
    void OnTriggerEnter(Collider other)
    {
        Position.PlayerX = other.transform.position.x;
        Position.PlayerY = other.transform.position.y;
        Position.PlayerZ = other.transform.position.z;
    }
}
