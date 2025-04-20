using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;

public class FirestoreManager : MonoBehaviour {
    public static FirestoreManager Instance;

    [Header("Firebase project ID")]
    public string projectId = "your-project-id";

    [Header("玩家 ID")]
    public string playerId = "default-player";

    private string baseUrl;

    //"https://firebasestorage.googleapis.com/v0/b/test-98e14.firebasestorage.app/sword.png?alt=media"
    void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start() {
        baseUrl = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/";
        StartCoroutine(CheckPlayerExist());
        StartCoroutine(CreateShopMenu());
        StartCoroutine(CreateMapItem());
    }
    IEnumerator CheckPlayerExist() {
        string url = baseUrl + "Players/" + playerId;

        // 先嘗試 GET 文件
        UnityWebRequest getRequest = UnityWebRequest.Get(url);
        yield return getRequest.SendWebRequest();
        Debug.Log(getRequest.url);
        if (getRequest.result == UnityWebRequest.Result.Success) {
            Debug.Log("玩家已存在，略過建立。直接抓玩家數據下來");
            StartCoroutine(DownloadPlayerData());
        } else if (getRequest.responseCode == 404) {
            Debug.Log("玩家不存在，建立新玩家...");
            StartCoroutine(UploadPlayerData());
        } else {
            Debug.LogError($"檢查玩家失敗: {getRequest.error}");
        }
    }



    IEnumerator CreateShopMenu() {
        string url = baseUrl + "Shop/Weapon";
        UnityWebRequest getRequest = UnityWebRequest.Get(url);
        yield return getRequest.SendWebRequest();

        if (getRequest.result == UnityWebRequest.Result.Success) {
            Debug.Log("商店資料已存在，略過建立。");
            yield break; // 中斷，不再建立
        } else if (getRequest.responseCode == 404) {
            Debug.Log("商店資料不存在，建立預設資料...");


            string json = @"{
            ""fields"": {
                ""Title"": { ""stringValue"": ""Basic Sword"" },
                ""Description"": { ""stringValue"": ""Sword for you."" },
                ""Price"": { ""integerValue"": ""100"" }
            }
        }";

            yield return StartCoroutine(SendFirestorePatchRequest(
                url,
                json,
                () => Debug.Log("商店預設資料成功建立"),
                (err) => Debug.LogError("建立商店資料失敗：" + err)
            ));
        } else {
            Debug.LogError($"取得商店資料失敗: {getRequest.error}");
            Debug.LogError($"回應內容: {getRequest.downloadHandler.text}");
        }
    }

    IEnumerator CreateMapItem() {
        string url = baseUrl + "MapItem/Chest";
        UnityWebRequest getRequest = UnityWebRequest.Get(url);
        yield return getRequest.SendWebRequest();

        if (getRequest.result == UnityWebRequest.Result.Success) {
            Debug.Log("地圖物件資料已存在，略過建立。");
            yield break; // 中斷，不再建立
        } else if (getRequest.responseCode == 404) {
            Debug.Log("地圖物件資料不存在，建立預設資料...");


            string json = @"{
            ""fields"": {

                ""GoldValue"": { ""integerValue"": ""999"" }
            }
        }";

            yield return StartCoroutine(SendFirestorePatchRequest(
                url,
                json,
                () => Debug.Log("商店預設資料成功建立"),
                (err) => Debug.LogError("建立商店資料失敗：" + err)
            ));
        } else {
            Debug.LogError($"取得商店資料失敗: {getRequest.error}");
            Debug.LogError($"回應內容: {getRequest.downloadHandler.text}");
        }
    }


    public void GetGoldData(Action<int> onSuccess) {
        StartCoroutine(GetGoldDataCoroutine(onSuccess));
    }

    IEnumerator GetGoldDataCoroutine(Action<int> onSuccess) {
        string url = baseUrl + "MapItem/Chest";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            var node = JSON.Parse(request.downloadHandler.text);
            int gold = node["fields"]["GoldValue"]["integerValue"].AsInt;
            onSuccess?.Invoke(gold);
        } else {
            Debug.LogError("Failed to get gold: " + request.error);
        }
    }


    public IEnumerator DownloadPlayerData(System.Action onComplete = null) {
        string url = baseUrl + "Players/" + playerId;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            PlayerPrefStatManager.LoadFromFirestoreJson(request.downloadHandler.text);
            Debug.Log("讀取完成");
            onComplete?.Invoke();
        } else {
            Debug.LogError("讀取失敗: " + request.error);
        }
    }


    public IEnumerator UploadPlayerData() {
        string json = PlayerPrefStatManager.ToFirestoreJson();
        string url = baseUrl + "Players/" + playerId;

        yield return StartCoroutine(SendFirestorePatchRequest(
            url, json,
            () => Debug.Log("上傳成功"),
            (err) => Debug.LogError("上傳失敗：" + err)
        ));
    }



    public void GetChestGold() {
        GetGoldData((gold) => {
            
            int coin = PlayerPrefs.GetInt("playerCoin", 0);
            PlayerPrefs.SetInt("playerCoin", coin + gold);
            PlayerPrefs.Save();
            StartCoroutine(UploadPlayerData());
        });
    }

    public IEnumerator LoadShopData(System.Action<JSONArray> onSuccess) {
        string url = baseUrl + "Shop";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            JSONNode node = JSON.Parse(request.downloadHandler.text);
            JSONArray documents = node["documents"].AsArray;
            onSuccess?.Invoke(documents);
        } else {
            Debug.LogError("讀取商店資料失敗：" + request.error);
        }
    }

    IEnumerator SendFirestorePatchRequest(string url, string json, System.Action onSuccess = null, System.Action<string> onError = null) {
        UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            Debug.Log($"PATCH 請求成功：{url}");
            onSuccess?.Invoke();
        } else {
            Debug.LogError($"PATCH 請求失敗：{request.error}");
            Debug.LogError($"回應內容：{request.downloadHandler.text}");
            onError?.Invoke(request.error);
        }
    }

    public IEnumerator LoadImageToSprite(string url, Image targetImage) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
            targetImage.sprite = sprite;
        } else {
            Debug.LogError("圖片載入失敗：" + request.error);
        }
    }



    public string GetItemIdFromFirestorePath(string path) {
        var parts = path.Split('/');
        return parts[^1]; // 取最後一段就是 documentId（例如 "Weapon"）
    }


}






