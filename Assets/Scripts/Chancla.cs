using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Chancla : MonoBehaviour
{
    // velocidad minima del golpe para que cuente cuando la tiras o choca de verdad
    public float fuerzaMinima = 1.5f;
    // chancletazo desde arriba: llega hasta el piso para no tener que agacharse
    public float alcance = 1f;
    public float radio = 0.5f;
    // con que baje alcanza, en el simulador la mano se mueve a 0,2 m/s
    public float bajadaMinima = 0.15f;

    XRGrabInteractable grab;
    float yAnterior;
    float espera;

    void Start()
    {
        grab = GetComponent<XRGrabInteractable>();
        yAnterior = transform.position.y;
    }

    void Update()
    {
        float bajada = (yAnterior - transform.position.y) / Time.deltaTime;
        yAnterior = transform.position.y;
        espera -= Time.deltaTime;
        if (!grab.isSelected || espera > 0f || bajada < bajadaMinima) return;

        Vector3 abajo = transform.position + Vector3.down * alcance;
        foreach (var c in Physics.OverlapCapsule(transform.position, abajo, radio))
        {
            var a = c.GetComponentInParent<Arana>();
            if (a == null) continue;
            a.Morir();
            espera = 0.3f;
            break;
        }
    }
}
