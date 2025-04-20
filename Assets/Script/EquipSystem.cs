using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;

public class EquipSystem : MonoBehaviour {
    [SerializeField] Transform weaponHolder;//手持時,武器應該在的地方(手)
    [SerializeField] GameObject weapon;//武器本人
    [SerializeField] Transform weaponSheath;//腰間配戴時,武器應該在的地方(腰)

    GameObject currentWeaponInHand;//代表手上那把
    GameObject currentWeaponInSheath;//代表腰上那把
                                     // Start is called before the first frame update
    /* IEnumerator Start() {
         yield return StartCoroutine(FirestoreManager.Instance.DownloadPlayerData());
         int weaponValue = PlayerPrefs.GetInt("Weapon",0);
         if (weapon == null || weaponSheath == null || weaponValue !=1) {//初始化檢查
             Debug.LogError("Weapon or WeaponSheath is not assigned.");
             yield break;
         }
         //進場先用Instantiate生成一把weapon在weaponSheath的transform上
         currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
         if (currentWeaponInSheath != null) {//狀態檢查
             Debug.Log("Weapon instantiated in sheath.");
         } else {
             Debug.LogError("Failed to instantiate weapon in sheath.");
         }
     }*/

    // Update is called once per frame
    void Start() {
        int weaponValue = PlayerPrefs.GetInt("Weapon", 0);
        if (weapon == null || weaponSheath == null) {//初始化檢查
            Debug.LogError("Weapon or WeaponSheath is not assigned.");
            return;
        }
        //進場先用Instantiate生成一把weapon在weaponSheath的transform上
        currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
        if (currentWeaponInSheath != null) {//狀態檢查
            Debug.Log("Weapon instantiated in sheath.");
        } else {
            Debug.LogError("Failed to instantiate weapon in sheath.");
        }
    }
    void Update() {

    }

    public void DrawWeapon() {//拿起武器方法
        if (currentWeaponInHand == null && currentWeaponInSheath != null) {
            currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
            Destroy(currentWeaponInSheath);//destroy會在下一禎執行
            currentWeaponInSheath = null;//但為了防止Dangling Reference 直接設為null
            //這裡邏輯比較難解釋 但就是為了防止重複引用一個已經該被消失的對象
        }
    }

    public void SheathWeapon() {//收武器方法
        if (currentWeaponInSheath == null && currentWeaponInHand != null) {
            currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
            Destroy(currentWeaponInHand);
            currentWeaponInHand = null;//理論同上
        }
    }

}