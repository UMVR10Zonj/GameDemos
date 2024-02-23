public class PlayerFSM : StateManager<PlayerFSM.EState>
{
    public enum EState { Spawn, Idle, Move, Jump, Dead, Transporting, UnCtrlable, ReSpawnable }

    private void Awake()
    {
        States[EState.Spawn] = new PlayerSpawnState();
        States[EState.Idle] = new PlayerIdleState();
        States[EState.Move] = new PlayerMoveState();
        States[EState.Jump] = new PlayerJumpState();
        States[EState.Dead] = new PlayerDeadState();
        States[EState.Transporting] = new PlayerTransportingState();
        States[EState.ReSpawnable] = new PlayerReSpawnableState();
        States[EState.UnCtrlable] = new PlayerUnCtrlableState();

        Init(States[EState.Idle]);
    }
}

public class PlayerSpawnState : BaseState<PlayerFSM.EState>
{
    public PlayerSpawnState() => stateId = PlayerFSM.EState.Spawn;
}
public class PlayerIdleState : BaseState<PlayerFSM.EState>
{
    public PlayerIdleState() => stateId = PlayerFSM.EState.Idle;
}
public class PlayerMoveState : BaseState<PlayerFSM.EState>
{
    public PlayerMoveState() => stateId = PlayerFSM.EState.Move;
}
public class PlayerJumpState : BaseState<PlayerFSM.EState>
{
    public PlayerJumpState() => stateId = PlayerFSM.EState.Jump;
}
public class PlayerDeadState : BaseState<PlayerFSM.EState>
{
    public PlayerDeadState() => stateId = PlayerFSM.EState.Dead;
}
public class PlayerTransportingState : BaseState<PlayerFSM.EState>
{
    public PlayerTransportingState() => stateId = PlayerFSM.EState.Transporting;
}
public class PlayerUnCtrlableState : BaseState<PlayerFSM.EState>
{
    public PlayerUnCtrlableState() => stateId = PlayerFSM.EState.UnCtrlable;
}
public class PlayerReSpawnableState : BaseState<PlayerFSM.EState>
{
    public PlayerReSpawnableState() => stateId = PlayerFSM.EState.ReSpawnable;
}
