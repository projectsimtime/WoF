using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoF
{
	public class ButtonController : MonoBehaviour
	{
		[SerializeField] 
		private Button _button;
		[SerializeField]
		protected GameSession _gameSession;
		
		public Button Button => _button;

		private void OnValidate()
		{
			_button = GetComponent<Button>();
			_gameSession = FindObjectOfType<GameSession>();
		}

		protected virtual void OnEnable()
		{
			_button.onClick.AddListener(OnButtonClicked);
		}

		protected virtual void OnDisable()
		{
			_button.onClick.RemoveAllListeners();
		}

		protected virtual void OnButtonClicked() {}

		public void SetButtonInteractable(bool interactable)
		{
			_button.interactable = interactable;
		}
	}
}