// Optional interface for attacks that have multiple phases, such as charging.
public interface IEnemyAttackState
{
    bool IsAttackInProgress { get; }
}
