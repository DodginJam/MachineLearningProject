using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    [field:SerializeField]
    public LayerMask LayersToHit
    { get; private set; }

    public bool TargetDetected
    { get; set; }

    public Target DetectedTarget
    {  get; private set; }

    [field: SerializeField]
    public MeshRenderer MeshRendererRef
    { get; private set; }

    [field: SerializeField]
    public Material NonFiringMaterial
    { get; private set; }

    [field: SerializeField]
    public Material FiringMaterial
    { get; private set; }

    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * 100f, Color.magenta);

        // Fire out a raycast to detect targets, while also hiting walls to allow obstacles like walls to block ray.
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, LayersToHit))
        {
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Target"))
            {
                if (hitInfo.transform.gameObject.TryGetComponent<Target>(out Target target))
                {
                    TargetDetected = true;
                    DetectedTarget = target;
                }
            }
            else
            {
                RemoveDetectedInfo();
            }
        }
        else
        {
            RemoveDetectedInfo();
        }
    }

    /// <summary>
    /// Returns true if a target has been assigned, and provides an out reference to that target object.
    /// </summary>
    /// <param name="detectedTarget"></param>
    /// <returns></returns>
    public bool IsTargetDetected(out Target detectedTarget)
    {
        detectedTarget = DetectedTarget;
        return TargetDetected;
    }

    /// <summary>
    /// Fire the weapon at the detected target if a reference to one is found.
    /// </summary>
    /// <param name="damageToFire"></param>
    public void FireWeapon(float damageToFire)
    {
        if (DetectedTarget != null)
        {
            DetectedTarget.TakeDamage(damageToFire * Time.fixedDeltaTime);
        }
    }

    public void RemoveDetectedInfo()
    {
        TargetDetected = false;
        DetectedTarget = null;
    }

    public void SetFiringMaterial(bool isFiring)
    {
        MeshRendererRef.material = isFiring ? FiringMaterial : NonFiringMaterial;
    }
}
