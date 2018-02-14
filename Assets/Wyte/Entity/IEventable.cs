/// <summary>
/// イベントの実行をサポートするインターフェイスです．
/// </summary>
public interface IEventable
{
	EventCondition EventWhen { get; }
	string Label { get; }
}

/// <summary>
/// Novel イベントの発生条件。
/// </summary>
public enum EventCondition
{
	/// <summary>
	/// 話しかけた時。
	/// </summary>
	Talked,
	/// <summary>
	/// イベントを実行しません。
	/// </summary>
	None,
	/// <summary>
	/// プレイヤーが触れた時。
	/// </summary>
	Touched,
	/// <summary>
	/// 攻撃された時。
	/// </summary>
	Punched
}