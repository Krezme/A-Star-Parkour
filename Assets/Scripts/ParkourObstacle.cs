using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourObstacle : MonoBehaviour {

    [Range(0, 180)]
    public float displacement = 20f;
    public int animationIndex;
    
    float getDisplacement(Vector3 eul1, Vector3 eul2)
    {
        return eul1.y - eul2.y;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.root.tag == "Player")
        {
            float displace = Mathf.Abs(getDisplacement(transform.eulerAngles, col.transform.eulerAngles));
            if (displace < displacement || displace > 360 - displacement)
            {
                col.transform.root.GetComponent<TPSController>().LockMovement();

                Animator anim = col.transform.root.GetComponent<Animator>();
                if (anim.GetInteger("parkourState") == 0)
                    anim.SetInteger("parkourState", animationIndex);
            }
        }
    }
}
