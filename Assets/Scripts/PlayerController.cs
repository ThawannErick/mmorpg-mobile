using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterData _characterData;
    private VirtualJoystick _joystick;
    private Rigidbody _rigidbody;
    private Vector3 _moveDirection = Vector3.zero;
    private float _moveSpeed = 5f;
    private float _updatePositionTimer = 0f;
    private float _updatePositionInterval = 0.5f;
    
    private void Start()
    {
        _characterData = CharacterManager.Instance.GetCurrentCharacter();
        _joystick = FindObjectOfType<VirtualJoystick>();
        _rigidbody = GetComponent<Rigidbody>();
        
        if (_characterData != null)
        {
            transform.position = new Vector3(_characterData.x, _characterData.y, _characterData.z);
        }
        
        if (_joystick != null)
        {
            _joystick.OnJoystickMove.AddListener(OnJoystickMove);
        }
    }
    
    private void FixedUpdate()
    {
        if (_rigidbody != null)
        {
            _rigidbody.velocity = _moveDirection * _moveSpeed;
        }
        else
        {
            transform.Translate(_moveDirection * _moveSpeed * Time.fixedDeltaTime);
        }
        
        _updatePositionTimer += Time.fixedDeltaTime;
        if (_updatePositionTimer >= _updatePositionInterval)
        {
            SendPositionUpdate();
            _updatePositionTimer = 0f;
        }
    }
    
    private void OnJoystickMove(Vector2 direction)
    {
        _moveDirection = new Vector3(direction.x, 0, direction.y);
        
        if (_moveDirection.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(_moveDirection);
        }
    }
    
    private void SendPositionUpdate()
    {
        if (!NetworkClient.Instance.IsConnected || _characterData == null)
            return;
        
        UpdatePositionRequest request = new UpdatePositionRequest
        {
            type = "update_position",
            payload = new UpdatePositionPayload
            {
                character_id = _characterData.id,
                x = transform.position.x,
                y = transform.position.y,
                z = transform.position.z,
                map_id = _characterData.map_id
            }
        };
        
        string json = JsonUtility.ToJson(request);
        NetworkClient.Instance.SendMessage(json);
    }
}

[System.Serializable]
public class UpdatePositionRequest
{
    public string type;
    public UpdatePositionPayload payload;
}

[System.Serializable]
public class UpdatePositionPayload
{
    public int character_id;
    public float x;
    public float y;
    public float z;
    public int map_id;
}
