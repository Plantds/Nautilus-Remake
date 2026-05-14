using ArtNotes.PhysicalInteraction;
using UnityEngine;

public abstract class RepairableObject : InteractableObject
{
    [HideInInspector]
    public bool broken = false;
    [HideInInspector]
    public bool interact = false;
    float repairTimer;
    public float repairTime = 3;
    RepairBar repairBar;
    Vector3 repairBarScale;
    void Start()
    {
        repairBar = FindAnyObjectByType<RepairBar>();
        repairBar.enabled = false;
    }
    void OnValidate()
    {
        repairBarScale = repairBar.transform.localScale;
    }
    public void RepairUpdate()
    {
        if (interact && broken)
        {
            repairBar.enabled = true;
            repairTimer += Time.deltaTime;
            repairBar.progressBar.transform.localScale = new Vector3 (repairTimer/repairTime, repairBarScale.y, repairBarScale.z);

            if (repairTimer >= repairTime)
            {
                OnRepared();
            }
        }
    }

    public virtual void OnRepared()
    {
        broken = false;
        repairBar.enabled = false;
        repairBar.progressBar.transform.localScale = new Vector3(0, repairBarScale.y, repairBarScale.z);
    }

    public virtual void Break()
    {
        repairTimer = 0;
        broken = true;
    }


    
    public override void InteractStart(RaycastHit hit)
    {
        base.InteractStart(hit);
        interact = true;
    }
    public override void InteractEnd()
    {
        base.InteractEnd();
        interact = false;
        repairBar.enabled = false;
        repairBar.progressBar.transform.localScale = new Vector3(0, repairBarScale.y, repairBarScale.z);
    }
}
