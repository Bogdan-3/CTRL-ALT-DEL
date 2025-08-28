using System.Collections;
using UnityEngine;

public class DeleteCopy : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            StartCoroutine(Delete(other.gameObject));
            return;
        }
        var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var obj in allObjects)
            if (obj.name.Contains("(Clone)"))
                StartCoroutine(Delete(obj));
    }

    public IEnumerator Delete(GameObject obj)
    {
        obj.transform.position = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(0.25f); //delay here

        Destroy(obj); // remove original
    }
}
