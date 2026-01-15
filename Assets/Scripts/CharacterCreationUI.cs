using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterCreationUI : MonoBehaviour
{
    [SerializeField] private InputField _characterNameInput;
    [SerializeField] private Dropdown _classDropdown;
    [SerializeField] private Button _createButton;
    [SerializeField] private Text _statusText;
    [SerializeField] private GameObject _classDescriptionPanel;
    [SerializeField] private Text _classDescriptionText;
    
    private string[] _classDescriptions = new string[]
    {
        "Guerreiro: Alta defesa e HP. Especialista em combate corpo a corpo.",
        "Mago: Alto dano mágico e MP. Fraco em defesa física.",
        "Arqueiro: Dano à distância e velocidade. Equilibrado.",
        "Gatuno: Crítico alto e furtividade. Baixa defesa.",
        "Paladino: Defesa e cura. Suporte e proteção.",
        "Xamã: Suporte mágico e buff. Dano moderado."
    };
    
    private void Start()
    {
        _classDropdown.options.Clear();
        foreach (string className in Config.Classes.AvailableClasses)
        {
            _classDropdown.options.Add(new Dropdown.OptionData(className));
        }
        
        _classDropdown.onValueChanged.AddListener(OnClassSelected);
        _createButton.onClick.AddListener(OnCreateClicked);
        
        CharacterManager.Instance.OnCharacterCreated.AddListener(OnCharacterCreated);
        CharacterManager.Instance.OnError.AddListener(OnError);
        
        OnClassSelected(0);
    }
    
    private void OnClassSelected(int index)
    {
        if (index < _classDescriptions.Length)
        {
            _classDescriptionText.text = _classDescriptions[index];
        }
    }
    
    private void OnCreateClicked()
    {
        if (string.IsNullOrEmpty(_characterNameInput.text))
        {
            _statusText.text = "Digite um nome para seu personagem";
            _statusText.color = Color.red;
            return;
        }
        
        string selectedClass = Config.Classes.AvailableClasses[_classDropdown.value];
        CharacterManager.Instance.CreateCharacter(_characterNameInput.text, selectedClass);
        
        _statusText.text = "Criando personagem...";
        _statusText.color = Color.white;
    }
    
    private void OnCharacterCreated(string message)
    {
        _statusText.text = message;
        _statusText.color = Color.green;
    }
    
    private void OnError(string message)
    {
        _statusText.text = message;
        _statusText.color = Color.red;
    }
}
