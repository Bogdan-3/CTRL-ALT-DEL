using UnityEngine;

public class DoorRequirement : MonoBehaviour
{
    public float pressure_plate_needed;
    public Animator anim;
    [HideInInspector] public float pressure_plate_pressed;
    string current_state;
    string name_active = "Door Open", name_deactive = "Door Close";
    void Update()
    {
        if (pressure_plate_needed == pressure_plate_pressed)
        {
            if (current_state == name_active)
                return;
            current_state = name_active;
            anim.CrossFade(name_active, 0.2f);
        }
        else
        {
            if (current_state == name_deactive)
                return;
            current_state = name_deactive;
            anim.CrossFade(name_deactive, 0.2f);
        }
    }
}
