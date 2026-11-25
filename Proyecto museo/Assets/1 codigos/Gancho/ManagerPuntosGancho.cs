using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerPuntosGancho : MonoBehaviour
{
    public Transform camara;
    public Transform origenDeteccion;

    [Header("Puntos")]
    public PuntoGancho[] puntos;

    [Header("Deteccion")]
    public float alcance = 15f;
    public float anguloMaximo = 45f;

    PuntoGancho puntoSeleccionado;
    bool bloqueado;

    void Update()
    {
        if (bloqueado)
            return;

        BuscarMejorPunto();
    }

    void BuscarMejorPunto()
    {
        if (origenDeteccion == null || camara == null)
            return;

        PuntoGancho mejorPunto = null;
        float mejorValor = -1f;

        float limiteAngulo =
            Mathf.Cos(
                anguloMaximo * Mathf.Deg2Rad
            );

        for (int i = 0; i < puntos.Length; i++)
        {
            if (puntos[i] == null)
                continue;

            Vector3 direccion =
                puntos[i].transform.position -
                origenDeteccion.position;

            float distancia =
                direccion.magnitude;

            if (distancia > alcance)
                continue;

            direccion.Normalize();

            float direccionCamara =
                Vector3.Dot(
                    camara.forward,
                    direccion
                );

            if (direccionCamara < limiteAngulo)
                continue;

            if (direccionCamara > mejorValor)
            {
                mejorValor = direccionCamara;
                mejorPunto = puntos[i];
            }
        }

        CambiarPunto(mejorPunto);
    }

    void CambiarPunto(PuntoGancho nuevo)
    {
        if (nuevo == puntoSeleccionado)
            return;

        if (puntoSeleccionado != null)
            puntoSeleccionado.Desactivar();

        puntoSeleccionado = nuevo;

        if (puntoSeleccionado != null)
            puntoSeleccionado.Activar();
    }

    public PuntoGancho ObtenerPunto()
    {
        return puntoSeleccionado;
    }

    public void Bloquear()
    {
        bloqueado = true;

        if (puntoSeleccionado != null)
            puntoSeleccionado.Desactivar();
    }

    public void Desbloquear()
    {
        bloqueado = false;
        puntoSeleccionado = null;
    }

    void OnDrawGizmosSelected()
    {
        if (origenDeteccion == null || camara == null)
            return;

        Vector3 origen =
            origenDeteccion.position;

        Gizmos.DrawWireSphere(
            origen,
            alcance
        );

        int lineas = 20;

        for (int i = 0; i < lineas; i++)
        {
            float angulo =
                (360f / lineas) * i;

            Quaternion rotacionBorde =
                Quaternion.AngleAxis(
                    anguloMaximo,
                    Vector3.up
                );

            Vector3 borde =
                rotacionBorde *
                Vector3.forward;

            Quaternion alrededor =
                Quaternion.AngleAxis(
                    angulo,
                    Vector3.forward
                );

            borde =
                alrededor * borde;

            borde =
                camara.rotation * borde;

            Gizmos.DrawLine(
                origen,
                origen + borde * alcance
            );
        }

        Gizmos.DrawLine(
            origen,
            origen +
            camara.forward * alcance
        );
    }
}
