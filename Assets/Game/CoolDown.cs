using TMPro;
using UnityEngine;

public class CoolDown : MonoBehaviour
{
    private bool _LastGOActivation;
    [SerializeField] private TextMeshProUGUI _CoolDownTxt;
    private float _CoolDown = 3;
    [SerializeField] private ObsGenerator _Generator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (_Generator._ShowCoolDown)
        {
            _CoolDownTxt.color = new Color(_CoolDownTxt.color.r, _CoolDownTxt.color.g, _CoolDownTxt.color.b, 1);
            _CoolDownTxt.text = ((int)_CoolDown).ToString();
            _CoolDown -= Time.deltaTime;
        }
        else
        {
            _CoolDown = 4;
            _CoolDownTxt.color = new Color(_CoolDownTxt.color.r, _CoolDownTxt.color.g, _CoolDownTxt.color.b, 0);
        }
    }
}
