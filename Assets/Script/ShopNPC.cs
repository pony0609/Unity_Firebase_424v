using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable {
    // Start is called before the first frame update
    public bool isOpened = false;
    public GameObject ShopWindow;
    private GameObject currentShopWindowInstance;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact() {

        Debug.Log("商店頁面打開");
        if (currentShopWindowInstance == null) {
            // 實例化 UI 到canvas中
            GameObject canvas = GameObject.Find("Canvas"); 
            currentShopWindowInstance = Instantiate(ShopWindow, canvas.transform);
        } else {
           
            currentShopWindowInstance.SetActive(true);
        }
    }

    public string GetPromptMessage() {
        return isOpened ? "" : "按 F 開啟寶箱";
    }


}
