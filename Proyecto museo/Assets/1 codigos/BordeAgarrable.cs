using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordeAgarrable : MonoBehaviour
{
    public Transform puntoAgarre;
    public Transform limiteIzquierda;
    public Transform limiteDerecha;

    void OnDrawGizmos()
    {
        if (limiteIzquierda == null ||
            limiteDerecha == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            limiteIzquierda.position,
            limiteDerecha.position
        );

        Gizmos.DrawSphere(
            limiteIzquierda.position,
            0.08f
        );

        Gizmos.DrawSphere(
            limiteDerecha.position,
            0.08f
        );
    }
}
