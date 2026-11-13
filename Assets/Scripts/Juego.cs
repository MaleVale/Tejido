using UnityEngine;

public class Juego : MonoBehaviour
{
    public static Juego instancia;

    public GameObject prefabArana;
    public Transform[] escondites;
    public TextMesh cartel;

    public int nivel = 1;
    public int puntos = 0;
    public int aranasPorNivel = 3;

    int vivas;
    int salvadas;
    int aplastadas;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        EmpezarNivel();
    }

    void EmpezarNivel()
    {
        vivas = 0;
        int cantidad = aranasPorNivel + nivel - 1;
        for (int i = 0; i < cantidad; i++)
        {
            var e = escondites[Random.Range(0, escondites.Length)];
            var go = Instantiate(prefabArana, e.position, Quaternion.identity);
            var a = go.GetComponent<Arana>();
            // cada nivel se animan mas
            a.velocidad = 0.5f + 0.25f * nivel;
            a.radioRonda = Mathf.Max(0.8f, 2.5f - 0.5f * nivel);
            a.tiempoRonda = Mathf.Max(1.5f, 7f - 1.5f * nivel);
            vivas++;
        }
        Mostrar();
    }

    public void AranaResuelta(bool aplastada)
    {
        if (aplastada)
            aplastadas++;
        else
            salvadas++;

        puntos += 10;
        vivas--;
        //Debug.Log("quedan " + vivas);
        if (vivas <= 0)
        {
            nivel++;
            Invoke("EmpezarNivel", 2f);
        }
        Mostrar();
    }

    public void Picadura()
    {
        puntos -= 5;
        Mostrar();
    }

    void Mostrar()
    {
        if (cartel == null) return;
        cartel.text = "Nivel " + nivel + "\nPuntos " + puntos + "\nQuedan " + vivas;
    }
}
