using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Esconderse : MonoBehaviour
{
    public float radioDeteccion = 2f;
    public float distanciaPared = 0.5f;

    [Header("Visual")]
    public Transform modelo;

    Jugador jugador;
    accionesJugador acciones;

    BoxCollider paredActual;

    bool oculto;
    int caraActual;

    Vector3 escalaOriginal;

    void Start()
    {
        jugador = GetComponent<Jugador>();
        acciones = GetComponent<accionesJugador>();

        escalaOriginal = modelo.localScale;
    }

    void Update()
    {
        if (acciones.AccionPresionada(
            accionesJugador.Accion.Esconderse))
        {
            if (oculto)
                Salir();
            else
                BuscarPared();
        }

        if (!oculto)
            return;

        if (Input.GetKeyDown(KeyCode.D))
            CambiarCara(false);

        if (Input.GetKeyDown(KeyCode.A))
            CambiarCara(true);
    }

    void BuscarPared()
    {
        Collider[] objetos = Physics.OverlapSphere(
            transform.position,
            radioDeteccion
        );

        BoxCollider mejorPared = null;
        float mejorDistancia = Mathf.Infinity;

        for (int i = 0; i < objetos.Length; i++)
        {
            if (!objetos[i].CompareTag("ParedEspecial"))
                continue;

            BoxCollider pared =
                objetos[i].GetComponent<BoxCollider>();

            if (pared == null)
                continue;

            Vector3 punto =
                pared.ClosestPoint(transform.position);

            float distancia =
                (transform.position - punto).sqrMagnitude;

            if (distancia < mejorDistancia)
            {
                mejorDistancia = distancia;
                mejorPared = pared;
            }
        }

        if (mejorPared == null)
            return;

        paredActual = mejorPared;
        Ocultarse();
    }

    void Ocultarse()
    {
        Vector3 posicionLocal =
            paredActual.transform.InverseTransformPoint(
                transform.position
            );

        float x =
            posicionLocal.x / paredActual.size.x;

        float z =
            posicionLocal.z / paredActual.size.z;

        if (Mathf.Abs(x) > Mathf.Abs(z))
            caraActual = x > 0 ? 1 : 3;
        else
            caraActual = z > 0 ? 0 : 2;

        ColocarEnCara();

        oculto = true;
        jugador.estado = Jugador.Estado.Oculto;
    }

    void CambiarCara(bool derecha)
    {
        if (derecha)
        {
            if (caraActual == 0)
                caraActual = 1;
            else if (caraActual == 1)
                caraActual = 2;
            else if (caraActual == 2)
                caraActual = 3;
            else
                caraActual = 0;
        }
        else
        {
            if (caraActual == 0)
                caraActual = 3;
            else if (caraActual == 3)
                caraActual = 2;
            else if (caraActual == 2)
                caraActual = 1;
            else
                caraActual = 0;
        }

        EspejarModelo(derecha);
        ColocarEnCara();
    }

    void EspejarModelo(bool derecha)
    {
        Vector3 escala = escalaOriginal;

        if (derecha)
            escala.x *= -1;

        modelo.localScale = escala;
    }

    void ColocarEnCara()
    {
        Vector3 direccionLocal;

        switch (caraActual)
        {
            case 0:
                direccionLocal = Vector3.forward;
                break;

            case 1:
                direccionLocal = Vector3.right;
                break;

            case 2:
                direccionLocal = Vector3.back;
                break;

            default:
                direccionLocal = Vector3.left;
                break;
        }

        Vector3 posicionLocal =
            paredActual.center;

        posicionLocal.x +=
            direccionLocal.x *
            (paredActual.size.x * 0.5f + distanciaPared);

        posicionLocal.z +=
            direccionLocal.z *
            (paredActual.size.z * 0.5f + distanciaPared);

        Vector3 posicionMundo =
            paredActual.transform.TransformPoint(
                posicionLocal
            );

        Vector3 direccionMundo =
            paredActual.transform.TransformDirection(
                direccionLocal
            );

        transform.position = new Vector3(
            posicionMundo.x,
            transform.position.y,
            posicionMundo.z
        );

        transform.rotation =
            Quaternion.LookRotation(-direccionMundo);
    }

    void Salir()
    {
        oculto = false;
        paredActual = null;

        modelo.localScale = escalaOriginal;

        jugador.estado = Jugador.Estado.Normal;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            radioDeteccion
        );
    }
}
