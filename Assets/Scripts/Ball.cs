using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        Vector3 p1 = transform.position;
        float distanceToObstacle = 0;


        LayerMask mask = LayerMask.GetMask("PlatformGround");
        Debug.DrawRay(p1, transform.up * -1, Color.red);
        if (Physics.SphereCast(p1, this.transform.localScale.y / 2, transform.up * -1, out hit, 10, mask))
        {
            distanceToObstacle = hit.distance;
            //Vector3 position = this.transform.position 
            //Debug.Log(hit.collider.name + " : " + distanceToObstacle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PickerController>() != null)
        {
            Debug.Log("x");
            //rigidbody.velocity = this.transform.forward * Time.deltaTime * 1000;
        }
    }
}
