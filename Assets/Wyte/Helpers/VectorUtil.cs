using UnityEngine;
namespace WyteEngine.Helper
{
	/// <summary>
	/// ベクター関連の便利メソッドを用意します．
	/// </summary>
	public static class VectorUtil
	{
		/// <summary>
		/// 自分と相手の位置を指定して，自分から見て相手を向くベクトルを取得します．
		/// </summary>
		/// <param name="vec1"></param>
		/// <param name="vec2"></param>
		/// <param name="speed">実数倍する値．デフォルト値は<c>1</c>．</param>
		/// <returns></returns>
		public static Vector3 GetTargetingVelocity(this Vector3 vec1, Vector3 vec2, float speed = 1)
		{
			// 相手と自分のベクトルの差分を取り，正規化し，スピードを掛ける．
			return (vec2 - vec1).normalized * speed;
		}
	}
}