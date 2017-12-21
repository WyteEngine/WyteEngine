using System.IO;
using UnityEngine;

/// <summary>
/// お好みの名前で、 .NET オブジェクトをセーブデータとして格納し、読み込むことができます。
/// </summary>
public static class SaveDataHelper
{
	public static void Save(string relativePath, object data) => File.WriteAllText(Combine(relativePath), JsonUtility.ToJson(data));

	public static T Load<T>(string relativePath) => File.Exists(Combine(relativePath)) ? JsonUtility.FromJson<T>(File.ReadAllText(Combine(relativePath))) : default(T);

	private static string Combine(string relativePath) => Path.Combine(Application.persistentDataPath, relativePath);
}

/// <summary>
/// リソース読み込みなど、ゲーム起動時に利用可能になるまで時間がかかる処理を行うオブジェクトはこれを実装します。
/// </summary>
public interface ILoadable
{
	/// <summary>
	/// 必要なリソースの読み込みが完了したかどうかを取得します。
	/// </summary>
	/// <value><c>true</c> if ready; otherwise, <c>false</c>.</value>
	bool Ready { get; }
}

/// <summary>
/// Wyte Engine によるカスタムされた MonoBehaviour。
/// </summary>
public abstract class BaseBehaviour : MonoBehaviour
{
	protected Keys KeyBind => KeyBinding.Instance.Binding;
	protected SfxManager Sfx => SfxManager.Instance;
	protected MusicManager Bgm => MusicManager.Instance;
	protected FlagManager Flag => FlagManager.Instance;
	protected GameMaster Wyte => GameMaster.Instance;
	protected EventController Novel => EventController.Instance;

	/// <summary>
	/// タッチパネルをサポートしているかどうか。
	/// </summary>
	protected bool IsSmartDevice => EnvironmentFlag.IsSmartDevice;


	/// <summary>
	/// BGM を終了します。
	/// </summary>
	public void BgmStop()
	{
		var bgmplayer = GameObject.FindGameObjectWithTag("Bgm");
		if (!bgmplayer) return;

		var audio = bgmplayer.GetComponent<AudioSource>();
		if (!audio) return;

		audio.Stop();
	}

}

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

/// <summary>
/// Singleton base behaviour.
/// </summary>
public abstract class SingletonBaseBehaviour<T> : BaseBehaviour where T : SingletonBaseBehaviour<T>
{
	protected static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<T>();

				if (instance == null)
				{
					Debug.LogWarning(typeof(T) + "is nothing");
				}
			}

			return instance;
		}
	}

	/// <summary>
	/// 内部処理です。シングルトンオブジェクトが生成されているか判定し、反映します。
	/// </summary>
	protected virtual void Awake()
	{
		CheckInstance();
	}

	/// <summary>
	/// インスタンスが存在するか判定し、あれば設定します。
	/// </summary>
	/// <returns>存在すればtrue、しなければfalseを返します。</returns>
	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = (T)this;
			return true;
		}
		else if (Instance == this)
		{
			return true;
		}

		Destroy(this);
		return false;
	}
}

/// <summary>
/// 補完移動などをサポートする機能を持ちます。
/// </summary>
public static class MathHelper
{
	/// <summary>
	/// 加減速移動を計算します。
	/// </summary>
	/// <returns>The in out.</returns>
	/// <param name="time">Time.</param>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public static float EaseInOut(float time, float start, float end) =>
			(time /= 0.5f) < 1
			? (end - start) * 0.5f * time * time * time + start
			: (end - start) * 0.5f * ((time -= 2) * time * time + 2) + start;

	/// <summary>
	/// 加速移動を計算します。
	/// </summary>
	/// <returns>The in.</returns>
	/// <param name="time">Time.</param>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public static float EaseIn(float time, float start, float end) => (end - start) * time * time * time + start;

	/// <summary>
	/// 減速移動を計算します。
	/// </summary>
	/// <returns>The out.</returns>
	/// <param name="time">Time.</param>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public static float EaseOut(float time, float start, float end) => (end - start) * (--time * time * time + 1) + start;

	/// <summary>
	/// 減速移動を計算します。
	/// </summary>
	/// <returns>The out.</returns>
	/// <param name="time">Time.</param>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public static Vector3 EaseOut(float time, Vector3 start, Vector3 end) => new Vector3(EaseOut(time, start.x, end.x), EaseOut(time, start.y, end.y), EaseOut(time, start.z, end.z));

	/// <summary>
	/// 角度を弧度に変換します。
	/// </summary>
	/// <returns>The radian.</returns>
	/// <param name="degree">Degree.</param>
	public static double ToRadian(double degree) => degree / 180 * Mathf.PI;
}
