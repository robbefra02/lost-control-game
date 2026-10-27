using LostControl.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostControl.View
{
    /// <summary>
    /// The introduction shown before the level starts (D63). Its overlay blocks the game until the player clicks
    /// the start button; only then does the simulation begin to run.
    /// </summary>
    public class IntroView : MonoBehaviour
    {
        [SerializeField] GameController game;
        [SerializeField] InfoTexts texts;
        [Tooltip("Overlay shown before the level starts; it also blocks clicks on the game behind it.")]
        [SerializeField] GameObject overlay;
        [SerializeField] TMP_Text titleLabel;
        [SerializeField] TMP_Text bodyLabel;
        [SerializeField] Button startButton;

        void Start()
        {
            titleLabel.text = texts.introductionTitle;
            bodyLabel.text = texts.introduction;
            overlay.SetActive(!game.Started);
            startButton.onClick.AddListener(OnStart);
        }

        void OnStart()
        {
            overlay.SetActive(false);
            game.StartLevel();
        }
    }
}
