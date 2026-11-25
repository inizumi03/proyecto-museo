using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Deslizar : MonoBehaviour
{
    public float velocidad = 12f;
    public float duracion = 0.2f;
    public float cooldown = 3f;

    public LayerMask obstaculos;
    public float distanciaDeteccion = 0.7f;

    Rigidbody rb;
    Mov mov;
    Jugador jugador;
    accionesJugador acciones;

    public bool deslizando { get; private set; }

    float tiempoDash;
    float tiempoCooldown;

    Vector3 direccionDash;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mov = GetComponent<Mov>();
        jugador = GetComponent<Jugador>();
        acciones = GetComponent<accionesJugador>();
    }

    void Update()
    {
        if (tiempoCooldown > 0)
            tiempoCooldown -= Time.deltaTime;

        if (acciones.AccionPresionada(
            accionesJugador.Accion.Deslizarse) &&
            PuedeHacerDash())
        {
            IniciarDash();
        }
    }

    void FixedUpdate()
    {
        if (!deslizando)
            return;

        tiempoDash -= Time.fixedDeltaTime;

        if (tiempoDash <= 0)
        {
            TerminarDash();
            return;
        }

        if (Physics.Raycast(
            transform.position,
            direccionDash,
            distanciaDeteccion,
            obstaculos))
        {
            TerminarDash();
            return;
        }

        rb.velocity = new Vector3(
            direccionDash.x * velocidad,
            rb.velocity.y,
            direccionDash.z * velocidad
        );
    }

    bool PuedeHacerDash()
    {
        return !deslizando &&
               tiempoCooldown <= 0 &&
               jugador.PuedeUsar(
                   Jugador.FuncionJugador.Dash
               );
    }

    void IniciarDash()
    {
        if (mov.direccionMovimiento != Vector3.zero)
            direccionDash = mov.direccionMovimiento;
        else
            direccionDash = transform.forward;

        direccionDash.y = 0;
        direccionDash.Normalize();

        deslizando = true;

        tiempoDash = duracion;
        tiempoCooldown = cooldown;

        mov.PuedeMoverse(false);
    }

    void TerminarDash()
    {
        deslizando = false;

        rb.velocity = new Vector3(
            0,
            rb.velocity.y,
            0
        );

        mov.PuedeMoverse(
            jugador.PuedeUsar(
                Jugador.FuncionJugador.Moverse
            )
        );
    }
}
