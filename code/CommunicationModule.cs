using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class CommunicationModule : MonoBehaviour
{
    //SingleTon Pattern
    private static CommunicationModule _instance = null;

    private CommunicationModule() { }

    public static CommunicationModule getInstance()
    {
	if (_instance == null)
	{
	    var obj = FindObjectOfType<CommunicationModule>();

	    if (obj != null)
		_instance = obj;
	    else
		_instance = new GameObject("Communication Module").AddComponent<CommunicationModule>();
	}
	return _instance;
    }

    private void Awake()
    {
        var objs = FindObjectsOfType<CommunicationModule>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);

            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    //Rest API Configure
    private const string Content_Type = "application/json";

    private const string AuthorType = "Bearer ";
    private const string api_key = "e2BSjK4RoPSD1hnxnXywcuzfumPgnjYTFX3dCEnYAqLbQ7oa2tSUeQakh42FJTHo";
    private const string Authorization = AuthorType + api_key;

    private const string walletAddress = "0xbf6f276beaf7e7b7e53bc22c48ce140dd9916acc";

    private const string apiURI = "https://api.luniverse.io/tx/v1.1/";
    private const string transactions = "transactions/";
    private const string histories = "histories/";
    private const string receipts = "receipts/";

    private const string transactionURI = apiURI + transactions;
    private const string historiesURI = apiURI + histories;
    private const string receiptsURI = apiURI + receipts;

    private const string post = "POST";
    private const string get = "GET";
    private const string put = "PUT";
    private const string delete = "DELETE";

    private static string func;
    private static string callURI;
    private static string callRest;
    private static string parameter;

    private class HttpBody
    {
	public string from = walletAddress;
	public string inputs;
    }

    private static HttpBody body = new HttpBody();

    public int createPlayer(int userIndex)
    {
	func = "createPlayer";
	callRest = post;

	GetterClass obj = new GetterClass(userIndex);
	string str = JsonUtility.ToJson(obj);
	body.inputs = str;
	str = JsonUtility.ToJson(body);
	parameter = multiJsonProcessing(str);

	transaction();

	func = "getPlayerIndex";
	body.inputs = "";
	str = JsonUtility.ToJson(body);
	parameter = multiJsonProcessing(str);
	transaction();

	int retUserIndex = -1;
	char[] result = responseStr.ToCharArray();
	int i = 0;
	while(i < result.Length)
	{
	    if(result[i] == '[')
	    {
		i += 2;
		int j = i;
		while(result[j] != '"')
		    j++;
		int length = j - i;
		char[] temp = new char[length];
		int k = 0;
		while(k < length)
		{
		    temp[k] = result[i + k];
		    k++;
		}
		string tempStr = new string(temp);
		retUserIndex = int.Parse(tempStr);
		break;
	    }
	    i++;
	}

	return retUserIndex - 1;//it has some error so minus 1
    }

    private class CreatePlayerClass
    {
	public int _index;

	public CreatePlayerClass(int index)
	{
	    this._index = index;
	}
    }

    public void update(int userIndex, int score, int[] itemRemain)
    {
	func = "update";
	callRest = post;

	UpdateClass obj = new UpdateClass(userIndex, score, itemRemain);
	string str = JsonUtility.ToJson(obj);
	body.inputs = str;
	str = JsonUtility.ToJson(body);
	parameter = multiJsonProcessing(str);

	transaction();
    }

    private class UpdateClass
    {
	public int _index;
	public int _current_score;
	public int[] _used_itemlist;

	public UpdateClass(int userIndex, int score, int[] itemRemain)
	{
	    this._index = userIndex;
	    this._current_score = score;
	    this._used_itemlist = itemRemain;
	}
    }

    private string multiJsonProcessing(string json)
    {
	json = json.Replace("\\\"", "\"");
	json = json.Replace("\"{", "{");
	json = json.Replace("}\"", "}");

	return json;
    }

    public string getProperties(int userIndex)
    {
	string returnStr = "";
	callRest = post;

	GetterClass obj = new GetterClass(userIndex);
	string str = JsonUtility.ToJson(obj);
	body.inputs = str;
	str = JsonUtility.ToJson(body);
	parameter = multiJsonProcessing(str);

	//Delay();
	func = "getHighestScore";
	transaction();
	char[] result = responseStr.ToCharArray();
	int i = 0;
	while(i < result.Length)
	{
	    if(result[i] == '[')
	    {
		i += 2;
		int j = i;
		while(result[j] != '"')
		    j++;
		int length = j - i;
		char[] temp = new char[length];
		int k = 0;
		while(k < length)
		{
		    temp[k] = result[i + k];
		    k++;
		}
		returnStr = new string(temp) + ",";
		break;
	    }
	    i++;
	}

	func = "getItemList";
	transaction();
	result = responseStr.ToCharArray();
	i = 0;
	while(i < result.Length)
	{
	    if(result[i] == '[')
	    {
		i += 2;
		int j = i;
		while(result[j] != ']')
		    j++;
		int length = j - i;
		char[] temp = new char[length];
		int k = 0;
		while(k < length)
		{
		    temp[k] = result[i + k];
		    k++;
		}
		string tempStr = "[" + new string(temp) + "]" + ",";
		returnStr += tempStr;
		break;
	    }
	    i++;
	}

	func = "getReward";
	transaction();
	result = responseStr.ToCharArray();
	i = 0;
	while(i < result.Length)
	{
	    if(result[i] == '[')
	    {
		i += 2;
		int j = i;
		while(result[j] != '"')
		    j++;
		int length = j - i;
		char[] temp = new char[length];
		int k = 0;
		while(k < length)
		{
		    temp[k] = result[i + k];
		    k++;
		}
		returnStr += new string(temp);
		break;
	    }
	    i++;
	}

	return returnStr;
    }

    private class GetterClass
    {
	public int _index;

	public GetterClass(int userIndex)
	{
	    this._index = userIndex;
	}
    }

    private bool isRecentTradeSuccess(int userIndex)
    {
	func = "isRecentTradeSuccess";
	callRest = post;

	GetterClass obj = new GetterClass(userIndex);
	string str = JsonUtility.ToJson(obj);
	body.inputs = str;
	str = JsonUtility.ToJson(body);
	parameter = multiJsonProcessing(str);

	//Delay();
	transaction();

	string temp = responseStr.Substring(30);
	char[] cTemp = temp.ToCharArray();
	if(cTemp[0] == 'f')
	    return false;
	else
	    return true;
    }

    public bool send(int from, int to, int itemIndex, int count)
    {
	func = "send";
	callRest = post;

	TransferClass obj = new TransferClass(from, to, itemIndex, count);
	string str = JsonUtility.ToJson(obj);
	body.inputs = str;
	str = JsonUtility.ToJson(body);
	parameter = multiJsonProcessing(str);

	transaction();

	return isRecentTradeSuccess(from);
    }

    private class TransferClass
    {
	public int fromUserIndex;
	public int toUserIndex;
	public int itemIndex;
	public int itemCount;

	public TransferClass(int from, int to, int itemIndex, int count)
	{
	    this.fromUserIndex = from;
	    this.toUserIndex = to;
	    this.itemIndex = itemIndex;
	    this.itemCount = count;
	}
    }

    //this code is deprecated
    /*
    public bool trade(int from, int to, int itemIndex, int count)
    {
	func = "trade";
	callRest = post;

	TransferClass obj = new TransferClass(from, to, itemIndex, count);
	string str = JsonUtility.ToJson(obj);
	body.inputs = str;
	str = JsonUtility.ToJson(body);
	parameter = multiJsonProcessing(str);

	transaction();

	LogPrinter.getInstance().print("CommunicationModule trade : need some return value check");
	LogPrinter.getInstance().print("CommunicationModule trade responseString : " + responseStr);
	return true;
    }
    */

    public bool buy(int userIndex, int[] itemCount)
    {
	callRest = post;
	bool success = true;

	int i = 0;
	do
	{
	    BuyClass obj = new BuyClass(userIndex, i, itemCount[i]);
	    string str = JsonUtility.ToJson(obj);
	    body.inputs = str;
	    str = JsonUtility.ToJson(body);
	    parameter = multiJsonProcessing(str);

	    func = "buy";
	    transaction();

	    success = isRecentTradeSuccess(userIndex);
	    i++;
	} while(i < itemCount.Length && success);

	return success;
    }

    private class BuyClass
    {
	public int _index;
	public int itemIndex;
	public int itemCount;

	public BuyClass(int userIndex, int itemIndex, int itemCount)
	{
	    this._index = userIndex;
	    this.itemIndex = itemIndex;
	    this.itemCount = itemCount;
	}
    }

    private static string responseStr;

    private void transaction()
    {
	callURI = transactionURI + func;
	LogPrinter.getInstance().print("URI : " + callURI);
	LogPrinter.getInstance().print("Rest API : " + callRest);
	LogPrinter.getInstance().print("Parameters : " + parameter);

	UploadHandler up = new UploadHandlerRaw(Encoding.UTF8.GetBytes(parameter));
	up.contentType = Content_Type;

	DownloadHandler down = new DownloadHandlerBuffer();

	UnityWebRequest transactionRequest = new UnityWebRequest(callURI, callRest, down, up);
	transactionRequest.SetRequestHeader("Authorization", Authorization);
	LogPrinter.getInstance().print("Encoding Data : " + Encoding.Default.GetString(up.data));

	UnityWebRequestAsyncOperation res = transactionRequest.SendWebRequest();
	while(!res.isDone);

	if (transactionRequest.isNetworkError || transactionRequest.isHttpError)
	    LogPrinter.getInstance().print("Transaction Request Error : " + transactionRequest.error + down.text);
	else
	{
	    responseStr = down.text;
	    LogPrinter.getInstance().print("Transaction Response : " + responseStr);
	}
    }
}
