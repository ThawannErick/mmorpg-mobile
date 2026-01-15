using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private Text _characterNameText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _mpText;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _mpSlider;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Button _questButton;
    [SerializeField] private Button _characterButton;
    [SerializeField] private Button _settingsButton;
    
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _questPanel;
    [SerializeField] private GameObject _characterPanel;
    [SerializeField] private GameObject _settingsPanel;
    
    private CharacterData _characterData;
    
    private void Start()
    {
        _characterData = CharacterManager.Instance.GetCurrentCharacter();
        
        if (_characterData != null)
        {
            UpdateHUD();
        }
        
        _inventoryButton.onClick.AddListener(() => TogglePanel(_inventoryPanel));
        _questButton.onClick.AddListener(() => TogglePanel(_questPanel));
        _characterButton.onClick.AddListener(() => TogglePanel(_characterPanel));
        _settingsButton.onClick.AddListener(() => TogglePanel(_settingsPanel));
    }
    
    private void UpdateHUD()
    {
        _characterNameText.text = _characterData.name;
        _levelText.text = $"Nível {_characterData.level}";
        _hpText.text = $"{_characterData.hp} HP";
        _mpText.text = $"{_characterData.mp} MP";
        
        _hpSlider.value = (float)_characterData.hp / 100f;
        _mpSlider.value = (float)_characterData.mp / 100f;
    }
    
    private void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
    
    public void UpdateCharacterStats(CharacterData data)
    {
        _characterData = data;
        UpdateHUD();
    }
}
