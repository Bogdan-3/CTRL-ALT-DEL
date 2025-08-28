using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class ShortcutManager : MonoBehaviour
{
    GameObject obj;
    GameObject DeleteObj;
    public LayerMask Selectable;
    public LayerMask DoNotDelete;
    private Controls controls;
    public Transform Player;
    public Camera cam;
    GameObject ThePastePrefab;
    float PlayerX;
    float PlayerZ;
    float PlayerY;
    bool selected = false;
    private ObjectProperties selectedProps;

    void Awake()
    {
        controls = new Controls();
    }

    void Update()
    {
        if (obj != null)
        {
            Vector3 targetPos = cam.transform.position + cam.transform.forward * 3f;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            Vector3 dir = targetPos - rb.position;
            rb.linearVelocity = dir * 10f; // tune multiplier for snappiness
        }
    }

    void OnEnable()
    {
        controls.Enable();

        // Subscribe to shortcuts
        controls.Gameplay.Copy.performed += ctx => OnCopy();     //copy in thePastePrefab
        controls.Gameplay.Paste.performed += ctx => OnPaste();   //creaza o copie activa la thePastePrefab
        controls.Gameplay.Cut.performed += ctx => OnCut();       //taie in thePastePrefab
        controls.Gameplay.Delete.performed += ctx => OnDelete(); //Da delete la gameobject
        controls.Gameplay.Save.performed += ctx => OnSave();     //salveaza cordonate player
        controls.Gameplay.Load.performed += ctx => OnLoad();     //tp player la cordonate salvate
        controls.Gameplay.Reload.performed += ctx => reload();   //reload scene
        controls.Gameplay.Interact.performed += ctx => Interact();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void ReplaceClipboard(GameObject source)
    {
        if (ThePastePrefab != null)
            Destroy(ThePastePrefab); // remove old one

        ThePastePrefab = Instantiate(source);
        ThePastePrefab.SetActive(false);
    }

    // Handlers
    void OnCopy()
    {
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, 10f, Selectable))
        {
            selectedProps = hit.collider.GetComponent<ObjectProperties>();
            if (selectedProps.IsCopyable == false)
                return;
            ReplaceClipboard(hit.collider.gameObject);
        }
    }

    void OnPaste()
    {
        if (ThePastePrefab != null)
        {
            if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, 5f))
            {
                ObjectProperties map = hit.collider.GetComponent<ObjectProperties>();
                if (map.IsMap)
                    return;
            }
            // Calculate position in front of camera
                Vector3 spawnPos = cam.transform.position + cam.transform.forward * 5f;

            GameObject newObj = Instantiate(ThePastePrefab, spawnPos, ThePastePrefab.transform.rotation);
            newObj.SetActive(true);
        }
    }

    void OnCut()
    {
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, 10f, Selectable))
        {
            selectedProps = hit.collider.GetComponent<ObjectProperties>();
            if (selectedProps.IsCopyable == false)
                return;
            DeleteObj = hit.collider.gameObject;
            ReplaceClipboard(DeleteObj);
            StartCoroutine(Delete(DeleteObj));
        }
    }

    void OnSave()
    {
        PlayerX = Player.transform.position.x;
        PlayerZ = Player.transform.position.z;
        PlayerY = Player.transform.position.y;
    }

    void OnLoad()
    {
        if (PlayerX != 0 && PlayerY != 0 && PlayerZ != 0)
            Player.transform.position = new Vector3(PlayerX, PlayerY, PlayerZ);
    }

    void OnDelete()
    {
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, 10f))
            if (((1 << hit.collider.gameObject.layer) & DoNotDelete) == 0)
            {
                DeleteObj = hit.collider.gameObject;
                StartCoroutine(Delete(DeleteObj));
            }
    }

    void reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator Delete(GameObject obj)
    {
        obj.transform.position = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(0.25f); //delay here

        Destroy(obj); // remove original
    }

    void Interact()
    {
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, 4f, Selectable))
        {
            selectedProps = hit.collider.GetComponent<ObjectProperties>();
            if (selectedProps.IsMoveable == false)
                return;
            if (selected == false)
            {
                obj = hit.collider.gameObject;
                selected = true;
            }
            else
            {
                obj = null;
                selected = false;
            }
        }
        else if (selected == true)
        {
            obj = null;
            selected = false;
        }
    }
}
