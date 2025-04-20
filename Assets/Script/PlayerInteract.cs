using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class PlayerInteract : MonoBehaviour {
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public KeyCode interactKey = KeyCode.F;
    public GameObject InteractpromptUI;
    private IInteractable currentTarget;
   

    void Update() {
        HandleInteraction();

    }

    void HandleInteraction() {
        currentTarget = null;
        InteractpromptUI.SetActive(false);

        IInteractable[] all = GameObject.FindObjectsOfType<MonoBehaviour>(true)
    .OfType<IInteractable>()
    .ToArray();

        foreach (var target in all) {
            MonoBehaviour mb = target as MonoBehaviour;
            if (mb == null) continue;
            Debug.Log(mb.gameObject.name);
            GameObject go = mb.gameObject;

            // Layer 條件
            if (((1 << go.layer) & interactLayer.value) == 0)
                continue;

            // 距離條件
            float dist = Vector3.Distance(transform.position, go.transform.position);
            if (dist < interactDistance) {
                currentTarget = target;
                InteractpromptUI.SetActive(true);

                break; // 找到一個就跳出
            }
        }

        // 處理互動鍵輸入
        if (currentTarget != null && Input.GetKeyDown(interactKey)) {
            currentTarget.Interact();
        }
    }
}
