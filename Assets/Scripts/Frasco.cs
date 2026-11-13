using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Frasco : MonoBehaviour
{
    public Arana atrapada;

    XRGrabInteractable grab;

    void Start()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (atrapada != null) return;
        if (!grab.isSelected) return; // apoyado en el piso no atrapa nada

        var a = other.GetComponentInParent<Arana>();
        if (a == null || a.EstaAgarrada()) return;

        atrapada = a;
        a.Atrapar(transform);
    }
}
