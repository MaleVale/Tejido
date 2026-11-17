using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Spray : MonoBehaviour
{
    public Transform pico;
    public LineRenderer chorro;
    public Transform barra;
    public float alcance = 3f;
    public float grosor = 0.15f;
    public float tiempoParaMatar = 1f;
    public float capacidad = 10f; // segundos de chorro que trae la lata

    XRGrabInteractable grab;
    bool apretado;
    float carga;
    float altoBarra;
    Renderer rendBarra;

    void Start()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.activated.AddListener(a => apretado = true);
        grab.deactivated.AddListener(a => apretado = false);
        chorro.enabled = false;
        carga = capacidad;
        altoBarra = barra.localScale.y;
        rendBarra = barra.GetComponent<Renderer>();
    }

    void Update()
    {
        bool rociando = apretado && grab.isSelected && carga > 0f;
        chorro.enabled = rociando;
        if (!rociando) return;

        carga -= Time.deltaTime;
        if (carga < 0f)
            carga = 0f;
        Barra();

        Vector3 fin = pico.position + pico.forward * alcance;
        RaycastHit hit;
        // spherecast para no tener que apuntar justo, las arañas son chicas
        if (Physics.SphereCast(pico.position, grosor, pico.forward, out hit, alcance, ~0, QueryTriggerInteraction.Ignore))
        {
            fin = pico.position + pico.forward * hit.distance;
            var a = hit.collider.GetComponentInParent<Araña>();
            if (a != null)
                a.Rociar(Time.deltaTime / tiempoParaMatar);
        }

        chorro.SetPosition(0, pico.position);
        chorro.SetPosition(1, fin);
    }

    void Barra()
    {
        float f = carga / capacidad;
        // la barra se achica desde arriba, el pie queda fijo
        var e = barra.localScale;
        e.y = altoBarra * f;
        barra.localScale = e;
        var p = barra.localPosition;
        p.y = -altoBarra / 2f + e.y / 2f;
        barra.localPosition = p;
        rendBarra.material.color = Color.Lerp(Color.red, Color.green, f);
    }
}
