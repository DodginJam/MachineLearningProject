using UnityEngine;
using ProjectEnums;

public abstract class Target : MonoBehaviour, ITarget
{
    public TargetType TargetTyping
    { get; private set; }

    public TargetManager TargetManager
    { get; private set; }

    protected virtual void Awake()
    {
        TargetManager = transform.parent.GetComponentInChildren<TargetManager>();

        Initialise();
    }

    public abstract void Initialise();

    public void SetTargetTyping(TargetType type)
    {
        TargetTyping = type;
    }

    public virtual void SetSize(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public int GetGameObjectsInstanceID()
    {
        return this.gameObject.GetInstanceID();
    }
}
