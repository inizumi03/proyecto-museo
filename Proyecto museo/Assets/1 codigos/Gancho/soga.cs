using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soga : MonoBehaviour
{
    public LineRenderer linea;

    Transform destino;

    void Start()
    {
        if (linea == null)
            linea = GetComponent<LineRenderer>();

        if (linea == null)
            return;

        linea.positionCount = 2;
        linea.enabled = false;
    }

    void LateUpdate()
    {
        if (linea == null ||
            destino == null ||
            !linea.enabled)
            return;

        linea.SetPosition(
            0,
            transform.position
        );

        linea.SetPosition(
            1,
            destino.position
        );
    }

    public void Activar(Transform nuevoDestino)
    {
        if (linea == null ||
            nuevoDestino == null)
            return;

        destino = nuevoDestino;

        linea.positionCount = 2;

        linea.SetPosition(
            0,
            transform.position
        );

        linea.SetPosition(
            1,
            destino.position
        );

        linea.enabled = true;
    }

    public void Desactivar()
    {
        destino = null;

        if (linea != null)
            linea.enabled = false;
    }
}
