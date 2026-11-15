using UnityEngine;

public class Ventana : MonoBehaviour
{
    public static Ventana instancia;

    // que tan lejos del hueco se puede soltar la araña para que se vaya
    public float radio = 1f;
    public Vector3 hueco = new Vector3(2f, 1.6f, 0.6f);

    void Awake()
    {
        instancia = this;
    }

    // mido contra el hueco entero y no contra el centro, si no habia que
    // levantar la araña hasta la mitad del vidrio para que contara
    public float Distancia(Vector3 p)
    {
        var caja = new Bounds(transform.position, hueco);
        return Mathf.Sqrt(caja.SqrDistance(p));
    }

    public bool EstaCerca(Vector3 p)
    {
        return Distancia(p) < radio;
    }

    // si la tiraste apuntando a la ventana cuenta aunque la sueltes lejos
    public bool Tirada(Vector3 desde, Vector3 vel)
    {
        if (vel.magnitude < 1.5f) return false;
        var caja = new Bounds(transform.position, hueco);
        caja.Expand(radio);
        // sin gravedad, con que vaya para ese lado alcanza
        float d;
        return caja.IntersectRay(new Ray(desde, vel), out d) && d < vel.magnitude * 1.5f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, hueco);
    }
}
