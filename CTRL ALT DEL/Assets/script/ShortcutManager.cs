using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShortcutManager : MonoBehaviour
{
    [Header("Shortcut Toggles")]
    public bool canCopy = true;
    public bool canCut = true;
    public bool canSave = true;
    public bool canLoad = true;
    public bool canDelete = true;

    [Header("Shortcut Limits")]
    public int maxPasteUses = -1; // -1 = unlimited
    [HideInInspector] public int pasteUses = 0;
    public GameObject copyed;
    public GameObject cut;
    public TextMeshProUGUI uses;
    public TextMeshProUGUI Powers;
    GameObject obj;
    GameObject DeleteObj;
    public LayerMask Selectable;
    public LayerMask DoNotDelete;
    private Controls controls;
    public Transform Player;
    public Camera cam;
    GameObject ThePastePrefab;
    public float PlayerX;
    public float PlayerZ;
    public float PlayerY;
    bool selected = false;
    private ObjectProperties selectedProps;

    public GameObject SaveParticle, LoadParticle;
    bool cutted = false;
    bool isPaused = false;
    public GameObject pauseMenu, normalUI;

    void Awake()
    {
        controls = new Controls();
    }

    void Update()
    {
        Power();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;
            pauseMenu.SetActive(isPaused);
            normalUI.SetActive(!isPaused);
        }
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (obj != null)
        {
            Vector3 targetPos = cam.transform.position + cam.transform.forward * 3f;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            Vector3 dir = targetPos - rb.position;
            rb.linearVelocity = dir * 10f; // tune multiplier for snappiness
        }

        if (!canCopy && !canCut)
            uses.text = "Can't Copy";
        else if (!canCopy && canCut)
            uses.text = "Can Cut";
        else if (maxPasteUses != -1)
            uses.text = pasteUses + "/" + maxPasteUses;
        else
            uses.text = pasteUses + "/unlimited";
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
        if (!canCopy)
            return;
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, 4f, Selectable))
        {
            StartCoroutine(CopyIndicator(copyed));
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
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            Vector3 spawnPos;

            // Create a mask that includes everything except the Player layer
            int ignorePlayerMask = ~LayerMask.GetMask("Player");

            if (Physics.Raycast(ray, out hit, 100f, ignorePlayerMask))
            {
                // Spawn at the hit point on the map, slightly offset
                spawnPos = hit.point + hit.normal * 0.1f;
            }
            else
            {
                // If nothing was hit, fallback to 4 units in front of the camera
                spawnPos = cam.transform.position + cam.transform.forward * 4f;
            }

            if (cutted == true)
            {
                GameObject fuck = Instantiate(ThePastePrefab, spawnPos, ThePastePrefab.transform.rotation);
                fuck.SetActive(true);
                StartCoroutine(Delete(ThePastePrefab));
                cutted = false;
                return;
            }
            if (maxPasteUses != -1 && pasteUses >= maxPasteUses)
                return;

            GameObject newObj = Instantiate(ThePastePrefab, spawnPos, ThePastePrefab.transform.rotation);
            newObj.SetActive(true);

            pasteUses++;
        }
    }



    void OnCut()
    {
        if (!canCut)
            return;
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, 4f, Selectable))
        {
            StartCoroutine(CopyIndicator(cut));
            cutted = true;
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
        if (!canSave)
            return;
        PlayerX = Player.transform.position.x;
        PlayerZ = Player.transform.position.z;
        PlayerY = Player.transform.position.y;
        SaveParticle.SetActive(false);
        SaveParticle.SetActive(true);
    }

    void OnLoad()
    {
        if (!canLoad)
            return;
        StartCoroutine(ParticleLoad(LoadParticle));
    }

    void OnDelete()
    {
        if (!canDelete)
            return;

        int ignorePlayerMask = ~LayerMask.GetMask("Player");
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, 10f, ignorePlayerMask))
        {
            ObjectProperties objProps = hit.collider.GetComponent<ObjectProperties>();

            // Skip if object has ObjectProperties and is marked as DoNotDelete
            if ((objProps != null && objProps.IsDoNotDelete) || ((DoNotDelete.value & (1 << hit.collider.gameObject.layer)) != 0))
                return;


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

    public IEnumerator CopyIndicator(GameObject text)
    {
        text.SetActive(true);
        yield return new WaitForSeconds(1f);
        text.SetActive(false);
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

    public IEnumerator ParticleLoad(GameObject obj)
    {
        obj.SetActive(false);
        obj.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (PlayerX != 0 && PlayerY != 0 && PlayerZ != 0)
            Player.transform.position = new Vector3(PlayerX, PlayerY, PlayerZ);
    }

    void Power()
    {
        Powers.text = "E - Interact" + '\n' + "R - Reload Level" + '\n' + "Paste - CTRL+V" + '\n';
        if (canCopy)
            Powers.text += "Copy - CTRL+C" + '\n';
        if (canCut)
            Powers.text += "Cut - CTRL+X" + '\n';
        if (canSave)
            Powers.text += "Save - CTRL+S" + '\n';
        if (canLoad)
            Powers.text += "Load - CTRL+L" + '\n';
        if (canDelete)
            Powers.text += "Delete - CTRL+D" + '\n';
    }
}