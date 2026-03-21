using UnityEngine;
using System.Collections;


public class CommonData : MonoBehaviour {
	
	public Vector2 screenSize = Vector2.zero;
	public static Vector2 win_sz = new Vector2(1024.0f, 768.0f);
	public static float widthRatio = Screen.width/win_sz.x;
	public static float heightRatio = Screen.height/win_sz.y;
	
	private static float mfInversePTMRatio = 0.03125f; // 1 divided by PTM RATIO (unity 1 unit == 32 pixcel)

	// Use this for initialization
	void Start () {
		if(screenSize.x != 0 && screenSize.y != 0) win_sz = screenSize;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static float getInversePTM(){return mfInversePTMRatio;}
	
	public static bool isMouseTouchInsideObject(GameObject _object)
	{		
		Vector3 position = _object.transform.position;
		Vector3 size = _object.GetComponent<BoxCollider>().size;			
		Vector2 scale = new Vector2(1,1);// new Vector2(gameObject.transform.localScale.x,gameObject.transform.localScale.y);
		//if(gameObject.transform.parent != null)
		//	scale = new Vector2(gameObject.transform.parent.transform.localScale.x * gameObject.transform.localScale.x,gameObject.transform.parent.transform.localScale.y * gameObject.transform.localScale.y);
		Vector4 rect = new Vector4(position.x - size.x/2 * scale.x, position.y - size.y/2 * scale.y, size.x * scale.x, size.y * scale.y);
			
		return isMouseTouchInsideRect(rect);
	}
	
	public static bool isMouseTouchDownInsideObject(GameObject _object)
	{
		if(Input.GetMouseButtonDown(0))
			return isMouseTouchInsideObject(_object);
		return false;
	}
	
	public static bool isMouseTouchInsideRect(Vector4 rect)
	{
//		float xPos = ((Input.mousePosition.x - Screen.width/2)*CommonData.getInversePTM())/CommonData.widthRatio + Camera.mainCamera.gameObject.transform.position.x;
//		float yPos = ((Input.mousePosition.y - Screen.height/2)*CommonData.getInversePTM())/CommonData.heightRatio + Camera.mainCamera.gameObject.transform.position.y;
		Vector2 touchLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);//new Vector3(xPos, yPos,Input.mousePosition.z);					
		return isRectContainsPoint(rect, touchLocation);
	}
	
	public static bool isMouseTouchDownInsideRect(Vector4 rect)
	{
		if(Input.GetMouseButtonDown(0))
			return isMouseTouchInsideRect(rect);
		return false;
	}
	
	public static bool isRectContainsPoint(Vector4 rect, Vector2 point)
	{
		if(point.x > rect.x && point.x < rect.x + rect.z && point.y > rect.y && point.y < rect.y + rect.w)
			return true;
		return false;
	}
	
	public static bool isRectContainsRect(Vector4 rect1, Vector4 rect2)
	{
		Vector2[] _vertices = new Vector2[4];
		_vertices[0] = new Vector2(rect2.x			 , rect2.y);			// Left Bottom
		_vertices[1] = new Vector2(rect2.x + rect2.z , rect2.y);			// Right Bottom
		_vertices[2] = new Vector2(rect2.x + rect2.z , rect2.y + rect2.w);	// Right Top
		_vertices[3] = new Vector2(rect2.x			 , rect2.y + rect2.w);	// Left Top
		for(int i=0; i<4; i++)
			if(!isRectContainsPoint(rect1, _vertices[i]))
				return false;
		return true;
	}
	
	public static Rect getScreenBoundary()
	{
		Vector2 screenOffset = new Vector2(0,0.0f);
		Vector2 sizeOffset = new Vector2(30.0f,40.0f);
		Vector2 screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-sizeOffset.x,Screen.height-sizeOffset.y,0));
		Rect screenBoundary = new Rect(-screenSize.x+screenOffset.x,-screenSize.y+screenOffset.x,screenSize.x*2+screenOffset.x,screenSize.y*2+screenOffset.y);
		return screenBoundary;
	}
	
	public static Vector4 getBoundingBox(Vector2 position, Vector2 size)
	{
		return new Vector4(position.x - size.x/2,position.y - size.y/2,size.x,size.y);
	}
	
	public static Vector4 getBoundingBox(GameObject _gameObject)
	{
		Vector2 position = new Vector2(_gameObject.transform.position.x, _gameObject.transform.position.y);
		Vector2 size = _gameObject.GetComponent<BoxCollider>().size;	
		return getBoundingBox(position, size);
	}
	
	public static float getAngle(Vector3 point1, Vector3 point2)
	{
		Vector2 pos1 = new Vector2(point1.x,point1.y) + new Vector2(Mathf.Min(point1.x,point2.x),Mathf.Min(point1.y,point2.y));
		Vector2 pos2 = new Vector2(point2.x,point2.y) + new Vector2(Mathf.Min(point1.x,point2.x),Mathf.Min(point1.y,point2.y));
		float numerator = pos1.y - pos2.y;
		float denominator = pos1.x - pos2.x;
		if(numerator == 0 && denominator == 0) return 0;
		if(numerator == 0)
		{
			if(denominator < 0.0f) return 0;
			else return 180;
		}
		if(denominator == 0)
		{
			if(numerator > 0.0f) return 270;
			else return 90;
		}
		return Mathf.Atan2(numerator,denominator) * 180 / Mathf.PI + 180;
	}
		
	public static int[] generateRandomNumbers(int min, int max, int noOfItems)
	{
		int arrCount = max - min + 1;
		if(noOfItems == 0) noOfItems = arrCount;
		int[] arrInt = new int[noOfItems];
		ArrayList arrTemp = new ArrayList();
		for(int i= 0 ; i< arrCount; i++)
			arrTemp.Add (min+i);		
		
		int j = 0;
		while(noOfItems>0)
		{	
			int rand_ = Random.Range(0,arrCount--);
			arrInt[j] = (int)arrTemp[rand_];
			arrTemp.RemoveAt(rand_);
			j++;
			noOfItems--;
		}
		return arrInt;
	}
		
	public static int[] generateRandomNumbers(int min, int max)
	{
		return generateRandomNumbers(min, max, 0);
	}
	
	// HELPER FUNCTIONS
	// Destroy Objects
	public static void destroyMyObject(GameObject _gameObject)
	{
		if(_gameObject.GetComponent<SpriteData>() != null)
		_gameObject.GetComponent<SpriteData>().removeSprite();
		DestroyObject(_gameObject);
	}
	
	public static void destroyMyLabel(CLabelAtlas _label)
	{
		_label.destroyLabel();
		DestroyObject(_label.gameObject);
	}
	
	public static void destroyMyProgress(CProgressBar _progress)
	{
		_progress.destroyProgressBar();
		DestroyObject(_progress.gameObject);
	}
	
	// Create Objects
	public static GameObject createGameObject(string objectName, GameObject parent, Vector3 position, LinkedSpriteManager spriteManger, SpriteAtlasDataHandler spriteAtlasDataHandler, string fileName, int zOrder)
	{
		GameObject _gameObject = new GameObject();
		_gameObject.name = objectName;
		if(parent!=null) _gameObject.transform.parent = parent.transform;
		_gameObject.transform.position = position + (parent!=null?parent.transform.position:Vector3.zero);			
		SpriteData sprData = _gameObject.AddComponent<SpriteData>();
		sprData._SpriteManager = spriteManger;
		sprData._SpriteAtlasDataHandler = spriteAtlasDataHandler;
		sprData._SpriteFrameName = fileName;
		sprData._ZOrder = zOrder;
		return _gameObject;
	}
	
	public static GameObject createGameObjectWithInit(string objectName, GameObject parent, Vector3 position, LinkedSpriteManager spriteManger, SpriteAtlasDataHandler spriteAtlasDataHandler, string fileName, int zOrder)
	{
		GameObject _gameObject = createGameObject(objectName, parent, position, spriteManger, spriteAtlasDataHandler, fileName, zOrder);
		_gameObject.GetComponent<SpriteData>().Init();
		return _gameObject;
	}
	
	public static GameObject createEmptyGameObject(string objectName, GameObject parent, Vector3 position)
	{
		GameObject _gameObject = new GameObject();
		_gameObject.name = objectName;
		if(parent!=null) _gameObject.transform.parent = parent.transform;
		_gameObject.transform.position = position + (parent!=null?parent.transform.position:Vector3.zero);	
		return _gameObject;
	}
	
	public static GameObject createBackGround(string frameName,int zOrder)
	{
		GameObject BackGround = GameObject.CreatePrimitive(PrimitiveType.Plane);
		BackGround.name = "BackGround";
		Destroy(BackGround.GetComponent<MeshCollider>());
		BackGround.transform.localScale = new Vector3(3.2f,0,2.4f);
		BackGround.transform.position = new Vector3(0,0,zOrder);
		BackGround.transform.Rotate(new Vector3(90,180,0));
		BackGround.transform.GetComponent<Renderer>().material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
		BackGround.transform.GetComponent<Renderer>().material.mainTexture = Resources.Load("BGs/"+frameName) as Texture2D;
		return BackGround;
	}
	
	private static AnimationDataParams createNewAnimationDataParam(SpriteData sprData)
	{
		if(sprData._ArrAnimationdata == null)
			sprData._ArrAnimationdata = new AnimationDataParams[1];
		else
		{
			AnimationDataParams[] tempArray = new AnimationDataParams[sprData._ArrAnimationdata.Length];
			for(int i=0; i<tempArray.Length; i++)
				tempArray[i] = sprData._ArrAnimationdata[i];
			sprData._ArrAnimationdata = new AnimationDataParams[tempArray.Length + 1];
			for(int i=0; i<tempArray.Length; i++)
				sprData._ArrAnimationdata[i] = tempArray[i];
		}
		int index = sprData._ArrAnimationdata.Length-1;
		sprData._ArrAnimationdata[index] = new AnimationDataParams();
		return sprData._ArrAnimationdata[index];;
	}
	
	public static void addSingleFrameAnimationTo(GameObject _gameObject, string namePath, string animName)
	{
		SpriteData sprData = _gameObject.GetComponent<SpriteData>();
		if(sprData == null) return;		
		AnimationDataParams animData = CommonData.createNewAnimationDataParam(sprData);
		animData._name = animName;
		animData._frameNames = new string[1];
		animData._frameNames[0] = namePath+".png";
		animData._fps = 0;
		animData._loopCycles = -1;
	}
	
	public static void addAnimationTo(GameObject _gameObject, string namePath, string animName, int[] frames, int fps, int loop)
	{
		SpriteData sprData = _gameObject.GetComponent<SpriteData>();
		if(sprData == null) return;		
		AnimationDataParams animData = createNewAnimationDataParam(sprData);
		animData._name = animName;
		animData._frameNames = new string[frames.Length];
		for(int i=0; i<frames.Length; i++)
			animData._frameNames[i] = namePath+frames[i]+".png";
		animData._fps = fps;
		animData._loopCycles = loop;
	}
	
	public static void addAnimationTo(GameObject _gameObject, string namePath, string animName, int frames, int fps, int loop)
	{
		int[] frameList = new int[frames];
		for(int i=0; i<frames; i++)
			frameList[i] = i+1;
		addAnimationTo(_gameObject, namePath, animName, frameList, fps, loop);
	}
	
	public static void addDefaultAnimationTo(GameObject _gameObject, string namePath, string animName, int[] frames, int fps, int loop)
	{
		addAnimationTo(_gameObject, namePath, animName, frames, fps, loop);
		_gameObject.GetComponent<SpriteData>().mdefaultAnimation = animName;
	}
	
	public static void addDefaultAnimationTo(GameObject _gameObject, string namePath, string animName, int frames, int fps, int loop)
	{
		addAnimationTo(_gameObject, namePath, animName, frames, fps, loop);
		_gameObject.GetComponent<SpriteData>().mdefaultAnimation = animName;
	}
	
	public static void addIdealAnimationTo(GameObject _gameObject, string namePath, int[] frames, int fps)	{
		addAnimationTo(_gameObject, namePath, "ideal", frames, fps, -1);	}
	
	public static void addIdealAnimationTo(GameObject _gameObject, string namePath, int frames, int fps)	{
		addAnimationTo(_gameObject, namePath, "ideal", frames, fps, -1);	}
	
	public static void addReverseAnimationTo(GameObject _gameObject, string namePath, string animName, int frames, int fps, int loop)
	{
		int[] frameList = new int[frames];
		for(int i=frames-1; i>=0; i--)
			frameList[i] = i+1;
		addAnimationTo(_gameObject, namePath, animName, frameList, fps, loop);
	}
	
	public static void removeAllActionsFrom(GameObject _gameObject)
	{
		CAction[] arrActions = _gameObject.GetComponents<CAction>();
		foreach(CAction action in arrActions)
			DestroyObject(action);
	}
}
