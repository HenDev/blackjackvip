using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChipDragger : MonoBehaviour {

	private MenuButton menuButton;
	private bool isDragging = false;
	private GameObject ghostChip;
	private Vector3 startMousePos;
	
	private static bool isAnyChipDragging = false;
	private static List<GameObject> allGhostChips = new List<GameObject>();

	private static LinkedSpriteManager dragSpriteManager;
	private static SpriteAtlasDataHandler dragDataHandler;

	void Start () {
		menuButton = GetComponent<MenuButton>();
	}

	private void InitializeDragManager() {
		if (dragSpriteManager != null) return;
		
		GameObject go = new GameObject("DragChipsManager");
		dragSpriteManager = go.AddComponent<LinkedSpriteManager>();
		dragDataHandler = go.AddComponent<SpriteAtlasDataHandler>();
		
		if (BJ_BottomBar.mInstance != null) {
			dragSpriteManager.material = BJ_BottomBar.mInstance.mChipsSpriteManager.material;
			dragSpriteManager.winding = SpriteManager.WINDING_ORDER.CW;
			dragSpriteManager.autoUpdateBounds = true;
			// Awake() already ran and set the MeshRenderer material to null, so we must set it again
			if (go.GetComponent<MeshRenderer>() != null) {
				go.GetComponent<MeshRenderer>().material = dragSpriteManager.material;
			}
			
			dragSpriteManager.allocBlockSize = 5;
			dragDataHandler._spriteAtlastextAsset = BJ_BottomBar.mInstance.mChipsSpriteAtlasDataHandler._spriteAtlastextAsset;
			
			// Manual initialization since we're creating it at runtime
			dragDataHandler.ParseWithLightWeightParser(dragDataHandler._spriteAtlastextAsset.text);
		}
	}
	
	void Update () {
		if (menuButton == null) return;

		// Prevent other chips from responding to hover/touch while one is being dragged
		if (isAnyChipDragging && !isDragging) return;

		// Only allow starting a drag if touch is enabled (e.g., betting state)
		if (!menuButton.isTouchEnabled() && !isDragging) return;

		if (Input.GetMouseButtonDown(0)) {
			if (IsMouseOver()) {
				startMousePos = Input.mousePosition;
			}
		}

		if (Input.GetMouseButton(0)) {
			if (!isAnyChipDragging && !isDragging && Vector3.Distance(startMousePos, Input.mousePosition) > 20f) {
				if (IsMouseOver()) {
					InitializeDragManager();
					StartDrag();
				}
			}

			if (isDragging) {
				UpdateDrag();
			}
		}

		if (Input.GetMouseButtonUp(0)) {
			if (isDragging) {
				EndDrag();
			}
		}
	}

	bool IsMouseOver() {
		Vector3 position = gameObject.transform.position;
		Vector2 size = menuButton.getContentSize();
		Vector2 scale = new Vector2(gameObject.transform.localScale.x, gameObject.transform.localScale.y);
		if (gameObject.transform.parent != null)
			scale = new Vector2(gameObject.transform.parent.transform.localScale.x * gameObject.transform.localScale.x, 
	gameObject.transform.parent.transform.localScale.y * gameObject.transform.localScale.y);
		Vector4 rect = new Vector4(position.x - size.x/2 * scale.x, position.y - size.y/2 * scale.y, size.x * scale.x, size.y * scale.y);
		return CommonData.isMouseTouchInsideRect(rect);
	}

	void StartDrag() {
		isDragging = true;
		isAnyChipDragging = true;
		
		// Disable interaction for the bar so we don't accidentally click other things
		if (BJ_BottomBar.mInstance != null) {
			BJ_BottomBar.mInstance.disableAllChipsButtons();
		}
		
		// Create the ghost chip using our dedicated Drag Manager
		string chipImage = "Chip_" + menuButton.getTag() + ".png";
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = -5f; // Set a visible Z position immediately
		
		ghostChip = CommonData.createGameObjectWithInit("GhostChip", null, mousePos, 
			dragSpriteManager, dragDataHandler, 
			chipImage, 200);
		if (BJ_GamePlayLayer.mInstance != null && BJ_GamePlayLayer.mInstance.player != null) {
			ghostChip.layer = BJ_GamePlayLayer.mInstance.player.gameObject.layer;
		}
		
		allGhostChips.Add(ghostChip);
	}

	void UpdateDrag() {
		if (ghostChip == null) return;

		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseWorldPos.z = -5f; // Floating above the table
		ghostChip.transform.position = mouseWorldPos;

		bool isValid = IsOverValidArea();
		// Visual indicator for valid drop zone
		ghostChip.transform.localScale = isValid ? new Vector3(1.1f, 1.1f, 1.1f) : Vector3.one;
	}

	void EndDrag() {
		isDragging = false;
		if (IsOverValidArea()) {
			AnimateAndPlace();
		} else {
			CleanupDrag();
		}
	}

	void CleanupDrag() {
		if (ghostChip != null) {
			allGhostChips.Remove(ghostChip);
			Destroy(ghostChip);
			ghostChip = null;
		}
		
		isDragging = false;
		isAnyChipDragging = false;
		
		if (BJ_BottomBar.mInstance != null) {
			BJ_BottomBar.mInstance.enableAllChipsButtons();
		}
	}

	bool IsOverValidArea() {
		Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		// Table Y boundary check
		if (mouseWorldPos.y < -5.0f) return false;

		// BJ_TableWood and its children (TableWoodProp) are invalid zones
		GameObject wood = GameObject.Find("Bj_TableWood");
		if (wood != null && wood.GetComponent<BoxCollider>() != null) {
			if (CommonData.isMouseTouchInsideObject(wood)) return false;
		}

		if (TableHandler.mInstance != null) {
			foreach (Transform child in TableHandler.mInstance.transform) {
				if (child.name.Contains("WoodProp") && child.GetComponent<BoxCollider>() != null) {
					if (CommonData.isMouseTouchInsideObject(child.gameObject)) return false;
				}
			}
		}

		return true;
	}

	void AnimateAndPlace() {
		BJ_Player player = BJ_GamePlayLayer.mInstance.player;
		Vector3 targetPos = player.mPlayerBetPosition.position;
		targetPos.z = -1.0f;
		
		GameObject currentGhost = ghostChip;
		ghostChip = null; 
		
		CMoveTo move = CMoveTo.create(currentGhost, targetPos, 0.2f);
		CCallFuncWithObj call = CCallFuncWithObj.create(OnAnimationComplete, currentGhost);
		
		CSequence seq = CSequence.create(move, call);
		seq.runAction();
	}
	
	void OnAnimationComplete(GameObject sender) {
		if (BJ_BottomBar.mInstance != null) {
			BJ_BottomBar.mInstance.chipsSelected(menuButton);
		}
		
		isAnyChipDragging = false;
		if (BJ_BottomBar.mInstance != null) {
			BJ_BottomBar.mInstance.enableAllChipsButtons();
		}
		
		if (sender != null) {
			allGhostChips.Remove(sender);
			Destroy(sender);
		}
	}

	void OnDisable() {
		if (isDragging || ghostChip != null) CleanupDrag();
	}

	void OnDestroy() {
		if (isDragging || ghostChip != null) CleanupDrag();
	}
	
	public static void ForceCleanupAll() {
		foreach (var chip in allGhostChips) {
			if (chip != null) Destroy(chip);
		}
		allGhostChips.Clear();
		isAnyChipDragging = false;
	}
}





