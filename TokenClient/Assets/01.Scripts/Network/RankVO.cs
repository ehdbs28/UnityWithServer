using System;
using UnityEngine;

[Serializable]
public class RankVO : Payload
{
    public string user_name;
    public int score;
    public string memo;
    
    public string GetJsonString()
    {
        return JsonUtility.ToJson(this);
    }

    public string GetQueryString()
    {
        return null;
    }

    public WWWForm GetWWWForm()
    {
        WWWForm form = new WWWForm();
        form.AddField("user_name", user_name);
        form.AddField("score", score);
        form.AddField("memo", memo);
        return form;
    }
}
