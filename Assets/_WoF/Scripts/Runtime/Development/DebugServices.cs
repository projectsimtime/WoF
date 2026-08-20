#if UNITY_EDITOR || DEVELOPMENT_BUILD
using WoF.Core;

namespace WoF.Development
{
	/// <summary>
	/// A hand-off point for development tooling that lives outside the scene and therefore
	/// cannot be injected. It is compiled out of release builds, so gameplay code has no way
	/// to reach for services instead of being handed them.
	/// </summary>
	public static class DebugServices
	{
		public static IServiceResolver Current { get; private set; }

		public static void Publish(IServiceResolver services)
		{
			Current = services;
		}

		public static void Clear()
		{
			Current = null;
		}
	}
}
#endif
