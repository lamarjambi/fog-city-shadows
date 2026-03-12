using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    private UIDocument _document;

    private Button _button;

    private Button _homeButton;
    private Button _retryButton;

    private List<Button> _menuButtons = new List<Button>();

    private void Awake() {
        _document = GetComponent<UIDocument>();

        var root = _document.rootVisualElement;

        _homeButton = root.Q<Button>("Home");
        _retryButton = root.Q<Button>("Retry");

        _homeButton?.RegisterCallback<ClickEvent>(OnHometButtonClick);
        _retryButton?.RegisterCallback<ClickEvent>(OnRetrysButtonClick);
    }

    private void OnDisable() {
        _homeButton?.UnregisterCallback<ClickEvent>(OnHometButtonClick);
        _retryButton?.UnregisterCallback<ClickEvent>(OnRetrysButtonClick);
    }

    private void OnHometButtonClick(ClickEvent evt) {
        SceneManager.LoadScene("MenuScene");
    }

    private void OnRetrysButtonClick(ClickEvent evt) {
        SceneManager.LoadScene("GameScene");
    }
}
