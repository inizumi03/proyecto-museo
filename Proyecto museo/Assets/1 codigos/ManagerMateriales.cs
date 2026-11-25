using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerMateriales : MonoBehaviour
{
    public static ManagerMateriales instancia;

    public Material materialUtilizable;

    Dictionary<Renderer, Material> originales = new Dictionary<Renderer, Material>();

    void Awake()
    {
        instancia = this;
    }

    public void Activar(Renderer objeto)
    {
        if (!originales.ContainsKey(objeto))
            originales.Add(objeto, objeto.sharedMaterial);

        objeto.sharedMaterial = materialUtilizable;
    }

    public void Desactivar(Renderer objeto)
    {
        if (!originales.TryGetValue(objeto, out Material original))
            return;

        objeto.sharedMaterial = original;
    }
}
