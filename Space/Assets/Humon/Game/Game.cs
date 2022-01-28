
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public enum GameState
{
    Inactive,
    Paused,
    LoadingLevel,
    PlayingLevel
}
public enum WorkshopItemSource : byte
{
    BuiltIn = 0,
    EditorPick = 1,
    Subscription = 2,
    LocalWorkshop = 3,
    BuiltInLobbies = 4,
    SubscriptionLobbies = 5,
    NotSpecified = 99
}
public class Game : MonoBehaviour, IGame, IDependency
{
 

    public static Game instance;

    public string[] levels;

    public string[] editorPickLevels;

    public int levelCount;

    public int currentLevelNumber = -1;

    public int currentCheckpointNumber;

    public int currentCheckpointSubObjectives;

    public List<int> currentSolvedCheckpoints = new List<int>();

    public WorkshopItemSource currentLevelType;

    public string editorLanguage = "english";

    public int editorStartLevel = 3;

    public int editorStartCheckpoint = 3;

    public bool passedCheckpoint_ForSteelSeriesEvent;

    public Light defaultLight;

    public const int currentBuiltLevels = 13;

    public const int kMaxBuiltInLevels = 16;

    public const int kMaxBuiltInLobbies = 128;

    public static uint currentLevelID;

   

    public GameState state;

    public bool passedLevel;

   

    public Camera cameraPrefab;

    public Ragdoll ragdollPrefab;

    public Material skyboxMaterial;

    [NonSerialized]
    public static ulong multiplayerLobbyLevel;

    [NonSerialized]
    public bool singleRun;

    [NonSerialized]
    public bool HasSceneLoaded;

    public static Action<Human> OnDrowning;

    private AssetBundle bundle;



    public bool workshopLevelIsCustom;

    private Color skyColor;

    private const string kDefaultMixerIfNull = "Effects";

    private const int kInitialAudioSources = 200;

    public Game()
    {
    }

    public void AfterLoad(int checkpointNumber, int subobjectives)
    {
      
        this.state = GameState.PlayingLevel;
      
       
        this.currentCheckpointSubObjectives = subobjectives;
      
       
    }

    public void AfterUnload()
    {
       
        this.currentLevelNumber = -1;
        this.state = GameState.Inactive;
    }

    private void Awake()
    {
     
    }

    public void BeforeLoad()
    {
      
    }




    public void EnterCheckpoint(int checkpoint, int subObjectives)
    {
        if (this.state != GameState.PlayingLevel)
        {
            return;
        }
        bool flag = false;
        if (this.currentCheckpointNumber < checkpoint)
        {
            flag = true;
        }
      
        else if (this.currentCheckpointNumber == checkpoint && subObjectives != 0)
        {
            flag = true;
        }
        if (flag)
        {
            //Debug.Log(string.Concat("Passed ", checkpoint.ToString(), ", subobjectives: ", subObjectives.ToString()));
            this.passedCheckpoint_ForSteelSeriesEvent = true;
            int num = this.currentCheckpointNumber;
            int num1 = this.currentCheckpointSubObjectives;
            if (this.currentCheckpointNumber != checkpoint)
            {
                this.currentCheckpointSubObjectives = 0;
            }
            if (subObjectives != 0)
            {
                this.currentCheckpointSubObjectives = this.currentCheckpointSubObjectives | 1 << (subObjectives - 1 & 31);
            }
            this.currentCheckpointNumber = checkpoint;
        
        }
    }

    public void EnterPassZone()
    {
        if (this.state != GameState.PlayingLevel)
        {
            return;
        }
     
        this.passedCheckpoint_ForSteelSeriesEvent = true;
        this.passedLevel = true;
    }

   
   
   

  
    private void FixedUpdate()
    {
    
    }

    private void FixupLoadedBundle(Scene scene)
    {
        
    }

   
 
    public static bool GetKey(KeyCode key)
    {
        return Input.GetKey(key);
    }

    public static bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    public static bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    public void Fall(HumanBase human, bool drown = false, bool fallAchievement = true)
    {
        throw new NotImplementedException();
    }

    public void Initialize()
    {
        throw new NotImplementedException();
    }
}