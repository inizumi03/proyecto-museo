using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PuntoGancho : MonoBehaviour
{
    public Renderer objeto;
    public Transform puntoLlegada;

    public void Activar()
    {
        if (ManagerMateriales.instancia != null &&
            objeto != null)
        {
            ManagerMateriales.instancia.Activar(objeto);
        }
    }

    public void Desactivar()
    {
        if (ManagerMateriales.instancia != null &&
            objeto != null)
        {
            ManagerMateriales.instancia.Desactivar(objeto);
        }
    }

    public Vector3 ObtenerPuntoLlegada()
    {
        if (puntoLlegada != null)
            return puntoLlegada.position;

        return transform.position;
    }
}
