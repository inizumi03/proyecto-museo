using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoEmpujable : MonoBehaviour
{
    public float velocidad = 2f;

    [Header("Colision")]
    public float margenColision = 0.05f;

    Rigidbody rb;
    BoxCollider caja;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        caja = GetComponent<BoxCollider>();

        rb.isKinematic = true;
        rb.useGravity = false;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousSpeculative;
    }

    public void ObtenerDatosEmpuje(
        Vector3 posicionJugador,
        out Vector3 puntoEmpuje,
        out Vector3 direccionEmpuje)
    {
        Vector3 posicionLocal =
            transform.InverseTransformPoint(
                posicionJugador
            );

        Vector3 centro =
            caja.center;

        Vector3 mitad =
            caja.size * 0.5f;

        Vector3 diferencia =
            posicionLocal -
            centro;

        float ladoX =
            Mathf.Abs(diferencia.x) /
            mitad.x;

        float ladoZ =
            Mathf.Abs(diferencia.z) /
            mitad.z;

        Vector3 puntoLocal =
            centro;

        Vector3 direccionLocal;

        if (ladoX > ladoZ)
        {
            if (diferencia.x > 0f)
            {
                puntoLocal.x += mitad.x;
                direccionLocal = Vector3.left;
            }
            else
            {
                puntoLocal.x -= mitad.x;
                direccionLocal = Vector3.right;
            }
        }
        else
        {
            if (diferencia.z > 0f)
            {
                puntoLocal.z += mitad.z;
                direccionLocal = Vector3.back;
            }
            else
            {
                puntoLocal.z -= mitad.z;
                direccionLocal = Vector3.forward;
            }
        }

        puntoEmpuje =
            transform.TransformPoint(
                puntoLocal
            );

        direccionEmpuje =
            transform.TransformDirection(
                direccionLocal
            );

        direccionEmpuje.y = 0f;
        direccionEmpuje.Normalize();
    }

    public Vector3 Mover(
        Vector3 direccion)
    {
        direccion.y = 0f;
        direccion.Normalize();

        float distancia =
            velocidad *
            Time.fixedDeltaTime;

        RaycastHit hit;

        if (rb.SweepTest(
            direccion,
            out hit,
            distancia + margenColision))
        {
            return rb.position;
        }

        Vector3 nuevaPosicion =
            rb.position +
            direccion *
            distancia;

        rb.MovePosition(
            nuevaPosicion
        );

        return nuevaPosicion;
    }

    public Vector3 ObtenerPosicion()
    {
        return rb.position;
    }
}
