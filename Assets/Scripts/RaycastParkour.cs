using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastParkour : MonoBehaviour {

    public RaycastDistance[] RaycastDistances;
    RaycastHit hitInfo;
    Animator anim;

    void Start()
    {
        anim = GetComponent < Animator > ();
    }

    void Update()
    {
        for(int i = 0; i < RaycastDistances.Length; ++i)
        {
            if(Physics.Raycast(RaycastDistances[i].start.position, RaycastDistances[i].dir, out hitInfo))
            {
                if(hitInfo.distance <= RaycastDistances[i].maxDistance && hitInfo.distance >= RaycastDistances[i].minDistance)
                {
                    anim.SetInteger("parkourState", RaycastDistances[i].parkourState);
                }
            }
        }
    }
}

[System.Serializable]
public class RaycastDistance
{
    public Transform start;
    public Vector3 dir;
    public float minDistance, maxDistance;
    public int parkourState;
}