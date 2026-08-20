namespace WoF.Core
{
	/// <summary>
	/// Marks a contract that a scene component publishes to the composition root.
	/// Every interface deriving from this marker is registered automatically, so a view
	/// never has to be looked up with a scene-wide search.
	/// </summary>
	public interface ISceneService
	{
	}
}
