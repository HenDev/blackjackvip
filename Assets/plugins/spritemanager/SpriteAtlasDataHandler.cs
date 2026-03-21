using UnityEngine;
using System.Collections;
using System.Text;
//using System.Xml;
using System.IO;

public class SpriteAtlasDataHandler : MonoBehaviour 
{
	public TextAsset _spriteAtlastextAsset;
	private ArrayList marrLoadedSpriteDataParams = new ArrayList();
	LinkedSpriteManager mspriteManager;

	// Use this for initialization
	
	void Start () 
	{
		ParseWithLightWeightParser(_spriteAtlastextAsset.text);
//		Debug.Log("----- From Start list length : " + marrLoadedSpriteDataParams.Count);
	}
	
	public LoadedSpriteDataParams getLoadedSpriteData(string strfilename)
	{
		//Debug.Log("----- list length : " + marrLoadedSpriteDataParams.Count);
		
		foreach(LoadedSpriteDataParams spriteLoadedData in marrLoadedSpriteDataParams)
		{
			if(spriteLoadedData._filename == strfilename)
			{
				return spriteLoadedData;
			}
		}
		return null;
	}
	
	public void Parse(string filename)
	{
		//Debug.Log("------ parsing -------");
//		XmlDocument xmlDoc = new XmlDocument();
//		xmlDoc.LoadXml(filename);
//		
//		XmlNodeList nodeList = xmlDoc.GetElementsByTagName("dict");
//		
//		foreach(XmlNode spriteNode in nodeList)
//		{
//			XmlNodeList spritecontentParams = spriteNode.ChildNodes;
//			
//			string strfilename = "";
//			string strsizeParams = "";
//			
//			//foreach(XmlNode spriteParam in spritecontentParams)
//			for(int i = 0; i < spritecontentParams.Count; i++)
//			{
//				XmlNode spriteParam = spritecontentParams[i];
//				if(spriteParam.Name == "key")
//				{
//					strfilename = spriteParam.InnerText;
//					i++;
//					if(!strfilename.Contains(".png"))
//						return;
//					//Debug.Log(" Name : " + spriteParam.Name + " Value : " + spriteParam.InnerText);
//				}
//				
//				spriteParam = spritecontentParams[i];
//				
//				XmlNodeList spritepossizeParams = spriteParam.ChildNodes;
//				
//				foreach(XmlNode spriteEndParam in spritepossizeParams)
//				{
//					if(spriteEndParam.Name == "string")
//					{
//						strsizeParams = spriteEndParam.InnerText;
//						
//						//Debug.Log("Size Params " + strsizeParams);
//						
//						ConvertAndAddToSpriteParamsList(strfilename,strsizeParams);
//						break;
//					}
//				}
//			}
//		}
	}
	
	public void ParseWithLightWeightParser(string readText)
	{
		TinyXmlReader reader = new TinyXmlReader(readText);
		reader.Read();// read off heading dict
		
		while (reader.Read())
		{
			if(!reader.content.Contains(".png"))
			{
				continue;
			}
			string strFilename = "";
			string strSizeParams = "";
			//Debug.Log("---- read data : " + reader.tagName + " -- value " + reader.content);
			strFilename = reader.content;
			//if(reader.tagName != "dict")
			reader.Read();reader.Read();
			reader.Read();reader.Read();
			reader.Read();
			//Debug.Log("------------------ read data : " + reader.tagName + " -- value " + reader.content);
			strSizeParams = reader.content;
			ConvertAndAddToSpriteParamsList(strFilename,strSizeParams);
			
		}
	}
	
	void ConvertAndAddToSpriteParamsList(string strFileName, string strContentParams)
	{
		
		//Debug.Log(" Name " + strFileName + " " + strContentParams);
		if(strContentParams.Length < 4)
			return;
		
		LoadedSpriteDataParams spriteDataParam = new LoadedSpriteDataParams();
		spriteDataParam._filename = strFileName;
		strContentParams = strContentParams.Substring(2);
		
		int index = strContentParams.IndexOf('}');
		
		if(index < 1)
			return;
		string strUV = strContentParams.Substring(0,index);
		int indexU = strContentParams.IndexOf(',');
		
		if(indexU < 1)
			return;
		string strU = strUV.Substring(0,indexU);
		string strV = strUV.Substring(indexU+1,(index - (indexU+1)));
		
//		Debug.Log(strUV + " " + strUV.Length + " " + indexU + " " + index + " " + strU + " " + strV);
//		return;
		
		if(index < 1 || (index + 2) >= strContentParams.Length)
			return;
		strContentParams = strContentParams.Substring(index+3);
		
		index = strContentParams.IndexOf(',');
		
		if(index < 1 || (index + 1) >= strContentParams.Length)
			return;
		string strWidth = strContentParams.Substring(0,index);
		
		
		strContentParams = strContentParams.Substring(index+1);
		index = strContentParams.IndexOf('}');
		
		if(index < 1)
			return;
		string strHeight = strContentParams.Substring(0,index);

//		Debug.Log(" width -> " + strWidth + "full string " + strContentParams + " " + strWidth + " " + strHeight);
//		return;

		spriteDataParam._filename = strFileName;
		int fU = int.Parse(strU);
		int fV = int.Parse(strV);
		int iWidth = int.Parse(strWidth);
		int iHeight = int.Parse(strHeight);
		
		if(strFileName == "packagebg.png")
		{
			//Debug.Log(" UV : " + fU + " " + fV + "dimensions" + iWidth + " " + iHeight);
		}
		
		spriteDataParam._iLeftPixel = fU;
		spriteDataParam._iBottomPixel = fV + iHeight;
		spriteDataParam._iWidth = iWidth;
		spriteDataParam._iHeight = iHeight;
		marrLoadedSpriteDataParams.Add(spriteDataParam);
		
		//spriteDataParam._vecUVdimensions = mspriteManager.PixelSpaceToUVSpace(iWidth, iHeight);
		//Debug.Log("-----------------------------------------------------");
		//Debug.Log("Read data : " + spriteDataParam._filename + " " + spriteDataParam._veclowerLeftUV + " " + 
		//	spriteDataParam._vecUVdimensions + " length " + marrLoadedSpriteDataParams.Count);
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
