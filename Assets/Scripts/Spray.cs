using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Spray : MonoBehaviour
{
    public Transform pico;
    public LineRenderer chorro;
    public float alcance = 2f;
    public float tiempoParaMatar = 1.5f;

    XRGrabInteractable grab;
    bool apretado;

    void Start()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.activated.AddListener(a => apretado = true);
        grab.deactivated.AddListener(a => apretado = false);
        chorro.enabled = false;
    }

    void Update()
    {
        bool rociando = apretado && grab.isSelected;
        chorro.enabled = rociando;
        if (!rociando) return;

        Vector3 fin = pico.position + pico.forward * alcance;
        RaycastHit hit;
        if (Physics.Raycast(pico.position, pico.forward, out hit, alcance))
        {
            fin = hit.point;
            var a = hit.collider.GetComponentInParent<Arana>();
            if (a != null)
                a.Rociar(Time.deltaTime / tiempoParaMatar);
        }

        chorro.SetPosition(0, pico.position);
        chorro.SetPosition(1, fin);
    }
}
