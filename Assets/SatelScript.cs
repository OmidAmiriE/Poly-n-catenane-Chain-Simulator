using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SatelScript : MonoBehaviour
{
    public int linkNum;
    public Transform dish;
    private Rigidbody rb;
    public float closestDist=10f;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (Mathf.Abs(other.GetComponent<SatelScript>().linkNum -linkNum)!=2) return;
        Transform TF = other.transform;
        Vector3 thisToTF = TF.position - transform.position;
        float sqrD = thisToTF.sqrMagnitude;
        if (sqrD<closestDist)
        {
            closestDist = sqrD;
            dish.localRotation = Quaternion.LookRotation(new Vector3(thisToTF.x,0f,thisToTF.z));
            //Debug.DrawLine(TF.position+new Vector3(0f,0.1f,0f), transform.position, Color.red);
        }

        Vector3 finalForce = (ChainGen.stackForceSTATIC / (sqrD + 1f)) * (thisToTF).normalized;
        rb.AddForceAtPosition(finalForce , transform.position, ForceMode.Force);
    }
}
