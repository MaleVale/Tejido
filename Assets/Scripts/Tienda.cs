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
    public GameObject prefabSpray;
    public Transform lugarSpray;
    public int precioSpray = 30;

    int latas;

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

    // las latas se gastan, asi que se puede comprar mas de una
    public void ComprarSpray()
    {
        if (Juego.instancia.puntos < precioSpray) return;
        Juego.instancia.Gastar(precioSpray);
        // cada lata un poquito corrida para que no se apilen
        Vector3 lugar = lugarSpray.position + new Vector3(0.15f * latas, 0f, 0f);
        Instantiate(prefabSpray, lugar, Quaternion.identity);
        latas++;
    }
}
