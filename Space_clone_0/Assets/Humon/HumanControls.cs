
using System;
using System.Collections.Generic;
using UnityEngine;
public enum VerticalLookMode
{
    FollowStick,
    Relative
}
public enum MouseLookMode
{
    Classic,
    SpringBackNoGrab
}
public class HumanControls : MonoBehaviour
{
    public bool allowMouse;

    public SteelSeriesHFF steelSeriesHFF;

    private Human humanScript;

    private Game gameScript;

   
    public static bool freezeMouse;

    private Vector2 keyLookCache;

    public float leftExtend;

    public float rightExtend;

    private float leftExtendCache;

    private float rightExtendCache;

    public bool leftGrab;

    public bool rightGrab;

    public bool unconscious;

    public bool holding;

    public bool jump;

    public bool shootingFirework;

    public VerticalLookMode verticalLookMode;

    public MouseLookMode mouseLookMode;

    public bool mouseControl = true;

    public float cameraPitchAngle;

    public float cameraYawAngle;

    public float targetPitchAngle;

    public float targetYawAngle;

    public Vector3 walkLocalDirection;

    public Vector3 walkDirection;

    public float unsmoothedWalkSpeed;

    public float walkSpeed;

    private List<float> mouseInputX = new List<float>();

    private List<float> mouseInputY = new List<float>();

    private Vector3 stickDirection;

    private Vector3 oldStickDirection;

    private float previousLeftExtend;

    private float previousRightExtend;

    private float shootCooldown;
    public Vector2 calc_joyLook
    {
        get
        {
            return new Vector2(Input.GetAxis("Joystick Look Horizontal"), Input.GetAxis("Joystick Look Vertical"));
        }
    }

    public Vector3 calc_joyWalk
    {
        get
        {
            return new Vector3(Input.GetAxis("Joystick Walk Horizontal"), 0f, Input.GetAxis("Joystick Walk Vertical"));
        }
    }
    public Vector2 calc_keyLook
    {
        get
        {
            if (!this.allowMouse)
            {
                return Vector2.zero;
            }
            if (!HumanControls.freezeMouse)
            {
                float axis = Input.GetAxis("Mouse Look Horizontal");
                this.keyLookCache = new Vector2(axis, Input.GetAxis("Mouse Look Vertical"));
            }
            return this.keyLookCache;
        }
    }

    public Vector3 calc_keyWalk
    {
        get
        {
            if (!this.allowMouse)
            {
                return Vector3.zero;
            }
          
            return new Vector3(Input.GetAxis("Keyboard Walk Horizontal"), 0f, Input.GetAxis("Keyboard Walk Vertical"));
        }
    }

    public float lookYnormalized
    {
        get
        {
            float axis = Input.GetAxis("Joystick Look Vertical");
            return Mathf.Sign(axis) * Mathf.Pow(Mathf.Abs(axis), 1f);
        }
    }

    public float vScale
    {
        get
        {
            return 1f;
        }
    }

    public HumanControls()
    {
    }

    public bool ControllerFireworksPressed()
    {
        return false;
    }

    public bool ControllerJumpPressed()
    {
        return false;
    }

    private float FilterAxisAcceleration(float currentValue, float desiredValue)
    {
        float _fixedDeltaTime = Time.fixedDeltaTime / 1f;
        float single = 0.2f;
        if (currentValue * desiredValue <= 0f)
        {
            currentValue = 0f;
        }
        if (Mathf.Abs(currentValue) > Mathf.Abs(desiredValue))
        {
            currentValue = desiredValue;
        }
        if (Mathf.Abs(currentValue) < single)
        {
            _fixedDeltaTime = Mathf.Max(_fixedDeltaTime, single - Mathf.Abs(currentValue));
        }
        if (Mathf.Abs(currentValue) > 0.8f)
        {
            _fixedDeltaTime /= 3f;
        }
        return Mathf.MoveTowards(currentValue, desiredValue, _fixedDeltaTime);
    }

    private bool GetFireworks()
    {
        return Input.GetAxis("Shoot Fireworks") > 0.01f;
    }

    private bool GetJump()
    {
        return Input.GetAxis("Jump") > 0.01f;
    }

    private float GetLeftExtend()
    {
        return Input.GetAxis("Left Grab");
    }

    private float GetRightExtend()
    {
        return Input.GetAxis("Right Grab");
    }

    private bool GetUnconscious()
    {
        return Input.GetAxis("Unconscious") > 0.01f;
    }

    public void HandleInput(float walkForward, float walkRight, float cameraPitch, float cameraYaw, float leftExtend, float rightExtend, bool jump, bool playDead, bool holding, bool shooting)
    {
        this.walkLocalDirection = new Vector3(walkRight, 0f, walkForward);
        this.cameraPitchAngle = cameraPitch;
        this.cameraYawAngle = cameraYaw;
        this.leftExtend = leftExtend;
        this.rightExtend = rightExtend;
        this.leftGrab = leftExtend > 0f;
        this.rightGrab = rightExtend > 0f;

        this.jump = jump;
        this.unconscious = playDead;
        this.holding = holding;
        this.shootingFirework = shooting;
        this.targetPitchAngle = Mathf.MoveTowards(this.targetPitchAngle, this.cameraPitchAngle, 180f * Time.fixedDeltaTime / 0.1f);
        this.targetYawAngle = this.cameraYawAngle;
        Vector3 vector3 = Quaternion.Euler(0f, this.cameraYawAngle, 0f) * this.walkLocalDirection;
        this.unsmoothedWalkSpeed = vector3.magnitude;
        vector3 = new Vector3(this.FilterAxisAcceleration(this.oldStickDirection.x, vector3.x), 0f, this.FilterAxisAcceleration(this.oldStickDirection.z, vector3.z));
        this.walkSpeed = vector3.magnitude;
        if (this.walkSpeed > 0f)
        {
            this.walkDirection = vector3;
        }
        this.oldStickDirection = vector3;
    }

    private void OnEnable()
    {
       this.SetAny();
        //this.verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
        //this.mouseLookMode = (MouseLookMode)Options.mouseLookMode;
         this.humanScript = base.transform.Find("Ball").GetComponent<Human>();
        // this.gameScript = GameObject.Find("Game(Clone)").GetComponent<Game>();
    }

    public void ReadInput(out float walkForward, out float walkRight, out float cameraPitch, out float cameraYaw, out float leftExtend, out float rightExtend, out bool jump, out bool playDead, out bool shooting)
    {
        bool flag;
        Vector2 calcJoyLook = this.calc_joyLook;
        Vector2 calcKeyLook = this.calc_keyLook;
        Vector3 calcJoyWalk = this.calc_joyWalk;
        Vector3 calcKeyWalk = this.calc_keyWalk;
        if (calcJoyLook.sqrMagnitude > calcKeyLook.sqrMagnitude)
        {
            this.mouseControl = false;
        }
        if (calcKeyLook.sqrMagnitude > calcJoyLook.sqrMagnitude)
        {
            this.mouseControl = true;
        }
        if (calcJoyWalk.sqrMagnitude > calcKeyWalk.sqrMagnitude)
        {
            this.mouseControl = false;
        }
        if (calcKeyWalk.sqrMagnitude > calcJoyWalk.sqrMagnitude)
        {
            this.mouseControl = true;
        }
        cameraPitch = this.cameraPitchAngle;
        cameraYaw = this.cameraYawAngle;
        if (!this.mouseControl)
        {
            cameraYaw = cameraYaw + calcJoyLook.x * Time.deltaTime * 120f;
            if (this.verticalLookMode != VerticalLookMode.Relative)
            {
                float single = -80f * this.lookYnormalized;
                flag = (single * cameraPitch < 0f ? true : Mathf.Abs(single) > Mathf.Abs(cameraPitch));
                float single1 = (this.leftGrab || this.rightGrab ? Mathf.Abs(this.lookYnormalized) : 1f);
                float single2 = (flag ? single1 : (this.leftGrab || this.rightGrab ? 0.0125f : 0.25f));
                cameraPitch = Mathf.Lerp(cameraPitch, single, single2 * 5f * Time.fixedDeltaTime * this.vScale);
                cameraPitch = Mathf.MoveTowards(cameraPitch, single, single2 * 30f * Time.fixedDeltaTime * this.vScale);
            }
            else
            {
                //if (Options.controllerInvert > 0)
                //{
                //    calcJoyLook.y = -calcJoyLook.y;
                //}
                //cameraPitch = cameraPitch - calcJoyLook.y * Time.get_deltaTime() * 120f * 2f;
                //cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
            }
        }
        else
        {
            //cameraYaw += Smoothing.SmoothValue(this.mouseInputX, calcKeyLook.x);
            //cameraPitch -= Smoothing.SmoothValue(this.mouseInputY, calcKeyLook.y);
            cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
            if (this.mouseLookMode == MouseLookMode.SpringBackNoGrab && !this.leftGrab && !this.rightGrab)
            {
                int num = 0;
                float single3 = 0.25f;
                cameraPitch = Mathf.Lerp(cameraPitch, (float)num, single3 * 5f * Time.fixedDeltaTime);
                cameraPitch = Mathf.MoveTowards(cameraPitch, (float)num, single3 * 30f * Time.fixedDeltaTime);
            }
        }

        Vector3 vector3 = (calcJoyWalk.sqrMagnitude > calcKeyWalk.sqrMagnitude ? calcJoyWalk : calcKeyWalk);
        walkForward = vector3.z;
        walkRight = vector3.x;
        //if (MenuSystem.keyboardState != KeyboardState.None)
        //{
        //    leftExtend = this.previousLeftExtend;
        //    rightExtend = this.previousRightExtend;
        //    jump = false;
        //    playDead = false;
        //    shooting = false;
        //    return;
        //}
        leftExtend = this.GetLeftExtend();
        rightExtend = this.GetRightExtend();
        jump = this.GetJump();
        playDead = this.GetUnconscious();
        this.previousLeftExtend = leftExtend;
        this.previousRightExtend = rightExtend;
        shooting = this.GetFireworks();
    }

    public void SetAny()
    {
        //this.verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
       // this.mouseLookMode = (MouseLookMode)Options.mouseLookMode;
        this.allowMouse = true;
    }

    public void SetController()
    {
        //this.verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
       // this.mouseLookMode = (MouseLookMode)Options.mouseLookMode;
        this.allowMouse = false;
    }

    //    public void SetMouse()
    //    {
    //        this.verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
    //        this.mouseLookMode = (MouseLookMode)Options.mouseLookMode;
    //        this.allowMouse = true;
    //    }
    //}
}