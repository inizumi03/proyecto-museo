using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamB : MonoBehaviour
{
    public float velocidad = 5f;

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        transform.position +=
            transform.right * x * velocidad * Time.deltaTime;

        transform.position +=
            transform.up * y * velocidad * Time.deltaTime;
    }
}
