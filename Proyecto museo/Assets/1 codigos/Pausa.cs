using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausa : MonoBehaviour
{
    bool pausado;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            pausado = !pausado;

            Time.timeScale =
                pausado ? 0f : 1f;
        }
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
