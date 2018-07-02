using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WyteEngine.Module
{
	public class WyteModuleLoader : SingletonBaseBehaviour<WyteModuleLoader>
	{
		List<WyteModuleBase> modules;

		protected override void Awake()
		{
			base.Awake();

			modules = typeof(WyteModuleLoader).Assembly.GetExportedTypes().Where(t => t.GetCustomAttributes<WyteModuleAttribute>().Any()).Select(Activator.CreateInstance).OfType<WyteModuleBase>().ToList();
			modules.Select(m => m.GetType().Name).ToList().ForEach(Debug.Log);
		}

		public T GetModule<T>() where T : WyteModuleBase
		{
			T module;

			if (!TryGetModule(out module))
				throw new ArgumentException("そのモジュールは見つかりませんでした．");

			return module;
		}

		public bool TryGetModule<T>(out T module) where T : WyteModuleBase
		{
			module = modules.OfType<T>().FirstOrDefault();
			return module != default(T);
		}

	}
}