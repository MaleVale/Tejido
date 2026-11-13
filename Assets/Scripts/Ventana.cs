using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Ventana : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        var a = other.GetComponentInParent<Arana>();
        if (a == null) return;

        var grab = a.GetComponent<XRGrabInteractable>();
        if (grab != null && grab.isSelected) return; // todavia la tenes en la mano

        a.Liberar();
    }
}
