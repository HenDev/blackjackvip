using UnityEngine;
using System.Collections;

public class XmlParshingDelegate : MonoBehaviour {
	virtual public void parsedData(string elementName, XmlDict dict)
	{ Debug.Log("XmlLoaded Node : "+elementName); }
	virtual public void parseCompleted() { Debug.Log("XmlParsing Completed"); }
}

public class XmlParsedData {
	public string elementName = "";
	public XmlDict xmlDict = null;
	
	public XmlParsedData(string _elementName, XmlDict dict)
	{
		elementName = _elementName;
		xmlDict = dict;
	}
}

public class XmlDict {
	ArrayList paramName = null;
	ArrayList paramValue = null;
	
	public void addParam(string _paramName, string _paramValue)
	{
		if(paramName == null) paramName = new ArrayList();
		if(paramValue == null) paramValue = new ArrayList();		
		paramName.Add(_paramName);
		paramValue.Add(_paramValue);
	}
	
	public string getStringForKey(string key)
	{
		if(paramName == null) return "";		
		int index = paramName.IndexOf((object)key);
		if(index == -1) return "";		
		return (string)paramValue[index];
	}
	
	public int getIntegerForKey(string key)	{
		return (int)getFloatForKey(key);	}
	
	public float getFloatForKey(string key)
	{
		string _value = getStringForKey(key);
		float _valueFloat = 0.0f;
		float.TryParse(_value, out _valueFloat);
		return _valueFloat;
	}
	
	public bool getBoolForKey(string key)
	{
		string _value = getStringForKey(key);
		bool _valueBool = false;
		bool.TryParse(_value, out _valueBool);
		return _valueBool || getIntegerForKey(key)!=0;
	}
}

public class XmlParsher : MonoBehaviour {
	
	public XmlParshingDelegate mDelegate = null;
	public TextAsset _XmlTextAsset = null;
	private bool isParseStarted = false;
	private bool isParseCompleted = false;
	private bool isAutoRelease = false;
	// Use this for initialization
	void Start () {	}
	
	// Update is called once per frame
	void Update () {
		if(_XmlTextAsset != null && !isParseStarted)
		{
			isParseStarted = true;
			ParseWithLightWeightParser(_XmlTextAsset.text);
		}
		if(isParseCompleted && isAutoRelease) { if(mDelegate != null) mDelegate.parseCompleted(); DestroyObject(gameObject); }
	}
	
	public static void ParseWithFileName(string fileName, XmlParshingDelegate _delegate, bool autoRelease)
	{
		GameObject _object = new GameObject();
		_object.name = "XmlParsher";
		XmlParsher parser = _object.AddComponent<XmlParsher>();
		parser.isAutoRelease = autoRelease;
		parser.ParseWithFileName(fileName, _delegate);
	}
	
	public void ParseWithFileName(string fileName, XmlParshingDelegate _delegate)
	{
		_XmlTextAsset = Resources.Load(fileName) as TextAsset;
		mDelegate = _delegate;
		isParseStarted = false;
	}
	
	private void ParseWithLightWeightParser(string readText)
	{	
		//Temporary variables to hold the current tag, attributes and value.		
		ArrayList tagList=new ArrayList();
		ArrayList attributesList=new ArrayList();
		ArrayList valueList=new ArrayList();
		string currentTag=null;
		string currentAttribute=null;
		string currentValue=null;
		
		//To change the text in the file into particular format like removing extra spaces.
		readText.Trim();
		while(readText.Contains("  "))
			readText=readText.Replace("  "," ");
		readText=readText.Replace("< ","<");
		readText=readText.Replace("\"/","\" /");
		readText=readText.Replace(" = ","=");
		readText=readText.Replace("= ","=");
		readText=readText.Replace(" =","=");
		readText=readText.Replace("\t","");		
		
		//Splitting the text and storing into string array.
		string [] arrStr=readText.Split(' ','=','>','\n');
		for(int i=0; i<arrStr.Length; i++)
		{			
#if UNITY_FLASH
			int endIndex = 0;
#else
			if(arrStr[i].IndexOf("\"") == 0 && arrStr[i].LastIndexOf("\"") != arrStr[i].Length-1)
			{
				//Debug.Log("str : "+arrStr[i]);
				int nextIndex = i+1;
				while(nextIndex < arrStr.Length)
				{
					arrStr[i] += " "+arrStr[nextIndex];
					arrStr[nextIndex++] = "";	
					if(arrStr[nextIndex].LastIndexOf("\"") != arrStr[nextIndex].Length-1) break;
				}
			}
#endif
			//Debug.Log("str : "+arrStr[i]);
		}
		
		//Identify tag, attribtues ana values in the string array.
		foreach(string str in arrStr)
		{
			if(str!="" && str!=null)
			{
				//Debug.Log("str : "+str);
				if(str.IndexOf("<")!=-1 && str.IndexOf("/")==-1)
				{	
					currentTag=str.Replace("<","");
					tagList.Add(currentTag);
					valueList.Clear();
					attributesList.Clear();
				}
				if(str.IndexOf("<")!=-1 && str.IndexOf("/")!=-1)
				{
					currentTag=str.Replace("</","");
					tagList.RemoveAt(tagList.Count-1);
				}
				else if(str.IndexOf("<")==-1 && str.IndexOf("\"")==-1 && str.IndexOf("/")==-1)
				{
					currentAttribute=str;
					attributesList.Add(currentAttribute);
				}
				else if(str.IndexOf("/")!=-1 && str.IndexOf("\"")==-1)
				{
					currentValue=str.Replace("/","");
					currentValue=currentValue.Replace("\"","");
					valueList.Add(currentValue);
					
					XmlDict dict = new XmlDict();
					for(int i=0; i<attributesList.Count; i++)
						dict.addParam((string)attributesList[i], (string)valueList[i]);
					if(mDelegate != null)
						mDelegate.parsedData(currentTag, dict);
					
					valueList.Clear();
					attributesList.Clear();
					tagList.RemoveAt(tagList.Count-1);
				}
				else if(str.IndexOf("\"")!=-1)
				{
					currentValue=str.Replace("\"","");
					valueList.Add(currentValue);					
				}
			}
		}
		
		isParseCompleted = true;
	}
}
