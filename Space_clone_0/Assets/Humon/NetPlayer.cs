//using System;
//using System.Collections.Generic;
//using System.Threading;
//using UnityEngine;

//public class NetPlayer : MonoBehaviour
//{


//    public bool isLocalPlayer;

//    public Human human;

//   // public CameraController3 cameraController;

//   // public RagdollCustomization customization;

//  //  public RagdollPresetMetadata skin;

//    public uint localCoopIndex;

//    public string skinUserId;

//    public byte[] skinCRC;


//    public HumanControls controls;

// //   public OverheadNameTag overHeadNameTag;

//    public bool ChatUserAdded;

//    private object moveLock = new object();

//    private int moveFrames;

//    private float walkForward;

//    private float walkRight;

//    private float cameraPitch;

//    private float cameraYaw;

//    private float leftExtend;

//    private float rightExtend;

//    private bool jump;

//    private bool playDead;

//    private bool holding;

//    private bool shooting;

//    private int nextIdentityId = 1;



//    public NetPlayer()
//    {
//    }

//    //public void ApplyPreset(RagdollPresetMetadata preset, bool bake = true)
//    //{
//    //    if (this.customization == null)
//    //    {
//    //        this.customization = this.human.ragdoll.get_gameObject().AddComponent<RagdollCustomization>();
//    //    }
//    //    this.customization.ApplyPreset(preset, true);
//    //    this.customization.RebindColors(bake, true);
//    //    this.customization.ClearOutCachedClipVolumes();
//    //}

//    //public void ApplySkin(byte[] bytes)
//    //{
//    //    this.skin = RagdollPresetMetadata.Deserialize(bytes);
//    //    this.skin.SaveNetSkin(this.localCoopIndex, this.skinUserId);
//    //    this.ApplyPreset(this.skin, true);
//    //}








//    public void FixedUpdate()
//    {
      
//        this.human.groundManager.PostFixedUpdate();
//    }

//    public override void PreFixedUpdate()
//    {
//        base.PreFixedUpdate();
//        object obj = this.moveLock;
//        Monitor.Enter(obj);
//        try
    
//        }
//        finally
//        {
//            Monitor.Exit(obj);
//        }
//    }

  
//    public void ReceiveMove(NetStream stream)
//    {
//        float single = NetFloat.Dequantize(stream.ReadInt32(8), 1f, 8);
//        float single1 = NetFloat.Dequantize(stream.ReadInt32(8), 1f, 8);
//        object obj = this.moveLock;
//        Monitor.Enter(obj);
//        try
//        {
//            this.moveFrames++;
//            this.walkForward = Mathf.Lerp(this.walkForward, single, 1f / (float)this.moveFrames);
//            this.walkRight = Mathf.Lerp(this.walkRight, single1, 1f / (float)this.moveFrames);
//            this.cameraPitch = NetFloat.Dequantize(stream.ReadInt32(9), 180f, 9);
//            this.cameraYaw = NetFloat.Dequantize(stream.ReadInt32(9), 180f, 9);
//            if (this.moveFrames == 1)
//            {
//                float single2 = 0f;
//                float single3 = single2;
//                this.rightExtend = single2;
//                this.leftExtend = single3;
//                int num = 0;
//                bool flag = (bool)num;
//                this.playDead = (bool)num;
//                this.jump = flag;
//                this.shooting = false;
//            }
//            this.leftExtend = Mathf.Max(this.leftExtend, NetFloat.Dequantize(stream.ReadInt32(5), 1f, 5));
//            this.rightExtend = Mathf.Max(this.rightExtend, NetFloat.Dequantize(stream.ReadInt32(5), 1f, 5));
//            this.jump |= stream.ReadBool();
//            this.playDead |= stream.ReadBool();
//            this.shooting |= stream.ReadBool();
//            if (this.shooting)
//            {
//                Debug.LogError("shooting = true in NetPlayer.cs:357");
//            }
//            NetStream netStream = NetGame.BeginMessage(NetMsgId.Move);
//            try
//            {
//                netStream.WriteNetId(this.netId);
//                netStream.Write(this.holding);
//                NetGame.instance.SendUnreliable(this.host, netStream, -1);
//            }
//            finally
//            {
//                if (netStream != null)
//                {
//                    netStream = netStream.Release();
//                }
//            }
//        }
//        finally
//        {
//            Monitor.Exit(obj);
//        }
//    }

//    public void ReceiveMoveAck(NetStream stream)
//    {
//        this.holding = stream.ReadBool();
//    }

   

  

//    private void SetupBodies()
//    {
//        int num = 3;
//        Ragdoll ragdoll = this.human.ragdoll;
//        this.RegisterBody(ragdoll.partBall, null, true, true, num);
//        this.RegisterBody(ragdoll.partHips, null, true, true, num);
//        this.RegisterBody(ragdoll.partWaist, ragdoll.partHips, false, true, num);
//        this.RegisterBody(ragdoll.partChest, ragdoll.partHips, false, true, num);
//        this.RegisterBody(ragdoll.partHead, ragdoll.partHips, true, true, num + 2);
//        this.RegisterBody(ragdoll.partLeftArm, ragdoll.partHips, false, true, num);
//        this.RegisterBody(ragdoll.partLeftForearm, ragdoll.partHips, false, true, num);
//        this.RegisterBody(ragdoll.partLeftThigh, ragdoll.partHips, false, true, num);
//        this.RegisterBody(ragdoll.partLeftLeg, ragdoll.partHips, false, true, num);
//        this.RegisterBody(ragdoll.partRightArm, ragdoll.partHips, false, true, 0);
//        this.RegisterBody(ragdoll.partRightForearm, ragdoll.partHips, false, true, num);
//        this.RegisterBody(ragdoll.partRightThigh, ragdoll.partHips, false, true, num);
//        this.RegisterBody(ragdoll.partRightLeg, ragdoll.partHips, false, true, num);
//    }

//}
