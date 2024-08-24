using UnityEngine;

public class Spectrum : MonoBehaviour
{
    //public AudioSource song;
    //public GameObject cube;
    //public Material _CubeMaterial;
    //float[] historyBuffer = new float[43];

    public int _band;
    public ObsGenerator _ObsGenerator;
    public float _scaleMultiplier, _startScale;
    private float _lastBand;
    [SerializeField][Range(0.5f, 2)] private float _BandLeverage = 1f;
    private MeshRenderer _ColorRender;
    private Material _StartingMat;
    public Material _BlinkingMat;

    public int _checkedBands = 3;
    private float[] _lastBands;

    public string Tolerence;

    private float timer;
    private float delay = 0.3f;


    // Use this for initialization
    void Start()
    {
        _lastBands = new float[_checkedBands];
        _ColorRender = GetComponent<MeshRenderer>();
        _StartingMat = _ColorRender.material;
    }

    // Update is called once per frame
    //void Update()
    //{

    //    //compute instant sound energy
    //    float[] channelRight = song.GetSpectrumData(1024, 0, FFTWindow.Hamming);
    //    float[] channelLeft = song.GetSpectrumData(1024, 0, FFTWindow.Hamming);

    //    float e = sumStereo(channelLeft, channelRight);

    //    //compute local average sound evergy
    //    float E = sumLocalEnergy() / historyBuffer.Length; // E being the average local sound energy

    //    //calculate variance
    //    float sumV = 0;
    //    for (int i = 0; i < 43; i++)
    //        sumV += (historyBuffer[i] - E) * (historyBuffer[i] - E);

    //    float V = sumV / historyBuffer.Length;
    //    float constant = (float)((-0.0025714 * V) + 1.5142857);

    //    float[] shiftingHistoryBuffer = new float[historyBuffer.Length]; // make a new array and copy all the values to it

    //    for (int i = 0; i < (historyBuffer.Length - 1); i++)
    //    { // now we shift the array one slot to the right
    //        shiftingHistoryBuffer[i + 1] = historyBuffer[i]; // and fill the empty slot with the new instant sound energy
    //    }

    //    shiftingHistoryBuffer[0] = e;

    //    for (int i = 0; i < historyBuffer.Length; i++)
    //    {
    //        historyBuffer[i] = shiftingHistoryBuffer[i]; //then we return the values to the original array
    //    }

    //    //float constant = 1.5f;

    //    if (e > (constant * E))
    //    { // now we check if we have a beat
    //        _CubeMaterial.color = Color.red;
    //    }
    //    else
    //    {
    //        _CubeMaterial.color = Color.yellow;
    //    }

    //    Debug.Log("Avg local: " + E);
    //    Debug.Log("Instant: " + e);
    //    Debug.Log("History Buffer: " + historybuffer());

    //    Debug.Log("sum Variance: " + sumV);
    //    Debug.Log("Variance: " + V);

    //    Debug.Log("Constant: " + constant);
    //    Debug.Log("--------");
    //}

    //float sumStereo(float[] channel1, float[] channel2)
    //{
    //    float e = 0;
    //    for (int i = 0; i < channel1.Length; i++)
    //    {
    //        e += ((channel1[i] * channel1[i]) + (channel2[i] * channel2[i]));
    //    }

    //    return e;
    //}

    //float sumLocalEnergy()
    //{
    //    float E = 0;

    //    for (int i = 0; i < historyBuffer.Length; i++)
    //    {
    //        E += historyBuffer[i] * historyBuffer[i];
    //    }

    //    return E;
    //}

    //string historybuffer()
    //{
    //    string s = "";
    //    for (int i = 0; i < historyBuffer.Length; i++)
    //    {
    //        s += (historyBuffer[i] + ",");
    //    }
    //    return s;
    //}

    private void Update()
    {
        timer += Time.deltaTime;

        transform.localScale = new Vector3(transform.localScale.x, (_ObsGenerator._FrequencyBands[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);

        _lastBand = _ObsGenerator._FrequencyBands[_band];

        for (int i = 0; i < _lastBands.Length; i++)
        {
            if (i != _lastBands.Length - 1)
            {
                _lastBands[i] = _lastBands[i + 1];
            }
            else
            {
                _lastBands[i] = _lastBand;
            }
        }

        if (GetLeverage(_BandLeverage))
        {
            ColorBlink();

            if (_band != 0 && timer >= delay)
            {
                //this._ObsGenerator.CreateOrbs();
                timer = 0;
            }

            for (int i = 0; i < _lastBands.Length; i++)
            {
                _lastBands[i] = _lastBand;
            }
        }
    }

    bool GetLeverage(float Leverage)
    {
        for (int i = 0; i < _lastBands.Length; i++)
        {
            if (_ObsGenerator._FrequencyBands[_band] - _lastBands[i] >= Leverage)
            {
                Tolerence = (_ObsGenerator._FrequencyBands[_band] - _lastBands[i]).ToString();
                return true;
            }
        }

        return false;
    }

    void ColorBlink()
    {
        _ColorRender.material = _BlinkingMat;
        Invoke("StopBlink", 0.2f);
    }

    void StopBlink()
    {
        _ColorRender.material = _StartingMat;
    }
}