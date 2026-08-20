using System;
using System.Collections.Generic;

namespace WoF.Core
{
	/// <summary>
	/// Minimal contract-to-instance map. It is deliberately not a service locator:
	/// only the composition root holds a reference to it, everything else receives
	/// its dependencies through <see cref="IInjectable.Inject"/> or a constructor.
	/// </summary>
	public sealed class ServiceContainer : IServiceRegistry, IServiceResolver
	{
		private readonly Dictionary<Type, object> _servicesByContract = new Dictionary<Type, object>();

		public void Register<T>(T service) where T : class
		{
			RegisterAs(typeof(T), service);
		}

		public void RegisterAs(Type contract, object service)
		{
			if (contract == null)
			{
				throw new ArgumentNullException(nameof(contract));
			}

			if (service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}

			if (!contract.IsInstanceOfType(service))
			{
				throw new ArgumentException($"{service.GetType().Name} does not implement {contract.Name}.", nameof(service));
			}

			_servicesByContract[contract] = service;
		}

		/// <summary>
		/// Registers a scene component under every <see cref="ISceneService"/> contract it implements.
		/// Adding a new view is therefore a matter of implementing its contract, not of editing this class.
		/// </summary>
		public void RegisterSceneContracts(ISceneService service)
		{
			if (service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}

			Type markerType = typeof(ISceneService);

			foreach (Type contract in service.GetType().GetInterfaces())
			{
				if (contract == markerType || !markerType.IsAssignableFrom(contract))
				{
					continue;
				}

				RegisterAs(contract, service);
			}
		}

		public T Resolve<T>() where T : class
		{
			if (TryResolve(out T service))
			{
				return service;
			}

			throw new InvalidOperationException($"No service is registered for {typeof(T).Name}.");
		}

		public bool TryResolve<T>(out T service) where T : class
		{
			if (_servicesByContract.TryGetValue(typeof(T), out object registered))
			{
				service = (T)registered;

				return true;
			}

			service = null;

			return false;
		}

		public void Clear()
		{
			_servicesByContract.Clear();
		}
	}
}
