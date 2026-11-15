using System.Collections;
using UnityEngine;

public class Juego : MonoBehaviour
{
    public static Juego instancia;

    public GameObject[] prefabsArana; // una por nivel, cada vez mas fea
    public float[] escalaArana = { 1f, 1.4f, 2f };
    public Transform[] escondites;
    public Transform[] esconditesPasillo; // de noche salen por la puerta
    public Transform puerta;
    public float anguloPuerta = 90f;
    public Collider piso;
    public TextMesh cartel;
    public Tienda tienda;
    public Light sol;
    public Light velador;
    public Renderer ventana;

    public int nivel = 1;
    public int puntos = 0;
    public bool bailando;
    [HideInInspector] public Bounds cuarto;

    // mañana, tarde y noche
    public int[] totalPorNivel = { 6, 8, 10 };
    public int[] vivasMax = { 3, 4, 5 };
    public float[] luzSol = { 1.2f, 0.5f, 0.03f };
    public Color[] colorSol = { new Color(1f, 0.96f, 0.85f), new Color(1f, 0.7f, 0.4f), new Color(0.3f, 0.35f, 0.6f) };
    public Color[] colorVentana = { new Color(0.6f, 0.85f, 1f), new Color(1f, 0.6f, 0.3f), new Color(0.05f, 0.05f, 0.15f) };
    public float cadaCuanto = 2f;

    string[] momentos = { "mañana", "tarde", "noche" };

    int vivas;
    int aparecidas;
    int salvadas;
    int aplastadas;
    bool enNivel;
    float timer;

    void Awake()
    {
        instancia = this;
        // hasta aca llegan las arañas, un poco menos que el piso para que no se metan en la pared
        cuarto = piso.bounds;
        cuarto.Expand(new Vector3(-0.3f, 20f, -0.3f));
    }

    void Start()
    {
        EmpezarNivel();
    }

    void Update()
    {
        if (!enNivel) return;
        int i = nivel - 1;
        if (aparecidas >= totalPorNivel[i] || vivas >= vivasMax[i]) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = cadaCuanto;
        Aparecer();
    }

    void EmpezarNivel()
    {
        int i = nivel - 1;
        sol.intensity = luzSol[i];
        sol.color = colorSol[i];
        ventana.material.color = colorVentana[i];
        velador.enabled = nivel == 3;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = colorSol[i] * (nivel == 3 ? 0.15f : 0.5f);

        aparecidas = 0;
        vivas = 0;
        timer = 0f;
        if (nivel == 3 && puerta != null)
        {
            StartCoroutine(AbrirPuerta());
            timer = 2.5f; // que termine de abrirse antes de que salgan
        }
        enNivel = true;
        tienda.Cerrar();
        Mostrar();
    }

    IEnumerator AbrirPuerta()
    {
        Quaternion cerrada = puerta.rotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 2f;
            puerta.rotation = Quaternion.AngleAxis(Mathf.SmoothStep(0f, anguloPuerta, t), Vector3.up) * cerrada;
            yield return null;
        }
    }

    void Aparecer()
    {
        int i = nivel - 1;
        var lista = escondites;
        if (nivel == 3 && esconditesPasillo.Length > 0)
            lista = esconditesPasillo;
        var e = lista[Random.Range(0, lista.Length)];
        var go = Instantiate(prefabsArana[i], e.position, Quaternion.identity);
        go.transform.localScale *= escalaArana[i];
        var a = go.GetComponent<Arana>();
        a.altura *= escalaArana[i];
        // cada nivel se animan mas
        a.velocidad = 0.5f + 0.25f * nivel;
        a.radioRonda = Mathf.Max(0.8f, 2.5f - 0.5f * nivel);
        a.tiempoRonda = Mathf.Max(1.5f, 7f - 1.5f * nivel);
        aparecidas++;
        vivas++;
        Mostrar();
    }

    public void AranaResuelta(bool aplastada)
    {
        if (aplastada)
        {
            aplastadas++;
            puntos += 10;
        }
        else
        {
            // liberar vale mas
            salvadas++;
            puntos += 20;
        }

        vivas--;
        Mostrar();
        if (aparecidas >= totalPorNivel[nivel - 1] && vivas <= 0)
            Invoke("TerminarNivel", 1.5f);
    }

    void TerminarNivel()
    {
        enNivel = false;
        if (nivel >= 3)
            tienda.MostrarFinal(salvadas, aplastadas);
        else
            tienda.Abrir("Terminó la " + momentos[nivel - 1], "Empezar la " + momentos[nivel]);
    }

    public void SiguienteNivel()
    {
        nivel++;
        EmpezarNivel();
    }

    public void Picadura()
    {
        puntos -= 5;
        Mostrar();
    }

    public void Gastar(int cuanto)
    {
        puntos -= cuanto;
        Mostrar();
    }

    void Mostrar()
    {
        if (cartel == null) return;
        int faltan = totalPorNivel[nivel - 1] - aparecidas + vivas;
        cartel.text = momentos[nivel - 1] + "\nPuntos " + puntos + "\nFaltan " + faltan;
    }
}
