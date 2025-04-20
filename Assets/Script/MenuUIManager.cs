using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP;

public class MenuUIManager : MonoBehaviour
{
    public ButtonManager coinDisplayButton;
    public NotificationManager notificationPrefab;
    public Transform notificationParent;
    // Start is called before the first frame update
    void Start()
    {
        
        UpdateCoinText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCoinText();
    }

    public void UpdateCoinText() {
        int coin = PlayerPrefs.GetInt("playerCoin", 0);
        if (coinDisplayButton != null) {
            coinDisplayButton.SetText($"{coin}");
        }
    }

    public void ShowNotification(string title, string message) {
        if (notificationPrefab == null || notificationParent == null) {
            Debug.Log(" prefab 或 parent 未指定");
            return;
        }

        NotificationManager notify = Instantiate(notificationPrefab, notificationParent);
        notify.title = title;
        notify.description = message;
        notify.UpdateUI();
        notify.Open();
    }

    public void OnPurchaseCompleted() {
        UpdateCoinText();
    }
}
