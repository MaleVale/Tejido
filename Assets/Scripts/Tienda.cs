using UnityEngine;
using UnityEngine.UI;

public class Tienda : MonoBehaviour
{
    public GameObject panelCompras;
    public Text textoTitulo;
    public Text textoPuntos;
    public Text textoBotonEmpezar;
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
    }

    public void Cerrar()
    {
        panelCompras.SetActive(false);
    }

    public void Empezar()
    {
        Juego.instancia.SiguienteNivel();
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
