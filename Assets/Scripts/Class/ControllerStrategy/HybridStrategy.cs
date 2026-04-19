using EmotivUnityPlugin;

public class HybridStrategy : IControlStrategy
{
    public void ReadInput(InputState state, BCIInputHandler context)
    {
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
    }
}
