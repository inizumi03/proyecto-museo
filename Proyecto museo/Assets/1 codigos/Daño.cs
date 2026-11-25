using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daño : MonoBehaviour
{
    public LayerMask jugadorLayer;

    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & jugadorLayer) == 0)
            return;

        Jugador jugador = collision.gameObject.GetComponent<Jugador>();

        if (jugador != null)
        {
            jugador.estado = Jugador.Estado.Derrota;
        }
    }
}
