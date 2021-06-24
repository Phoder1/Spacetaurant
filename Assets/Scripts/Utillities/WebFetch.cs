using DataSaving;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class WebFetch
{
    const int timeout = 30;

    private static bool IsError(this UnityWebRequest webReq)
        => webReq.result == UnityWebRequest.Result.ConnectionError
            || webReq.result == UnityWebRequest.Result.ProtocolError
            || webReq.result == UnityWebRequest.Result.DataProcessingError;
    public static IEnumerator ConnectToAPI(string api, Action<string> callback)
    {

        UnityWebRequest webReq = new UnityWebRequest
        {
            downloadHandler = new DownloadHandlerBuffer(),

            // build the url and query
            url = api
        };

        yield return webReq.SendWebRequest();

        if (webReq.IsError())
        {
            Debug.Log(webReq.error);
        }
        else
        {

            callback?.Invoke(Encoding.UTF8.GetString(webReq.downloadHandler.data));

        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="callback">
    /// Call the callback action on finish with the values: (success, json, error)
    /// </param>
    /// <returns></returns>
    public static IEnumerator HttpGet<T>
        (string uri, Action<HttpResponse<T>> onSuccessCallback = null, Action<HttpResponse<T>> onFailureCallback = null) 
        where T : class
    {
        using (UnityWebRequest webReq = UnityWebRequest.Get(uri))
        {
            webReq.timeout = timeout;
            yield return webReq.SendWebRequest();

            if (webReq.IsError())
            {
                Debug.Log("Error");
                onFailureCallback?.Invoke(new HttpResponse<T>(false, default, webReq.error));
            }
            else
            {
                string data = Encoding.UTF8.GetString(webReq.downloadHandler.data);

                if (JsonParser.TryParseJson(data, out T body))
                    onSuccessCallback?.Invoke(new HttpResponse<T>(true, body, ""));
                else
                    onFailureCallback?.Invoke(new HttpResponse<T>(false, body, "Failed to parse json."));
            }
        }
    }

    private const string defaultContentType = "application/json";
    public static IEnumerator HttpPost(string uri, string jsonBody, Action<string> callback = null)
    {

        using (UnityWebRequest webReq = UnityWebRequest.Get(uri))
        {
            webReq.SetRequestHeader("Content-Type", defaultContentType);
            webReq.uploadHandler.contentType = defaultContentType;
            webReq.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));

            yield return webReq.SendWebRequest();

            if (webReq.IsError())
            {
                callback?.Invoke("Error");
            }
            else
            {
                //string data = System.Text.Encoding.UTF8.GetString(webReq.downloadHandler.data);
                //Debug.Log(data);

                callback?.Invoke("Send");
            }
        }

    }
}
public struct HttpResponse<T>
{
    public bool success;
    public T body;
    public string errorMessage;

    public HttpResponse(bool success, T body, string errorMessage)
    {
        this.success = success;
        this.body = body;
        this.errorMessage = errorMessage;
    }
}
