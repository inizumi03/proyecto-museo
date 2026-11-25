using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gancho : MonoBehaviour
{

    [Header("Movimiento")]
    public float velocidad = 12f;
    public float distanciaLlegada = 0.5f;

    [Header("Referencias")]
    public ManagerPuntosGancho managerPuntos;
    public soga soja;

    Rigidbody rb;
    Jugador jugador;
    accionesJugador acciones;
    AnimacionesJugador animaciones;

    PuntoGancho puntoActual;

    public bool enganchado { get; private set; }
    public bool subiendo { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jugador = GetComponent<Jugador>();
        acciones = GetComponent<accionesJugador>();
        animaciones = GetComponent<AnimacionesJugador>();
    }

    void Update()
    {
        if (enganchado)
            return;

        if (acciones.AccionPresionada(
                accionesJugador.Accion.Gancho) &&
            jugador.PuedeUsar(
                Jugador.FuncionJugador.Gancho))
        {
            PuntoGancho punto =
                managerPuntos.ObtenerPunto();

            if (punto != null)
                Conectar(punto);
        }
    }

    void FixedUpdate()
    {
        if (!enganchado || !subiendo)
            return;

        Vector3 destino =
            puntoActual.ObtenerPuntoLlegada();

        Vector3 direccion =
            destino - rb.position;

        if (direccion.magnitude <= distanciaLlegada)
        {
            Terminar(destino);
            return;
        }

        rb.velocity =
            direccion.normalized * velocidad;
    }

    void Conectar(PuntoGancho punto)
    {
        puntoActual = punto;

        managerPuntos.Bloquear();

        enganchado = true;
        subiendo = false;

        if (animaciones != null)
            animaciones.LanzarGancho();

        if (soja != null)
            soja.Activar(puntoActual.transform);
    }

    public void Subir()
    {
        if (!enganchado)
            return;

        rb.velocity = Vector3.zero;

        jugador.estado =
            Jugador.Estado.Colgando;

        subiendo = true;
    }

    void Terminar(Vector3 destino)
    {
        enganchado = false;
        subiendo = false;

        rb.velocity = Vector3.zero;
        rb.position = destino;

        puntoActual = null;

        jugador.estado =
            Jugador.Estado.Normal;

        if (soja != null)
            soja.Desactivar();

        managerPuntos.Desbloquear();
    }
}

