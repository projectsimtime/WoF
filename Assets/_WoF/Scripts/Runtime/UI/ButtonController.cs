using UnityEngine;
using UnityEngine.UI;

namespace WoF.UI
{
	public abstract class ButtonController : MonoBehaviour
	{
		[SerializeField]
		private Button _button;

		protected virtual void OnValidate()
		{
			_button = GetComponent<Button>();
		}

		protected virtual void OnEnable()
		{
			_button.onClick.AddListener(OnButtonClicked);
		}

		protected virtual void OnDisable()
		{
			_button.onClick.RemoveListener(OnButtonClicked);
		}

		protected abstract void OnButtonClicked();

		public void SetButtonInteractable(bool interactable)
		{
			_button.interactable = interactable;
		}
	}
}
