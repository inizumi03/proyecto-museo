using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public Transform jugador;

    [Header("Mouse")]
    public float sensibilidad = 2.5f;

    [Header("Limites")]
    public float limiteAbajo = -25f;
    public float limiteArriba = 55f;

    [Header("Anticipacion Salto")]
    public float distanciaActivarAnticipacion = 1f;
    public float distanciaAnticipacion = 0.8f;
    public float suavizadoAnticipacion = 5f;

    [Header("Acomodar Empuje")]
    public float suavizadoAcomodarEmpuje = 8f;

    Vector3 distanciaInicial;

    Rigidbody rbJugador;
    Salto salto;

    float rotacionX;
    float rotacionY;

    float alturaInicioSalto;
    float anticipacionActual;

    bool estabaEnSuelo = true;

    bool acomodandoEmpuje;
    float rotacionYEmpuje;

    void Start()
    {
        Cursor.lockState =
            CursorLockMode.Confined;

        rbJugador =
            jugador.GetComponent<Rigidbody>();

        salto =
            jugador.GetComponent<Salto>();

        distanciaInicial =
            transform.position -
            jugador.position;

        Vector3 angulos =
            transform.eulerAngles;

        rotacionX = angulos.x;
        rotacionY = angulos.y;

        if (rotacionX > 180f)
            rotacionX -= 360f;

        alturaInicioSalto =
            jugador.position.y;
    }

    void LateUpdate()
    {
        LeerMouse();
        AcomodarEmpuje();
        DetectarSalto();
        ActualizarAnticipacion();
        MoverCamara();
    }

    void LeerMouse()
    {
        if (!acomodandoEmpuje)
        {
            rotacionY +=
                Input.GetAxisRaw("Mouse X") *
                sensibilidad;
        }

        rotacionX -=
            Input.GetAxisRaw("Mouse Y") *
            sensibilidad;

        rotacionX =
            Mathf.Clamp(
                rotacionX,
                limiteAbajo,
                limiteArriba
            );
    }

    void DetectarSalto()
    {
        if (estabaEnSuelo &&
            !salto.enSuelo)
        {
            alturaInicioSalto =
                jugador.position.y;
        }

        estabaEnSuelo =
            salto.enSuelo;
    }

    void ActualizarAnticipacion()
    {
        float anticipacionObjetivo =
            0f;

        if (!salto.enSuelo &&
            rbJugador.velocity.y > 0f)
        {
            float alturaSubida =
                jugador.position.y -
                alturaInicioSalto;

            if (alturaSubida >=
                distanciaActivarAnticipacion)
            {
                anticipacionObjetivo =
                    distanciaAnticipacion;
            }
        }

        anticipacionActual =
            Mathf.Lerp(
                anticipacionActual,
                anticipacionObjetivo,
                suavizadoAnticipacion *
                Time.deltaTime
            );
    }

    void AcomodarEmpuje()
    {
        if (!acomodandoEmpuje)
            return;

        rotacionY =
            Mathf.LerpAngle(
                rotacionY,
                rotacionYEmpuje,
                suavizadoAcomodarEmpuje *
                Time.deltaTime
            );

        if (Mathf.Abs(
            Mathf.DeltaAngle(
                rotacionY,
                rotacionYEmpuje
            )) > 0.5f)
            return;

        rotacionY =
            rotacionYEmpuje;

        acomodandoEmpuje =
            false;
    }

    void MoverCamara()
    {
        Quaternion rotacion =
            Quaternion.Euler(
                rotacionX,
                rotacionY,
                0f
            );

        Vector3 posicion =
            jugador.position +
            rotacion *
            distanciaInicial;

        posicion.y +=
            anticipacionActual;

        transform.position =
            posicion;

        transform.rotation =
            rotacion;
    }

    public void AcomodarParaEmpuje(
        Vector3 direccionJugador)
    {
        direccionJugador.y = 0f;

        if (direccionJugador.sqrMagnitude <
            0.01f)
            return;

        rotacionYEmpuje =
            Quaternion.LookRotation(
                direccionJugador
            ).eulerAngles.y;

        acomodandoEmpuje =
            true;
    }
}
