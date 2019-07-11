using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Injured : MonoBehaviour 
{
    public enum InjuryType { MINOR, SERIOUS }

    public bool isRescued;
    public InjuryType type;

    protected virtual void Start()
    {
        isRescued = false;
    }

    public virtual void Rescue(AmonController player) // 부상자 구조 시
    {
        isRescued = true;
        gameObject.tag = "Rescued";
    }
}
