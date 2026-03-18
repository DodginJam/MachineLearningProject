using Unity.Mathematics;
using UnityEngine;

public class DetectFacingTarget : MonoBehaviour
{
    [field:SerializeField]
    public LayerMask LayersToHit
    { get; private set; }

    public bool TargetDetected
    { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, LayersToHit))
        {
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Target"))
            {
                TargetDetected = true;
            }
        }
    }
}
