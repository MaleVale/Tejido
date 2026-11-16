using UnityEngine;
using UnityEngine.UI;

// se queda abajo de la vista y te sigue con un poco de retraso
public class Hud : MonoBehaviour
{
    public Text momento;
    public Text puntos;
    public Text faltan;
    public Text aviso;
    public float distancia = 1.1f;
    public float abajo = 0.33f;
    public float suavidad = 6f;
    public float escala = 1f; // por si en el visor queda chico o grande
    public float duracionAviso = 1.5f;
    public Color bueno = new Color(0.5f, 0.82f, 0.54f);
    public Color malo = new Color(0.93f, 0.42f, 0.36f);

    Transform cam;
    float avisoHasta;

    void Start()
    {
        cam = Camera.main.transform;
        transform.localScale *= escala;
        aviso.text = "";
        Ubicar(1f);
    }

    void LateUpdate()
    {
        Ubicar(Time.deltaTime * suavidad);
        if (Time.time > avisoHasta)
            aviso.text = "";
    }

    void Ubicar(float cuanto)
    {
        Vector3 objetivo = cam.position + cam.forward * distancia - cam.up * abajo;
        transform.position = Vector3.Lerp(transform.position, objetivo, cuanto);
        transform.rotation = Quaternion.LookRotation(transform.position - cam.position);
    }

    public void Mostrar(string m, int p, int f)
    {
        momento.text = char.ToUpper(m[0]) + m.Substring(1);
        puntos.text = p.ToString();
        faltan.text = f.ToString();
    }

    // lo que resta en rojo, lo que suma en verde
    public void Avisar(string texto)
    {
        aviso.text = texto;
        aviso.color = texto.StartsWith("-") ? malo : bueno;
        avisoHasta = Time.time + duracionAviso;
    }
}
