using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mov : MonoBehaviour
{
    public float velocidad = 5f;
    public float velocidadAgachado = 2.5f;
    public float velocidadRotacion = 10f;

    [Header("Giro")]
    public float anguloFrenado = 70f;
    public float velocidadMinimaGiro = 0.15f;

    [Header("Camara")]
    public Transform camara;

    [Header("Obstaculos")]
    public LayerMask obstaculos;
    public float margenObstaculo = 0.05f;

    Rigidbody rb;
    CapsuleCollider capsula;
    Jugador jugador;

    float x;
    float z;

    bool puedeMoverse = true;

    public Vector3 direccionMovimiento { get; private set; }
    public bool estaMoviendose { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsula = GetComponent<CapsuleCollider>();
        jugador = GetComponent<Jugador>();
    }

    void Update()
    {
        if (!puedeMoverse)
        {
            direccionMovimiento = Vector3.zero;
            estaMoviendose = false;
            return;
        }

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        Vector3 adelante = camara.forward;
        Vector3 derecha = camara.right;

        adelante.y = 0f;
        derecha.y = 0f;

        adelante.Normalize();
        derecha.Normalize();

        direccionMovimiento =
            (adelante * z +
             derecha * x).normalized;

        if (direccionMovimiento == Vector3.zero)
            estaMoviendose = false;
    }

    void FixedUpdate()
    {
        estaMoviendose = false;

        if (!puedeMoverse ||
            direccionMovimiento == Vector3.zero)
            return;

        float multiplicadorMovimiento =
            CalcularMovimientoDuranteGiro();

        Rotar();

        float velocidadActual =
            jugador.estado ==
            Jugador.Estado.Agachado
            ? velocidadAgachado
            : velocidad;

        float distancia =
            velocidadActual *
            multiplicadorMovimiento *
            Time.fixedDeltaTime;

        Vector3 direccionPermitida =
            ObtenerDireccionPermitida(
                direccionMovimiento,
                distancia
            );

        if (direccionPermitida.sqrMagnitude <
            0.001f)
            return;

        rb.MovePosition(
            rb.position +
            direccionPermitida *
            distancia
        );

        estaMoviendose = true;
    }

    Vector3 ObtenerDireccionPermitida(
        Vector3 direccion,
        float distancia)
    {
        Vector3 puntoArriba;
        Vector3 puntoAbajo;
        float radio;

        ObtenerCapsula(
            out puntoArriba,
            out puntoAbajo,
            out radio
        );

        RaycastHit hit;

        if (!Physics.CapsuleCast(
            puntoAbajo,
            puntoArriba,
            radio,
            direccion,
            out hit,
            distancia + margenObstaculo,
            obstaculos,
            QueryTriggerInteraction.Ignore))
        {
            return direccion;
        }

        Vector3 deslizamiento =
            Vector3.ProjectOnPlane(
                direccion,
                hit.normal
            );

        deslizamiento.y = 0f;

        if (deslizamiento.sqrMagnitude <
            0.01f)
            return Vector3.zero;

        deslizamiento.Normalize();

        if (Physics.CapsuleCast(
            puntoAbajo,
            puntoArriba,
            radio,
            deslizamiento,
            out hit,
            distancia + margenObstaculo,
            obstaculos,
            QueryTriggerInteraction.Ignore))
        {
            return Vector3.zero;
        }

        return deslizamiento;
    }

    void ObtenerCapsula(
        out Vector3 puntoArriba,
        out Vector3 puntoAbajo,
        out float radio)
    {
        Vector3 centro =
            transform.TransformPoint(
                capsula.center
            );

        float escalaHorizontal =
            Mathf.Max(
                Mathf.Abs(transform.lossyScale.x),
                Mathf.Abs(transform.lossyScale.z)
            );

        float escalaVertical =
            Mathf.Abs(
                transform.lossyScale.y
            );

        radio =
            capsula.radius *
            escalaHorizontal;

        float altura =
            Mathf.Max(
                capsula.height *
                escalaVertical,
                radio * 2f
            );

        float distanciaCentro =
            altura * 0.5f -
            radio;

        puntoArriba =
            centro +
            Vector3.up *
            distanciaCentro;

        puntoAbajo =
            centro -
            Vector3.up *
            distanciaCentro;
    }

    float CalcularMovimientoDuranteGiro()
    {
        float angulo =
            Vector3.Angle(
                transform.forward,
                direccionMovimiento
            );

        if (angulo <= 5f)
            return 1f;

        float porcentaje =
            1f - Mathf.Clamp01(
                angulo /
                anguloFrenado
            );

        return Mathf.Lerp(
            velocidadMinimaGiro,
            1f,
            porcentaje
        );
    }

    void Rotar()
    {
        Quaternion rotacionObjetivo =
            Quaternion.LookRotation(
                direccionMovimiento
            );

        Quaternion nuevaRotacion =
            Quaternion.Slerp(
                rb.rotation,
                rotacionObjetivo,
                velocidadRotacion *
                Time.fixedDeltaTime
            );

        rb.MoveRotation(
            nuevaRotacion
        );
    }

    public void PuedeMoverse(bool valor)
    {
        puedeMoverse = valor;

        if (!puedeMoverse)
        {
            x = 0f;
            z = 0f;

            direccionMovimiento =
                Vector3.zero;

            estaMoviendose =
                false;
        }
    }
}
