using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverCabeza : MonoBehaviour
{
    [Header("Referencias")]
    public Transform cabeza;
    public Transform camara;

    [Header("Movimiento")]
    public float suavizado = 8f;
    public float zonaMuerta = 10f;

    [Header("Limites")]
    public float limiteHorizontal = 55f;
    public float limiteArriba = 30f;
    public float limiteAbajo = 25f;

    Jugador jugador;

    Quaternion offsetActual =
        Quaternion.identity;

    void Start()
    {
        jugador = GetComponent<Jugador>();
    }

    void LateUpdate()
    {
        if (cabeza == null ||
            camara == null ||
            jugador == null)
            return;

        Quaternion objetivo =
            Quaternion.identity;

        if (jugador.PuedeUsar(
            Jugador.FuncionJugador.GirarCabeza))
        {
            objetivo =
                CalcularRotacion();
        }

        offsetActual =
            Quaternion.Slerp(
                offsetActual,
                objetivo,
                suavizado * Time.deltaTime
            );

        cabeza.localRotation *=
            offsetActual;
    }

    Quaternion CalcularRotacion()
    {
        Vector3 haciaCamara =
            camara.position -
            transform.position;

        haciaCamara.y = 0;

        if (haciaCamara.sqrMagnitude < 0.01f)
            return Quaternion.identity;

        haciaCamara.Normalize();

        float posicionCamara =
            Vector3.Dot(
                transform.forward,
                haciaCamara
            );

        if (posicionCamara > 0f)
            return Quaternion.identity;

        Quaternion rotacionCamara =
            Quaternion.Inverse(
                transform.rotation
            ) *
            camara.rotation;

        float vertical =
            Mathf.DeltaAngle(
                0,
                rotacionCamara.eulerAngles.x
            );

        float horizontal =
            Mathf.DeltaAngle(
                0,
                rotacionCamara.eulerAngles.y
            );

        if (Mathf.Abs(vertical) < zonaMuerta)
            vertical = 0;

        if (Mathf.Abs(horizontal) < zonaMuerta)
            horizontal = 0;

        vertical =
            Mathf.Clamp(
                vertical,
                -limiteArriba,
                limiteAbajo
            );

        horizontal =
            Mathf.Clamp(
                horizontal,
                -limiteHorizontal,
                limiteHorizontal
            );

        return Quaternion.Euler(
            vertical,
            horizontal,
            0
        );
    }
}
