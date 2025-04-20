using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {
    void Interact(); // 所有可互動物件都需實作這個方法
    string GetPromptMessage(); // 提示UI要顯示的文字
}
