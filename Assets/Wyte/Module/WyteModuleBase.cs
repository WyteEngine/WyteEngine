using System;
namespace WyteEngine.Module
{
	public abstract class WyteModuleBase
	{
		/// <summary>
		/// 全てのモジュールが完全に初期化されるよりも前に呼ばれます．システムの読み込みと同等か，それ以前に行うべき初期化処理を記述します．
		/// </summary>
		public abstract void OnPreInit();
		/// <summary>
		/// 基本システムが完全に読み込まれた後に呼ばれます．システムに対する登録処理などや，メインの初期化処理を記述します．スクリプト向けのコマンドはこのメソッドで登録してください．
		/// </summary>
		public abstract void OnInit();
		/// <summary>
		/// 全てのモジュールが完全に初期化された後に呼ばれます．他のモジュールへの登録処理などを記述します．
		/// </summary>
		public abstract void OnPostInit();
	}
}