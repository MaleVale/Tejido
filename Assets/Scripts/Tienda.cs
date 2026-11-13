using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tienda : MonoBehaviour
{
    public GameObject panelCompras;
    public GameObject panelFinal;
    public Text textoTitulo;
    public Text textoPuntos;
    public Text textoBotonEmpezar;
    public Text textoFinal;
    public GameObject prefabFrasco;
    public GameObject prefabSpray;
    public Transform lugarFrasco;
    public Transform lugarSpray;
    public int precioFrasco = 20;
    public int precioSpray = 30;

    bool frascoComprado;
    bool sprayComprado;

    void Update()
    {
        if (panelCompras.activeSelf)
            textoPuntos.text = "Puntos: " + Juego.instancia.puntos;
    }

    public void Abrir(string titulo, string textoBoton)
    {
        textoTitulo.text = titulo;
        textoBotonEmpezar.text = textoBoton;
        panelCompras.SetActive(true);
        panelFinal.SetActive(false);
    }

    public void Cerrar()
    {
        panelCompras.SetActive(false);
        panelFinal.SetActive(false);
    }

    public void MostrarFinal(int salvadas, int aplastadas)
    {
        panelCompras.SetActive(false);
        panelFinal.SetActive(true);
        textoFinal.text = "Se terminó el día\nSalvaste " + salvadas + "\nAplastaste " + aplastadas;
    }

    public void Empezar()
    {
        Juego.instancia.SiguienteNivel();
    }

    public void DeNuevo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ComprarFrasco()
    {
        if (frascoComprado) return;
        if (Juego.instancia.puntos < precioFrasco) return;
        Juego.instancia.Gastar(precioFrasco);
        frascoComprado = true;
        Instantiate(prefabFrasco, lugarFrasco.position, Quaternion.identity);
    }

    public void ComprarSpray()
    {
        if (sprayComprado) return;
        if (Juego.instancia.puntos < precioSpray) return;
        Juego.instancia.Gastar(precioSpray);
        sprayComprado = true;
        Instantiate(prefabSpray, lugarSpray.position, Quaternion.identity);
    }
}
