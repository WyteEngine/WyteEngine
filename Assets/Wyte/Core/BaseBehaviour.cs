using UnityEngine;
using Novel.Exceptions;
using WyteEngine.Inputing;
using WyteEngine.Sfx;
using WyteEngine.Music;
using WyteEngine.Event;
using WyteEngine.Map;
using WyteEngine.UI;
using WyteEngine.Graphics;
using WyteEngine.Item;
using WyteEngine.Entities;
using WyteEngine.UI.TextFormatting;
using WyteEngine.I18n;

namespace WyteEngine
{

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
		protected CoroutineHelper Coroutine => CoroutineHelper.Instance;
		protected MapManager Map => MapManager.Instance;
		protected PlayerCamera Camera => PlayerCamera.Instance;
		protected FPSCounter Debugger => FPSCounter.Instance;
		protected NpcManager Npc => NpcManager.Instance;
		protected AnimationManager AnimMan => AnimationManager.Instance;
		protected ItemManager ItemMan => ItemManager.Instance;
		protected TextManager TextMan => TextManager.Instance;
		protected TileAPI Tile => TileAPI.Instance;
		protected BossGaugeBinder BossGauge => BossGaugeBinder.Instance;
		protected I18nProvider I18n => I18nProvider.Instance;

		/// <summary>
		/// タッチパネルをサポートしているかどうか。
		/// </summary>
		protected bool IsSmartDevice => EnvironmentFlag.IsSmartDevice;

		public bool IsPostInitialized { get; set; }

		/// <summary>
		/// 条件式が通らない場合エラーを返します．
		/// </summary>
		/// <param name="expr"></param>
		protected void NArgsAssert(bool expr, int? line = null)
		{
			if (!expr)
				throw new NRuntimeException((line != null ? line + "番目の" : "") + "引数が一致しません．");
		}

		protected virtual void Update()
		{
			if (!IsPostInitialized)
			{
				PostStart();
				IsPostInitialized = true;
			}
		}

		protected virtual void PostStart() { }
	}
}