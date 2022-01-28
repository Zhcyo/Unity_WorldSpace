
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//public enum CameraMode
//{
//    Far,
//    Close,
//    FirstPerson
//}
//public class CameraController3 : MonoBehaviour
//{
//    public CameraMode mode;

//    public LayerMask wallLayers;

//    public Vector3 farTargetOffset;

//    public float farRange;

//    public float farRangeAction;

//    public Vector3 closeTargetOffset;

//    public float closeRange;

//    public float closeRangeLookUp;

//    public Vector3 fpsTargetOffset;

//    private float nearClip = 0.05f;

//    public bool ignoreWalls;

//    public Human human;

//    public Camera gameCam;

 

//    private Ragdoll ragdoll;

//    public static float fovAdjust;

//    public static float smoothingAmount;

//    private List<float> offsetSmoothingFast = new List<float>();

//    private List<float> offsetSmoothingSlow = new List<float>();

//    private float wallHold = 1000f;

//    private float wallHoldTime;

//    [NonSerialized]
//    public float offset = 4f;

//    private Vector3 oldTarget;

//    private Vector3 smoothTarget;

//    private int oldFrame;

//    private Vector3 fixedupdateSmooth;

//    private float offsetSpeed;

//    public float headPivotAhead;

//    private float pitchRange = 30f;

//    private float oldPitchSign;

//    private float pitchExpandTimer;

//    private float cameraPitchAngle;

//    private float standPhase;

//    private float holdPhase;

//    private Vector3[] rayStarts = new Vector3[4];

//    private float cameraTransitionSpeed = 1f;

//    private float cameraTransitionPhase;

//    private float startFov;

//    private Vector3 startOffset;

//    private Quaternion startRotation = Quaternion.identity;

//    private Vector3 startHumanCameraPos;

//    static CameraController3()
//    {
//        CameraController3.fovAdjust = 0f;
//        CameraController3.smoothingAmount = 0.5f;
//    }

//    public CameraController3()
//    {
//    }

//    public void ApplyCamera(Vector3 target, Vector3 position, Quaternion rotation, float fov)
//    {
//        Vector3 vector3;
//        this.cameraTransitionPhase = this.cameraTransitionPhase + this.cameraTransitionSpeed * Time.deltaTime;
//        if (this.cameraTransitionPhase < 1f)
//        {
//            Ease.easeInOutSine(0f, 1f, Mathf.Clamp01(this.cameraTransitionPhase));
//            base.transform.rotation=(rotation);
//            base.transform.position=(position);
//            this.gameCam.fieldOfView=(fov);
//            Vector3 viewportPoint = this.gameCam.WorldToViewportPoint(this.human.transform.position);
//            Vector3 vector31 = Vector3.Lerp(this.startHumanCameraPos, viewportPoint, this.cameraTransitionPhase);
//            base.transform.rotation=(Quaternion.Lerp(this.startRotation, rotation, this.cameraTransitionPhase));
//            this.gameCam.fieldOfView=(Mathf.Lerp(this.startFov, fov, this.cameraTransitionPhase));
//            Vector3 worldPoint = this.gameCam.ViewportToWorldPoint(vector31);
//            Transform _transform = base.transform;
//            _transform.position=(_transform.position + (this.human.transform.position - worldPoint));
//        }
//        else
//        {
//            base.transform.position=(position);
//            base.transform.rotation=(rotation);
//            this.gameCam.fieldOfView=(fov);
//        }
//        if ((this.human.controls.leftGrab ? false : !this.human.controls.rightGrab))
//        {
//            vector3 = target - position;
//            Mathf.Clamp(vector3.magnitude, 6f, 6f);
//            return;
//        }
//        vector3 = target - position;
//        Mathf.Clamp(vector3.magnitude, 4f, 5f);
//    }

//    private void CalculateCloseCam(out Vector3 targetOffset, out float yaw, out float pitch, out float camDist, out float minDist, out float fov, out float nearClip)
//    {
//        fov = 70f;
//        nearClip = this.nearClip;
//        yaw = this.human.controls.cameraYawAngle;
//        pitch = this.human.controls.cameraPitchAngle + 10f;
//        if (pitch < 0f)
//        {
//            pitch *= 0.8f;
//        }
//        Quaternion quaternion = Quaternion.Euler(pitch, yaw, 0f);
//        targetOffset = quaternion * this.closeTargetOffset;
//        Vector3 vector3 = Quaternion.Euler(this.human.controls.cameraPitchAngle, this.human.controls.cameraYawAngle, 0f) * Vector3.forward;
//        camDist = Mathf.Lerp(this.closeRange, this.closeRangeLookUp, vector3.y);
//        minDist = 0.025f;
//    }

//    private void CalculateFarCam(out Vector3 targetOffset, out float yaw, out float pitch, out float camDist, out float minDist, out float fov, out float nearClip)
//    {
//        fov = 70f;
//        nearClip = this.nearClip;
//        yaw = this.human.controls.cameraYawAngle;
//        this.cameraPitchAngle = this.human.controls.cameraPitchAngle;
//        float single = 0.5f + 0.5f * Mathf.InverseLerp(0f, 80f, Mathf.Abs(this.cameraPitchAngle));
//        if (!this.human.controls.holding)
//        {
//            this.holdPhase = Mathf.MoveTowards(this.holdPhase, 0f, Time.fixedDeltaTime / 0.5f);
//        }
//        else
//        {
//            this.holdPhase = Mathf.MoveTowards(this.holdPhase, 1f, (this.human.state == HumanState.Climb ? 0f : Time.fixedDeltaTime * Mathf.InverseLerp(0.5f, 0f, this.human.controls.walkSpeed)));
//        }
//        single *= Mathf.Lerp(Mathf.InverseLerp(0.5f, 0f, this.human.controls.walkSpeed), 1f, this.holdPhase);
//        this.standPhase = Mathf.MoveTowards(this.standPhase, single, Time.fixedDeltaTime / 1f);
//        this.pitchRange = Mathf.Lerp(30f, 60f, this.standPhase);
//        int num = -50;
//        int num1 = 80;
//        float single1 = this.cameraPitchAngle / 80f * 60f + 10f;
//        float single2 = this.cameraPitchAngle / 80f * 20f + 10f;
//        pitch = Mathf.Lerp(single2, single1, this.standPhase);
//        Quaternion quaternion = Quaternion.Euler(pitch, yaw, 0f);
//        targetOffset = quaternion * this.farTargetOffset;
//        if (this.human.controls.leftGrab || this.human.controls.rightGrab)
//        {
//            camDist = Options.LogMap(pitch, (float)num, 0f, (float)num1, this.farRangeAction * 0.6f, this.farRangeAction, (this.farRange + this.farRangeAction) / 2f);
//        }
//        else
//        {
//            camDist = Options.LogMap(pitch, (float)num, 0f, (float)num1, this.farRangeAction * 0.6f, this.farRangeAction, this.farRange);
//        }
//        fov = Mathf.Lerp(70f, 80f, Mathf.InverseLerp(0f, (float)num, pitch));
//        minDist = 0.025f;
//    }

//    private void CalculateFirstPersonCam(out Vector3 targetOffset, out float yaw, out float pitch, out float camDist, out float minDist, out float fov, out float nearClip)
//    {
//        fov = 70f;
//        nearClip = this.nearClip;
//        yaw = this.human.controls.cameraYawAngle;
//        pitch = this.human.controls.cameraPitchAngle + 10f;
//        if (pitch < 0f)
//        {
//            pitch *= 0.8f;
//        }
//        Quaternion quaternion = Quaternion.Euler(pitch, yaw, 0f);
//        targetOffset = quaternion * this.fpsTargetOffset;
//        camDist = 0f;
//        minDist = 0f;
//    }

//    private float CompensateForWallsNearPlane(Vector3 targetPos, Quaternion lookRot, float desiredDist, float minDist)
//    {
//        RaycastHit raycastHit = new RaycastHit();
//        Vector3 vector3 = lookRot * Vector3.forward;
//        float _nearClipPlane = this.gameCam.nearClipPlane;
//        //base.get_transform().get_position();
//        //base.get_transform().get_rotation();
//        base.transform.rotation=(lookRot);
//        base.transform.position=(targetPos - (vector3 * (this.gameCam.nearClipPlane + minDist)));
//        this.rayStarts[0] = this.gameCam.ViewportToWorldPoint(new Vector3(0f, 0f, _nearClipPlane));
//        this.rayStarts[1] = this.gameCam.ViewportToWorldPoint(new Vector3(0f, 1f, _nearClipPlane));
//        this.rayStarts[2] = this.gameCam.ViewportToWorldPoint(new Vector3(1f, 0f, _nearClipPlane));
//        this.rayStarts[3] = this.gameCam.ViewportToWorldPoint(new Vector3(1f, 1f, _nearClipPlane));
//        float _distance = desiredDist - _nearClipPlane;
//        for (int i = 0; i < (int)this.rayStarts.Length; i++)
//        {
//            if (Physics.Raycast(new Ray(this.rayStarts[i], -vector3), out raycastHit, _distance, this.wallLayers) && raycastHit.distance < _distance)
//            {
//                _distance = raycastHit.distance;
//            }
//        }
//        _distance = _distance + (_nearClipPlane + minDist);
//        if (_distance < minDist * 2f)
//        {
//            _distance = minDist * 2f;
//        }
//        return _distance;
//    }

//    private void IntegrateDirect(float target, ref float pos, float stepTime, int steps, float minSpeed, float spring)
//    {
//        for (int i = 0; i < steps; i++)
//        {
//            pos = Mathf.MoveTowards(pos, target, (minSpeed + Mathf.Abs(spring * (target - pos))) * stepTime);
//        }
//    }

//    private void IntegrateSpring(float target, ref float pos, ref float speed, float stepTime, int steps, float spring, float damper, float maxForce)
//    {
//        for (int i = 0; i < steps; i++)
//        {
//            if (speed * (target - pos) <= 0f)
//            {
//                speed = 0f;
//            }
//            speed = speed + Mathf.Clamp(spring * (target - pos), -maxForce, maxForce) * stepTime;
//            speed = Mathf.MoveTowards(speed, 0f, Mathf.Abs(speed * damper * stepTime));
//            pos = Mathf.MoveTowards(pos, target, Mathf.Abs(speed * stepTime));
//        }
//    }

//    public void LateUpdate()
//    {
//        float single;
//        float single1;
//        float single2;
//        float single3;
//        float single4;
//        float single5;
//        Vector3 vector3;
//        RaycastHit raycastHit = new RaycastHit();
//        bool flag = true;
//        //bool flag = (NetGame.isClient ? false : !ReplayRecorder.isPlaying);
//        switch (this.mode)
//        {
//            case CameraMode.Far:
//                {
//                    this.CalculateFarCam(out vector3, out single, out single1, out single2, out single3, out single4, out single5);
//                    break;
//                }
//            case CameraMode.Close:
//                {
//                    this.CalculateCloseCam(out vector3, out single, out single1, out single2, out single3, out single4, out single5);
//                    break;
//                }
//            case CameraMode.FirstPerson:
//                {
//                    this.CalculateFirstPersonCam(out vector3, out single, out single1, out single2, out single3, out single4, out single5);
//                    break;
//                }
//            default:
//                {
//                    this.CalculateCloseCam(out vector3, out single, out single1, out single2, out single3, out single4, out single5);
//                    break;
//                }
//        }
//        if (CameraController3.fovAdjust != 0f)
//        {
//            single2 = single2 * (Mathf.Tan(0.0174532924f * single4 / 2f) / Mathf.Tan(0.0174532924f * (single4 + CameraController3.fovAdjust) / 2f));
//            single4 += CameraController3.fovAdjust;
//        }
//        single2 /= MenuCameraEffects.instance.cameraZoom;
//        single2 = Mathf.Max(single2, single3);
//        if (MenuCameraEffects.instance.creditsAdjust > 0f)
//        {
//            single1 = Mathf.Lerp(single1, 90f, MenuCameraEffects.instance.creditsAdjust * 0.7f);
//            single2 = single2 + MenuCameraEffects.instance.creditsAdjust * 20f;
//            single4 = Mathf.Lerp(single4, 40f, MenuCameraEffects.instance.creditsAdjust);
//        }
//        Quaternion quaternion = Quaternion.Euler(single1, single, 0f);
//        Vector3 _forward = quaternion * Vector3.forward;
//        Vector3 vector31 = (flag ? this.fixedupdateSmooth : this.SmoothCamera(this.ragdoll.partHead.transform.position ,Time.unscaledDeltaTime)) + vector3;
//        Vector3 _position = this.gameCam.transform.position;
//        single5 *= Mathf.Clamp(_position.magnitude / 500f, 1f, 2f);
//        this.gameCam.nearClipPlane=(single5);
//        this.gameCam.fieldOfView=(single4);
//        float single6 = (this.ignoreWalls ? 10000f : this.CompensateForWallsNearPlane(vector31, quaternion, this.farRange * 1.2f, single3));
//        this.offset = this.SpringArm(this.offset, single2, single6, Time.unscaledDeltaTime);
//        if (single6 < this.offset && !Physics.SphereCast(vector31 - (_forward * this.offset), single5 * 2f, _forward, out raycastHit, this.offset - single6, this.wallLayers, 1))
//        {
//            this.offset = single6;
//            this.offsetSpeed = 0f;
//        }
//        this.ApplyCamera(vector31, vector31 - (_forward * this.offset), quaternion, single4);
//    }

//    private static void OnCameraSmooth(string param)
//    {
//        float single;
//        if (!string.IsNullOrEmpty(param))
//        {
//            param = param.ToLowerInvariant();
//            if (float.TryParse(param, out single))
//            {
//                Options.cameraSmoothing = Mathf.Clamp((int)(single * 20f), 0, 40);
//            }
//            else if ("off".Equals(param))
//            {
//                Options.cameraSmoothing = 0;
//            }
//            else if ("on".Equals(param))
//            {
//                Options.cameraSmoothing = 20;
//            }
//        }
//        Shell.Print(string.Concat("Camera smoothing ", CameraController3.smoothingAmount.ToString()));
//    }

//    private static void OnFOV(string param)
//    {
//        float single;
//        if (!string.IsNullOrEmpty(param) && float.TryParse(param, out single))
//        {
//            Options.cameraFov = Mathf.Clamp((int)(single / 2f + 5f), 0, 20);
//        }
//        Shell.Print(string.Concat("FOV adjust ", CameraController3.fovAdjust.ToString()));
//    }

//    private void OnHDR(string param)
//    {
//        if (string.IsNullOrEmpty(param))
//        {
//            Options.advancedVideoHDR = (Options.advancedVideoHDR > 0 ? 0 : 1);
//        }
//        else
//        {
//            param = param.ToLowerInvariant();
//            if ("off".Equals(param))
//            {
//                Options.advancedVideoHDR = 0;
//            }
//            else if ("on".Equals(param))
//            {
//                Options.advancedVideoHDR = 1;
//            }
//        }
//        Options.ApplyAdvancedVideo();
//        Shell.Print(string.Concat("HDR ", (Options.advancedVideoHDR > 0 ? "on" : "off")));
//    }

//    public void PostSimulate()
//    {
//        if (!base.get_enabled())
//        {
//            return;
//        }
//        if ((NetGame.isClient ? false : !ReplayRecorder.isPlaying))
//        {
//            this.fixedupdateSmooth = this.SmoothCamera(this.ragdoll.partHead.transform.get_position(), Time.get_fixedDeltaTime());
//        }
//    }

//    public void Scroll(Vector3 offset)
//    {
//        this.oldTarget += offset;
//        this.smoothTarget += offset;
//        Transform _transform = base.transform ;
//        _transform.position=(_transform.position + offset);
//        this.fixedupdateSmooth += offset;
//    }

//    public static void SetFov(float v)
//    {
//        CameraController3.fovAdjust = (v * 20f - 5f) * 2f;
//    }

//    public static void SetSmoothing(float v)
//    {
//        CameraController3.smoothingAmount = v;
//    }

//    private Vector3 SmoothCamera(Vector3 target, float deltaTime)
//    {
//        Vector3 vector3;
//        if (ReplayRecorder.isPlaying)
//        {
//            deltaTime = (float)Mathf.Abs(ReplayRecorder.instance.currentFrame - this.oldFrame) * Time.get_fixedDeltaTime();
//            this.oldFrame = ReplayRecorder.instance.currentFrame;
//            if (deltaTime == 0f)
//            {
//                return this.smoothTarget;
//            }
//        }
//        if (NetGame.isClient)
//        {
//            deltaTime = (float)Mathf.Abs(this.human.player.renderedFrame - this.oldFrame) * Time.get_fixedDeltaTime();
//            this.oldFrame = this.human.player.renderedFrame;
//            if (deltaTime == 0f)
//            {
//                return this.smoothTarget;
//            }
//        }
//        int num = Mathf.RoundToInt(deltaTime / (Time.fixedDeltaTime / 10f));
//        if (num < 1)
//        {
//            num = 1;
//        }
//        if (CameraController3.smoothingAmount == 0f || deltaTime == 0f || num > 1000 || (target - this.oldTarget).get_magnitude() > 10f)
//        {
//            deltaTime = 0f;
//            Vector3 vector31 = target;
//            vector3 = vector31;
//            this.oldTarget = vector31;
//            this.smoothTarget = vector3;
//            return target;
//        }
//        float single = deltaTime / (float)num;
//        float single1 = this.offset * 0.1f * CameraController3.smoothingAmount;
//        Vector3 vector32 = (target - this.oldTarget) / deltaTime;
//        for (int i = 0; i < num; i++)
//        {
//            this.oldTarget = this.oldTarget + (vector32 * single);
//            Vector3 vector33 = this.oldTarget + Vector3.ClampMagnitude(this.smoothTarget - this.oldTarget, single1);
//            vector3 = vector33 - this.oldTarget;
//            float single2 = Mathf.SmoothStep(0.05f, 2f, vector3.magnitude / single1);
//            this.smoothTarget = Vector3.MoveTowards(vector33, this.oldTarget, single * single2);
//        }
//        deltaTime = 0f;
//        return this.smoothTarget;
//    }

//    private float SpringArm(float current, float target, float limit, float deltaTime)
//    {
//        int num = Mathf.RoundToInt(deltaTime / (Time.fixedDeltaTime / 10f));
//        if (num < 1)
//        {
//            num = 1;
//        }
//        if (deltaTime == 0f || num > 1000)
//        {
//            return target;
//        }
//        float single = deltaTime / (float)num;
//        if (limit < target)
//        {
//            target = limit;
//            if (target < current)
//            {
//                this.offsetSpeed = 0f;
//                this.IntegrateDirect(target, ref current, single, num, 5f, 10f);
//                return current;
//            }
//        }
//        if (target >= current)
//        {
//            this.IntegrateSpring(target, ref current, ref this.offsetSpeed, single, num, 2f, 1f, 6f);
//        }
//        else
//        {
//            this.IntegrateSpring(target, ref current, ref this.offsetSpeed, single, num, 100f, 10f, 500f);
//        }
//        return current;
//    }

//    private void Start()
//    {
//        this.ragdoll = this.human.ragdoll;

//        Shell.RegisterCommand("smooth", new Action<string>(CameraController3.OnCameraSmooth), "smooth <smoothing>\r\nSet camera smoothing amount (0-none, 0.5-default, 1-max)");
//        Shell.RegisterCommand("fov", new Action<string>(CameraController3.OnFOV), "fov <fov_adjust>\r\nSet FOV adjust (-10 - narrow, 0 - default, 30 - wide)");
//        Shell.RegisterCommand("hdr", new Action<string>(this.OnHDR), "hdr\r\nToggle high dynamic range");
//    }

//    public void TransitionFrom(GameCamera gameCamera, float focusDist, float duration)
//    {
//        if (duration == 0f)
//        {
//            throw new ArgumentException("duration can't be 0", "duration");
//        }
//        this.cameraTransitionPhase = 0f;
//        this.cameraTransitionSpeed = 1f / duration;
//        this.startFov = this.gameCam.fieldOfView;
//        this.startOffset = gameCamera.get_transform().get_position() - this.human.transform.position;
//        this.startRotation = gameCamera.get_transform().get_rotation();
//        this.startHumanCameraPos = gameCamera.gameCam.WorldToViewportPoint(this.human.transform.position);
//    }

//    public void TransitionFromCurrent(float duration)
//    {
//        if (duration == 0f)
//        {
//            throw new ArgumentException("duration can't be 0", "duration");
//        }
//        this.cameraTransitionPhase = 0f;
//        this.cameraTransitionSpeed = 1f / duration;
//        this.startFov = this.gameCam.fieldOfView;
//        this.startOffset = base.transform.position - this.human.transform.position;
//        this.startRotation = base.transform.rotation;
//        this.startHumanCameraPos = this.gameCam.WorldToViewportPoint(this.human.transform.position);
//    }
//}