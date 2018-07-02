using UnityEngine;

namespace WyteEngine.Helper
{
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
		/// 加速移動を計算します。
		/// </summary>
		/// <returns>The out.</returns>
		/// <param name="time">Time.</param>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		public static Vector3 EaseIn(float time, Vector3 start, Vector3 end) => new Vector3(EaseIn(time, start.x, end.x), EaseIn(time, start.y, end.y), EaseIn(time, start.z, end.z));


		/// <summary>
		/// 角度を弧度に変換します。
		/// </summary>
		/// <returns>The radian.</returns>
		/// <param name="degree">Degree.</param>
		public static double ToRadian(double degree) => degree / 180 * Mathf.PI;
	}
}