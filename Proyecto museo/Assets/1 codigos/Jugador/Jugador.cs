using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Jugador : MonoBehaviour

{
    public enum Estado
    {
        Normal,
        Quieto,
        Colgando,
        Derrota,
        Oculto,
        AgarradoBorde,
        Agachado,
        EnElAire,
        HaciendoFuerza
    }

    public enum FuncionJugador
    {
        GirarCabeza,
        Dash,
        Saltar,
        Gancho,
        Moverse,
        HacerFuerza
    }

    [System.Serializable]
    public class ConfiguracionEstado
    {
        public Estado estado;
        public FuncionJugador[] funcionesBloqueadas;
    }

    public Estado estado;

    [Header("Funciones por estado")]
    public ConfiguracionEstado[] configuraciones;

    Estado estadoAnterior;
    Mov mov;
    Vector3 posicionInicial;

    void Start()
    {
        mov = GetComponent<Mov>();
        posicionInicial = transform.position;

        estadoAnterior = estado;
        AplicarEstado();
    }

    void Update()
    {
        if (estado != estadoAnterior)
        {
            estadoAnterior = estado;
            AplicarEstado();
        }
    }

    void AplicarEstado()
    {
        mov.PuedeMoverse(
            PuedeUsar(FuncionJugador.Moverse)
        );

        if (estado == Estado.Derrota)
        {
            transform.position = posicionInicial;
            estado = Estado.Normal;
        }
    }

    public bool PuedeUsar(FuncionJugador funcion)
    {
        for (int i = 0; i < configuraciones.Length; i++)
        {
            if (configuraciones[i].estado == estado)
            {
                FuncionJugador[] bloqueadas =
                    configuraciones[i].funcionesBloqueadas;

                for (int j = 0; j < bloqueadas.Length; j++)
                {
                    if (bloqueadas[j] == funcion)
                        return false;
                }

                return true;
            }
        }

        return true;
    }
}

