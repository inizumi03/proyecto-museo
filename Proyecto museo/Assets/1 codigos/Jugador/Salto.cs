using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salto : MonoBehaviour
{
    public Transform pie;
    public LayerMask suelo;

    public float fuerzaSalto = 7f;
    public float radioPie = 0.2f;
    public float fuerzaCaida = 2f;

    Rigidbody rb;
    Jugador jugador;
    AnimacionesJugador animaciones;

    public bool enSuelo { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jugador = GetComponent<Jugador>();
        animaciones = GetComponent<AnimacionesJugador>();
    }

    void Update()
    {
        enSuelo = Physics.CheckSphere(
            pie.position,
            radioPie,
            suelo
        );

        ActualizarEstadoAire();

        if (Input.GetKeyDown(KeyCode.Space) &&
            enSuelo &&
            jugador.PuedeUsar(
                Jugador.FuncionJugador.Saltar
            ))
        {
            rb.velocity = new Vector3(
                rb.velocity.x,
                fuerzaSalto,
                rb.velocity.z
            );

            jugador.estado =
                Jugador.Estado.EnElAire;

            if (animaciones != null)
                animaciones.IniciarSalto();
        }

        if (Input.GetKeyUp(KeyCode.Space) &&
            rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(
                rb.velocity.x,
                rb.velocity.y * 0.5f,
                rb.velocity.z
            );
        }
    }

    void ActualizarEstadoAire()
    {
        if (enSuelo)
        {
            if (jugador.estado ==
                Jugador.Estado.EnElAire)
            {
                jugador.estado =
                    Jugador.Estado.Normal;
            }

            return;
        }

        if (jugador.estado ==
                Jugador.Estado.Normal ||
            jugador.estado ==
                Jugador.Estado.Agachado)
        {
            jugador.estado =
                Jugador.Estado.EnElAire;
        }
    }

    void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.AddForce(
                Vector3.down * fuerzaCaida,
                ForceMode.Acceleration
            );
        }
    }
}
