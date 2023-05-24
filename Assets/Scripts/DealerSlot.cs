using UnityEngine.UI;

public class DealerSlot : GenericSlot
{
    private Dealer _dealer;

    private void Start()
    {
        _dealer = Manager.Instance.dealer;

        gameObject.GetComponent<Button>().onClick.AddListener(delegate { _dealer.SelectItem(Attached_Item); });
    }
}