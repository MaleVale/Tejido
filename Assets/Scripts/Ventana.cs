using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Ventana : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        // si venis con el frasco, sale la que esta adentro
        var f = other.GetComponentInParent<Frasco>();
        if (f != null && f.atrapada != null)
        {
            f.atrapada.Liberar();
            f.atrapada = null;
            return;
        }

        var a = other.GetComponentInParent<Arana>();
        if (a == null) return;

        var grab = a.GetComponent<XRGrabInteractable>();
        if (grab != null && grab.isSelected) return; // todavia la tenes en la mano

        a.Liberar();
    }
}
