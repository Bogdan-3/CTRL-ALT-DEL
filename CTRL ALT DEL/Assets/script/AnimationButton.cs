using UnityEngine;
using System.Collections;

public class AnimationButton : MonoBehaviour
{
    public GameObject Menu1;
    public GameObject Menu2;
    public Animator Cam;
    public string animation_name;

    public void Button()
    {
        StartCoroutine(AnimationChange());
    }

    public IEnumerator AnimationChange()
    {
        Menu1.SetActive(false);
        Cam.Play(animation_name);
        yield return new WaitForSeconds(1f);
        Menu2.SetActive(true);
    } 
}
