using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WoF
{
	public class ButtonController : MonoBehaviour
	{
		[SerializeField] 
		private Button _button;
		[SerializeField] 
		private TextMeshProUGUI _buttonText;
		[SerializeField]
		protected GameSession _gameSession;
		
		public Button Button => _button;

		protected virtual void OnValidate()
		{
			_button = GetComponent<Button>();
			_buttonText = GetComponentInChildren<TextMeshProUGUI>(true);

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

		public void SetButtonText(string text)
		{
			_buttonText.text = text;
		}
	}
}