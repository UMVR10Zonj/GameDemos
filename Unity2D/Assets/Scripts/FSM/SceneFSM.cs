public class SceneFSM : StateManager<SceneFSM.EState>
{
    public enum EState { Ttitle, Gaming, Settle }

    private void Awake()
    {
        States[EState.Ttitle] = new TtitleState();
        States[EState.Gaming] = new GamingState();
        States[EState.Settle] = new SettleState();

        Init(States[EState.Ttitle]);
    }
}

public class TtitleState : BaseState<SceneFSM.EState>
{
    public TtitleState() => stateId = SceneFSM.EState.Ttitle;
}
public class GamingState : BaseState<SceneFSM.EState>
{
    public GamingState() => stateId = SceneFSM.EState.Gaming;
}
public class SettleState : BaseState<SceneFSM.EState>
{
    public SettleState() => stateId = SceneFSM.EState.Settle;
}
