using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Properties
{
    //SingleTon Pattern
    private static Properties _instance = null;

    private Properties()
    {
	readConfigFile();
	getProperties();
    }

    public static Properties getInstance()
    {
	if (_instance == null)
	{
	    _instance = new Properties();
	}
	return _instance;
    }

    //client properties
    private static int _userIndex;
    private static int _reward;
    private static int[] _itemList;
    private static int _highestScore;

    //private string configPath = Application.dataPath;
    private string configPath = "UserDataConfig.txt";

    private void readConfigFile()
    {
	if(File.Exists(configPath))
	{
	    LogPrinter.getInstance().print("Config file is Exist");
	    /*StreamReader sr = new StreamReader(configPath);
	    string str = sr.ReadLine();
	    sr.Close();*/
	    string str = Encoding.Default.GetString(File.ReadAllBytes(configPath));

	    ConfigClass cc = JsonUtility.FromJson<ConfigClass>(str);

	    _userIndex = cc._userIndex;
	}
	else
	{
	    LogPrinter.getInstance().print("Config file is not Exist");
	    _userIndex = -1;
	    _userIndex = CommunicationModule.getInstance().createPlayer(_userIndex);
	    getProperties();
	    saveConfigFile();
	}
    }

    private class ConfigClass
    {
	public int _userIndex;

	public ConfigClass(int userIndex)
	{
	    this._userIndex = userIndex;
	}
    }

    private void saveConfigFile()
    {
	LogPrinter.getInstance().print("Create Config file");
	/*FileStream fs = File.Create(configPath);

	ConfigClass cc = new ConfigClass(_userIndex);
	string str = JsonUtility.ToJson(cc);
	fs.Write(Encoding.UTF8.GetBytes(str), 0, str.Length);

	fs.Close();*/

	ConfigClass cc = new ConfigClass(_userIndex);
	string str = JsonUtility.ToJson(cc);
	File.WriteAllBytes(configPath, Encoding.UTF8.GetBytes(str));
    }

    //A function when game is over
    public void update(int score)
    {
	CommunicationModule.getInstance().update(_userIndex, score, _itemList);
    }

    //================property update need some time==============================//
    //A function when user game start
    public void getProperties()
    {
	string str = CommunicationModule.getInstance().getProperties(_userIndex);
	LogPrinter.getInstance().print("Response of getProperties : " + str);

	LogPrinter.getInstance().print("Properties");
	LogPrinter.getInstance().print("User Index : " + _userIndex);

	char[] arr = str.ToCharArray();
	char[] temp;
	int start = 0;
	int end = start;
	{
	    while(arr[end] != ',')
		end++;
	    int length = end - start;
	    temp = new char[length];
	    int i_ = 0;
	    while(i_ < length)
	    {
		temp[i_] = arr[start + i_];
		i_++;
	    }
	}
	string tempStr = new string(temp);
	start = end + 1;
	end = start;
	_highestScore = int.Parse(tempStr);
	LogPrinter.getInstance().print("Highest Score : " + _highestScore);

	int count = 1;
	while(arr[end] != ']')
	{
	    if(arr[end] == ',')
		count++;
	    end++;
	}
	_itemList = new int[count];
	int arrayPointer = 0;
	while(start < end)
	{
	    if(arr[start] == '"')
	    {
		start++;
		int endPointer = start + 1;
		while(arr[endPointer] != '"')
		    endPointer++;
		int length = endPointer - start;
		temp = new char[length];
		int i_ = 0;
		while(i_ < length)
		{
		    temp[i_] = arr[start + i_];
		    i_++;
		}
		tempStr = new string(temp);
		_itemList[arrayPointer] = int.Parse(tempStr);
		arrayPointer++;

		start = endPointer + 1;
	    }
	    start++;
	}
	int i = 0;
	while(i < _itemList.Length)
	{
	    LogPrinter.getInstance().print("Item Number " + i + " : " + _itemList[i]);
	    i++;
	}

	start += 1;
	end = start + 1;
	while(end < arr.Length)
	    end++;
	{
	    int length = end - start;
	    temp = new char[length];
	    int i_ = 0;
	    while(i_ < length)
	    {
		temp[i_] = arr[start + i_];
		i_++;
	    }
	}
	tempStr = new string(temp);
	_reward = int.Parse(tempStr);
	LogPrinter.getInstance().print("Reward : " + _reward);
    }

    public void send(int toUser, int itemIndex, int count)
    {
	CommunicationModule.getInstance().send(_userIndex, toUser, itemIndex, count);
	getProperties();
    }

    //trade function is deprecated
    /*
    public bool trade(int toUser, int itemIndex, int count)
    {
	bool result = CommunicationModule.getInstance().trade(_userIndex, toUser, itemIndex, count);
	getProperties();

	return result;
    }*/

    //buy transaction will occur each action
    public bool buy(int[] itemList)
    {
	bool result = CommunicationModule.getInstance().buy(_userIndex, itemList);
	getProperties();

	return result;
    }

    //when using this method, must update data when game finished.
    public bool itemUse(int itemIndex)
    {
	if(_itemList[itemIndex] > 0)
	{
	    _itemList[itemIndex] -= 1;
	    return true;
	}

	return false;
    }

    //simple get functions
    public int getUserIndex()
    {
	return _userIndex;
    }

    public int getReward()
    {
	return _reward;
    }

    public int itemListLength()
    {
	return _itemList.Length;
    }

    public int getItemCount(int itemIndex)
    {
	return _itemList[itemIndex];
    }

    public int getHighestScore()
    {
	return _highestScore;
    }
}
