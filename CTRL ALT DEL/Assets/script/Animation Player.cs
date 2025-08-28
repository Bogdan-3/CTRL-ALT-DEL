using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    public Animator anim;
    public string animation_name;
    public string animation_name_reverse;
    string current_state;

    void OnTriggerEnter(Collider other)
    {
        if (current_state == animation_name)
            return;
        anim.CrossFade(animation_name, 0.2f);
        current_state = animation_name;
    }

    void OnTriggerExit(Collider other)
    {
        if (current_state == animation_name_reverse)
            return;
        anim.CrossFade(animation_name_reverse, 0.2f);
        current_state = animation_name_reverse;
    }
}
