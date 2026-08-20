namespace WoF.Core
{
	/// <summary>
	/// Implemented by scene components that need services handed to them by the
	/// composition root instead of reaching out for them.
	/// </summary>
	public interface IInjectable
	{
		void Inject(IServiceResolver services);
	}
}
