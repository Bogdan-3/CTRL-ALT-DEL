using UnityEngine;

[System.Flags]
public enum ObjectPropertiesFlags
{
    None = 0,
    Copyable = 1 << 0,
    Moveable = 1 << 1,
    DoNotDelete=1<<2,
    // add more here when needed
}

public class ObjectProperties : MonoBehaviour
{
    public ObjectPropertiesFlags flags;

    public bool IsCopyable => flags.HasFlag(ObjectPropertiesFlags.Copyable);
    public bool IsMoveable => flags.HasFlag(ObjectPropertiesFlags.Moveable);
    public bool IsDoNotDelete => flags.HasFlag(ObjectPropertiesFlags.DoNotDelete);
}
