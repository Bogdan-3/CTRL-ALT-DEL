using UnityEngine;

public class ShortcutTrigger : MonoBehaviour
{
    public bool enableCopy = true;
    public bool enableCut = true;
    public bool enableSave = true;
    public bool enableLoad = true;
    public bool enableDelete = true;
    public ShortcutManager manager;

    public int newMaxPasteUses = -1; // set to -1 for unlimited

    private void OnTriggerEnter(Collider other)
    {

        if (manager != null)
        {
            manager.canCopy = enableCopy;
            manager.canCut = enableCut;
            manager.canSave = enableSave;
            manager.canLoad = enableLoad;
            manager.canDelete = enableDelete;

            manager.maxPasteUses = newMaxPasteUses;
            manager.pasteUses = 0;
        }
    }
}
