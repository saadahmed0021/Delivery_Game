using TMPro;
using UnityEngine;

public class Bike_Repair_Station : MonoBehaviour
{
    [Header("Repair")]
    [SerializeField, Min(1)] private int repairCost = 500;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("References")]
    [SerializeField] private Currency_Transaction currencyTransaction;
    [SerializeField] private TMP_Text interactionPrompt;

    private Order_Damage activeBikeDamage;

    private void Awake()
    {
        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (activeBikeDamage == null)
            return;

        if (Input.GetKeyDown(interactionKey))
            TryRepair();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!TryGetPlayerBike(other, out Order_Damage bikeDamage))
            return;

        activeBikeDamage = bikeDamage;
        ShowPrompt("Press E to Repair");
        SetPromptActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (activeBikeDamage == null)
            return;

        if (other.GetComponentInParent<Order_Damage>() == activeBikeDamage)
            ClearActiveBike();
    }

    private void TryRepair()
    {
        if (currencyTransaction == null)
        {
            ShowPrompt("Currency system is not assigned");
            return;
        }

        if (activeBikeDamage == null)
            return;

        if (!activeBikeDamage.IsDamaged)
        {
            ShowPrompt("Bike is already repaired");
            return;
        }

        if (!currencyTransaction.CanAfford(repairCost))
        {
            ShowPrompt("Not Enough Currency");
            return;
        }

        if (!currencyTransaction.Spend(repairCost))
        {
            ShowPrompt("Not Enough Currency");
            return;
        }

        activeBikeDamage.ResetDamage();
        ShowPrompt("Bike repaired successfully");
    }

    private bool TryGetPlayerBike(
        Collider other,
        out Order_Damage bikeDamage
    )
    {
        bikeDamage = null;

        if (other == null)
            return false;

        Transform bike = other.transform;
        Bike_Input bikeInput = bike.GetComponentInParent<Bike_Input>();
        Order_Damage damage = bike.GetComponentInParent<Order_Damage>();

        if (bikeInput == null || damage == null)
            return false;

        bikeDamage = damage;
        return true;
    }

    private void ShowPrompt(string message)
    {
        if (interactionPrompt != null)
            interactionPrompt.text = message;
    }

    private void SetPromptActive(bool isActive)
    {
        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(isActive);
    }

    private void ClearActiveBike()
    {
        activeBikeDamage = null;
        SetPromptActive(false);
    }
}
