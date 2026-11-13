using UnityEngine;

public class Juego : MonoBehaviour
{
    public static Juego instancia;

    public GameObject prefabArana;
    public Transform[] escondites;
    public TextMesh cartel;
    public Tienda tienda;
    public Light sol;
    public Light velador;
    public Renderer ventana;

    public int nivel = 1;
    public int puntos = 0;
    public bool bailando;

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
        enNivel = true;
        tienda.Cerrar();
        Mostrar();
    }

    void Aparecer()
    {
        var e = escondites[Random.Range(0, escondites.Length)];
        var go = Instantiate(prefabArana, e.position, Quaternion.identity);
        var a = go.GetComponent<Arana>();
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
