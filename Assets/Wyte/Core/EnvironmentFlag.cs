
/// <summary>
/// 実行環境に依存する機能の実装に役立つフラグを用意しています。
/// </summary>
public static class EnvironmentFlag
{
	/// <summary>
	/// タッチを主なインターフェイスとするデバイスであるかどうか。
	/// </summary>
	public static readonly bool IsSmartDevice =
#if UNITY_IOS || UNITY_ANDROID
		true;
#else
		false;
#endif
}
