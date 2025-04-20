using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable {
    // Start is called before the first frame update\
    public Transform chestLid;//寶相蓋子
    public List<GameObject> ChestVFX;// 金幣特效的 prefab
    public bool isOpened = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    public void Interact() {
        if (isOpened) return;
        isOpened = true;
        Debug.Log("寶箱打開");
        if (chestLid != null) {
            chestLid.localRotation = Quaternion.Euler(-120f, 0f, 0f);
            foreach (var vfxPrefab in ChestVFX) {
                if (vfxPrefab != null) {
                    Instantiate(vfxPrefab, chestLid.position + Vector3.up * 0.5f, Quaternion.identity);
                    //金幣特效會生成在ChestLid的position上方
                }
            }
        }
        FirestoreManager.Instance.GetChestGold();

    }

    public string GetPromptMessage() {
        return isOpened ? "" : "按 F 開啟寶箱";
    }
}
