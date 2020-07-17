using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusObserver : MonoBehaviour
{

	public GameObject _healthBar;
	private Slider _healthBarSlider;

	public PlayerController _playerController;
	

	private void OnEnable()
	{
		_playerController.TriggerUIUpdate += UpdateUI;

	}

	private void OnDisable()
	{
		_playerController.TriggerUIUpdate -= UpdateUI;
	}

	private void Start()
	{
		_healthBarSlider = _healthBar.GetComponent<Slider>();
	}

	private void UpdateUI()
	{
		_healthBarSlider.normalizedValue = _playerController._currentHealth / _playerController._maxHealth;
	}

}
