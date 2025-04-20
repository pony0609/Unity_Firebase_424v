using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Michsky.MUIP;
using UnityEditor.ShaderGraph;
using UnityEngine.UI;

public class ShopList : MonoBehaviour {

    public GameObject shopItemPrefab;       // 商店項目 Prefab
    public Transform shopListContainer;     // 項目生成的父物件
    // Start is called before the first frame update

    //private string SwordIconUrl = https://firebasestorage.googleapis.com/v0/b/test-98e14.firebasestorage.app/o/sword.png?alt=media&token=10ec0c13-d467-4e94-b33e-3cc367b5aa64
    void Start() {
        StartCoroutine(FirestoreManager.Instance.LoadShopData(OnShopDataLoaded)); // 重新載入 Firestore 資料
    }

    void OnShopDataLoaded(JSONArray documents) {
        // 清空
        foreach (Transform child in shopListContainer) {
            Destroy(child.gameObject);
        }

        foreach (JSONNode doc in documents) {
            string itemId = FirestoreManager.Instance.GetItemIdFromFirestorePath(doc["name"]); // 抓 document id
            string title = doc["fields"]["Title"]["stringValue"];
            string description = doc["fields"]["Description"]["stringValue"];
            int price = doc["fields"]["Price"]["integerValue"].AsInt;
            // string iconUrl = doc["fields"]["Icon"]["stringValue"];
            Debug.Log($"載入商店物品 [Shop Item] Title: {title} | Description: {description} | Price: {price}");

            GameObject item = Instantiate(shopItemPrefab, shopListContainer);

            var titleText = item.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            var descText = item.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
            var priceButton = item.transform.Find("Price")?.GetComponent<ButtonManager>();
            // var iconImage = item.transform.Find("Icon")?.GetComponent<Image>();

            if (titleText != null) titleText.text = title;
            if (descText != null) descText.text = description;

            if (priceButton != null) {
                priceButton.SetText($"{price}");
                int coin = PlayerPrefs.GetInt("playerCoin", 0);
                bool alreadyBought = PlayerPrefs.GetInt(itemId, 0) == 1;
                // 是否可以互動
                priceButton.Interactable(!alreadyBought && coin >= price);

                priceButton.onClick.RemoveAllListeners();

                if (!alreadyBought && coin >= price) {
                    priceButton.onClick.AddListener(() => {
                        BuyItem(price, itemId, priceButton);
                    });
                } else if (alreadyBought) {
                    priceButton.SetText("Own");
                }
            }

            /* if (!string.IsNullOrEmpty(iconUrl) && iconImage != null) {
                StartCoroutine(FirestoreManager.Instance.LoadImageToSprite(iconUrl, iconImage));
            }*/


        }
    }

    void BuyItem(int price, string itemId, ButtonManager button) {
        int coin = PlayerPrefs.GetInt("playerCoin", 0);

        if (coin < price) {
            // 顯示通知
            FindObjectOfType<MenuUIManager>()?.ShowNotification("Fail", "No coin");
           
            return;
        }

        PlayerPrefs.SetInt("playerCoin", coin - price);
        PlayerPrefs.SetInt(itemId, 1);
        PlayerPrefs.Save();

        // 更新 UI
        button.Interactable(false);
        button.SetText("已購買");
        FindObjectOfType<MenuUIManager>()?.OnPurchaseCompleted();
        // 上傳資料
        StartCoroutine(FirestoreManager.Instance.UploadPlayerData());
        FindObjectOfType<MenuUIManager>()?.ShowNotification("Success", $"Bought {itemId}");
        // 顯示通知
    }

    public void CloseWindow() {
        gameObject.SetActive(false);
    }
}

