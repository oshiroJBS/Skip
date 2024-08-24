using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float m_speed = 1.5f;
    public float m_lifeSpan;
    private float Timer = 0;
    private float Transparency = 0;
    private Color _color;
    private Material _material;
    private ObsGenerator _generator;

    private void Start()
    {
        Transparency = 0;

        _generator = FindObjectOfType<ObsGenerator>();

        _material = this.transform.GetComponent<MeshRenderer>().material;
        this.transform.GetComponent<MeshRenderer>().material = _material;
        _color = _material.color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_generator._isInGame)
        {
            return;
        }

        Timer += Time.deltaTime;

        if (Transparency < 1)
        {
            Transparency += Time.deltaTime*5;
            this.transform.GetComponent<MeshRenderer>().material.color = new Vector4(_color.r, _color.g, _color.b, Transparency); ;
        }

        if (Timer >= m_lifeSpan)
        {
            Die();
        }

        this.transform.Translate(Vector3.back * m_speed);
    }


    private void OnCollisionEnter(Collision collision)
    {
        Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
