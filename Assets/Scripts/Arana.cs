using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Arana : MonoBehaviour
{
    public enum Estado { Rondar, Acercarse, Retirarse }

    public Estado estado = Estado.Rondar;
    public float velocidad = 0.6f;
    public float radioRonda = 2f;
    public float tiempoRonda = 6f;
    public float distPicar = 0.45f;
    public float altura = 0.08f;

    // el cuarto mide 5x5, esto es para que no se metan en la pared
    [SerializeField] float limiteCuarto = 2.3f;

    Transform jugador;
    XRGrabInteractable grab;
    float angulo;
    float timer;
    float veneno;
    bool muerta;
    bool atrapada;

    void Start()
    {
        jugador = Camera.main.transform;
        grab = GetComponent<XRGrabInteractable>();
        angulo = Random.Range(0f, 360f);
        timer = tiempoRonda;
    }

    public bool EstaAgarrada()
    {
        return grab.isSelected;
    }

    void Update()
    {
        if (muerta || atrapada) return;
        if (grab.isSelected) return;

        // si estas bailando se asustan y se van
        if (Juego.instancia.bailando && estado != Estado.Retirarse)
        {
            estado = Estado.Retirarse;
            timer = 1.5f;
        }

        Vector3 pj = jugador.position;
        pj.y = 0f;
        Vector3 pos = transform.position;
        pos.y = 0f;
        float dist = Vector3.Distance(pos, pj);

        Vector3 destino = pos;
        switch (estado)
        {
            case Estado.Rondar:
                angulo += 30f * Time.deltaTime;
                destino = pj + new Vector3(Mathf.Cos(angulo * Mathf.Deg2Rad), 0f, Mathf.Sin(angulo * Mathf.Deg2Rad)) * radioRonda;
                timer -= Time.deltaTime;
                if (timer <= 0f)
                    estado = Estado.Acercarse;
                break;

            case Estado.Acercarse:
                destino = pj;
                if (dist < distPicar)
                {
                    Juego.instancia.Picadura();
                    estado = Estado.Retirarse;
                    timer = 2f;
                }
                break;

            case Estado.Retirarse:
                destino = pos + (pos - pj).normalized * 2f;
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    estado = Estado.Rondar;
                    timer = tiempoRonda;
                }
                break;
        }

        Vector3 nueva = Vector3.MoveTowards(pos, destino, velocidad * Time.deltaTime);
        nueva.x = Mathf.Clamp(nueva.x, -limiteCuarto, limiteCuarto);
        nueva.z = Mathf.Clamp(nueva.z, -limiteCuarto, limiteCuarto);

        Vector3 dir = nueva - pos;
        if (dir.sqrMagnitude > 0.00001f)
            transform.rotation = Quaternion.LookRotation(dir);

        nueva.y = altura;
        transform.position = nueva;
    }

    void OnCollisionEnter(Collision c)
    {
        var ch = c.collider.GetComponentInParent<Chancla>();
        if (ch == null) return;
        // si la apoyas despacio no cuenta, hay que pegarle
        if (c.relativeVelocity.magnitude < ch.fuerzaMinima) return;
        Morir();
    }

    public void Rociar(float cantidad)
    {
        veneno += cantidad;
        if (veneno >= 1f)
            Morir();
    }

    public void Atrapar(Transform frasco)
    {
        atrapada = true;
        grab.enabled = false;
        GetComponent<Collider>().enabled = false;
        transform.SetParent(frasco);
        transform.localPosition = new Vector3(0f, -0.05f, 0f);
        transform.localRotation = Quaternion.identity;
    }

    public void Morir()
    {
        if (muerta) return;
        muerta = true;
        Juego.instancia.AranaResuelta(true);
        Destroy(gameObject);
    }

    public void Liberar()
    {
        if (muerta) return;
        muerta = true;
        Juego.instancia.AranaResuelta(false);
        Destroy(gameObject);
    }
}
