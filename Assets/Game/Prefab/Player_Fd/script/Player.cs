using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private bool _Stop = false;

    //Is a constante
    private Vector3 _PlayerStaticPos = new Vector3(0, 11, -0.45f);
    //

    [SerializeField] private Image _LifeBarBg;
    [SerializeField] private Image _LifeBar;
    private float _Transparency = 0;

    private float _HP;
    [SerializeField] private float _HPmax = 5;
    private float _LastTimeHit = 0;
    private float _RestoreCooldown = 1.5f;


    public Transform m_Pist;
    private int _PisteNb;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Pist == null)
        {
            m_Pist = FindObjectOfType<ObsGenerator>().transform;
        }
        _PisteNb = m_Pist.childCount / 2;
        _HP = _HPmax;
    }

    // Update is called once per frame
    void Update()
    {
        _Stop = !m_Pist.GetComponent<ObsGenerator>()._isInGame;

        _LifeBar.fillAmount = _HP / _HPmax;

        if (_Stop) { return; }

        _LastTimeHit = Mathf.Clamp(_LastTimeHit += Time.deltaTime, 0, _LastTimeHit);

        if (_LastTimeHit >= _RestoreCooldown)
        {
            _HP = Mathf.Clamp(_HP += Time.deltaTime, 0, _HPmax);
        }


        if (_HP == _HPmax && _Transparency != 0)
        {
            _Transparency -= Time.deltaTime;
            _Transparency = Mathf.Clamp(_Transparency, 0, 1);
        }

        _LifeBar.color = new Vector4(_LifeBar.color.r, _LifeBar.color.g, _LifeBar.color.b, _Transparency);
        _LifeBarBg.color = new Vector4(_LifeBarBg.color.r, _LifeBarBg.color.g, _LifeBarBg.color.b, _Transparency);

        //Debug.Log(HP);

        Mouvement();
    }

    private void Mouvement()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _PisteNb -= 1;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _PisteNb += 1;
        }

        _PisteNb = Mathf.Clamp(_PisteNb, 0, m_Pist.childCount - 1);

        Position(_PisteNb);
    }
    private void Position(int pistNumber)
    {
        this.transform.parent = m_Pist.GetChild(pistNumber);
        this.transform.localPosition = _PlayerStaticPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_LastTimeHit < 0.1f) { return; }
        _LastTimeHit = 0;
        _HP--;
        _Transparency = 1;

        m_Pist.GetComponent<ObsGenerator>()._PlayerHit++;

        if (_HP <= 0)
        {
            m_Pist.GetComponent<ObsGenerator>().StopMusic();
            _Stop = true;
            //Game End
        }
    }
}
