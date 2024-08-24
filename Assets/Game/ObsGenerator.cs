//using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ObsGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public int _PlayerHit = 0;
    private float[] _Timers;
    private const float _CoolDown = 0.25f;

    private float _GlobalTimer;
    private const float _GlobalCoolDown = 0.05f;

    [SerializeField] private Transform _Obstacle;

    private AudioClip _AudioClip;
    public AudioSource _AudioSource;
    private string _AudioLength;
    private float[] _samples = new float[512];
    [HideInInspector] public float[] _FrequencyBands = new float[8];

    [SerializeField] private float _TreshHold = 0.9f;
    public int[] _BandToCheck = new int[3];
    private int[] _checkedBands;
    private float[][] _lastBands = new float[8][];

    [HideInInspector] public bool _isInGame = false;
    [HideInInspector] public bool _ShowCoolDown = false;

    //Canvas
    [SerializeField] private TextMeshProUGUI _DurationTxt;
    [Space]
    [SerializeField] private TextMeshProUGUI _Title;
    [SerializeField] private TextMeshProUGUI _PercentTxt;

    [SerializeField] private Canvas[] _GameScreens = new Canvas[4];

    public ProjectileBehaviour _ProjectileBehaviour;

    AudioClip myClip;

    //string path;
    bool windowOpen;

    void Start()
    {
        ActivateScreen("StartingScreen");

        // Bands Instanciation 
        for (int i = 0; i < _lastBands.Length; i++)
        {
            _lastBands[i] = new float[3];
        }

        _checkedBands = new int[_BandToCheck.Length];

        for (int i = 0; i < _checkedBands.Length; i++)
        {
            _checkedBands[i] = _BandToCheck[i];
        }

        _Timers = new float[_checkedBands.Length];
        //

        //Get AudioSource 
        if (_AudioSource == null)
        {
            _AudioSource = GetComponent<AudioSource>();
        }

        _AudioClip = _AudioSource.clip;

        _AudioLength = ToFormat(_AudioClip.length);
        //
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape") && _isInGame)
        {
            _isInGame = false;
            PauseMusic();
            ActivateScreen("PauseScreen");
        }

        if (!_isInGame) { return; }

        // Timers
        for (int i = 0; i < _Timers.Length; i++)
        {
            _Timers[i] += Time.deltaTime;
            _Timers[i] = Mathf.Clamp(_Timers[i], 0, 1);
        }
        _GlobalTimer += Time.deltaTime;
        _GlobalTimer = Mathf.Clamp(_GlobalTimer, 0, 1);
        //

        CheckingBands();

        for (int i = 0; i < _lastBands.Length; i++)
        {
            for (int j = 0; j < _lastBands[i].Length; j++)
            {
                if (j != _lastBands[i].Length - 1)
                {
                    _lastBands[i][j] = _lastBands[i][j + 1];
                }
                else
                {
                    _lastBands[i][j] = _FrequencyBands[i];
                }
            }
        }

        _DurationTxt.text = ToFormat(_AudioSource.time) + " / " + _AudioLength;

        if (this._AudioSource.time >= this._AudioClip.length)
        {
            StopMusic(true);
        }

        GetSpectrum();
        MakeFrequencieBands();
    }


    // Fonctions

    public void OpenWindow()
    {
        if (!windowOpen)
        {
            ActivateScreen("BlankScreen");
            FileSelector.GetFile(GotFile, ".mp3"); //generate a new FileSelector window

            //FileSelector.windowStyle
            windowOpen = true; //record that we have a window open
        }
    }

    void GotFile(FileSelector.Status status, string path)
    {
        Debug.Log("File Status : " + status + ", Path : " + path);
        //this.path = path;

        if (status == FileSelector.Status.Successful)
        {
            StartCoroutine(GetAudioClip(path));
            Invoke("StartGame", 2);
            this.windowOpen = false;
        }
        else
        {
            ActivateScreen("StartingScreen");
            this.windowOpen = false;
        }
    }

    public IEnumerator GetAudioClip(string url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                myClip = DownloadHandlerAudioClip.GetContent(www);
                _AudioSource.clip = myClip;
                _AudioClip = _AudioSource.clip;

                _AudioLength = ToFormat(_AudioClip.length);
            }
        }
    }

    public void StopMusic(bool win = false)
    {
        this._AudioSource.Pause();

        _PercentTxt.text = "You cleared " + (int)((_AudioSource.time / _AudioClip.length) * 100) + "% of the song \n \n Nb Hit : " + _PlayerHit;

        if (win)
        {
            _PercentTxt.text = "You cleared 100% of the song \n \n Nb Hit : " + _PlayerHit;
            _Title.text = "Congratulations";
        }

        ActivateScreen("GameOverScreen");

        _isInGame = false;
    }

    public void PauseMusic()
    {

        this._AudioSource.Pause();
    }

    public void PlayMusic()
    {
        _ShowCoolDown = false;
        _isInGame = true;
        this._AudioSource.Play();
    }

    public void DelayedPlay()
    {
        ActivateScreen("InGameScreen");
        _ShowCoolDown = true;
        Invoke("PlayMusic", 3);
    }

    #region Screen
    private void ActivateScreen(string ScreenName)
    {
        for (int i = 0; i < _GameScreens.Length; i++)
        {
            if (_GameScreens[i].name != ScreenName)
            {
                _GameScreens[i].gameObject.SetActive(false);
            }
            else
            {
                _GameScreens[i].gameObject.SetActive(true);
            }
        }
    }

    private string ToFormat(float LenghtInSec) // FROM lenght(sss) TO lenght(mm:ss) 
    {
        int min = (int)LenghtInSec / 60;
        int sec = (int)(LenghtInSec - min * 60);

        if (sec < 10)
        {
            return min.ToString() + ":0" + sec.ToString();
        }
        else
        {
            return min.ToString() + ":" + sec.ToString();
        }
    }
    #endregion

    #region UI

    public void StartGame()
    {

        ActivateScreen("InGameScreen");

        this._isInGame = true;
        _AudioSource.Play();
    }

    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion

    #region INSTANCIATION
    public void CreateOrbs(int NbOrbs)
    {
        NbOrbs = Mathf.Clamp(NbOrbs, 0, 3);
        int[] RandNumb = new int[NbOrbs];

        for (int i = 0; i < RandNumb.Length; i++)
        {
            RandNumb[i] = Random.Range(0, this.transform.childCount);

            Transform Obs;
            Obs = Instantiate(_Obstacle);

            for (int j = 0; j < this.transform.GetChild(RandNumb[i]).childCount; j++)
            {
                if (this.transform.GetChild(RandNumb[i]).GetChild(j).tag == "Respawn")
                {
                    Obs.position = this.transform.GetChild(RandNumb[i]).GetChild(j).transform.position;
                }
            }
            Obs.name = NbOrbs.ToString();
            Obs.parent = this.transform.GetChild(RandNumb[i]);
        }
    }

    #endregion

    #region MUSIC RELATED
    private void CheckingBands()
    {
        if (_GlobalTimer < _GlobalCoolDown)
        {
            return;
        }
        _GlobalTimer = 0;

        int Orbs = 0;

        for (int k = 0; k < _checkedBands.Length; k++)
        {
            int BandLenght = _lastBands[_checkedBands[k]].Length; // Checked band Lenght

            for (int i = 0; i < BandLenght; i++)
            {
                bool TimerOK = _Timers[k] >= _CoolDown;

                if (Orbs > 0)
                {
                    TimerOK = true;
                }

                if (Mathf.Abs(_FrequencyBands[_checkedBands[k]] - _lastBands[_checkedBands[k]][i]) >= _TreshHold && TimerOK)
                {
                    Orbs++;

                    for (int j = 0; j < BandLenght; j++)
                    {
                        _lastBands[_checkedBands[k]][j] = _lastBands[_checkedBands[k]][BandLenght - 1];
                    }

                    _Timers[k] = 0;

                    i = BandLenght;
                }
            }
        }

        CreateOrbs(Orbs);
    }

    private void GetSpectrum()
    {
        _AudioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    public void MakeFrequencieBands()
    {
        /* nb Hertz Max in sound = 22050 
         * nb sample = 512
         * so nb hertz in a sample = 22050/512 = 43
         * 
         * frequencie band = Sub Bass 0-60 /Bass -250 /Low Midrange -500 /Midrange-2000 /Upper Midrange-4000 /Presence-6000 /brillance-20000
         * Brillance is divided in 2 Bands as the nb of band has to by divisible by 4
         * 
         * repartition of the sample    SB:2| B:4| LM:8| M:16| UM:32| P:64| B1:128| B2:256 
         * total 510 so 2 samples are lost
        */

        int count = 0;
        for (int i = 0; i < _FrequencyBands.Length; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }
            average /= count;
            _FrequencyBands[i] = average * 10;
        }
    }
    #endregion
}
