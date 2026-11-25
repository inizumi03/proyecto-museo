using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class accionesJugador : MonoBehaviour
{
    public enum Accion
    {
        Gancho,
        SoltarGancho,
        Esconderse,
        Deslizarse,
        AgarrarseBorde,
        Agacharse,
        Empujar
    }

    [System.Serializable]
    public class AccionJugador
    {
        public Accion accion;
        public KeyCode tecla;
    }

    public AccionJugador[] acciones;

    public bool AccionPresionada(Accion accion)
    {
        for (int i = 0; i < acciones.Length; i++)
        {
            if (acciones[i].accion == accion)
                return Input.GetKeyDown(acciones[i].tecla);
        }

        return false;
    }
}
