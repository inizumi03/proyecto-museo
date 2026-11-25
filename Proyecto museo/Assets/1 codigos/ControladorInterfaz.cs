using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControladorInterfaz : MonoBehaviour
{
    [System.Serializable]
    public class AccionInterfaz
    {
        public accionesJugador.Accion accion;
        public string textoAccion;
    }

    [Header("HUD")]
    public GameObject hudAccion;
    public TMP_Text textoAccion;

    [Header("Referencias")]
    public accionesJugador acciones;
    public Jugador jugador;
    public Gancho gancho;
    public AgarrarseBorde agarrarseBorde;
    public Empujar empujar;

    [Header("Acciones")]
    public AccionInterfaz[] accionesHUD;

    void Start()
    {
        if (hudAccion != null)
            hudAccion.SetActive(false);
    }

    void Update()
    {
        ActualizarHUD();
    }

    void ActualizarHUD()
    {
        for (int i = 0; i < accionesHUD.Length; i++)
        {
            if (!AccionDisponible(
                accionesHUD[i].accion))
                continue;

            MostrarAccion(
                accionesHUD[i]
            );

            return;
        }

        OcultarHUD();
    }

    void MostrarAccion(
        AccionInterfaz elemento)
    {
        if (hudAccion != null &&
            !hudAccion.activeSelf)
        {
            hudAccion.SetActive(true);
        }

        if (textoAccion == null)
            return;

        string tecla =
            ObtenerTecla(
                elemento.accion
            );

        textoAccion.text =
            "[" + tecla + "] " +
            elemento.textoAccion;
    }

    void OcultarHUD()
    {
        if (hudAccion != null &&
            hudAccion.activeSelf)
        {
            hudAccion.SetActive(false);
        }
    }

    bool AccionDisponible(
        accionesJugador.Accion accion)
    {
        switch (accion)
        {
            case accionesJugador.Accion.Gancho:

                if (gancho == null ||
                    jugador == null ||
                    gancho.enganchado ||
                    gancho.managerPuntos == null)
                    return false;

                return
                    gancho.managerPuntos.ObtenerPunto() != null &&
                    jugador.PuedeUsar(
                        Jugador.FuncionJugador.Gancho
                    );

            case accionesJugador.Accion.AgarrarseBorde:

                if (agarrarseBorde == null)
                    return false;

                return
                    agarrarseBorde.PuedeAgarrarse();

            case accionesJugador.Accion.Empujar:

                if (empujar == null)
                    return false;

                return
                    empujar.PuedeEmpujar();

            default:
                return false;
        }
    }

    string ObtenerTecla(
        accionesJugador.Accion accion)
    {
        if (acciones == null)
            return "";

        for (int i = 0;
             i < acciones.acciones.Length;
             i++)
        {
            if (acciones.acciones[i].accion ==
                accion)
            {
                return acciones
                    .acciones[i]
                    .tecla
                    .ToString();
            }
        }

        return "";
    }
}
