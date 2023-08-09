using UnityEngine;

public class LoginDTO : Payload
{
    public string email;
    public string password;
    
    public string GetJsonString()
    {
        return JsonUtility.ToJson(this);
    }

    public string GetQueryString()
    {
        return $"?email={email}&password={password}";
    }

    public WWWForm GetWWWForm()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        return form;
    }
}