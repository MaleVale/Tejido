using UnityEngine;

public class Ventana : MonoBehaviour
{
    public static Ventana instancia;

    // que tan cerca hay que soltar la araña para que se vaya
    public float radio = 1.2f;

    void Awake()
    {
        instancia = this;
    }

    public float Distancia(Vector3 p)
    {
        return Vector3.Distance(p, transform.position);
    }

    public bool EstaCerca(Vector3 p)
    {
        return Distancia(p) < radio;
    }
}
