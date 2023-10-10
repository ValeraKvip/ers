using Unity.Netcode;

public class NetworkCard : NetworkBehaviour
{
    private Card _card;
    private NetworkVariable<NetworkString> _name = new();
    private NetworkVariable<int> _order = new();
    protected Card card
    {
        get
        {
            if (_card == null)
            {
                _card = gameObject.GetComponent<Card>();               
            }
            return _card;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();        

        if (IsClient)
        {
            OnNameChanged("", _name.Value);
            OnOrderChanged(0, _order.Value);
            _name.OnValueChanged += OnNameChanged;
            _order.OnValueChanged += OnOrderChanged;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _name.OnValueChanged -= OnNameChanged;
        _order.OnValueChanged -= OnOrderChanged;

    }
    private void OnNameChanged(NetworkString prev, NetworkString current)
    {       
        gameObject.name = current;
        card.SetupSprite();
    }

    private void OnOrderChanged(int prev, int current)
    {      
        card?.SetDisplayingOrder(current);
    }

    public void Setup(string name, int order)
    {
        gameObject.name = name;     
        if (IsServer)
        {
            _name.Value = name;
            _order.Value = order;       
        }       
    }
}
