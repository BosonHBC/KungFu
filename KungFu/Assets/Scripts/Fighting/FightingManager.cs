using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;

public class FightingManager : MonoBehaviour
{

    public enum FightMode
    {
        Wait, Offense, Defense
    }
    public static FightingManager instance;
    private void Awake()
    {
        if (instance == null || instance != this)
        {
            instance = this;
        }
    }
    [Header("Prefab")]
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject playerCenterPrefab;
    //[SerializeField] private Character[] playerPrefabs;
    [SerializeField] private Player[] playerPrefabs;
    [SerializeField] private Enemy[] enemyPrefabs;
    [Header("Parents")]
    [SerializeField] private Transform playerReleventParent;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TimelineAsset timelines;

    [SerializeField] GameObject[] cameraList;

    [Header("Parameter")]
    private SideViewCam sideVCam;
    public int iFightingSceneID;
    [HideInInspector] public Character[] characters = new Character[2];
    [HideInInspector] public Canvas myCanvas;
    public UnityAction onPositioned;
    private float fThresholdOfTime = 0.02f;
    private float fTimeToPlayFightPrepare = 5f;
    public FightMode fightMode = FightMode.Wait;
    bool bPrepared = false;
    private ModeHint hint;
    private int[] randomID = { 0, 1, 2 };
    public bool bFightOver;
    private bool bGameStart;

    private float fDmgToEnemyPerAttack;
    private int iAllowAttackCount = 30;
    // Mapping, should be written in json data
    class AnimationData
    {
        public AnimationData(int _Joint, float _delay, float _attackDirH, float _attackDirV)
        {
            iJointID = _Joint;
            fDelay = _delay;
            fa_AttackDir = new float[2];
            fa_AttackDir[0] = _attackDirH;
            fa_AttackDir[1] = _attackDirV;
        }
        private int iJointID;
        private float fDelay;
        private float[] fa_AttackDir;

        public int GetJoint() { return iJointID; }
        public float GetDelay() { return fDelay; }
        public float[] GetAttackDir() { return fa_AttackDir; }
    };
    List<AnimationData> animDatas = new List<AnimationData>();

    //Hard code song name data
    Dictionary<int, string> songNameData;
    // Start is called before the first frame update
    void Start()
    {
        randomizeArray(randomID, randomID.Length);
        animDatas.Add(new AnimationData(0, 0.5f, 0f, 1f));
        animDatas.Add(new AnimationData(1, 0.53f, -1f, 0f));
        animDatas.Add(new AnimationData(1, 0.3f, -1f, 0.65f));
        animDatas.Add(new AnimationData(1, 0.33f, -0.5f, 1f));
        animDatas.Add(new AnimationData(4, 0.68f, 0f, 1f));
        animDatas.Add(new AnimationData(2, 0.3f, 0.5f, 1f));
        animDatas.Add(new AnimationData(2, 0.3f, 0.67f, 1f));
        animDatas.Add(new AnimationData(2, 0.53f, 1f, 0f));
        animDatas.Add(new AnimationData(5, 0.75f, 0.3f, -1f));
        animDatas.Add(new AnimationData(6, 0.75f, -1f, -1f));
        animDatas.Add(new AnimationData(3, 0.66f, 0f, -1f));

        songNameData = new Dictionary<int, string>()
        {
            {0, "BattleGirl_H" },
            {1, "Sailor_H" }
        };

        fDmgToEnemyPerAttack = 200f / MyGameInstance.instance.GetComponent<DataLoader>().GetNumOfAttackByName(songNameData[MyGameInstance.instance.SongIndex]);

        CreateObjects();

        Invoke("StartGame", 2f);

    }


    // Update is called once per frame
    void Update()
    {
        Debug_SwitchCamera();

        //Debug.Log(director.time);
        if (bGameStart && !bPrepared && director.time >= fTimeToPlayFightPrepare - fThresholdOfTime && director.time <= fTimeToPlayFightPrepare + fThresholdOfTime)
        {
            bPrepared = true;
            Debug.Log("Start to Prepare!");
            fightMode = FightMode.Defense;
            if (onPositioned != null)
                onPositioned.Invoke();
            // Debug Game Over
            //StartCoroutine(ie_DelayGameOverTest(6f, 1));

        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            LevelLoader.instance.LoadScene("FightingScene_" + iFightingSceneID);
        }

    }

    public void StartGame()
    {
        if (!bGameStart)
        {
            bGameStart = true;
            GetComponent<BeatGenerator>().StartGenerateBeat();
            director.Play();
        }
    }

    void CreateObjects()
    {
        /// Canvas
        GameObject _canvasGo = Instantiate(playerUIPrefab);
        _canvasGo.GetComponent<Canvas>().worldCamera = Camera.main;
        // _canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        _canvasGo.name = "PlayerUI";
        myCanvas = _canvasGo.GetComponent<Canvas>();
        director.SetGenericBinding(timelines.GetOutputTrack(4), _canvasGo.GetComponent<Animator>());
        // Set UIs
        MyGameInstance.instance.SetScoreUI(
            _canvasGo.transform.Find("Combo").GetComponent<ComboController>()
            , _canvasGo.transform.Find("Score").GetChild(0).GetComponent<Text>());
        hint = _canvasGo.transform.GetChild(8).GetComponent<ModeHint>();
        /// Characters 0-> player 1-> enemy

        for (int i = 0; i < 2; i++)
        {
            Character _character;
            if (i == 0)
                _character = Instantiate(playerPrefabs[MyGameInstance.instance.PlayerCharacterIndex]);
            else
            {
                int rng = (MyGameInstance.instance.PlayerCharacterIndex + Random.Range(1, 3)) % enemyPrefabs.Length;
                _character = Instantiate(enemyPrefabs[rng]);
            }
            //Character _character = Instantiate(playerPrefabs[i]).GetComponent<Character>();
            characters[i] = _character;
            _character.transform.SetParent(playerReleventParent);
            _character.transform.position = spawnPoints[i].position;

            Animator _anim = _character.GetComponent<Animator>();
            float _prepareID = randomID[i] / 2f;
            Debug.Log("characrer " + i + " play " + _prepareID);
            _anim.SetFloat("rd_PrepareAnim", _prepareID);
        }



        for (int i = 0; i < characters.Length; i++)
        {
            Image _fillbar = _canvasGo.transform.Find("HpBar").GetChild(i).GetChild((i + 1) % 2).GetChild(1).GetChild(0).GetComponent<Image>();

            Transform _opponentTr = characters[(i + 1) % characters.Length].transform;
            characters[i].SetData(_fillbar,
               _opponentTr, i);
        }
        /// Player center
        PlayerCenter center = Instantiate(playerCenterPrefab).GetComponent<PlayerCenter>();
        center.name = "PlayerCenter";
        center.transform.SetParent(playerReleventParent);
        center.trP1 = characters[0].transform;
        center.trP2 = characters[1].transform;

        // Player Specialize
        Player thePlayer = (Player)characters[0];
        thePlayer.SetLookAt(center.transform);

        // Set Audio Source
        transform.GetChild(0).GetComponent<AudioPeer>().SetSource(characters[1].GetComponent<AudioSource>());

        // Player animation
        onPositioned = new UnityAction(delegate
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].GetComponent<BaseAnimController>().PlayPrepareFight();
            }
            cameraList[2].gameObject.SetActive(false);
        });

        // Set Enemy attack joint and beat generator
        _canvasGo.GetComponentInChildren<RingIndicatorControl>().SetData(characters[1].transform);
        GetComponent<BeatGenerator>().SetData(characters[1].transform, _canvasGo.GetComponentInChildren<HintGenerator>(), _canvasGo.GetComponentInChildren<ResultControl>(), songNameData[MyGameInstance.instance.SongIndex]);
        _canvasGo.GetComponentInChildren<HintGenerator>().SetData(GetComponent<BeatGenerator>(), songNameData[MyGameInstance.instance.SongIndex]);

        /// Set Camera
        IEnumerable<TimelineClip> clips = timelines.GetOutputTrack(1).GetClips();
        IEnumerator iEnumeratorOfClips = clips.GetEnumerator();
        while (iEnumeratorOfClips.MoveNext())
        {
            TimelineClip currClip = iEnumeratorOfClips.Current as TimelineClip;
            // 1. Set the player camera to the timeline
            if (currClip.displayName == "PlayerCamera")
            {
                Cinemachine.Timeline.CinemachineShot _shot = (Cinemachine.Timeline.CinemachineShot)currClip.asset;
                //_shot.VirtualCamera.exposedName = System.Guid.NewGuid().ToString(); 
                CinemachineVirtualCameraBase _playerVCam = characters[0].transform.GetChild(characters[0].transform.childCount - 2).GetComponent<CinemachineVirtualCameraBase>();
                cameraList[1] = _playerVCam.gameObject;
                director.SetReferenceValue(_shot.VirtualCamera.exposedName, _playerVCam);
            }
        }

        // 2. Set side virtual camera to
        sideVCam = cameraList[0].GetComponent<SideViewCam>();
        sideVCam.SetData(center.transform);
    }

    public void SwitchCamera(int _switchTo)
    {

        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].SetActive(false);
        }
        cameraList[_switchTo].SetActive(true);
    }

    public void SetFightMode(FightMode _fightMode)
    {
        if (fightMode != _fightMode)
        {
            fightMode = _fightMode;
            // Different mode
            // Switch between attack and defense
            // Changes in UI
            bool bIsPlayerAttack = false;
            if (_fightMode == FightMode.Offense)
                bIsPlayerAttack = true;
            else if (_fightMode == FightMode.Defense)
            {
                bIsPlayerAttack = false;
                characters[0].GetComponent<BaseAnimController>().DashVertically(-1, 1, 1);
            }

            characters[1].GetComponent<Animator>().SetBool("PlayerAttacking_b", bIsPlayerAttack);
            hint.SwitchMode(bIsPlayerAttack);
        }
    }

    public void FightOver(int _characterDie)
    {

        bFightOver = true;
        // Stop playing beat;
        GetComponent<BeatGenerator>().bCanPlay = false;
        // Switch to side view
        SwitchCamera(0);
        // Start play KO UI animation
        Transform KOUI = myCanvas.transform.Find("KOUI");
        KOUI.gameObject.SetActive(true);
        KOUI.GetComponent<Animator>().Play("KOAnim");

        //characters[(_characterDie + 1) % 2].ExecuteOpponent();

        // Start to play player finish Animation;
        characters[0].GameOver(_characterDie == 1);
        characters[1].GameOver(_characterDie == 0);

        //LevelLoader.instance.LoadScene("FightingScene_" + iFightingSceneID);
        StartCoroutine(ie_ShowEndUI());
    }

    IEnumerator ie_ShowEndUI()
    {
       
        yield return new WaitForSeconds(1.5f);
        DisablePlayerUI();
        yield return new WaitForSeconds(1.5f);
        MyGameInstance.instance.CalcualteScore();
    }

    IEnumerator ie_DelayGameOverTest(float _time, int _id)
    {
        yield return new WaitForSeconds(_time);
        FightOver(_id);
    }

    void Debug_SwitchCamera()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchCamera(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchCamera(1);
        }
    }

    public void ApplyDamageToCharacter(int _characterID, float _dmgAmount, float[] _attackDir)
    {
        // 0 -> player, 1 -> enemy
        characters[_characterID].GetDamage(_dmgAmount, _attackDir);
    }

    public void PlayerGuard(int releativeAttackID)
    {
        characters[0].GetComponent<PlayerAnimController>().PlayGuardAnimation(releativeAttackID);
    }

    IEnumerator ie_DelayAttackScore(int _buttonID)
    {
        yield return new WaitForSeconds(animDatas[_buttonID].GetDelay());
        Vector3 showPos = characters[1].GetJointPositionByJointID(animDatas[_buttonID].GetJoint()).position;
        ParticleGenerator.instance.GenerateOneTimeParticleAtPosition(0, showPos);
        //Debug.Log("H: " + animDatas[_buttonID].GetAttackDir()[0] + ", V: " + animDatas[_buttonID].GetAttackDir()[1]);
        ApplyDamageToCharacter(1, fDmgToEnemyPerAttack, animDatas[_buttonID].GetAttackDir());
    }
    public void FM_Score(HitResult hr, float _attackAnimationID = 0, bool bCombo = false)
    {
        MyGameInstance.instance.Score(hr);
        switch (fightMode)
        {
            case FightMode.Wait:
                break;
            case FightMode.Offense:
                if (!bCombo)
                {
                    int _buttonID = (int)_attackAnimationID;
                    // Play Player Attack animation
                    characters[0].GetComponent<PlayerAnimController>().PlayPlayerAttackAnimation(_attackAnimationID / 10f);
                    // Give Damage to Enemy when damage was given to enemy
                    StartCoroutine(ie_DelayAttackScore(_buttonID));
                }
                else
                {
                    // Combo Need to be re-structure
                    //ApplyDamageToCharacter(1, 5f);
                }
                break;
            case FightMode.Defense:
                PlayerGuard((int)(_attackAnimationID));
                Vector3 showPos = characters[0].GetJointPositionByJointID(animDatas[(int)(_attackAnimationID)].GetJoint()).position;
                ParticleGenerator.instance.GenerateOneTimeParticleAtPosition(1, showPos);
                break;
        }
    }
    public void FM_Miss(int number, float[] knockbackDir)
    {
        MyGameInstance.instance.Miss(number);
        switch (fightMode)
        {
            case FightMode.Wait:
                break;
            case FightMode.Offense:

                break;
            case FightMode.Defense:
                Vector3 showPos = characters[0].GetJointPositionByJointID(0).position;
                ParticleGenerator.instance.GenerateOneTimeParticleAtPosition(0, showPos);
                ApplyDamageToCharacter(0, 200f / iAllowAttackCount, knockbackDir);
                break;
        }
    }

    public void DisablePlayerUI()
    {
        myCanvas.GetComponent<UIFader>().FadeOut(0.2f);
    }

    void randomizeArray(int[] arr, int n)
    {
        // Start from the last element and 
        // swap one by one. We don't need to 
        // run for the first element  
        // that's why i > 0 
        for (int i = n - 1; i > 0; i--)
        {

            // Pick a random index 
            // from 0 to i 
            int j = UnityEngine.Random.Range(0, i + 1);

            // Swap arr[i] with the 
            // element at random index 
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

    }
}
