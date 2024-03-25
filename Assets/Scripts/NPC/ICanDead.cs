/// <summary>
/// 死亡フラグを持つ
/// </summary>
public interface ICanDead
{
    /// <summary> 死ぬか </summary>
    public bool IsDead { get; set; }
}