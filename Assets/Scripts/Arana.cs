using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
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

    Transform jugador;
    XRGrabInteractable grab;
    Animator anim;
    float angulo;
    float timer;
    float veneno;
    bool muerta;
    bool entro;
    bool volando;
    Vector3 velMano;
    Vector3 posAnterior;

    void Start()
    {
        jugador = Camera.main.transform;
        grab = GetComponent<XRGrabInteractable>();
        anim = GetComponentInChildren<Animator>();
        angulo = Random.Range(0f, 360f);
        timer = tiempoRonda;
        grab.selectEntered.AddListener(Agarrada);
        grab.selectExited.AddListener(Soltada);
    }

    void Agarrada(SelectEnterEventArgs e)
    {
        posAnterior = transform.position;
        velMano = Vector3.zero;
    }

    // si la soltas cerca de la ventana o la tiras para ese lado, chau
    void Soltada(SelectExitEventArgs e)
    {
        var v = Ventana.instancia;
        //Debug.Log("solte la araña a " + v.Distancia(transform.position).ToString("0.00") + " m de la ventana, vel " + velMano.magnitude.ToString("0.0"));
        if (v.EstaCerca(transform.position))
            Liberar();
        else if (v.Tirada(transform.position, velMano))
            StartCoroutine(Volar());
    }

    // sale volando hasta la ventana
    IEnumerator Volar()
    {
        volando = true;
        Vector3 desde = transform.position;
        Vector3 hasta = Ventana.instancia.transform.position;
        for (float t = 0f; t < 1f; t += Time.deltaTime * 3f)
        {
            transform.position = Vector3.Lerp(desde, hasta, t);
            yield return null;
        }
        Liberar();
    }

    public bool EstaAgarrada()
    {
        return grab.isSelected;
    }

    void Update()
    {
        if (muerta || volando) return;
        if (grab.isSelected)
        {
            if (anim != null) anim.speed = 0f;
            // me guardo a que velocidad la lleva la mano por si la tira
            velMano = Vector3.Lerp(velMano, (transform.position - posAnterior) / Time.deltaTime, 0.3f);
            posAnterior = transform.position;
            return;
        }

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

        // las que vienen del pasillo primero tienen que entrar, despues ya no salen del cuarto
        var cuarto = Juego.instancia.cuarto;
        if (!entro && cuarto.Contains(nueva))
            entro = true;
        if (entro)
        {
            nueva.x = Mathf.Clamp(nueva.x, cuarto.min.x, cuarto.max.x);
            nueva.z = Mathf.Clamp(nueva.z, cuarto.min.z, cuarto.max.z);
        }

        Vector3 dir = nueva - pos;
        if (dir.sqrMagnitude > 0.00001f)
            transform.rotation = Quaternion.LookRotation(dir);
        if (anim != null)
            anim.speed = dir.sqrMagnitude > 0.00001f ? 1f : 0f;

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
        // se va poniendo verde para que se note que le estas pegando
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.material.color = Color.Lerp(r.material.color, Color.green, cantidad * 2f);
        if (veneno >= 1f)
            Morir();
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
