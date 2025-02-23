// 1. State System (for state-based effects)
public interface IState
{
    void Enter(IStateHolder target);
    void Exit(IStateHolder target);
    void Update(float deltaTime);
    bool CanEnter(IStateHolder target);
    StateType Type { get; }
}

public interface IStateHolder
{
    bool HasState(StateType type);
    bool TryGetState<T>(out T state) where T : IState;
    void AddState(IState state);
    void RemoveState(StateType type);
}

// 2. Example of a State-based Effect
public class FreezeEffect : BaseEffect
{
    private readonly float m_Duration;
    
    protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
    {
        if (target.TryGetComponent<IStateHolder>(out var stateHolder))
        {
            // Delegate to state system for freeze behavior
            var state = new FrozenState(m_Duration);
            stateHolder.AddState(state);
        }
    }
}

// 3. Example of an Effect with its own logic
public class SplitEffect : BaseEffect
{
    // Keeps its existing complex logic for splitting
    // Doesn't need to use the state system since it has its own behavior
    protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
    {
        // Existing split logic...
    }
}

// 4. Example State Implementation
public class FrozenState : IState
{
    private readonly float m_Duration;
    private float m_ElapsedTime;
    private IStateHolder m_CurrentTarget;
    
    public StateType Type => StateType.Frozen;
    
    public FrozenState(float duration)
    {
        m_Duration = duration;
    }
    
    public void Enter(IStateHolder target)
    {
        m_CurrentTarget = target;
        m_ElapsedTime = 0;
        
        // Apply frozen state effects
        if (target is MonsterMine monster)
        {
            monster.IsMovementAllowed = false;
            monster.IsActionAllowed = false;
        }
    }
    
    public void Exit(IStateHolder target)
    {
        if (target is MonsterMine monster)
        {
            monster.IsMovementAllowed = true;
            monster.IsActionAllowed = true;
        }
    }
    
    public void Update(float deltaTime)
    {
        m_ElapsedTime += deltaTime;
        if (m_ElapsedTime >= m_Duration)
        {
            m_CurrentTarget.RemoveState(Type);
        }
    }
    
    public bool CanEnter(IStateHolder target) => !target.HasState(StateType.Frozen);
}

// 5. Modified MonsterMine
public class MonsterMine : IDamagingMine, IStateHolder
{
    private readonly Dictionary<StateType, IState> m_States = new();
    
    // State-related properties that states can modify
    public bool IsMovementAllowed { get; set; } = true;
    public bool IsActionAllowed { get; set; } = true;
    
    public bool HasState(StateType type) => m_States.ContainsKey(type);
    
    public bool TryGetState<T>(out T state) where T : IState
    {
        if (m_States.TryGetValue(typeof(T), out var existingState) && existingState is T typedState)
        {
            state = typedState;
            return true;
        }
        state = default;
        return false;
    }
    
    public void AddState(IState state)
    {
        if (state.CanEnter(this))
        {
            if (m_States.TryGetValue(state.Type, out var existingState))
            {
                existingState.Exit(this);
            }
            m_States[state.Type] = state;
            state.Enter(this);
        }
    }
    
    public void RemoveState(StateType type)
    {
        if (m_States.TryGetValue(type, out var state))
        {
            state.Exit(this);
            m_States.Remove(type);
        }
    }
    
    public void Update(float deltaTime)
    {
        // Update all active states
        foreach (var state in m_States.Values.ToList())
        {
            state.Update(deltaTime);
        }
        
        // Normal monster update logic, affected by states
        if (IsMovementAllowed)
        {
            // Movement logic...
        }
        if (IsActionAllowed)
        {
            // Action logic...
        }
    }
    
    public int CalculateDamage()
    {
        if (!IsActionAllowed) return 0;
        
        var damage = m_BaseDamage;
        if (HasState(StateType.Enraged))
        {
            damage = Mathf.RoundToInt(damage * m_EnrageDamageMultiplier);
        }
        return damage;
    }
}