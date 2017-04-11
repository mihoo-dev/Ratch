using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonPlayer : MonoBehaviour {

    public bool Alive = true;

    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.name == "eyes")
        {
            Other.transform.parent.GetComponent<monster>().CheckSight();
        }
    }
}
