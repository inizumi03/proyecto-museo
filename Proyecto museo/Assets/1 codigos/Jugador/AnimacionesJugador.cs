using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacionesJugador : MonoBehaviour
{
    public Animator animator;

    [Header("Borde")]
    public float suavizadoMovimientoBorde = 0.12f;

    AgarrarseBorde agarrarseBorde;
    Mov mov;
    Deslizar deslizar;
    Jugador jugador;
    Salto salto;
    Gancho gancho;
    Rigidbody rb;
    accionesJugador acciones;

    bool saltando;

    static readonly int
        CorriendoID = Animator.StringToHash("Corriendo"),
        DashID = Animator.StringToHash("Dash"),
        OcultoID = Animator.StringToHash("Oculto"),
        GanchoID = Animator.StringToHash("Gancho"),
        LanzarGanchoID = Animator.StringToHash("LanzarGancho"),
        AgarradoBordeID = Animator.StringToHash("AgarradoBorde"),
        MovimientoBordeID = Animator.StringToHash("MovimientoBorde"),
        AgachadoID = Animator.StringToHash("Agachado"),
        CaminarAgachadoID = Animator.StringToHash("CaminarAgachado"),
        EnSueloID = Animator.StringToHash("EnSuelo"),
        SaltandoID = Animator.StringToHash("Saltando"),
        CaidaID = Animator.StringToHash("Caida"),
        HaciendoFuerzaID = Animator.StringToHash("HaciendoFuerza"),
        MoviendoObjetoID = Animator.StringToHash("MoviendoObjeto");

    void Start()
    {
        mov = GetComponent<Mov>();
        deslizar = GetComponent<Deslizar>();
        jugador = GetComponent<Jugador>();
        salto = GetComponent<Salto>();
        gancho = GetComponent<Gancho>();
        rb = GetComponent<Rigidbody>();
        acciones = GetComponent<accionesJugador>();
        agarrarseBorde = GetComponent<AgarrarseBorde>();
    }

    void Update()
    {
        ActualizarAgachado();
        ActualizarAnimaciones();
    }

    void ActualizarAgachado()
    {
        if (!acciones.AccionPresionada(
            accionesJugador.Accion.Agacharse))
            return;

        if (jugador.estado ==
            Jugador.Estado.Normal)
        {
            jugador.estado =
                Jugador.Estado.Agachado;
        }
        else if (jugador.estado ==
            Jugador.Estado.Agachado)
        {
            jugador.estado =
                Jugador.Estado.Normal;
        }
    }

    void ActualizarAnimaciones()
    {
        bool enSuelo = salto.enSuelo;

        bool enMovimiento =
            mov.estaMoviendose;

        bool oculto =
            jugador.estado ==
            Jugador.Estado.Oculto;

        bool borde =
            jugador.estado ==
            Jugador.Estado.AgarradoBorde;

        bool agachado =
            jugador.estado ==
            Jugador.Estado.Agachado;

        bool haciendoFuerza =
            jugador.estado ==
            Jugador.Estado.HaciendoFuerza;

        bool moviendoObjeto =
            haciendoFuerza &&
            Input.GetAxisRaw("Vertical") > 0f;

        bool subiendoGancho =
            gancho.subiendo;

        float movimientoBorde =
            borde
            ? agarrarseBorde.movimientoBorde
            : 0f;

        bool bloqueado =
            borde ||
            subiendoGancho;

        bool caida =
            !enSuelo &&
            rb.velocity.y < -0.1f &&
            !bloqueado;

        bool dash =
            deslizar.deslizando &&
            enSuelo &&
            !oculto &&
            !agachado &&
            !haciendoFuerza &&
            !bloqueado;

        if (caida || borde)
            saltando = false;

        if (saltando &&
            enSuelo &&
            rb.velocity.y <= 0f)
        {
            saltando = false;
        }

        animator.SetBool(
            CorriendoID,
            enMovimiento &&
            !saltando &&
            !caida &&
            !dash &&
            !oculto &&
            !agachado &&
            !haciendoFuerza &&
            !bloqueado
        );

        animator.SetBool(
            DashID,
            dash
        );

        animator.SetBool(
            OcultoID,
            oculto
        );

        animator.SetBool(
            GanchoID,
            subiendoGancho
        );

        animator.SetBool(
            AgarradoBordeID,
            borde
        );

        animator.SetFloat(
            MovimientoBordeID,
            movimientoBorde,
            suavizadoMovimientoBorde,
            Time.deltaTime
        );

        animator.SetBool(
            AgachadoID,
            agachado
        );

        animator.SetBool(
            CaminarAgachadoID,
            agachado && enMovimiento
        );

        animator.SetBool(
            EnSueloID,
            enSuelo
        );

        animator.SetBool(
            SaltandoID,
            saltando
        );

        animator.SetBool(
            CaidaID,
            caida
        );

        animator.SetBool(
            HaciendoFuerzaID,
            haciendoFuerza &&
            !moviendoObjeto
        );

        animator.SetBool(
            MoviendoObjetoID,
            moviendoObjeto
        );
    }

    public void IniciarSalto()
    {
        saltando = true;
    }

    public void LanzarGancho()
    {
        animator.SetTrigger(
            LanzarGanchoID
        );
    }
}
