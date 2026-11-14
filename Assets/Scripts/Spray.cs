using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Spray : MonoBehaviour
{
    public Transform pico;
    public LineRenderer chorro;
    public float alcance = 3f;
    public float grosor = 0.15f;
    public float tiempoParaMatar = 1f;

    XRGrabInteractable grab;
    bool apretado;
    float avisar;

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
        // spherecast para no tener que apuntar justo, las arañas son chicas
        if (Physics.SphereCast(pico.position, grosor, pico.forward, out hit, alcance, ~0, QueryTriggerInteraction.Ignore))
        {
            fin = pico.position + pico.forward * hit.distance;
            var a = hit.collider.GetComponentInParent<Arana>();
            if (a != null)
                a.Rociar(Time.deltaTime / tiempoParaMatar);

            avisar -= Time.deltaTime;
            if (avisar <= 0f)
            {
                avisar = 0.5f;
                //Debug.Log("el spray toca " + hit.collider.name + " a " + hit.distance.ToString("0.00") + " m");
            }
        }

        chorro.SetPosition(0, pico.position);
        chorro.SetPosition(1, fin);
    }
}
