using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Inicio : MonoBehaviour
{
    public GameObject panelAdvertencia;
    public GameObject panelHistoria;
    public GameObject panelVictoria;
    public Text textoVictoria;
    public Text textoResumen;
    public float distancia = 1.8f;
    public float suavidad = 4f;

    Transform cam;
    string[] momentos = { "Mañana", "Tarde", "Noche" };

    void Start()
    {
        cam = Camera.main.transform;
        panelAdvertencia.SetActive(true);
        panelHistoria.SetActive(false);
        panelVictoria.SetActive(false);
        Ubicar(1f);
    }

    void LateUpdate()
    {
        Ubicar(Time.deltaTime * suavidad);
    }

    // sigue la cabeza solo al girar, la altura queda fija para poder leer tranquilo
    void Ubicar(float cuanto)
    {
        Vector3 f = cam.forward;
        f.y = 0f;
        if (f.sqrMagnitude < 0.001f) return;
        f.Normalize();
        Vector3 objetivo = cam.position + f * distancia;
        objetivo.y = cam.position.y - 0.15f;
        transform.position = Vector3.Lerp(transform.position, objetivo, cuanto);
        transform.rotation = Quaternion.LookRotation(f);
    }

    public void MeLaBanco()
    {
        panelAdvertencia.SetActive(false);
        panelHistoria.SetActive(true);
    }

    public void Salir()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void Continuar()
    {
        gameObject.SetActive(false);
        Juego.instancia.Empezar();
    }

    public void MostrarVictoria(int[] porNivel, int[] maximos, int salvadas, int aplastadas)
    {
        panelAdvertencia.SetActive(false);
        panelHistoria.SetActive(false);
        panelVictoria.SetActive(true);
        gameObject.SetActive(true);
        Ubicar(1f);

        int total = 0;
        int totalMax = 0;
        string t = string.Format("{0,-8}{1,8}{2,8}\n", "", "Puntos", "Máximo");
        for (int i = 0; i < 3; i++)
        {
            t += string.Format("{0,-8}{1,8}{2,8}\n", momentos[i], porNivel[i], maximos[i]);
            total += porNivel[i];
            totalMax += maximos[i];
        }
        t += string.Format("{0,-8}{1,8}{2,8}", "Total", total, totalMax);
        textoVictoria.text = t;
        textoResumen.text = "Salvaste " + salvadas + " y aplastaste " + aplastadas;
    }

    public void DeNuevo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
