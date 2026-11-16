using UnityEngine;
using Unity.XR.CoreUtils;

// los colliders frenan el stick, pero si cruzas una pared con la cabeza
public class Limites : MonoBehaviour
{
    public float margen = 0.4f;

    Transform origen;
    Transform cabeza;
    Bounds cuarto;

    void Start()
    {
        origen = FindFirstObjectByType<XROrigin>().transform;
        cabeza = Camera.main.transform;
        cuarto = Juego.instancia.piso.bounds;
        cuarto.Expand(new Vector3(-margen * 2f, 100f, -margen * 2f));
    }

    void LateUpdate()
    {
        Vector3 p = cabeza.position;
        Vector3 c = p;
        c.x = Mathf.Clamp(p.x, cuarto.min.x, cuarto.max.x);
        c.z = Mathf.Clamp(p.z, cuarto.min.z, cuarto.max.z);
        if (c != p)
            origen.position += c - p;
    }
}
