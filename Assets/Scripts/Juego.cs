using System.Collections;
using UnityEngine;

public class Juego : MonoBehaviour
{
    public static Juego instancia;

    public GameObject arañaNivel1;
    public GameObject[] arañasNivel2; // sale cualquiera de estas
    public GameObject arañaBoss;
    public float[] escalaAraña = { 1f, 1.4f, 4f }; // la jefa es enorme
    public Transform[] escondites;
    public Transform[] esconditesPasillo; // de noche salen por la puerta
    public Transform puerta;
    public float anguloPuerta = 90f;
    public Collider piso;
    public Hud hud;
    public Tienda tienda;
    public Light sol;
    public Light velador;
    public Renderer cielo; // lo que se ve por la ventana
    public Material[] cielos; // mañana, tarde y noche
    public AudioSource ambiente;
    public AudioClip[] sonidosDia;
    public AudioClip sonidoPuerta;
    public AudioClip sonidoVictoria;
    public Inicio inicio;

    public int nivel = 1;
    public int puntos = 0;
    public bool bailando;
    [HideInInspector] public Bounds cuarto;

    // mañana, tarde y noche
    public int[] totalPorNivel = { 6, 8, 10 };
    public int[] vivasMax = { 3, 4, 5 };
    public float[] luzSol = { 1.2f, 0.5f, 0.03f };
    public Color[] colorSol = { new Color(1f, 0.96f, 0.85f), new Color(1f, 0.7f, 0.4f), new Color(0.3f, 0.35f, 0.6f) };
    public float cadaCuanto = 2f;

    string[] momentos = { "mañana", "tarde", "noche" };

    int vivas;
    int aparecidas;
    int salvadas;
    int aplastadas;
    int[] puntosNivel = new int[3];
    int puntosAlEmpezar;
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
        // la luz de la mañana ya, las arañas recien cuando pasas las pantallas de inicio
        Luces();
        tienda.Cerrar();
        Mostrar();
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

    public void Empezar()
    {
        if (hud != null)
            hud.gameObject.SetActive(true);
        EmpezarNivel();
    }

    void Luces()
    {
        int i = nivel - 1;
        sol.intensity = luzSol[i];
        sol.color = colorSol[i];
        cielo.sharedMaterial = cielos[i];
        velador.enabled = nivel == 3;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = colorSol[i] * (nivel == 3 ? 0.15f : 0.5f);
        if (ambiente.clip != sonidosDia[i])
        {
            ambiente.clip = sonidosDia[i];
            ambiente.Play();
        }
    }

    void EmpezarNivel()
    {
        Luces();
        aparecidas = 0;
        vivas = 0;
        timer = 0f;
        string aviso = "Empieza la " + momentos[nivel - 1];
        if (nivel == 3 && puerta != null)
        {
            StartCoroutine(AbrirPuerta());
            timer = 2.5f; // que termine de abrirse antes de que salgan
            aviso = "Es de noche... se abre la puerta";
        }
        enNivel = true;
        tienda.Cerrar();
        Mostrar();
        Avisar(aviso);
    }

    IEnumerator AbrirPuerta()
    {
        AudioSource.PlayClipAtPoint(sonidoPuerta, puerta.position);
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
        GameObject prefab = arañaNivel1;
        if (nivel == 2)
            prefab = arañasNivel2[Random.Range(0, arañasNivel2.Length)];
        else if (nivel == 3)
            prefab = arañaBoss;
        var go = Instantiate(prefab, e.position, Quaternion.identity);
        go.transform.localScale *= escalaAraña[i];
        var a = go.GetComponent<Araña>();
        a.altura *= escalaAraña[i];
        // cada nivel se animan mas
        a.velocidad = 0.5f + 0.25f * nivel;
        a.radioRonda = Mathf.Max(0.8f, 2.5f - 0.5f * nivel);
        a.tiempoRonda = Mathf.Max(1.5f, 7f - 1.5f * nivel);
        aparecidas++;
        vivas++;
        Mostrar();
    }

    public void ArañaResuelta(bool aplastada)
    {
        if (aplastada)
        {
            aplastadas++;
            puntos += 10;
            Avisar("+10 aplastada");
        }
        else
        {
            // liberar vale mas
            salvadas++;
            puntos += 20;
            Avisar("+20 liberada");
        }

        vivas--;
        Mostrar();
        if (aparecidas >= totalPorNivel[nivel - 1] && vivas <= 0)
            Invoke("TerminarNivel", 1.5f);
    }

    void TerminarNivel()
    {
        enNivel = false;
        puntosNivel[nivel - 1] = puntos - puntosAlEmpezar;
        puntosAlEmpezar = puntos;
        if (nivel >= 3)
            Ganar();
        else
            tienda.Abrir("Terminó la " + momentos[nivel - 1], "Empezar la " + momentos[nivel]);
    }

    void Ganar()
    {
        int[] maximos = new int[3];
        for (int i = 0; i < 3; i++)
            maximos[i] = totalPorNivel[i] * 20;
        ambiente.Stop();
        ambiente.PlayOneShot(sonidoVictoria);
        if (hud != null)
            hud.gameObject.SetActive(false);
        inicio.MostrarVictoria(puntosNivel, maximos, salvadas, aplastadas);
    }

    public void SiguienteNivel()
    {
        nivel++;
        EmpezarNivel();
    }

    public void Picadura()
    {
        puntos -= 5;
        Avisar("-5 te picó");
        Mostrar();
    }

    public void Gastar(int cuanto)
    {
        puntos -= cuanto;
        Avisar("-" + cuanto + " spray");
        Mostrar();
    }

    void Avisar(string texto)
    {
        if (hud != null)
            hud.Avisar(texto);
    }

    void Mostrar()
    {
        int faltan = totalPorNivel[nivel - 1] - aparecidas + vivas;
        if (hud != null)
            hud.Mostrar(momentos[nivel - 1], puntos, faltan);
    }
}
