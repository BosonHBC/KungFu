using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;


public class FightingManager : MonoBehaviour
{
    public static FightingManager instance;
    private void Awake()
    {
        if(instance==null || instance != this)
        {
            instance = this;
        }
    }
    [Header("Prefab")]
    [SerializeField] GameObject playerUIPrefab;
    [SerializeField] GameObject playerCenterPrefab;
    [SerializeField] private Character[] playerPrefabs;
    [Header("Parents")]
    [SerializeField] Transform playerReleventParent;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] PlayableDirector director;
    [SerializeField] TimelineAsset timelines;

    [Header("Parameter")]
    public int iFightingSceneID;
    private Character[] characters = new Character[2];
    // Start is called before the first frame update
    void Start()
    {
        CreateObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateObjects()
    {
        /// Canvas
        GameObject _canvasGo = Instantiate(playerUIPrefab);
        _canvasGo.GetComponent<Canvas>().worldCamera = Camera.main;
        _canvasGo.name = "PlayerUI";

        director.SetGenericBinding(timelines.GetOutputTrack(4), _canvasGo.GetComponent<Animator>());
        // Set UIs

        /// Characters 0-> player 1-> enemy
        for (int i = 0; i < 2; i++)
        {
            Character _character = Instantiate(playerPrefabs[i]).GetComponent<Character>();
            characters[i] = _character;
            _character.transform.SetParent(playerReleventParent);
            _character.transform.position = spawnPoints[i].position;
        }
        for (int i = 0; i < characters.Length; i++)
        {
            Image _fillbar = _canvasGo.transform.GetChild(0).GetChild(i).GetChild((i + 1) % 2).GetChild(1).GetChild(0).GetComponent<Image>();

            Transform _opponentTr = characters[(i + 1) % characters.Length].transform;
            characters[i].SetData(_fillbar,
               _opponentTr);
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
    }
}
