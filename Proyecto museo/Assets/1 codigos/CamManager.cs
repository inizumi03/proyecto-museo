using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    public Camera camaraA;
    public Camera camaraB;
    public GameObject canvasB;
    public KeyCode boton = KeyCode.C;
    public Jugador jugador;

    private bool usandoCamaraB;

    void Start()
    {
        camaraA.gameObject.SetActive(true);
        camaraB.gameObject.SetActive(false);
        canvasB.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(boton))
        {
            usandoCamaraB = !usandoCamaraB;

            camaraA.gameObject.SetActive(!usandoCamaraB);
            camaraB.gameObject.SetActive(usandoCamaraB);
            canvasB.SetActive(usandoCamaraB);

            if (usandoCamaraB)
                jugador.estado = Jugador.Estado.Quieto;
            else
                jugador.estado = Jugador.Estado.Normal;
        }
    }
}
