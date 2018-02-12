using UnityEngine;
using System.Collections;

public class CoroutineHelper : BaseBehaviour
{
	protected static CoroutineHelper instance;
	public static CoroutineHelper Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<CoroutineHelper>();
				// それでもなおないようなら
				if (instance == null)
				{
					instance = new GameObject("CoroutineHelper", typeof(CoroutineHelper)).GetComponent<CoroutineHelper>();
				}
			}
			return instance;
		}
	}

	protected void Awake()
	{
		CheckInstance();
	}

	protected void Start()
	{
		
	}

	/// <summary>
	/// Start the specified coroutine.
	/// </summary>
	/// <returns>The start.</returns>
	/// <param name="coroutine">Coroutine.</param>
	public Coroutine Start(IEnumerator coroutine) => StartCoroutine(coroutine);

	public Coroutine StartSerially(params IEnumerator[] coroutines) => StartCoroutine(Combine(coroutines));

	public void StartParallelly(params IEnumerator[] coroutines)
	{
		foreach (var coroutine in coroutines)
		{
			StartCoroutine(coroutine);
		}
	}


	public void Stop(IEnumerator coroutine) => StopCoroutine(coroutine);

	public IEnumerator Combine(params IEnumerator[] coroutines) => coroutines.GetEnumerator();


	/// <summary>
	/// インスタンスが存在するか判定し、あれば設定します。
	/// </summary>
	/// <returns>存在すればtrue、しなければfalseを返します。</returns>
	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = (CoroutineHelper)this;
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
