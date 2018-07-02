using UnityEngine;

namespace WyteEngine
{

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
}