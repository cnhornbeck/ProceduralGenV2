using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{

    [SerializeField] float movementSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0, movementSpeed * 100 * Time.deltaTime);
    }
}
