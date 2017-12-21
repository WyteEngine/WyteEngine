using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfectCamera : MonoBehaviour
{

	public Vector2 displayArea = new Vector2(288, 512);
	public float referencePixelsPerUnit = 100f;
	private Vector2 lastScreenSize;

	void Awake()
	{
		UpdateSize();
	}


	/// <summary>
	/// ドット絵が綺麗に表示されるように Orthographic なカメラの Size を調整する.
	/// </summary>
	void UpdateSize()
	{
		var widthCoeff = (int)(Screen.width / displayArea.x);
		var heightCoeff = (int)(Screen.height / displayArea.y);
		// displayArea の幅と高さのどちらに合わせるかを決める.
		var coefficient = 1;
		if (widthCoeff < heightCoeff)
		{
			// 幅に合わせる場合.
			coefficient = widthCoeff;
		}
		else
		{
			// 高さに合わせる場合.
			coefficient = heightCoeff;
		}

		// Orthographic なカメラの Size を、ドット絵が綺麗に表示されるように調整する.
		GetComponent<Camera>().orthographicSize = Screen.height / (referencePixelsPerUnit * coefficient * 2);   // orthographicSizeは高さの半分なので2で割る.

		// 解像度の変化を感知するために現在の解像度を保存しておく.
		lastScreenSize.x = Screen.width;
		lastScreenSize.y = Screen.height;
	}

	void Update()
	{
		// 解像度が変化したら調整し直す.
		if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
		{
			UpdateSize();
		}

	}
}
