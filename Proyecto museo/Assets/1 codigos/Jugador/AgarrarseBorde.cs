using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgarrarseBorde : MonoBehaviour
{
    [Header("Jugador")]
    public Transform puntoManos;

    [Header("Movimiento Borde")]
    public float velocidadBorde = 2f;

    [Header("Entrada al Borde")]
    public float velocidadAgarre = 5f;
    public float distanciaLlegada = 0.02f;

    Jugador jugador;
    accionesJugador acciones;
    Rigidbody rb;
    Salto salto;
    AnimacionesJugador animaciones;
    BordeAgarrable bordeActual;

    bool puedeAgarrarse;
    bool agarrado;
    bool entrandoBorde;

    Vector3 posicionObjetivo;

    public float movimientoBorde { get; private set; }

    void Start()
    {
        jugador = GetComponent<Jugador>();
        acciones = GetComponent<accionesJugador>();
        rb = GetComponent<Rigidbody>();
        salto = GetComponent<Salto>();
        animaciones = GetComponent<AnimacionesJugador>();
    }

    void Update()
    {
        if (entrandoBorde)
        {
            MoverHaciaBorde();
            return;
        }

        if (agarrado)
        {
            movimientoBorde =
                Input.GetAxisRaw("Horizontal");

            MoverPorBorde();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SaltarDesdeBorde();
                return;
            }

            if (acciones.AccionPresionada(
                accionesJugador.Accion.AgarrarseBorde))
            {
                SoltarBorde();
            }

            return;
        }

        movimientoBorde = 0;

        if (puedeAgarrarse &&
            acciones.AccionPresionada(
                accionesJugador.Accion.AgarrarseBorde))
        {
            AgarrarBorde();
        }
    }

    void OnTriggerEnter(Collider otro)
    {
        BordeAgarrable borde =
            otro.GetComponent<BordeAgarrable>();

        if (borde == null)
            return;

        bordeActual = borde;
        puedeAgarrarse = true;
    }

    void OnTriggerStay(Collider otro)
    {
        BordeAgarrable borde =
            otro.GetComponent<BordeAgarrable>();

        if (borde == null)
            return;

        bordeActual = borde;
        puedeAgarrarse = true;
    }

    void OnTriggerExit(Collider otro)
    {
        BordeAgarrable borde =
            otro.GetComponent<BordeAgarrable>();

        if (borde == null)
            return;

        if (borde != bordeActual)
            return;

        if (agarrado || entrandoBorde)
            return;

        bordeActual = null;
        puedeAgarrarse = false;
    }

    void AgarrarBorde()
    {
        if (bordeActual == null ||
            bordeActual.puntoAgarre == null ||
            bordeActual.limiteIzquierda == null ||
            bordeActual.limiteDerecha == null ||
            puntoManos == null)
            return;

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        Transform objetivo =
            bordeActual.puntoAgarre;

        Vector3 direccion =
            objetivo.forward;

        direccion.y = 0;

        if (direccion.sqrMagnitude > 0.01f)
        {
            transform.rotation =
                Quaternion.LookRotation(
                    direccion.normalized,
                    Vector3.up
                );
        }

        Vector3 lateral =
            objetivo.right.normalized;

        float limiteA =
            Vector3.Dot(
                bordeActual.limiteIzquierda.position -
                objetivo.position,
                lateral
            );

        float limiteB =
            Vector3.Dot(
                bordeActual.limiteDerecha.position -
                objetivo.position,
                lateral
            );

        float minimo =
            Mathf.Min(limiteA, limiteB);

        float maximo =
            Mathf.Max(limiteA, limiteB);

        float posicionJugador =
            Vector3.Dot(
                puntoManos.position -
                objetivo.position,
                lateral
            );

        posicionJugador =
            Mathf.Clamp(
                posicionJugador,
                minimo,
                maximo
            );

        Vector3 puntoAgarreReal =
            objetivo.position +
            lateral * posicionJugador;

        Vector3 diferencia =
            puntoAgarreReal -
            puntoManos.position;

        posicionObjetivo =
            transform.position +
            diferencia;

        entrandoBorde = true;

        jugador.estado =
            Jugador.Estado.AgarradoBorde;
    }

    void MoverHaciaBorde()
    {
        transform.position =
            Vector3.MoveTowards(
                transform.position,
                posicionObjetivo,
                velocidadAgarre * Time.deltaTime
            );

        float distancia =
            Vector3.Distance(
                transform.position,
                posicionObjetivo
            );

        if (distancia > distanciaLlegada)
            return;

        transform.position =
            posicionObjetivo;

        entrandoBorde = false;
        agarrado = true;
    }

    void MoverPorBorde()
    {
        if (bordeActual == null ||
            bordeActual.puntoAgarre == null ||
            bordeActual.limiteIzquierda == null ||
            bordeActual.limiteDerecha == null)
            return;

        if (Mathf.Abs(movimientoBorde) < 0.01f)
            return;

        Vector3 inicio =
            bordeActual.limiteIzquierda.position;

        Vector3 fin =
            bordeActual.limiteDerecha.position;

        Vector3 direccionLinea =
            fin - inicio;

        float largo =
            direccionLinea.magnitude;

        if (largo <= 0.01f)
            return;

        direccionLinea.Normalize();

        float posicionActual =
            Vector3.Dot(
                transform.position - inicio,
                direccionLinea
            );

        bool enLimiteIzquierdo =
            posicionActual <= 0.01f;

        bool enLimiteDerecho =
            posicionActual >= largo - 0.01f;

        if (enLimiteIzquierdo &&
            movimientoBorde < 0)
        {
            movimientoBorde = 0;
            return;
        }

        if (enLimiteDerecho &&
            movimientoBorde > 0)
        {
            movimientoBorde = 0;
            return;
        }

        float nuevaPosicion =
            posicionActual +
            movimientoBorde *
            velocidadBorde *
            Time.deltaTime;

        nuevaPosicion =
            Mathf.Clamp(
                nuevaPosicion,
                0,
                largo
            );

        Vector3 objetivo =
            inicio +
            direccionLinea *
            nuevaPosicion;

        Vector3 movimiento =
            objetivo -
            transform.position;

        movimiento.y = 0;

        transform.position += movimiento;
    }

    void SoltarBorde()
    {
        agarrado = false;
        entrandoBorde = false;
        puedeAgarrarse = false;
        movimientoBorde = 0;

        bordeActual = null;

        rb.isKinematic = false;
        rb.velocity = Vector3.zero;

        jugador.estado =
            Jugador.Estado.Normal;
    }

    void SaltarDesdeBorde()
    {
        agarrado = false;
        entrandoBorde = false;
        puedeAgarrarse = false;
        movimientoBorde = 0;

        bordeActual = null;

        rb.isKinematic = false;

        rb.velocity =
            new Vector3(
                0,
                salto.fuerzaSalto,
                0
            );

        jugador.estado =
            Jugador.Estado.Normal;

        if (animaciones != null)
            animaciones.IniciarSalto();
    }

    public bool PuedeAgarrarse()
    {
        return puedeAgarrarse &&
               !agarrado &&
               !entrandoBorde;
    }
}
