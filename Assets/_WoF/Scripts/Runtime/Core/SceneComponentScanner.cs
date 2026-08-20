using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace WoF.Core
{
	/// <summary>
	/// Collects the components of a scene that publish or consume services, including the ones
	/// sitting on inactive panels. This is the single place in the project that searches the
	/// scene: it runs once, at start-up, in the composition root, instead of once per component.
	/// </summary>
	public static class SceneComponentScanner
	{
		public static List<T> FindAll<T>(Scene scene) where T : class
		{
			List<T> found = new List<T>();

			MonoBehaviour[] sceneComponents = Object.FindObjectsOfType<MonoBehaviour>(true);

			foreach (MonoBehaviour sceneComponent in sceneComponents)
			{
				if (sceneComponent is T candidate && sceneComponent.gameObject.scene == scene)
				{
					found.Add(candidate);
				}
			}

			return found;
		}
	}
}
