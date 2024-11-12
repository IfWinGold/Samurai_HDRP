using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleTest : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField] private float radius;
    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, radius);


        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, target.transform.position);


        Vector3 distance = target.transform.position - this.transform.position;
        float angle = Mathf.Atan2(distance.x,distance.z) * Mathf.Rad2Deg;
        Debug.Log(angle);
    }
}
