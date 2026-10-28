using LostControl.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostControl.View
{
    /// <summary>
    /// The introduction shown before the level starts (D63). Its overlay blocks the game until the player clicks
    /// the start button; only then does the simulation begin to run. Under a divider the player can type a minigame
    /// seed, or tick "Random", which greys the seed box out (D66); it starts ticked. A random seed is written into the
    /// box, so it can be replayed after a restart by unticking.
    /// </summary>
    public class IntroView : MonoBehaviour
    {
        // Remembered across restarts (scene reloads) during one run of the game.
        static string lastSeedText;
        static bool lastRandomSeed = true;

        [SerializeField] GameController game;
        [SerializeField] InfoTexts texts;
        [Tooltip("Overlay shown before the level starts; it also blocks clicks on the game behind it.")]
        [SerializeField] GameObject overlay;
        [SerializeField] TMP_Text titleLabel;
        [SerializeField] TMP_Text bodyLabel;
        [SerializeField] Button startButton;
        [SerializeField] TMP_InputField seedInput;
        [Tooltip("Faded while the random seed is on.")]
        [SerializeField] CanvasGroup seedInputGroup;
        [SerializeField] Toggle randomSeedToggle;
        [Range(0f, 1f)]
        [SerializeField] float disabledAlpha = 0.35f;

        void Start()
        {
            titleLabel.text = texts.introductionTitle;
            bodyLabel.text = texts.introduction;
            overlay.SetActive(!game.Started);

            seedInput.text = lastSeedText ?? game.Config.minigameSeed.ToString();
            randomSeedToggle.isOn = lastRandomSeed;
            randomSeedToggle.onValueChanged.AddListener(_ => UpdateSeedInput());
            UpdateSeedInput();
            startButton.onClick.AddListener(OnStart);
        }

        void UpdateSeedInput()
        {
            bool random = randomSeedToggle.isOn;
            seedInput.interactable = !random;
            seedInputGroup.alpha = random ? disabledAlpha : 1f;
        }

        void OnStart()
        {
            int seed;
            if (randomSeedToggle.isOn) seed = Random.Range(1, 1000000);
            else if (!int.TryParse(seedInput.text, out seed)) seed = game.Config.minigameSeed;

            seedInput.text = seed.ToString();
            lastSeedText = seedInput.text;
            lastRandomSeed = randomSeedToggle.isOn;
            overlay.SetActive(false);
            game.StartLevel(seed);
        }
    }
}
