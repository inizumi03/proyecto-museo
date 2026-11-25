using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empujar : MonoBehaviour
{
    [Header("Referencias")]
    public Transform puntoEmpujeJugador;
    public Cam camara;

    [Header("Acomodar")]
    public float velocidadAcomodar = 4f;
    public float velocidadRotacion = 360f;

    Jugador jugador;
    Mov mov;
    Rigidbody rb;
    accionesJugador acciones;

    ObjetoEmpujable objetoContacto;
    ObjetoEmpujable objetoEmpujando;

    Vector3 direccionEmpuje;
    Vector3 separacionJugador;
    Vector3 posicionAcomodar;

    Quaternion rotacionAcomodar;

    bool acomodando;
    bool contactoLateral;

    void Start()
    {
        jugador = GetComponent<Jugador>();
        mov = GetComponent<Mov>();
        rb = GetComponent<Rigidbody>();
        acciones = GetComponent<accionesJugador>();

        if (camara == null && Camera.main != null)
            camara = Camera.main.GetComponent<Cam>();
    }

    void Update()
    {
        if (jugador.estado ==
            Jugador.Estado.HaciendoFuerza)
        {
            if (PresionoEmpujar())
                TerminarEmpuje();

            return;
        }

        if (acomodando ||
            objetoContacto == null ||
            !jugador.PuedeUsar(
                Jugador.FuncionJugador.HacerFuerza))
            return;

        if (PresionoEmpujar())
            PrepararEmpuje();
    }

    void FixedUpdate()
    {
        if (acomodando)
        {
            AcomodarJugador();
            return;
        }

        if (jugador.estado !=
            Jugador.Estado.HaciendoFuerza)
            return;

        if (objetoEmpujando == null)
        {
            TerminarEmpuje();
            return;
        }

        if (Input.GetAxisRaw("Vertical") <= 0f)
            return;

        Vector3 posicionCaja =
            objetoEmpujando.Mover(
                direccionEmpuje
            );

        Vector3 diferencia =
            posicionCaja +
            separacionJugador -
            puntoEmpujeJugador.position;

        diferencia.y = 0f;

        rb.MovePosition(
            rb.position + diferencia
        );
    }

    void OnCollisionStay(Collision colision)
    {
        ObjetoEmpujable objeto =
            colision.collider.GetComponent<
                ObjetoEmpujable>();

        if (objeto == null)
            return;

        objetoContacto = objeto;

        if (EstaEmpujando())
            return;

        float horizontal =
            Mathf.Abs(
                Input.GetAxisRaw("Horizontal"));

        float vertical =
            Mathf.Abs(
                Input.GetAxisRaw("Vertical"));

        if (horizontal > 0f ||
            vertical > 0f)
        {
            contactoLateral =
                horizontal > vertical;
        }
    }

    void OnCollisionExit(Collision colision)
    {
        ObjetoEmpujable objeto =
            colision.collider.GetComponent<
                ObjetoEmpujable>();

        if (objeto != objetoContacto ||
            EstaEmpujando())
            return;

        objetoContacto = null;
        contactoLateral = false;
    }

    void PrepararEmpuje()
    {
        if (puntoEmpujeJugador == null ||
            objetoContacto == null)
            return;

        Vector3 puntoCaja;

        objetoContacto.ObtenerDatosEmpuje(
            transform.position,
            out puntoCaja,
            out direccionEmpuje
        );

        objetoEmpujando =
            objetoContacto;

        rb.velocity = Vector3.zero;

        mov.PuedeMoverse(false);

        if (contactoLateral)
        {
            PrepararAcomodado(
                puntoCaja
            );

            return;
        }

        IniciarEmpuje();
    }

    void PrepararAcomodado(
        Vector3 puntoCaja)
    {
        rotacionAcomodar =
            Quaternion.LookRotation(
                direccionEmpuje
            );

        Vector3 distanciaPunto =
            puntoEmpujeJugador.position -
            transform.position;

        distanciaPunto.y = 0f;

        posicionAcomodar =
            puntoCaja -
            direccionEmpuje *
            distanciaPunto.magnitude;

        posicionAcomodar.y =
            rb.position.y;

        acomodando = true;

        if (camara != null)
        {
            camara.AcomodarParaEmpuje(
                direccionEmpuje
            );
        }
    }

    void AcomodarJugador()
    {
        Vector3 posicion =
            Vector3.MoveTowards(
                rb.position,
                posicionAcomodar,
                velocidadAcomodar *
                Time.fixedDeltaTime
            );

        Quaternion rotacion =
            Quaternion.RotateTowards(
                rb.rotation,
                rotacionAcomodar,
                velocidadRotacion *
                Time.fixedDeltaTime
            );

        rb.MovePosition(posicion);
        rb.MoveRotation(rotacion);

        if (Vector3.Distance(
                posicion,
                posicionAcomodar) > 0.01f)
            return;

        if (Quaternion.Angle(
                rotacion,
                rotacionAcomodar) > 1f)
            return;

        acomodando = false;

        IniciarEmpuje();
    }

    void IniciarEmpuje()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.MoveRotation(
            Quaternion.LookRotation(
                direccionEmpuje
            )
        );

        separacionJugador =
            puntoEmpujeJugador.position -
            objetoEmpujando.ObtenerPosicion();

        jugador.estado =
            Jugador.Estado.HaciendoFuerza;
    }

    void TerminarEmpuje()
    {
        acomodando = false;
        contactoLateral = false;

        objetoEmpujando = null;
        objetoContacto = null;

        direccionEmpuje = Vector3.zero;
        separacionJugador = Vector3.zero;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        jugador.estado =
            Jugador.Estado.Normal;

        mov.PuedeMoverse(
            jugador.PuedeUsar(
                Jugador.FuncionJugador.Moverse
            )
        );
    }

    bool PresionoEmpujar()
    {
        return acciones.AccionPresionada(
            accionesJugador.Accion.Empujar
        );
    }

    bool EstaEmpujando()
    {
        return
            acomodando ||
            jugador.estado ==
            Jugador.Estado.HaciendoFuerza;
    }

    public bool PuedeEmpujar()
    {
        return
            objetoContacto != null &&
            !EstaEmpujando() &&
            jugador.PuedeUsar(
                Jugador.FuncionJugador.HacerFuerza
            );
    }

}

