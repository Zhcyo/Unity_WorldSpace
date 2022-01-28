using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class SteelSeriesHFF : MonoBehaviour
{
    private string address = "";

    private string gameName = "HUMANFALLFLAT";

    private string gameDisplayName = "Human Fall Flat";

    private string developerName = "L42";

    private static bool isSteelSeriesActive;

    private float heartbeatLastBeatTime;

    private float heartbeatPulse = 10f;

    private bool lastLeftVal;

    private bool lastRightVal;

    private bool lastBothArmsVal;

    private bool lastRespawning;

    private bool leftVal;

    private bool rightVal;

    private bool respawning;

    private bool celebrating;

    private float celebrateStart;

    private float celebrateLength = 2f;

    private float celebrateDeadTime = 3f;

    private List<string> SSActions = new List<string>()
    {
        "game_metadata",
        "remove_game",
        "bind_game_event",
        "register_game_event",
        "game_event",
        "game_heartbeat"
    };

    private List<string> HFFEvents = new List<string>()
    {
        "LEFT_GRAB",
        "RIGHT_GRAB",
        "BOTH_GRAB",
        "RESPAWN",
        "HIT_CHECKPOINT"
    };

    private List<int> KB_Left_Keys = new List<int>()
    {
        53,
        43,
        57,
        225,
        224
    };

    private List<int> KB_Right_Keys = new List<int>()
    {
        42,
        40,
        229,
        228
    };

    private List<int> KB_Both_Keys = new List<int>()
    {
        58,
        59,
        60,
        61,
        62,
        63,
        64,
        65,
        66,
        67,
        68,
        69,
        41
    };

    private List<int> KB_FullKeyboard = new List<int>()
    {
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        16,
        17,
        18,
        19,
        20,
        21,
        22,
        23,
        24,
        25,
        26,
        27,
        28,
        29,
        30,
        31,
        32,
        33,
        34,
        35,
        36,
        37,
        38,
        39,
        227,
        101,
        226,
        44,
        230,
        231,
        45,
        46,
        47,
        48,
        49,
        50,
        51,
        52,
        54,
        55,
        56
    };

    public SteelSeriesHFF()
    {
    }

    private SteelSeriesHFF.HandlerToAdd AddHandlerToList(string device, string zone)
    {
        return new SteelSeriesHFF.HandlerToAdd()
        {
            device = device,
            zone = zone
        };
    }

    private void Bind_Event(string eventName, List<SteelSeriesHFF.HandlerToAdd> handlersToAdd, int r, int g, int b, List<int> customZoneKeys = null, int frequency = 0, int repeatLimit = -1)
    {
        SteelSeriesHFF.BindEvent bindEvent = new SteelSeriesHFF.BindEvent()
        {
            game = this.gameName,
            @event = eventName,
            min_value = 0,
            max_value = 1,
            icon_id = 0
        };
        List<SteelSeriesHFF.Handler> handlers = new List<SteelSeriesHFF.Handler>();
        foreach (SteelSeriesHFF.HandlerToAdd handlerToAdd in handlersToAdd)
        {
            SteelSeriesHFF.Handler handler = new SteelSeriesHFF.Handler()
            {
                DeviceType = handlerToAdd.device
            };
            if (customZoneKeys != null && handler.DeviceType == "rgb-per-key-zones")
            {
                handler.CustomZoneKeys = customZoneKeys;
            }
            handler.zone = handlerToAdd.zone;
            handler.color = new SteelSeriesHFF.Color()
            {
                red = r,
                green = g,
                blue = b
            };
            handler.mode = "color";
            handler.rate = new SteelSeriesHFF.Rate()
            {
                frequency = frequency
            };
            if (repeatLimit >= 0)
            {
                handler.rate.repeat_limit = repeatLimit;
            }
            handlers.Add(handler);
        }
        bindEvent.handlers = handlers;
        string str = JsonUtility.ToJson(bindEvent).Replace("DeviceType", "device-type").Replace("CustomZoneKeys", "custom-zone-keys").Replace(",\"custom-zone-keys\":[]", "");
        this.SendJSON(str, this.SSActions[2]);
    }

    private void checkIsActive()
    {
        if (!SteelSeriesHFF.isSteelSeriesActive)
        {
            return;
        }
        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(string.Concat("http://", this.address, "/game_metadata")));
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";
        try
        {
            httpWebRequest.Timeout = 500;
            httpWebRequest.GetRequestStream();
            SteelSeriesHFF.isSteelSeriesActive = true;
            this.WriteDebug("checkIsActive - true");
        }
        catch (Exception exception)
        {
            SteelSeriesHFF.isSteelSeriesActive = false;
            this.WriteDebug("checkIsActive - false");
        }
    }

    private void CreateAnimEvent()
    {
    }

    private void DoHeartbeat()
    {
        SteelSeriesHFF.HeartBeat heartBeat = new SteelSeriesHFF.HeartBeat()
        {
            game = this.gameName
        };
        this.heartbeatLastBeatTime = Time.time;
        this.SendJSON(JsonUtility.ToJson(heartBeat), this.SSActions[5]);
    }

    public void Event_Toggle(string eventName, bool value)
    {
        SteelSeriesHFF.SendEvent sendEvent = new SteelSeriesHFF.SendEvent()
        {
            game = this.gameName,
            @event = eventName,
            data = new SteelSeriesHFF.Data()
            {
                @value = Convert.ToInt32(value)
            }
        };
        JsonUtility.ToJson(sendEvent);
        this.SendJSON(JsonUtility.ToJson(sendEvent), this.SSActions[4]);
    }

    private string GetAddress()
    {
        SteelSeriesHFF.isSteelSeriesActive = false;
        this.WriteDebug("GetAddress - false");
        return "";
    }

    private void HumanEvent(SteelSeriesHFF.HumanEvents currentEvent, bool value)
    {
        switch (currentEvent)
        {
            case SteelSeriesHFF.HumanEvents.LeftArm:
                {
                    this.Event_Toggle(this.HFFEvents[2], false);
                    this.Event_Toggle(this.HFFEvents[0], value);
                    return;
                }
            case SteelSeriesHFF.HumanEvents.RightArm:
                {
                    this.Event_Toggle(this.HFFEvents[2], false);
                    this.Event_Toggle(this.HFFEvents[1], value);
                    return;
                }
            case SteelSeriesHFF.HumanEvents.BothArms:
                {
                    this.Event_Toggle(this.HFFEvents[0], false);
                    this.Event_Toggle(this.HFFEvents[1], false);
                    this.Event_Toggle(this.HFFEvents[2], value);
                    return;
                }
            case SteelSeriesHFF.HumanEvents.Respawning:
                {
                    this.Event_Toggle(this.HFFEvents[3], value);
                    return;
                }
            case SteelSeriesHFF.HumanEvents.Checkpoint:
                {
                    this.Event_Toggle(this.HFFEvents[4], value);
                    return;
                }
            default:
                {
                    return;
                }
        }
    }

    private void OnEnable()
    {
        this.address = this.GetAddress();
        this.checkIsActive();
        if (SteelSeriesHFF.isSteelSeriesActive)
        {
            this.Register_Game();
            List<SteelSeriesHFF.HandlerToAdd> handlerToAdds = new List<SteelSeriesHFF.HandlerToAdd>()
            {
                this.AddHandlerToList("rgb-per-key-zones", null)
            };
            this.Bind_Event(this.HFFEvents[4], handlerToAdds, 255, 255, 255, this.KB_FullKeyboard, 5, 11);
            SteelSeriesHFF.HandlerToAdd handlerToAdd = new SteelSeriesHFF.HandlerToAdd();
            handlerToAdds = new List<SteelSeriesHFF.HandlerToAdd>()
            {
                this.AddHandlerToList("rgb-2-zone", "one"),
                this.AddHandlerToList("rgb-8-zone", "left"),
                this.AddHandlerToList("rgb-per-key-zones", null)
            };
            this.Bind_Event(this.HFFEvents[0], handlerToAdds, 0, 255, 0, this.KB_Left_Keys, 0, -1);
            handlerToAdds = new List<SteelSeriesHFF.HandlerToAdd>();
            SteelSeriesHFF.HandlerToAdd handlerToAdd1 = new SteelSeriesHFF.HandlerToAdd();
            handlerToAdds.Add(this.AddHandlerToList("rgb-2-zone", "two"));
            handlerToAdds.Add(this.AddHandlerToList("rgb-8-zone", "right"));
            handlerToAdds.Add(this.AddHandlerToList("rgb-per-key-zones", null));
            this.Bind_Event(this.HFFEvents[1], handlerToAdds, 0, 255, 0, this.KB_Right_Keys, 0, -1);
            handlerToAdds = new List<SteelSeriesHFF.HandlerToAdd>();
            SteelSeriesHFF.HandlerToAdd handlerToAdd2 = new SteelSeriesHFF.HandlerToAdd();
            handlerToAdds.Add(this.AddHandlerToList("headset", "earcups"));
            handlerToAdds.Add(this.AddHandlerToList("rgb-8-zone", "one"));
            handlerToAdds.Add(this.AddHandlerToList("rgb-per-key-zones", null));
            this.Bind_Event(this.HFFEvents[2], handlerToAdds, 0, 0, 255, this.KB_Both_Keys, 0, -1);
            handlerToAdds = new List<SteelSeriesHFF.HandlerToAdd>();
            SteelSeriesHFF.HandlerToAdd handlerToAdd3 = new SteelSeriesHFF.HandlerToAdd();
            handlerToAdds.Add(this.AddHandlerToList("rgb-8-zone", "two"));
            handlerToAdds.Add(this.AddHandlerToList("rgb-per-key-zones", "keypad"));
            this.Bind_Event(this.HFFEvents[3], handlerToAdds, 255, 0, 0, null, 5, -1);
        }
    }

    private void Register_Game()
    {
        SteelSeriesHFF.RegisterGame registerGame = new SteelSeriesHFF.RegisterGame()
        {
            developer = this.developerName,
            game = this.gameName,
            game_display_name = this.gameDisplayName,
            deinitialize_timer_length_ms = 60000
        };
        this.SendJSON(JsonUtility.ToJson(registerGame), this.SSActions[0]);
    }

    private void Remove_Game(string gameToRemove = null)
    {
        if (gameToRemove == null)
        {
            gameToRemove = this.gameName;
        }
        SteelSeriesHFF.RemoveGame removeGame = new SteelSeriesHFF.RemoveGame()
        {
            game = gameToRemove
        };
        this.SendJSON(JsonUtility.ToJson(removeGame), this.SSActions[1]);
    }

    private void SendJSON(string json, string action)
    {
        if (!SteelSeriesHFF.isSteelSeriesActive)
        {
            return;
        }
        try
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(string.Concat("http://", this.address, "/", action)));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 500;
            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }
            using (StreamReader streamReader = new StreamReader(((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream()))
            {
                streamReader.ReadToEnd();
            }
        }
        catch (Exception exception)
        {
            SteelSeriesHFF.isSteelSeriesActive = false;
        }
    }

    private void Start()
    {
        if (!SteelSeriesHFF.isSteelSeriesActive)
        {
            return;
        }
        this.Event_Toggle(this.HFFEvents[2], false);
        this.Event_Toggle(this.HFFEvents[0], false);
        this.Event_Toggle(this.HFFEvents[1], false);
        this.Event_Toggle(this.HFFEvents[3], false);
        this.Event_Toggle(this.HFFEvents[4], false);
        this.DoHeartbeat();
    }

    public void SteelSeriesEvent_CheckpointHit()
    {
        if (!SteelSeriesHFF.isSteelSeriesActive)
        {
            return;
        }
        if (Time.time > this.celebrateStart + this.celebrateDeadTime)
        {
            this.celebrateStart = Time.time;
            this.celebrating = true;
            this.HumanEvent(SteelSeriesHFF.HumanEvents.Checkpoint, true);
        }
    }

    public void SteelSeriesEvent_LeftArm(bool isLeftArmRaised)
    {
        if (!SteelSeriesHFF.isSteelSeriesActive)
        {
            return;
        }
        this.leftVal = isLeftArmRaised;
        if (this.leftVal != this.lastLeftVal)
        {
            if (!(this.leftVal & this.rightVal != this.lastBothArmsVal))
            {
                this.HumanEvent(SteelSeriesHFF.HumanEvents.LeftArm, this.leftVal);
            }
            else
            {
                this.HumanEvent(SteelSeriesHFF.HumanEvents.BothArms, this.leftVal & this.rightVal);
            }
        }
        this.lastLeftVal = this.leftVal;
        this.lastBothArmsVal = this.leftVal & this.rightVal;
    }

    public void SteelSeriesEvent_Respawning(bool isRespawning)
    {
        if (!SteelSeriesHFF.isSteelSeriesActive)
        {
            return;
        }
        this.respawning = isRespawning;
        if (this.respawning != this.lastRespawning)
        {
            this.HumanEvent(SteelSeriesHFF.HumanEvents.Respawning, this.respawning);
        }
        this.lastRespawning = this.respawning;
    }

    public void SteelSeriesEvent_RightArm(bool isRightArmRaised)
    {
        if (!SteelSeriesHFF.isSteelSeriesActive)
        {
            return;
        }
        this.rightVal = isRightArmRaised;
        if (this.rightVal != this.lastRightVal)
        {
            if (!(this.leftVal & this.rightVal != this.lastBothArmsVal))
            {
                this.HumanEvent(SteelSeriesHFF.HumanEvents.RightArm, this.rightVal);
            }
            else
            {
                this.HumanEvent(SteelSeriesHFF.HumanEvents.BothArms, this.leftVal & this.rightVal);
            }
        }
        this.lastRightVal = this.rightVal;
        this.lastBothArmsVal = this.leftVal & this.rightVal;
    }

    private void Update()
    {
        if (!SteelSeriesHFF.isSteelSeriesActive)
        {
            return;
        }
        if (Time.time > this.heartbeatLastBeatTime + this.heartbeatPulse)
        {
            this.DoHeartbeat();
        }
        if (this.celebrating && Time.time> this.celebrateStart + this.celebrateLength)
        {
            this.celebrating = false;
            this.HumanEvent(SteelSeriesHFF.HumanEvents.Checkpoint, false);
        }
    }

    private void WriteDebug(string debug)
    {
    }

    [Serializable]
    public class BindEvent
    {
        public string game;

        public string @event;

        public int min_value;

        public int max_value;

        public int icon_id;

        public List<SteelSeriesHFF.Handler> handlers;

        public BindEvent()
        {
        }
    }

    [Serializable]
    public class Color
    {
        public int red;

        public int green;

        public int blue;

        public Color()
        {
        }
    }

    [Serializable]
    public class CoreProps
    {
        public string address;

        public string encrypted_address;

        public string gg_encrypted_address;

        public string mercstealth_address;

        public CoreProps()
        {
        }
    }

    [Serializable]
    public class Data
    {
        public int @value;

        public Data()
        {
        }
    }

    [Serializable]
    public class Handler
    {
        public string DeviceType;

        public List<int> CustomZoneKeys;

        public string zone;

        public SteelSeriesHFF.Color color;

        public string mode;

        public SteelSeriesHFF.Rate rate;

        public Handler()
        {
        }
    }

    public class HandlerToAdd
    {
        public string device;

        public string zone;

        public HandlerToAdd()
        {
        }
    }

    [Serializable]
    public class HeartBeat
    {
        public string game;

        public HeartBeat()
        {
        }
    }

    private enum HFFEventsEnum
    {
        LEFT_GRAB,
        RIGHT_GRAB,
        BOTH_GRAB,
        RESPAWN,
        HIT_CHECKPOINT
    }

    public enum HumanEvents
    {
        LeftArm,
        RightArm,
        BothArms,
        Respawning,
        Checkpoint
    }

    [Serializable]
    public class Rate
    {
        public int frequency;

        public int repeat_limit;

        public Rate()
        {
        }
    }

    [Serializable]
    public class RegisterGame
    {
        public string game;

        public string game_display_name;

        public string developer;

        public int deinitialize_timer_length_ms;

        public RegisterGame()
        {
        }
    }

    [Serializable]
    public class RemoveGame
    {
        public string game;

        public RemoveGame()
        {
        }
    }

    [Serializable]
    public class SendEvent
    {
        public string game;

        public string @event;

        public SteelSeriesHFF.Data data;

        public SendEvent()
        {
        }
    }

    private enum SSActionsEnum
    {
        game_metadata,
        remove_game,
        bind_game_event,
        register_game_event,
        game_event,
        game_heartbeat
    }
}