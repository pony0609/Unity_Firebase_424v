using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public static class PlayerPrefStatManager {
    //擴充改這
    public static readonly string[] IntKeys = { "playerCoin", "Weapon"};



    public static string ToFirestoreJson() {
        var fields = new JSONObject();

        foreach (string key in IntKeys) {
            int value = PlayerPrefs.GetInt(key, 0);
            fields[key] = new JSONObject { ["integerValue"] = value.ToString() };
        }


        return new JSONObject { ["fields"] = fields }.ToString();
    }


    public static void LoadFromFirestoreJson(string json) {
        var node = JSON.Parse(json);
        var fields = node["fields"];

        foreach (string key in IntKeys) {
            if (fields[key] != null && fields[key]["integerValue"] != null) {
                int value = fields[key]["integerValue"].AsInt;
                PlayerPrefs.SetInt(key, value);
            }
        }


        PlayerPrefs.Save();
    }
}