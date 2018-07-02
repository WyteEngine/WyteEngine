using System.IO;
using UnityEngine;

namespace WyteEngine.Helper
{
	/// <summary>
	/// お好みの名前で、 .NET オブジェクトをセーブデータとして格納し、読み込むことができます。
	/// </summary>
	public static class SaveDataHelper
	{
		public static void Save(string relativePath, object data) => File.WriteAllText(Combine(relativePath), JsonUtility.ToJson(data));

		public static T Load<T>(string relativePath) => File.Exists(Combine(relativePath)) ? JsonUtility.FromJson<T>(File.ReadAllText(Combine(relativePath))) : default(T);

		private static string Combine(string relativePath) => Path.Combine(Application.persistentDataPath, relativePath);
	}
}