using UnityEngine;

public enum ControllerType { KeyBoard, GamePad }

public class InputHandler
{
    public float GetAxisH(ControllerType CTRLerType)
    {
        switch (CTRLerType)
        {
            case ControllerType.KeyBoard:
                return Input.GetAxisRaw(ControlKey.k_Hori);
            case ControllerType.GamePad:
                return Input.GetAxisRaw(ControlKey.p_Hori);
            default:
                return 0;
        }
    }

    public bool GetJumpBtnDown(ControllerType CTRLerType)
    {
        switch (CTRLerType)
        {
            case ControllerType.KeyBoard:
                return Input.GetButtonDown(ControlKey.k_Jump);
            case ControllerType.GamePad:
                return Input.GetButtonDown(ControlKey.p_Jump);
            default:
                return false;
        }
    }
}
