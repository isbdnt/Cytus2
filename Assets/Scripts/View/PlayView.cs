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

            _startButton.gameObject.SetActive(true);
            _startButton.onClick.AddListener(HandleStartClick);
            _pauseButton.gameObject.SetActive(false);
            _pauseButton.onClick.AddListener(HandlePauseClick);
            _restartButton.onClick.AddListener(HandleRestartClick);
            _reloadConfigButton.onClick.AddListener(HandleReloadConfigClick);
        }

        private void Start()
        {
            MakeButtonsReasonable();
            BindGridEvents();
        }

        private void HandleStartClick()
        {
            GridView.instance.StartGame();
            MakeButtonsReasonable();
        }

        private void HandlePauseClick()
        {
            GridView.instance.PauseGame();
            MakeButtonsReasonable();
        }

        private void HandleRestartClick()
        {
            GridView.instance.RestartGame();
            MakeButtonsReasonable();
            BindGridEvents();
        }

        private void HandleReloadConfigClick()
        {
            Main.TestOneWayLove();
            MakeButtonsReasonable();
            BindGridEvents();
        }

        private void MakeButtonsReasonable()
        {
            if (GridView.instance.playing)
            {
                _startButton.gameObject.SetActive(false);
                _pauseButton.gameObject.SetActive(true);
            }
            else
            {
                _startButton.gameObject.SetActive(true);
                _pauseButton.gameObject.SetActive(false);
            }
        }

        private void BindGridEvents()
        {
            GridView.instance.grid.onPointChange += HandleGridPointChange;
            GridView.instance.grid.onComboChange += HandleGridComboChange;
            HandleGridPointChange(0);
            HandleGridComboChange(0);
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