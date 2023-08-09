using UnityEngine;

public interface Payload
{
    public string GetJsonString();
    public string GetQueryString();
    public WWWForm GetWWWForm();
}
