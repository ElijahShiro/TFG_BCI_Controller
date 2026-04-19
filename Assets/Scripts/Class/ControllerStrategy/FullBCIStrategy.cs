using EmotivUnityPlugin;
using UnityEngine;

public class FullBCIStrategy : IControlStrategy
{
    private Vector2 _lastHeadJoystick = Vector2.zero;
    private bool _isCalibrated = false;
    private float _pitchOffset = 0f;
    private float _rollOffset = 0f;

    public void ReadInput(InputState state, BCIInputHandler context)
    {
        state.IsUsingHeadMovement = true;


        if (EmotivUnityItf.Instance != null && EmotivUnityItf.Instance.IsProfileLoaded && EmotivUnityItf.Instance.LatestMentalCommand != null)
        {
            state.Action = EmotivUnityItf.Instance.LatestMentalCommand.act.ToLower();
            if (state.Action == "lift")
            {
                state.Action = "jump";
            }
            state.Power = (float)EmotivUnityItf.Instance.LatestMentalCommand.pow;
            if (state.Action == "jump" && state.Power >= context.mentalPowerThreshhold) state.IsJumpTriggered = true;
        }


        if (EmotivUnityItf.Instance.GetNumberMotionSamples() > 0)
        {
            double[] accZData = EmotivUnityItf.Instance.GetMotionData(Channel_t.CHAN_ACCZ);
            double[] accYData = EmotivUnityItf.Instance.GetMotionData(Channel_t.CHAN_ACCY);

            if (accZData != null && accZData.Length > 0 && accYData != null && accYData.Length > 0)
            {
                float rawPitch = (float)accYData[accYData.Length - 1];
                float rawRoll = (float)accZData[accZData.Length - 1];

                if (!_isCalibrated || Input.GetKeyDown(KeyCode.C))
                {
                    _pitchOffset = rawPitch;
                    _rollOffset = rawRoll;
                    _isCalibrated = true;
                }

                float headPitch = rawPitch - _pitchOffset;
                float headRoll = rawRoll - _rollOffset;

                Vector2 newJoystick = Vector2.zero;
                if (Mathf.Abs(headPitch) > context.gyroDeadzone) newJoystick.y = Mathf.Clamp(-headPitch * context.gyroSensitivity, -1f, 1f);
                if (Mathf.Abs(headRoll) > context.gyroDeadzone) newJoystick.x = Mathf.Clamp(headRoll * context.gyroSensitivity, -1f, 1f);

                _lastHeadJoystick = newJoystick;
            }
        }
        state.HeadMove = _lastHeadJoystick;
    }
}
