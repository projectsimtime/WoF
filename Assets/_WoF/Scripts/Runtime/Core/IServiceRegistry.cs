using System;

namespace WoF.Core
{
	/// <summary>
	/// Write side of the container. Only the composition root depends on this.
	/// </summary>
	public interface IServiceRegistry
	{
		void Register<T>(T service) where T : class;

		void RegisterAs(Type contract, object service);

		void RegisterSceneContracts(ISceneService service);
	}
}
