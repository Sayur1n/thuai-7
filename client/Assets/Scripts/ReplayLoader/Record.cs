using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Thubg.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using Mono.Data.Sqlite;
using Unity.IO.LowLevel.Unsafe;


public class Record : MonoBehaviour
{
    public enum PlayState
    {
        Prepare,
        Play,
        Pause,
        End,
        Jump
    }

    public class RecordInfo
    {
        // 20 frame per second
        public const float FrameTime = 0.05f;
        public PlayState NowPlayState = PlayState.Pause;
        public int NowTick = 0;
        /// <summary>
        /// Now record serial number
        /// </summary>
        public int NowRecordNum = 0;
        /// <summary>
        /// The speed of the record which can be negative
        /// </summary>
        public float RecordSpeed = 1f;
        public const float MinSpeed = -5f;
        public const float MaxSpeed = 5f;

        /// <summary>
        /// Contains all the item in the game
        /// </summary>
        public float NowFrameTime
        {
            get
            {
                return FrameTime / RecordSpeed;
            }
        }
        /// <summary>
        /// If NowDeltaTime is larger than NowFrameTime, then play the next frame
        /// </summary>
        public float NowDeltaTime = 0;

        /// <summary>
        /// The target tick to jump
        /// </summary>
        public int JumpTargetTick = int.MaxValue;
        /// <summary>
        /// Current max tick
        /// </summary>
        public int MaxTick;
        public void Reset()
        {
            //this.RecordSpeed = 1f;
            NowTick = 0;
            NowRecordNum = 0;
            JumpTargetTick = int.MaxValue;
        }
    }
    // meta info
    public RecordInfo _recordInfo;

    // GUI
    private readonly Button _stopButton;
    private readonly Button _replayButton;
    private readonly Slider _recordSpeedSlider;
    private readonly TMP_Text _recordSpeedText;
    private readonly float _recordSpeedSliderMinValue;
    private readonly float _recordSpeedSliderMaxValue;
    private readonly Slider _processSlider;
    private readonly TMP_Text _jumpTargetTickText;
    private readonly TMP_Text _maxTickText;

    // record data
    private readonly string _recordFilePath = null;
    private List<CompetitionUpdate> _competitionUpdates;
    private Map _map;
    private List<Supply> _supplies;
    
    // viewer


    private void LoadRecordData()
    {
        JObject recordJsonObject = JsonUtility.UnzipRecord(_recordFilePath);
        JObject mapJsonObject = (JObject)recordJsonObject["map"];
        JArray suppliesJsonObject = (JArray)recordJsonObject["supplies"];
        JArray recordArray = (JArray)recordJsonObject["competitionUpdates"];
        if (mapJsonObject == null || suppliesJsonObject == null || recordArray == null)
        {
            Debug.Log("Initialization Failed!");
            return;
        }
        Debug.Log(recordArray.ToString());
        _map = mapJsonObject.ToObject<Map>();
        _supplies = suppliesJsonObject.ToObject<List<Supply>>();
        _competitionUpdates = recordArray.ToObject<List<CompetitionUpdate>>();
    }

    #region Event Definition

    private void GenerateMap()
    {
        
    }

    private void UpdatePlayers(CompetitionUpdate update)
    {
        foreach (CompetitionUpdate.Player player in update.players)
        {
            Dictionary<Items, int> inventory = new();
            foreach (CompetitionUpdate.Player.Inventory item in player.inventory)
            {
                switch(item.name)
                {
                    default:
                        break;
                }
            }

            PlayerSource.UpdatePlayer(
                new Player(
                    player.playerId,
                    player.health,
                    player.armor switch
                    {
                        "NO_ARMOR" => ArmorTypes.NoArmor,
                        "PRIMARY_ARMOR" => ArmorTypes.PrimaryArmor,
                        "PREMIUM_ARMOR" => ArmorTypes.PremiumArmor,
                        _ => ArmorTypes.NoArmor
                    },
                    player.speed,
                    player.firearm.name switch
                    {

                        _ => FirearmTypes.Fists,
                    },
                    player.position,
                    inventory
                )
            );
        }
    }

    private void AfterPlayerPickUpEvent()
    {
        
    }

    private void AfterPlayerAbandonEvent()
    {
        
    }

    private void AfterPlayerAttackEvent()
    {
        
    }

    private void AfterPlayerUseMedicineEvent()
    {
    }

    private void AfterPlayerSwitchArmEvent()
    {
        
    }

    private void AfterPlayerUseGrenadeEvent()
    {

    }

    #endregion

    private void Start()
    {

    }

    private void UpdateTick()
    {
        try
        {
            if (_recordInfo.RecordSpeed > 0)
            {

            }
        }
        catch
        {

        }
    }

    private void Update()
    {
        if ((_recordInfo.NowPlayState == PlayState.Play && _recordInfo.NowRecordNum < _recordInfo.MaxTick) || (_recordInfo.NowPlayState == PlayState.Jump))
        {
            if (_recordInfo.NowDeltaTime > _recordInfo.NowFrameTime || _recordInfo.NowPlayState == PlayState.Jump)
            {
                UpdateTick();
                _recordInfo.NowDeltaTime = 0;
            }
            _recordInfo.NowDeltaTime += Time.deltaTime;
        }
    }
}