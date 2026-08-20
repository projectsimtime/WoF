namespace WoF.Core
{
	/// <summary>
	/// Read side of the container. Consumers depend on this, never on the container itself.
	/// </summary>
	public interface IServiceResolver
	{
		T Resolve<T>() where T : class;

		bool TryResolve<T>(out T service) where T : class;
	}
}
