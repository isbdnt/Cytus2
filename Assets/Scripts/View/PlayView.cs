using UnityEngine;
using UnityEngine.UI;

namespace Cytus2
{
    public class PlayView : MonoBehaviour
    {
        private Text _comboText;
        private Text _pointText;
        private Button _startButton;
        private Button _pauseButton;
        private Button _restartButton;
        private Button _reloadConfigButton;

        private void Awake()
        {
            _startButton = transform.Find("Start").GetComponent<Button>();
            _pauseButton = transform.Find("Pause").GetComponent<Button>();
            _restartButton = transform.Find("Restart").GetComponent<Button>();
            _reloadConfigButton = transform.Find("ReloadConfig").GetComponent<Button>();
            _comboText = transform.Find("Combo").GetComponent<Text>();
            _pointText = transform.Find("Point").GetComponent<Text>();
        }

        private void Start()
        {
            _startButton.onClick.AddListener(HandleStartClick);
            _pauseButton.onClick.AddListener(HandlePauseClick);
            _restartButton.onClick.AddListener(HandleRestartClick);
            _reloadConfigButton.onClick.AddListener(HandleReloadConfigClick);
            BindGridEvents();
            HandleGridPause();
        }

        private void BindGridEvents()
        {
            GridView.instance.onPointChange += HandleGridPointChange;
            GridView.instance.onComboChange += HandleGridComboChange;
            GridView.instance.onStart += HandleGridPlay;
            GridView.instance.onContinue += HandleGridPlay;
            GridView.instance.onPause += HandleGridPause;
        }

        private void HandleStartClick()
        {
            GridView.instance.StartGame();
        }

        private void HandlePauseClick()
        {
            GridView.instance.PauseGame();
        }

        private void HandleRestartClick()
        {
            GridView.instance.RestartGame();
        }

        private void HandleReloadConfigClick()
        {
            ResourceManager.NewInstance();
            GridView.NewInstance();
            BindGridEvents();
            HandleGridPause();
            GridView.instance.StartGame();
        }

        private void HandleGridPlay()
        {
            _startButton.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(true);
            HandleGridPointChange(0);
            HandleGridComboChange(0);
        }

        private void HandleGridContinue()
        {
            _startButton.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(true);
        }

        private void HandleGridPause()
        {
            _startButton.gameObject.SetActive(true);
            _pauseButton.gameObject.SetActive(false);
        }

        private void HandleGridPointChange(int point)
        {
            _pointText.text = point.ToString();
        }

        private void HandleGridComboChange(int combo)
        {
            _comboText.text = combo.ToString();
        }
    }
}