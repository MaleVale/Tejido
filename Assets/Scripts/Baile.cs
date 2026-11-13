using UnityEngine;

public class Baile : MonoBehaviour
{
    public Transform cabeza;
    public Transform manoIzq;
    public Transform manoDer;
    public float minMovimiento = 0.5f;
    public float maxSeguido = 10f;

    Vector3 izqAnt;
    Vector3 derAnt;
    float seguido;
    float descanso;

    void Update()
    {
        bool arriba = manoIzq.position.y > cabeza.position.y && manoDer.position.y > cabeza.position.y;
        float mov = ((manoIzq.position - izqAnt).magnitude + (manoDer.position - derAnt).magnitude) / Time.deltaTime;
        izqAnt = manoIzq.position;
        derAnt = manoDer.position;

        bool bailando = arriba && mov > minMovimiento;
        if (bailando)
        {
            seguido += Time.deltaTime;
            descanso = 0f;
        }
        else
        {
            descanso += Time.deltaTime;
            if (descanso > 2f)
                seguido = 0f;
        }

        // cada nivel les dura menos la gracia
        float max = Mathf.Max(3f, maxSeguido - 2f * (Juego.instancia.nivel - 1));
        Juego.instancia.bailando = bailando && seguido < max;
    }
}
