using UnityEngine;

public class Inicio : MonoBehaviour
{
    public GameObject panelAdvertencia;
    public GameObject panelHistoria;
    public float distancia = 1.8f;
    public float suavidad = 4f;

    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        panelAdvertencia.SetActive(true);
        panelHistoria.SetActive(false);
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
}
