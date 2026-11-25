using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventosAnimacion : MonoBehaviour
{
    public Gancho gancho;
    public AnimacionesJugador animaciones;

    public void SubirGancho()
    {
        gancho.Subir();
    }

    
}

