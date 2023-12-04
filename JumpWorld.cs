// Super Odin World 64
using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Fishlabs;
using System.Globalization;
using System.Collections;
using TMPro;




[BepInPlugin("Rholor.SuperOdinWorld64", "Super Odin World 64", "1.0.0")]
public class JumpWorld : BaseUnityPlugin
{


	public static List<GameObject> currentHitObjects = new List<GameObject>();



	public static bool HasWallJumped2 = false;
	public static bool HasWallJumped3 = false;
	public static bool isJumping = false;
	public static float timer;
	public static float timer2;
	public static float TrampolineJumpTimer = 1f;
	public static float BounceTimer = 0f;
	public static float wallJumpCooldown = 0.1f;
	public static float wallJumpCooldown2 = 0.5f;
	public static Vector3 savedPos;
	public static Vector3 savedPos2;
	public static Vector3 spawnPos;
	public static float PlatformDistanceValue = 2f;
	public static int MatNumber = 0;
	public static int ChildNumber = 0;
	public static float TrampolinValue = 0f;
	public static float SizeValue = 1f;
	public static float TurnDirectionValue = 1f;
	public static float SpeedValue = 1f;
	public static float RadiusValue = 1f;
	public static float PlatformDirection = 1f;
	public static float IcePlatformValue = 0f;
	public static float StickyPlatformValue = 0f;
	public static float BhopValue = 0f;
	public static float SurfPlatformValue = 0f;

	public static JumpworldConfig config = new JumpworldConfig();
	public static bool stopwatchActive = false;
	public static float currentTime;
	public static float SinVar;
	public static float SinVar2;
	public static bool SinDir = true;
	public static bool CanHook = false;

	public static float SinTime;
	private static GameObject _panel;
	private static GameObject secondPanel;
	private static Text _KeyBindText;
	private static Text _timeText;
	private static Text _TPText;
	private static Text _CPText;
	private static Text _RIPText;
	public static int Biomes;
	public static int GroundType;
	public static bool helmet = false;
	public static bool cloak = false;
	public static bool shoes = false;
	public static bool chest = false;
	public static bool demister = false;
	public static bool belt = false;
	public static float StoredSinVar = 0f;
	public static float StoredSinDir = 1f;
	public static float SinDiff;

	public static float SphereRadius = 5f;
	public static GameObject DotTracker;
	public static GameObject DotTrackerParentingDistance;
	public static GameObject DotTrackerParentingDistance2;
	public static GameObject DotTrackerParentingDistanceBuilt;
	public static GameObject GhostObject;
	public static GameObject currentGhostObject;
	public static float currentSizeValue = 0f;
	public static float currentRadiusValue = 0f;
	public static GameObject ZoneCorner;
	public static GameObject ZoneCorner2;
	public static GameObject ZoneCorner3;
	public static GameObject ZoneCorner4;
	public static GameObject ZoneCenter;
	public static GameObject[] ZoneCorners = new GameObject[4];

	public static ParticleSystem.EmissionModule psEmission;
	public static ParticleSystem.EmissionModule psEmission2;

	public static string PrefabString = "";
	public static string TimeOfDay = "";
	public static string GroupingValue = "";
	public static string WeatherCondition = "";
	public static string OldPrefabString = "";
	public static string MatPrefabString = "";
	public static bool SkinnedToggle = false;
	public static bool ZoneToggle = false;
	public static bool RadiusToggle = false;

	public static bool HasTouchedIce;
	public static bool HasTouchedIce2;
	public static bool HasTouchedSurf = false;
	public static bool HasTouchedSurf2 = false;
	public static Vector3 OldSlopeNorm;
	public static float yForce = 80f; // requires tuning
	public static float dampenFactor = 0.8f; // requires tuning
	public static float offsetFactor = 0.5f; // requires tuning

	public static bool isSecondPanelVisible = false;
	public static string KeyBindString = "";

	public static int JumpNumber
	{
		get;
		private set;
	}
	public static int DashJumpNumber
	{
		get;
		private set;
	}

	public static int WaterJumpNumber
	{
		get;
		private set;
	}

	public static int DoubleJumpNumber
	{
		get;
		private set;
	}

	public static int StickyJumpNumber
	{
		get;
		private set;
	}


	////////////////////////////////////////////////////////////////////////////Set Up Custom HUD



	private void Update()
	{
		if ((bool)Player.m_localPlayer && (bool)Hud.instance && Traverse.Create((object)Hud.instance).Method("IsVisible", Array.Empty<object>()).GetValue<bool>())
		{
			if (_panel == null)
			{
				CreatePanel(Hud.instance);
			}
			RectTransform component = _panel.GetComponent<RectTransform>();
			RectTransform componentKB = secondPanel.GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(-140f, -265f);
			componentKB.anchoredPosition = new Vector2(-140f, -590f);
			component.sizeDelta = new Vector2(200f, 50f);
			componentKB.sizeDelta = new Vector2(200f, 600f);
			Image component2 = _panel.GetComponent<Image>();
			Image componentKB2 = secondPanel.GetComponent<Image>();
			component2.enabled = true;
			componentKB2.enabled = true;
			component2.color = new Color(0f, 0f, 0f, 0.3921569f);
			componentKB2.color = new Color(0f, 0f, 0f, 0.3921569f);
			_timeText.enabled = true;
			_KeyBindText.enabled = true;
			UpdateKeyBind();
			UpdateTime();
			UpdateTP();
			UpdateCP();
			UpdateRIP();
		}
	}



	private void OnDestroy()
	{
		if ((bool)Hud.instance && _panel != null)
		{
			UnityEngine.Object.Destroy(_panel);
		}
	}

	public static void CreateSecondPanel()
	{
		// Create a new GameObject for the second panel
		secondPanel = new GameObject("SecondPanel");
		secondPanel.layer = 5;
		secondPanel.transform.SetParent(Hud.instance.m_rootObject.transform);

		// Set the position and size of the second panel
		RectTransform secondPanelRectTransform = secondPanel.AddComponent<RectTransform>();
		secondPanelRectTransform.anchorMin = new Vector2(1f, 1f);
		secondPanelRectTransform.anchorMax = new Vector2(1f, 1f);
		secondPanelRectTransform.anchoredPosition = new Vector2(-140f, -590f); // Position it 50 units below the first panel
		secondPanelRectTransform.sizeDelta = new Vector2(200f, 600f); // Same width as the first panel

		// Create an Image component for the second panel
		Sprite sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault((Sprite s) => s.name == "InputFieldBackground");
		Image image = secondPanel.AddComponent<Image>();
		image.enabled = true;
		image.color = new Color(0f, 0f, 0f, 0.3921569f);
		image.sprite = sprite;
		image.type = Image.Type.Sliced;
		CreateKeyBind();
	}


	public static void CreatePanel(Hud hudInstance)
	{
		_panel = new GameObject("DayTimePanel")
		{
			layer = 5
		};
		_panel.transform.SetParent(hudInstance.m_rootObject.transform);
		RectTransform rectTransform = _panel.AddComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2(1f, 1f);
		rectTransform.anchorMax = new Vector2(1f, 1f);
		rectTransform.anchoredPosition = new Vector2(-140f, -255f);
		rectTransform.sizeDelta = new Vector2(200f, 30f);
		Sprite sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault((Sprite s) => s.name == "InputFieldBackground");
		Image image = _panel.AddComponent<Image>();
		image.enabled = true;
		image.color = new Color(0f, 0f, 0f, 0.3921569f);
		image.sprite = sprite;
		image.type = Image.Type.Sliced;
		CreateSecondPanel();
		CreateStopwatch();
		CreateTP();
		CreateCP();
		CreateRIP();
	}

	private static void CreateKeyBind()
	{
		GameObject gameObject = new GameObject("KeyBind");
		gameObject.layer = 5;
		gameObject.transform.SetParent(secondPanel.transform);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.anchoredPosition = new Vector2((150f / 4f - 10f), -10f);


		rectTransform.sizeDelta = new Vector2(200f, 600f);
		_KeyBindText = gameObject.AddComponent<Text>();
		_KeyBindText.color = new Color(1f, 1f, 1f, 0.791f);
		_KeyBindText.font = GetFont();
		_KeyBindText.fontSize = 16;
		_KeyBindText.enabled = true;
		_KeyBindText.alignment = (TextAnchor)3;
		Outline outline = gameObject.AddComponent<Outline>();
		outline.effectColor = Color.black;
		outline.effectDistance = new Vector2(1f, -1f);
		outline.useGraphicAlpha = true;
		outline.useGUILayout = true;
		outline.enabled = true;
	}
	private static void CreateStopwatch()
	{
		GameObject gameObject = new GameObject("Timer");
		gameObject.layer = 5;
		gameObject.transform.SetParent(_panel.transform);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.anchoredPosition = new Vector2((150f / 4f - 10f), 10f);
		rectTransform.sizeDelta = new Vector2(400f / 2f, 30f);
		_timeText = gameObject.AddComponent<Text>();
		_timeText.color = new Color(1f, 1f, 1f, 0.791f);
		_timeText.font = GetFont();
		_timeText.fontSize = 16;
		_timeText.enabled = true;
		_timeText.alignment = (TextAnchor)3;
		Outline outline = gameObject.AddComponent<Outline>();
		outline.effectColor = Color.black;
		outline.effectDistance = new Vector2(1f, -1f);
		outline.useGraphicAlpha = true;
		outline.useGUILayout = true;
		outline.enabled = true;
	}

	private static void CreateTP()
	{
		GameObject gameObject = new GameObject("TP");
		gameObject.layer = 5;
		gameObject.transform.SetParent(_panel.transform);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.anchoredPosition = new Vector2((300f / 4f - 10f), -10f);
		rectTransform.sizeDelta = new Vector2(200f / 2f, 30f);
		_TPText = gameObject.AddComponent<Text>();
		_TPText.color = new Color(1f, 1f, 1f, 0.791f);
		_TPText.font = GetFont();
		_TPText.fontSize = 16;
		_TPText.enabled = true;
		_TPText.alignment = (TextAnchor)3;
		Outline outline = gameObject.AddComponent<Outline>();
		outline.effectColor = Color.black;
		outline.effectDistance = new Vector2(1f, -1f);
		outline.useGraphicAlpha = true;
		outline.useGUILayout = true;
		outline.enabled = true;
	}

	private static void CreateCP()
	{
		GameObject gameObject = new GameObject("CP");
		gameObject.layer = 5;
		gameObject.transform.SetParent(_panel.transform);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.anchoredPosition = new Vector2((50f / 4f - 10f), -10f);
		rectTransform.sizeDelta = new Vector2(200f / 2f, 250f);
		_CPText = gameObject.AddComponent<Text>();
		_CPText.color = new Color(1f, 1f, 1f, 0.791f);
		_CPText.font = GetFont();
		_CPText.fontSize = 16;
		_CPText.enabled = true;
		_CPText.alignment = (TextAnchor)3;
		Outline outline = gameObject.AddComponent<Outline>();
		outline.effectColor = Color.black;
		outline.effectDistance = new Vector2(1f, -1f);
		outline.useGraphicAlpha = true;
		outline.useGUILayout = true;
		outline.enabled = true;
	}

	private static void CreateRIP()
	{
		GameObject gameObject = new GameObject("RIP");
		gameObject.layer = 5;
		gameObject.transform.SetParent(_panel.transform);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.anchoredPosition = new Vector2((550f / 4f - 10f), -10f);
		rectTransform.sizeDelta = new Vector2(200f / 2f, 250f);
		_RIPText = gameObject.AddComponent<Text>();
		_RIPText.color = new Color(1f, 1f, 1f, 0.791f);
		_RIPText.font = GetFont();
		_RIPText.fontSize = 16;
		_RIPText.enabled = true;
		_RIPText.alignment = (TextAnchor)3;
		Outline outline = gameObject.AddComponent<Outline>();
		outline.effectColor = Color.black;
		outline.effectDistance = new Vector2(1f, -1f);
		outline.useGraphicAlpha = true;
		outline.useGUILayout = true;
		outline.enabled = true;
	}

	private string GetCurrentTimeText()
	{
		if (!EnvMan.instance)
		{
			return null;
		}
		return PlayerPatcher.timing;
	}

	private string GetCurrentTPText()
	{
		if (!EnvMan.instance)
		{
			return null;
		}
		return PlayerPatcher.tpcount;
	}

	private string GetCurrentCPText()
	{
		if (!EnvMan.instance)
		{
			return null;
		}
		return PlayerPatcher.cpcount;
	}

	private string GetCurrentRIPText()
	{
		if (!EnvMan.instance)
		{
			return null;
		}
		return FastRespawn.ripcount;
	}

	private string GetCurrentKeyBind()
	{
		if (!EnvMan.instance)
		{
			return null;
		}
		return KeyBindString;
	}

	private void UpdateKeyBind()
	{
		RectTransform component = _KeyBindText.GetComponent<RectTransform>();
		component.anchoredPosition = new Vector2((150f / 4f - 10f), -10f);
		component.sizeDelta = new Vector2(200f, 600f);
		_KeyBindText.alignment = TextAnchor.UpperLeft;
		_KeyBindText.verticalOverflow = VerticalWrapMode.Overflow;
		_KeyBindText.color = new Color(1f, 1f, 1f, 0.791f);
		_KeyBindText.fontSize = 16;
		_KeyBindText.text = GetCurrentKeyBind();
		Outline component2 = _KeyBindText.GetComponent<Outline>();
		component2.enabled = true;
		component2.effectColor = Color.black;
	}


	private void UpdateTime()
	{
		RectTransform component = _timeText.GetComponent<RectTransform>();
		component.anchoredPosition = new Vector2((150f / 4f - 10f), 10f);
		component.sizeDelta = new Vector2(400f / 2f, 30f);
		_timeText.alignment = (TextAnchor)3;
		_timeText.color = new Color(1f, 1f, 1f, 0.791f);
		_timeText.fontSize = 16;
		_timeText.text = GetCurrentTimeText();
		Outline component2 = _timeText.GetComponent<Outline>();
		component2.enabled = true;
		component2.effectColor = Color.black;
	}

	private void UpdateTP()
	{
		RectTransform component = _TPText.GetComponent<RectTransform>();
		component.anchoredPosition = new Vector2((300f / 4f - 10f), -10f);
		component.sizeDelta = new Vector2(400f / 2f, 30f);
		_TPText.alignment = (TextAnchor)3;
		_TPText.color = new Color(1f, 1f, 1f, 0.791f);
		_TPText.fontSize = 16;
		_TPText.text = GetCurrentTPText();
		Outline component2 = _TPText.GetComponent<Outline>();
		component2.enabled = true;
		component2.effectColor = Color.black;
	}

	private void UpdateCP()
	{
		RectTransform component = _CPText.GetComponent<RectTransform>();
		component.anchoredPosition = new Vector2((50f / 4f - 10f), -10f);
		component.sizeDelta = new Vector2(400f / 2f, 30f);
		_CPText.alignment = (TextAnchor)3;
		_CPText.color = new Color(1f, 1f, 1f, 0.791f);
		_CPText.fontSize = 16;
		_CPText.text = GetCurrentCPText();
		Outline component2 = _CPText.GetComponent<Outline>();
		component2.enabled = true;
		component2.effectColor = Color.black;
	}

	private void UpdateRIP()
	{
		RectTransform component = _RIPText.GetComponent<RectTransform>();
		component.anchoredPosition = new Vector2((550f / 4f - 10f), -10f);
		component.sizeDelta = new Vector2(400f / 2f, 30f);
		_RIPText.alignment = (TextAnchor)3;
		_RIPText.color = new Color(1f, 1f, 1f, 0.791f);
		_RIPText.fontSize = 16;
		_RIPText.text = GetCurrentRIPText();
		Outline component2 = _RIPText.GetComponent<Outline>();
		component2.enabled = true;
		component2.effectColor = Color.black;
	}

	public static Font GetFont()
	{
		Font[] source = Resources.FindObjectsOfTypeAll<Font>();
		Font val = source.FirstOrDefault((Font f) => ((UnityEngine.Object)(object)f).name == "AveriaSansLibre-Bold");
		if ((UnityEngine.Object)(object)val == null)
		{
			return source.FirstOrDefault((Font f) => ((UnityEngine.Object)(object)f).name == "AveriaSansLibre-Bold");
		}
		return val;
	}

	///////////////////////////////////////////////////////////////////////Patching Code
	[HarmonyPatch(typeof(Character), "OnCollisionStay")]
	public static class WallGround
	{

		public static bool WallContact = false;
		public static bool AnyContact = false;
		public static bool Shoot;
		public static float number1;
		public static float number2;
		public static float number3;
		public static float shootnumber = 0;
		public static Vector3 GrabCurrentVelo;
		public static Vector3 ShotVector;
		public static float bouncenumber = 0;
		public static float bouncenumber2;
		public static float bouncenumber3;
		public static GameObject Player;

		public static Rigidbody rb;
		public static float thrust = 60f;

		public static Collider collider1;

		private static void Prefix(Collision collision, Character __instance, ref Rigidbody ___m_body, ref float ___m_airControl, ref Vector3 ___m_currentVel, ref Vector3 ___m_moveDir, ref float ___m_runSpeed, ref float ___m_slippage, ref float ___m_acceleration, ref Vector3 ___m_lastGroundPoint)
		{

			int mask2 = LayerMask.GetMask("Default", "static_solid", "terrain", "vehicle", "character", "piece", "character_net", "viewblock");
			int mask3 = LayerMask.GetMask("piece");





			ContactPoint[] contacts1 = collision.contacts;


			foreach (ContactPoint contact in contacts1)
			{
				// Check if the contact point's collider has a GameObject associated with it
				GameObject otherGameObject = contact.otherCollider.gameObject;

				if (otherGameObject != null)
				{
					// Try to get the 'Piece' component in the parent of the other GameObject
					Piece pieceComponent = otherGameObject.GetComponentInParent<Piece>();

					if (pieceComponent != null)
					{
						// Check the 'm_name' property of the 'Piece' component

						if (pieceComponent.m_name != null && pieceComponent.m_name.ToString() != "$piece_blackmarble_floor_triangle")
						{
							// The specific object is being touched
							AnyContact = true;

							break; // Exit the loop since we found what we're looking for
						}
					}
				}
			}


			for (int i = 0; i < contacts1.Length; i++)
			{
				ContactPoint contactPoint1 = contacts1[i];
				number2 = contactPoint1.point.y;
				number1 = contactPoint1.point.y - __instance.transform.position.y;



				if (PlayerPatcher.piecename == "$piece_table_oak")
				{
					if (Physics.Raycast(__instance.transform.position, __instance.transform.forward, out var hitInfo3, 1f, mask3))
					{
						shootnumber += 2;
						shootnumber = Mathf.Clamp(shootnumber, 0, 50);
						float factor = shootnumber / 50f;

						___m_runSpeed = Mathf.Lerp(7, 50, factor);
						___m_acceleration = Mathf.Lerp(1, 50, factor);
						float runspeeds = ___m_runSpeed;
						___m_airControl = 0.01f;
						Shoot = true;
						ShotVector = ___m_currentVel;

					}
				}

				if (number1 > 0.4f && Physics.Raycast(__instance.transform.position, __instance.transform.forward, out var hitInfo2, 0.6f, mask2))
				{
					GrabCurrentVelo = contactPoint1.normal;
					GrabCurrentVelo *= 8f;
					WallContact = true;
				}
				else
				{
					WallContact = false;
				}

			}
			//if (PlayerPatcher.piecename != "$piece_ironfloor" || !(bouncenumber < 1f))
			//{
			//	return;
			//}


		}
	}



	[HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
	public static class Walk
	{
		public static float Resetcounter;
		private static bool Prefix()
		{
			if (PlayerPatcher.piecename == "$piece_table_oak" && WallGround.ShotVector.magnitude > 20f)
			{
				return false;
			}
			if (TrackGrapple.HitVectorGrapple.magnitude > 0 && TrackGrapple.block != null && WallCheck.IsGrappling == true)
			{
				if (Resetcounter <= 0)
				{
					Player.m_localPlayer.gameObject.GetComponent<Character>().SetMoveDir(Vector3.zero);
					Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
					Resetcounter += 1;
				}
				return false;
			}
			if (!WallCheck.IsGrappling || WallCheck.IsSwinging)
			{
				return true;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Character), "CanWallRun")]
	public static class BounceWallRun2
	{
		private static void Postfix(ref bool __result)
		{
			
				__result = false;
			
		}
	}

	

	[HarmonyPatch(typeof(Projectile), "OnHit")]
	public static class TrackGrapple
	{
		public static WearNTear block = new WearNTear();
		public static Vector3 velocityY;
		public static Vector3 velocityXZ;
		public static Vector3 HitVectorGrapple;
		public static float overshootYAxis = 1f;


		private static void Prefix(Projectile __instance, Collider collider, Vector3 hitPoint)
		{
			//if(collider.gameObject.GetComponent<WearNTear>())
			//         {
			//	block = collider.gameObject.GetComponent<WearNTear>();

			//}
			if (Player.m_localPlayer != null && Player.m_localPlayer.gameObject.GetComponent<Character>() == __instance.m_owner)
			{
				if (Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin")
				{
					if (collider.transform.gameObject.GetComponent<Rigidbody>() != null && collider != null)

					{
						if (CanHook == false || collider.transform.gameObject.GetComponentInChildren<Piece>().m_name.ToString() != "$piece_blackmarble2x2x2" || collider.transform.gameObject.GetComponentInChildren<Piece>().m_name.ToString() != "$piece_blackmarble2x1x1" || collider.transform.gameObject.GetComponentInChildren<Piece>().m_name.ToString() != "$piece_blackmarble_column_1")
						{
							block = null;
						}




						if (CanHook == true && collider.transform.gameObject.GetComponent<Piece>().m_name.ToString() == "$piece_blackmarble2x2x2" || collider.transform.gameObject.GetComponent<Piece>().m_name.ToString() == "$piece_blackmarble2x1x1" || collider.transform.gameObject.GetComponent<Piece>().m_name.ToString() == "$piece_blackmarble_column_1")
						{
							block = (collider ? Projectile.FindHitObject(collider) : null)?.GetComponent<WearNTear>();

							HitVectorGrapple = hitPoint;

							float gravity = Physics.gravity.y;
							float displacementY = HitVectorGrapple.y - Handpoint.rightHand.y;
							Vector3 displacementXZ = new Vector3(HitVectorGrapple.x - Handpoint.rightHand.x, 0f, HitVectorGrapple.z - Handpoint.rightHand.z);
							Vector3 lowerstPoint = new Vector3(Handpoint.rightHand.x, Handpoint.rightHand.y, Handpoint.rightHand.z);
							float grapplePointRelativeYPos = HitVectorGrapple.y - lowerstPoint.y;
							float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

							if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

							velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * highestPointOnArc);
							velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * highestPointOnArc / gravity)
								+ Mathf.Sqrt(2 * (displacementY - highestPointOnArc) / gravity));

							PlayerPatcher.HookTimerOn = true;
						}
					}
				}
			}
			//if (block == null)
			//{
			//	WallCheck.ln22.enabled = false;
			//	return;
			//}
			//if (block.GetComponent<WearNTear>())
			//{

			//	WallCheck.ln22.enabled = true;

			//}
		}
	}


	[HarmonyPatch(typeof(Projectile), "UpdateRotation")]
	public static class TrackGrapple2
	{
		//public static WearNTear block = new WearNTear();
		//public static Vector3 velocityY;
		//public static Vector3 velocityXZ;
		//public static Vector3 HitVectorGrapple;
		//public static float overshootYAxis = 2f;

		public static RaycastHit[] sphereCastHit;
		public static float CounterSphere;

		private static void Prefix(ref float ___m_hitNoise, Character ___m_owner)
		{
			//XXX
			Debug.Log(CanHook);
			if (Player.m_localPlayer != null && Player.m_localPlayer.gameObject.GetComponent<Character>() == ___m_owner && CanHook == true)
			{
				if (Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin" && WallCheck.jointSwing == null)
				{

					int mask3 = LayerMask.GetMask("piece");

					//Physics.SphereCast(Handpoint.rightHand + Vector3.up, 1.5f, GameCamera.instance.transform.forward, out sphereCastHit, 50f, mask3);
					sphereCastHit = Physics.SphereCastAll(GameCamera.instance.transform.position, 1f, GameCamera.instance.transform.forward, 80f, mask3);
					float[] distances = new float[sphereCastHit.Length];

					for (int i = 0; i < sphereCastHit.Length; i++)
					{

						distances[i] = sphereCastHit[i].distance;
					}

					Array.Sort(distances, sphereCastHit);
					foreach (RaycastHit hit in sphereCastHit)
					{

						if (CounterSphere <= 0 && hit.point != Vector3.zero && TrackGrapple.HitVectorGrapple == Vector3.zero && TrackGrapple.block == null && Vector3.Distance(Handpoint.rightHand, hit.transform.position) > 3f && hit.transform.gameObject.GetComponent<Rigidbody>() != null && (hit.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x2x2" || hit.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x1x1" || hit.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_column_1") || CounterSphere <= 0 && hit.point != Vector3.zero && TrackGrapple.HitVectorGrapple == null && TrackGrapple.block == null && Vector3.Distance(Handpoint.rightHand, hit.transform.position) > 2f && hit.transform.gameObject.GetComponent<Rigidbody>() != null && (hit.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x2x2" || hit.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x1x1" || hit.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_column_1"))
						{


							TrackGrapple.block = hit.collider.GetComponentInParent<WearNTear>();
							TrackGrapple.HitVectorGrapple = hit.point;

							float gravity = Physics.gravity.y;
							float displacementY = TrackGrapple.HitVectorGrapple.y - Handpoint.rightHand.y;
							Vector3 displacementXZ = new Vector3(TrackGrapple.HitVectorGrapple.x - Handpoint.rightHand.x, 0f, TrackGrapple.HitVectorGrapple.z - Handpoint.rightHand.z);
							Vector3 lowerstPoint = new Vector3(Handpoint.rightHand.x, Handpoint.rightHand.y, Handpoint.rightHand.z);
							float grapplePointRelativeYPos = TrackGrapple.HitVectorGrapple.y - lowerstPoint.y;
							float highestPointOnArc = grapplePointRelativeYPos + TrackGrapple.overshootYAxis;
							if (grapplePointRelativeYPos < 0) highestPointOnArc = TrackGrapple.overshootYAxis;
							TrackGrapple.velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * highestPointOnArc);
							TrackGrapple.velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * highestPointOnArc / gravity)
								+ Mathf.Sqrt(2 * (displacementY - highestPointOnArc) / gravity));
							PlayerPatcher.HookTimerOn = true;
							CounterSphere = 1;

						}
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(Character), "UpdateWalking")]
	public static class StopWalkWhileSwing
	{
		public static float TransitionCounter = 0f;
		private static void Postfix(ref Vector3 ___m_currentVel, ref Rigidbody ___m_body, ref float ___m_runSpeed, ref float ___m_walkSpeed, ref float ___m_speed)
		{
			if ((WallCheck.IsGrappling == false && WallCheck.IsSwinging == true && TransitionCounter <= 0) || HasTouchedIce == true || HasTouchedSurf == true)
			{
				if (Input.GetKey(KeyCode.W))
				{

					Player.m_localPlayer.GetComponent<Character>().m_currentVel = Player.m_localPlayer.GetComponent<Character>().m_body.velocity;
					TransitionCounter = 1;
				}
				if (Input.GetKey(KeyCode.D))
				{
					Player.m_localPlayer.GetComponent<Character>().m_currentVel = Player.m_localPlayer.GetComponent<Character>().m_body.velocity;
					TransitionCounter = 1;
				}

				if (Input.GetKey(KeyCode.A))
				{
					Player.m_localPlayer.GetComponent<Character>().m_currentVel = Player.m_localPlayer.GetComponent<Character>().m_body.velocity;
					TransitionCounter = 1;

				}
				if (Input.GetKey(KeyCode.Space))
				{
					Player.m_localPlayer.GetComponent<Character>().m_currentVel = Player.m_localPlayer.GetComponent<Character>().m_body.velocity;
					TransitionCounter = 1;

				}
				if (Input.GetKey(KeyCode.S))
				{
					Player.m_localPlayer.GetComponent<Character>().m_currentVel = Player.m_localPlayer.GetComponent<Character>().m_body.velocity;
					TransitionCounter = 1;

				}

				___m_runSpeed = 18f;
				___m_speed = 15f;

			}
			if (WallCheck.IsGrappling == true && WallCheck.IsSwinging == false)
			{

				___m_runSpeed = 18f;
				___m_speed = 15f;
			}

		}
	}

	[HarmonyPatch(typeof(Character), "GetSlideAngle")]
	public static class WallRunning
	{
		private static bool Prefix(ref float __result, ref bool ___m_wallRunning, ref float ___m_slippage, ref Rigidbody ___m_body)
		{
			__result = 70f;
			return false;
		}
	}

	[HarmonyPatch(typeof(Character), "CanMove")]
	public static class StickWall
	{
		private static bool Prefix()
		{
			if (WallCheck.TouchingIce == true)
			{
				if (HasWallJumped2 != true && WallCheck.TouchingIceObj.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") == 0 && !Player.m_localPlayer.IsOnGround())
				{
					//return false;
				}
				else if (HasWallJumped2 != true && WallCheck.TouchingIceObj.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") > 0)
				{
					return true;
				}
			}
			return true;
		}
	}

	//[HarmonyPatch(typeof(WearNTear), "UpdateWear")]
	//public static class BoxCast
	//{
	//	public static WearNTear componentInParent;
	//	public static List<WearNTear.BoundData> BoundObj = new List<WearNTear.BoundData>();
	//	public static Collider[] AttachedPiece = new Collider[128];
	//	private static void Prefix(Collider[] ___m_tempCollider)
	//	{



	//		for (int i = 0; i < ___m_tempCollider.Length ; i++)
	//		{
	//			Collider collider = ___m_tempCollider[i];
	//			Debug.Log("attach" + collider);
	//		}





	//           //	foreach (WearNTear.BoundData bound in BoundObj)
	//           //	{
	//           //		int num = Physics.OverlapBoxNonAlloc(bound.m_pos, bound.m_size, AttachedPiece, bound.m_rot, mask3);
	//           //		for (int i = 0; i < num; i++)
	//           //		{
	//           //			Collider collider = AttachedPiece[i];

	//           //			componentInParent = collider.GetComponentInParent<WearNTear>();

	//           //		}
	//           //	}


	//       }
	//}

	[HarmonyPatch(typeof(Character), "CustomFixedUpdate")]
	public static class BounceII
	{
		public static Vector3 m_EulerAngleVelocity2 = new Vector3(10, 0, 10);


		public static Piece HitPiece;
		public static float RayDownLength2;


		public static float counter1 = 0;
		public static float counter2 = 0;

		public static float yPos = 0;
		public static float maxYPos = 0;

		public static float spring = 0;

		public static bool mover = false;
		public static bool TouchingStickyRoof = false;

		public static Collider[] colpiece22 = new Collider[2000];

		public static string[] destroname = new string[2000];
		public static string[] piecename = new string[2000];
		public static string[] piecename22 = new string[2000];


		public static Vector3[] spawnPosPiece = new Vector3[2000];
		public static Quaternion[] spawnRotPiece = new Quaternion[2000];
		public static Vector3 m_EulerAngleVelocity = new Vector3(0, 10, 0);
		public static Vector3 projectedSurfaceNormal = new Vector3(0, 0, 0);

		public static bool[] touched = new bool[2000];

		public static float[] distanceplat = new float[2000];

		public static List<Rigidbody> rb2 = new List<Rigidbody>();
		public static List<Vector3> rbpos2 = new List<Vector3>();

		public static Collider[] colpiecefromplayerRoof = new Collider[2000];

		public static float ScaleFactor = 0;


		public static bool AllowSurfing = false;
		public static double currentValue = 0.0;

		private static float boostDuration = 1.0f; // Adjust this duration as needed

		private static float initialBoostMultiplier = 1.0f;
		private static float finalBoostMultiplier = 1.3f;
		public static bool hasBounced = false;
		public static float BounceLimiter = 0f;
		public static float trampolineValue = 0f;
		public static Vector3 trampolineHitPoint = new Vector3(0f, 0f, 0f);
		public static Vector3 TrampPos = Vector3.zero;
		public static bool DvegarBounceAllowed = false;
		public static bool DvegarJumpAllowed = false;
		public static bool ActivateJump = false;
		public static bool IsBouncing = false;
		




		//public static Rigidbody attachedRigidbody;

		private static void Postfix(ref Rigidbody ___m_body, ref float ___m_slippage, ref float ___m_jumpForce, ref Player __instance, ref Vector3 ___m_lastGroundPoint, ref Vector3 ___m_moveDir, ref Vector3 ___m_currentVel, ref float ___m_jumpForceForward, ref Vector3 ___m_lastGroundNormal)
		{


			if (Player.m_localPlayer != null && __instance == Player.m_localPlayer)
			{
				
				int maskTrampoline = LayerMask.GetMask("piece");
				if (WallCheck.TouchingIce == false && WallCheck.WallSlide == true && WallCheck.within0_5fRange == true && !Physics.Raycast(Player.m_localPlayer.GetComponent<Character>().m_body.position, Vector3.up * -1, out var hitInfoWallSlide, 2f, maskTrampoline) && Player.m_localPlayer.GetComponent<Character>().m_moveDir.magnitude > 0f)
				{
					Player.m_localPlayer.GetComponent<Character>().m_body.velocity = Vector3.Lerp(Player.m_localPlayer.GetComponent<Character>().m_body.velocity, Vector3.zero, 4 * Time.fixedDeltaTime);
				}

				if ((PlayerPatcher.piecename == "$piece_ironwall" && PlayerPatcher.TrackPiece.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") < 0) || (PlayerPatcher.piecename == "$piece_dvergr_metal_wall" && PlayerPatcher.TrackPiece.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") < 0))
				{
					Debug.Log("nullValue");
					Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, 0f, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);

				}



				if (Physics.Raycast(Player.m_localPlayer.GetComponent<Character>().m_body.position + Vector3.up * 0.2f, Vector3.up * -1, out var hitInfoTramp, 6f, maskTrampoline))
				{

					if (hitInfoTramp.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") > 0 && hitInfoTramp.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") <= 15)
					{
						IsBouncing = true;
						trampolineHitPoint = hitInfoTramp.point;
						trampolineValue = hitInfoTramp.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey");

						if (TrampPos != hitInfoTramp.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()))
						{
							DvegarBounceAllowed = true;
							TrampPos = hitInfoTramp.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3());
						}
						// Define the minimum and maximum values for TrampolinFloatValueKey

						//CHANGE SCALING FACTOR & make trampoline more forgiving
						float minTrampolineValue = 1f;
						float maxTrampolineValue = 15f;

						// Define the scaling factor range
						float minScalingFactor = 0.9f;  // Adjust this value as needed
						float maxScalingFactor = 1.0f;  // Adjust this value as needed

						// Calculate the scaled factor based on TrampolinFloatValueKey
						float scalingFactor = minScalingFactor + (maxScalingFactor - minScalingFactor) * (trampolineValue - minTrampolineValue) / (maxTrampolineValue - minTrampolineValue);

						var HitInfoTrampNormY = hitInfoTramp.normal;

						var JumpLoadUpVector = HitInfoTrampNormY * PlayerPatcher.maxYVelo * Jump_Patch.JumpLoadUpFactor;

						Vector3 minimumValue = new Vector3(0f, 20f, 0f);
						var JumpLoadUpVectorNormTrampValue = JumpLoadUpVector * scalingFactor;
						JumpLoadUpVectorNormTrampValue.y = Mathf.Max(JumpLoadUpVectorNormTrampValue.y, minimumValue.y);

						if (Player.m_localPlayer.GetComponent<Character>().IsOnGround())
						{
							DvegarJumpAllowed = false;
						}

						if (/*Jump_Patch.hasjumped &&*/ isJumping && BounceLimiter == 0f)
						{
							if ((ActivateJump && Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y > 0f && hitInfoTramp.distance <= 6f && hitInfoTramp.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_ironwall") || (ActivateJump && !ValueChecker.IsDecreasing(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y) && hitInfoTramp.distance <= 6f && hitInfoTramp.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_dvergr_metal_wall" && DvegarJumpAllowed == true))
							{


								Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, JumpLoadUpVectorNormTrampValue.y, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);

								BounceLimiter = 1f;
								DvegarJumpAllowed = false;
							}
							else if ((ActivateJump && Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y <= 0f && hitInfoTramp.distance <= 2f && hitInfoTramp.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_ironwall") || (ActivateJump && ValueChecker.IsDecreasing(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y) && hitInfoTramp.distance <= 2f && hitInfoTramp.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_dvergr_metal_wall" && DvegarJumpAllowed == true))
							{


								Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, JumpLoadUpVectorNormTrampValue.y, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);

								BounceLimiter = 1f;
								DvegarJumpAllowed = false;
							}
						}
						else if ((hitInfoTramp.distance <= 1f && hasBounced == false && hitInfoTramp.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_ironwall") || (hitInfoTramp.distance <= 1f && hasBounced == false && hitInfoTramp.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_dvergr_metal_wall" && DvegarBounceAllowed == true))
						{

							Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, HitInfoTrampNormY.y * PlayerPatcher.maxYVelo * 0.9f, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);
							DvegarBounceAllowed = false;
							DvegarJumpAllowed = true;
							ActivateJump = true;
							hasBounced = true;
						}




					}
				}







				if (PlayerPatcher.piecename == "$piece_ironfloor" && WallGround.bouncenumber == 0 && UpdateGroundContact_Patch.jumpvelocity2 > 10)

				{
					Player.m_localPlayer.GetComponent<Character>().m_body.AddExplosionForce(UpdateGroundContact_Patch.jumpvelocity2 * 10, ___m_lastGroundPoint, 10f, 1f, ForceMode.VelocityChange);
					WallGround.bouncenumber++;
				}

				//Test For Velocity Clamp bug - Fixed
				//if(HasTouchedSurf == true || HasTouchedSurf2 == true)
				//            {
				//	Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, 0, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);

				//}

				if (((WallCheck.TouchingSurf == true || HasTouchedSurf == true || HasTouchedSurf2 == true) && Jump_Patch.hasjumped == true) || (WallCheck.IsSwinging == true && TrackGrapple.block != null && TrackGrapple.HitVectorGrapple != null && WallCheck.jointSwing != null) || (PlayerPatcher.HasSwung && TrackGrapple.block == null))

				{
					Player.m_localPlayer.GetComponent<Character>().m_airControl = 0.035f;


					if (Player.m_localPlayer.GetComponent<Character>().m_body.velocity.magnitude >= 70f && WallCheck.TouchingSurf == true)
					{

						Player.m_localPlayer.GetComponent<Character>().m_body.velocity = Vector3.ClampMagnitude(Player.m_localPlayer.GetComponent<Character>().m_body.velocity, 70f + Player.m_localPlayer.GetComponent<Character>().m_body.velocity.magnitude * 0.04f);
					}
					if (Player.m_localPlayer.GetComponent<Character>().m_body.velocity.magnitude >= 100f && WallCheck.TouchingSurf == true)
					{
						Player.m_localPlayer.GetComponent<Character>().m_body.velocity = Vector3.ClampMagnitude(Player.m_localPlayer.GetComponent<Character>().m_body.velocity, 100f);
					}
					// project the velocity onto the movevector
					Vector3 projVel = Vector3.Project(Player.m_localPlayer.GetComponent<Rigidbody>().velocity, Player.m_localPlayer.GetComponent<Character>().m_moveDir);

					// check if the movevector is moving towards or away from the projected velocity
					bool isAway = Vector3.Dot(Player.m_localPlayer.GetComponent<Character>().m_moveDir, projVel) <= 0f;

					// only apply force if moving away from velocity or velocity is below MaxAirSpeed
					if (projVel.magnitude < 50f || isAway)
					{
						// calculate the ideal movement force
						Vector3 vc = Player.m_localPlayer.GetComponent<Character>().m_moveDir.normalized * 5;

						// cap it if it would accelerate beyond MaxAirSpeed directly.
						if (!isAway)
						{
							vc = Vector3.ClampMagnitude(vc, 50f - projVel.magnitude);
							//___m_body.velocity = new Vector3(___m_body.velocity.x, 0, ___m_body.velocity.z);

						}
						else
						{

							vc = Vector3.ClampMagnitude(vc, 50f + projVel.magnitude);


						}

						// Apply the force



						var reductionfactor = 1f;

						if (WallCheck.DisableSurf == true)
						{
							reductionfactor = 0.1f;
							if (ValueChecker.IsDecreasing(Player.m_localPlayer.GetComponent<Character>().m_body.position.y) == false)
							{

								Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y * 0.99f, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);
							}
						}
						if (WallCheck.DisableSurf == false)
						{
							//Debug.Log("Eror1123");
							Player.m_localPlayer.GetComponent<Rigidbody>().AddForce(vc * 0.13f * reductionfactor, ForceMode.VelocityChange);
						}
					}
				}

				int mask254 = LayerMask.GetMask("piece");

				if (WallCheck.boost == true)
				{


					// Calculate the sigmoidal boost progression
					float t = WallCheck.boostTimer;
					float boostMultiplier = Mathf.Lerp(initialBoostMultiplier, finalBoostMultiplier, SigmoidCalculator.Sigmoid(t));

					// Check if the character and its body are valid
					if (Player.m_localPlayer.GetComponent<Character>() != null && Player.m_localPlayer.GetComponent<Character>().m_body != null)
					{


						Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(
							Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x * boostMultiplier,
							Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y,
							Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z * boostMultiplier
						);
					}

				}



				if (WallCheck.TouchingIce == true)
				{

					if (Physics.Raycast(__instance.transform.position, Vector3.up, out var hitInfo556, 2.5f, mask254))
					{

						if (hitInfo556.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_crystalwall1x1")
						{
							//Debug.Log("1 rooftouch: " + TouchingStickyRoof);
							TouchingStickyRoof = true;


						}
					}
					else if (Physics.SphereCast(__instance.transform.position, 3f, Vector3.up, out var HitinfoSphere2, 2.5f, mask254))
					{
						if (HitinfoSphere2.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_crystalwall1x1")
						{
							//Debug.Log("2 rooftouch: " + TouchingStickyRoof);
							TouchingStickyRoof = true;


						}
					}

					else
					{
						//Debug.Log("3 rooftouch: " + TouchingStickyRoof);
						TouchingStickyRoof = false;
					}

					//XXXXX
					___m_lastGroundNormal = new Vector3(0, 1, 0);
					int mask22 = LayerMask.GetMask("piece");


					if (WallCheck.TouchingIceObj.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") == 0)
					{


						if (Physics.CheckSphere(__instance.transform.position, 1f, mask22) && Physics.Raycast(__instance.transform.position, Vector3.up, out var hitInfo55, 4, mask22))
						{

							//Debug.Log("1");

							Player.m_localPlayer.GetComponent<Character>().m_body.useGravity = false;
							Player.m_localPlayer.GetComponent<Rigidbody>().useGravity = false;

						}

						else if (Physics.Raycast(__instance.transform.position, Vector3.up, 4f, mask22))
						{
							//Debug.Log("2");


							Player.m_localPlayer.GetComponent<Character>().m_body.useGravity = false;
							Player.m_localPlayer.GetComponent<Rigidbody>().useGravity = false;
							Vector3 velocity = new Vector3(0, 0, 0);
							Player.m_localPlayer.GetComponent<Character>().m_body.velocity = Vector3.SmoothDamp(Player.m_localPlayer.GetComponent<Character>().m_body.velocity, Vector3.down, ref velocity, 4 * Time.fixedDeltaTime);
							___m_jumpForceForward = 15f;


						}



						else if (Physics.SphereCast(__instance.transform.position, 3f, Vector3.up, out var HitinfoSphere, 4f, mask22) && !Physics.CheckSphere(__instance.transform.position, 1f, mask22))
						{
							//Debug.Log("2.5");


							Player.m_localPlayer.GetComponent<Character>().m_body.useGravity = false;
							Player.m_localPlayer.GetComponent<Rigidbody>().useGravity = false;
							Vector3 velocity = new Vector3(0, 0, 0);
							Player.m_localPlayer.GetComponent<Character>().m_body.velocity = Vector3.SmoothDamp(Player.m_localPlayer.GetComponent<Character>().m_body.velocity, Vector3.down, ref velocity, 4 * Time.fixedDeltaTime);
							___m_jumpForceForward = 15f;

						}

						else if (Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y <= 0f && Physics.CheckSphere(__instance.transform.position, 1f, mask22))

						{
							colpiecefromplayerRoof = Physics.OverlapSphere(__instance.transform.position, 1f, mask22);
							foreach (Collider colliderfromplayer in colpiecefromplayerRoof)
							{
								if (colliderfromplayer.GetComponentInParent<Piece>().m_name.ToString() != "$piece_crystalwall1x1")
								{
									continue;
								}
								if (colliderfromplayer.GetComponentInParent<Piece>().m_name.ToString() == "$piece_crystalwall1x1")
								{
									if (Physics.Raycast(__instance.transform.position, Vector3.down, out var hitInfo5555, 0.3f, mask22))
									{
										//Debug.Log("3");
										Player.m_localPlayer.GetComponent<Character>().m_body.useGravity = false;
										Player.m_localPlayer.GetComponent<Rigidbody>().useGravity = false;
										Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(0, 0, 0);
									}
									if (Physics.CheckBox(__instance.transform.position, new Vector3(1, 0.1f, 1), __instance.transform.rotation, mask22))
									{
										//Debug.Log("3");
										Player.m_localPlayer.GetComponent<Character>().m_body.useGravity = false;
										Player.m_localPlayer.GetComponent<Rigidbody>().useGravity = false;
										Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(0, 0, 0);
									}
								}
							}


						}


					}
				}
				//Debug.Log("TS " + WallCheck.TouchingSurf  + " AS " + AllowSurfing + " ES " + EndSurfStart + " BS " + WallCheck.BackupSurf);




				if (WallCheck.SurfEject == true && Jump_Patch.hasjumped == true)
				{
					Player.m_localPlayer.GetComponent<Character>().m_body.AddForce(Vector3.up * 2, ForceMode.VelocityChange);
				}

				if (WallCheck.TouchingSurf == true)
				{

					int mask22 = LayerMask.GetMask("piece");

					if (WallCheck.TouchingSurfObj.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") >= 0 && WallCheck.DisableSurf == false && Jump_Patch.hasjumped == true)
					{


						//xxx check if +vector.up make sense

						int mask2 = LayerMask.GetMask("piece");
						if (WallCheck.TouchingSurf == true && Physics.Raycast(Player.m_localPlayer.GetComponent<Character>().m_body.position + new Vector3(0, 0.1f, 0), Vector3.up * -1, out var hitInfo5, 2f, mask2) && AllowSurfing == true)
						{
							WallCheck.BackupSurf = false;
							//Physics.gravity = new Vector3(0, -20, 0);



							if (hitInfo5.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") >= 0)
							{
								if (OldSlopeNorm == Vector3.zero)
								{
									OldSlopeNorm = hitInfo5.normal;
								}

								if ((OldSlopeNorm != hitInfo5.normal && PlayerPatcher.GraceSurfSlopeTimer <= 0.1f) || (OldSlopeNorm != hitInfo5.normal && Player.m_localPlayer.GetComponent<Character>().m_body.velocity.magnitude <= 10f))
								{
									Debug.Log("ResetSlope");
									OldSlopeNorm = hitInfo5.normal;
								}


								if (Vector3.Angle(OldSlopeNorm, hitInfo5.normal) < 50f)
								{

									float angle = Vector3.Angle(Vector3.up, hitInfo5.normal);


									if (angle < 60 && angle != 0)
									{

										WallCheck.OnSlope = true;
									}
									else
									{
										//hasjumped -> change float -> feed into velocity	

										WallCheck.OnSlope = false;
									}
									if (WallCheck.OnSlope == true)
									{


										Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hitInfo5.normal);
										Vector3 vectorAlongSlope = rotation * Vector3.down;

										if (Vector3.Dot(vectorAlongSlope, Vector3.up) > 0)
										{
											vectorAlongSlope *= -0.1f;
										}
										Debug.Log("VAS " + vectorAlongSlope);
										Player.m_localPlayer.GetComponent<Rigidbody>().AddForce((vectorAlongSlope * -0.5f), ForceMode.VelocityChange);



										// Transform the local peak position to world coordinates
										//Vector3 truePeak = hitInfo5.collider.gameObject.transform.TransformPoint(localPeak);


										Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, 0f, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);

										Vector3 rayDir = (hitInfo5.point - Player.m_localPlayer.GetComponent<Character>().m_body.position).normalized;

										float x = hitInfo5.distance - 1.3f;
										float springforce = x * 40000;
										Vector3 forceCombined = rayDir * springforce;

										yPos = Player.m_localPlayer.GetComponent<Character>().m_body.position.y;



										if (yPos > maxYPos)
										{
											maxYPos = Player.m_localPlayer.GetComponent<Character>().m_body.position.y;
										}



										if (Player.m_localPlayer.GetComponent<Character>().m_moveDir.magnitude > 0)
										{
											ScaleFactor += (Time.fixedDeltaTime / 4) * (((maxYPos - yPos) / 200));
										}
										if (Player.m_localPlayer.GetComponent<Character>().m_moveDir.magnitude <= 0 && ScaleFactor >= 0f)
										{
											ScaleFactor -= (Time.fixedDeltaTime / 2) * (((maxYPos - yPos) / 100));
										}



										ScaleFactor = Mathf.Clamp(ScaleFactor, 1f, 1.02f);



										Vector3 velocity = Player.m_localPlayer.GetComponent<Character>().m_body.velocity;
										velocity.x *= ScaleFactor;
										velocity.z *= ScaleFactor;
										Player.m_localPlayer.GetComponent<Character>().m_body.velocity = velocity;

										//Player.m_localPlayer.GetComponent<Character>().m_body.velocity *= ScaleFactor;




										//?????
										Player.m_localPlayer.GetComponent<Character>().m_body.AddForce(forceCombined);






									}
								}
							}
						}

					}



				}
				else
				{
					Physics.gravity = new Vector3(0, -20, 0);
				}




				if (Jump_Patch.hasjumped != true && PlayerPatcher.piecename3 == "$piece_blackmarble_floor" && PlayerPatcher.piecename == "$piece_blackmarble_floor" && PlayerPatcher.TrackTerrain == null || PlayerPatcher.piecename3 == "$piece_blackmarble_floor" && PlayerPatcher.piecename == "$piece_blackmarble_floor" && PlayerPatcher.TrackTerrain == null)
				{

					if (UpdateGroundContact_Patch.OnIce == true)
					{
						Player.m_localPlayer.GetComponent<Character>().m_body.AddForce(UpdateGroundContact_Patch.MoveVector, ForceMode.VelocityChange);
					}
				}

				if (WallCheck.IsSwinging == true && TrackGrapple.block != null && TrackGrapple.HitVectorGrapple != null && WallCheck.jointSwing != null)
				{


					if (Input.GetKey(KeyCode.D))
					{

						Player.m_localPlayer.GetComponent<Character>().m_body.AddForce(Player.m_localPlayer.GetComponent<Character>().m_body.transform.right * 10 * Time.fixedDeltaTime, ForceMode.VelocityChange);

					}

					//XXX check why velocity drop
					if (Input.GetKey(KeyCode.A))
					{

						Player.m_localPlayer.GetComponent<Character>().m_body.AddForce(-Player.m_localPlayer.GetComponent<Character>().m_body.transform.right * 10 * Time.fixedDeltaTime, ForceMode.VelocityChange);
					}
					if (Input.GetKey(KeyCode.W))
					{

						Player.m_localPlayer.GetComponent<Character>().m_body.AddForce(Player.m_localPlayer.GetComponent<Character>().m_body.transform.forward * 20 * Time.fixedDeltaTime, ForceMode.VelocityChange);
					}
					if (Input.GetKey(KeyCode.Space))
					{


						Vector3 directionToPoint = TrackGrapple.HitVectorGrapple - Handpoint.rightHand;

						Player.m_localPlayer.GetComponent<Character>().m_body.AddForce(directionToPoint.normalized * 40 * (Time.fixedDeltaTime * 2f), ForceMode.VelocityChange);
					}

				}



				if (PlayerPatcher.HasSwung && TrackGrapple.block == null)
				{


					if (Input.GetKey(KeyCode.D))
					{

						Player.m_localPlayer.GetComponent<Character>().m_body.AddForce(Player.m_localPlayer.GetComponent<Character>().m_body.transform.right * 10 * Time.fixedDeltaTime, ForceMode.VelocityChange);

					}
					if (Input.GetKey(KeyCode.A))
					{

						Player.m_localPlayer.GetComponent<Character>().m_body.AddForce(-Player.m_localPlayer.GetComponent<Character>().m_body.transform.right * 10 * Time.fixedDeltaTime, ForceMode.VelocityChange);
					}
					if (Input.GetKey(KeyCode.W))
					{

						Player.m_localPlayer.GetComponent<Character>().m_body.AddForce(Player.m_localPlayer.GetComponent<Character>().m_body.transform.forward * 10 * Time.fixedDeltaTime, ForceMode.VelocityChange);
					}

				}


				
				foreach (var group in WallCheck.groupedPieces)
				{
					if (group.Value != null)
					{
						bool isAnyPieceInRange = false;
						
						foreach (WearNTear matchingPiece in group.Value)
						{
							// Check if matchingPiece is null
							if (matchingPiece == null)
							{
								continue; // Skip to the next iteration if matchingPiece is null
							}

							// Check distance to the player and other conditions
							Piece pieceComponent = matchingPiece.GetComponent<Piece>();

							// Check if pieceComponent is null
							if (pieceComponent != null && pieceComponent.m_nview.IsOwner())
							{
								ZNetView zNetView = matchingPiece.gameObject.GetComponent<ZNetView>();

								// Check if zNetView is null
								if (zNetView != null)
								{
									Vector3 zdopos = zNetView.GetZDO().GetVec3("ZDOPos", new Vector3());

									// Check if Player.m_localPlayer is not null before using it
									if (Player.m_localPlayer != null && Vector3.Distance(zdopos, Player.m_localPlayer.transform.position) <= 30f)
									{
										isAnyPieceInRange = true;
										break; // Exit the loop as soon as we find one piece within range
									}
								}
							}
						}
					
						// If at least one piece is within range, perform movement logic for all pieces in the group
						if (isAnyPieceInRange)
						{
							foreach (WearNTear matchingPiece in group.Value)
							{
							
								if (matchingPiece != null)
								{
									Rigidbody pieceRigidbody = matchingPiece.GetComponent<Rigidbody>();
									
									Piece pieceComponent = matchingPiece.GetComponent<Piece>();
									
									if (pieceRigidbody != null && pieceComponent != null && WallCheck.CanMove == true && pieceComponent.m_name != null && pieceComponent.m_name.ToString() == "$piece_blackmarble2x2x2")
									{
										
										ZNetView zNetView2 = pieceRigidbody.GetComponent<ZNetView>();
										if (zNetView2 != null && zNetView2.GetZDO() != null)
										{
											
											float? floatValue = zNetView2.GetZDO().GetFloat("floatValueKey");
											if (floatValue.HasValue && floatValue > 1)
											{
												matchingPiece.gameObject.GetComponent<Rigidbody>().MovePosition(matchingPiece.GetComponent<Piece>().transform.position + matchingPiece.GetComponent<Piece>().transform.forward * matchingPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("floatDirectionValueKey") * matchingPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") * Time.fixedDeltaTime);
												//matchingPiece.GetComponent<ZNetView>().m_zdo.SetPosition(matchingPiece.gameObject.GetComponent<Rigidbody>().position);			
												// Additional logic for child ZNetViews
												ZNetView[] zNetViews = matchingPiece.GetComponentsInChildren<ZNetView>();
												List<ZNetView> childZNetViews = new List<ZNetView>(zNetViews);
												ZNetView parentZNetView = matchingPiece.GetComponent<ZNetView>();
												
												if (parentZNetView != null)
												{
													childZNetViews.Remove(parentZNetView);
												}
												foreach (ZNetView zNetView in childZNetViews)
												{
													// Do something with each child ZNetView
													Vector3 position = zNetView.transform.position;
													zNetView.m_zdo.SetPosition(position);
												}
												
												//Debug.Log(matchingPiece.GetComponent<Piece>().transform.position - matchingPiece.gameObject.GetComponent<Piece>().GetComponent<ZNetView>().GetZDO().GetVec3("Vector3Position2", new Vector3()));
												if ((matchingPiece.GetComponent<Piece>().transform.position - matchingPiece.gameObject.GetComponent<Piece>().GetComponent<ZNetView>().GetZDO().GetVec3("Vector3Position2", new Vector3())).sqrMagnitude < 0.05f * 0.05f)
												{
													matchingPiece.gameObject.GetComponent<Rigidbody>().gameObject.GetComponent<ZNetView>().GetZDO().Set("floatDirectionValueKey", -1f);
												}

												if ((matchingPiece.GetComponent<Piece>().transform.position - matchingPiece.gameObject.GetComponent<Piece>().GetComponent<ZNetView>().GetZDO().GetVec3("Vector3Position", new Vector3())).sqrMagnitude < 0.05f * 0.05f)
												{
													matchingPiece.gameObject.GetComponent<Piece>().GetComponent<ZNetView>().GetZDO().Set("floatDirectionValueKey", 1f);
												}
											}
										}
									}
									
									if (pieceRigidbody != null && pieceComponent != null && WallCheck.CanMove == true && pieceComponent.m_name != null && pieceComponent.m_name.ToString() == "$piece_blackmarble2x1x1")
									{
										ZNetView zNetView2 = pieceRigidbody.GetComponent<ZNetView>();
										if (zNetView2 != null && zNetView2.GetZDO() != null)
										{

											matchingPiece.gameObject.GetComponent<Rigidbody>().MovePosition(matchingPiece.GetComponent<Rigidbody>().position + matchingPiece.GetComponent<Rigidbody>().transform.forward * matchingPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") * Time.fixedDeltaTime);

											if (Vector3.Distance(matchingPiece.gameObject.GetComponent<Rigidbody>().position, matchingPiece.gameObject.GetComponent<Rigidbody>().GetComponent<ZNetView>().GetZDO().GetVec3("Vector3Position2", new Vector3())) < 0.1f)
											{
												matchingPiece.gameObject.GetComponent<Rigidbody>().transform.position = matchingPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3());

											}

											matchingPiece.GetComponent<ZNetView>().m_zdo.SetPosition(matchingPiece.gameObject.GetComponent<Rigidbody>().position);
											ZNetView[] zNetViews = matchingPiece.GetComponentsInChildren<ZNetView>();
											List<ZNetView> childZNetViews = new List<ZNetView>(zNetViews);
											ZNetView parentZNetView = matchingPiece.GetComponent<ZNetView>();
											if (parentZNetView != null)
											{
												childZNetViews.Remove(parentZNetView);
											}
											foreach (ZNetView zNetView in childZNetViews)
											{
												// Do something with each child ZNetView
												Vector3 position = zNetView.transform.position;
												zNetView.m_zdo.SetPosition(position);
											}

										}
									}
									if (matchingPiece.GetComponent<Piece>().m_nview.IsOwner() && matchingPiece.GetComponent<Rigidbody>() && WallCheck.CanMove == true && matchingPiece.GetComponent<Piece>().m_name.ToString() == "$piece_blackmarble_column_1")
									{
										Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * matchingPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TurnDirectionFloatValueKey") * matchingPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") * Time.fixedDeltaTime);
										matchingPiece.gameObject.GetComponent<Rigidbody>().MoveRotation(matchingPiece.GetComponent<Rigidbody>().rotation * deltaRotation);

										matchingPiece.GetComponent<ZNetView>().m_zdo.SetPosition(matchingPiece.gameObject.GetComponent<Rigidbody>().position);
										matchingPiece.GetComponent<ZNetView>().m_zdo.SetRotation(matchingPiece.gameObject.GetComponent<Rigidbody>().rotation);
										ZNetView[] zNetViews = matchingPiece.GetComponentsInChildren<ZNetView>();
										List<ZNetView> childZNetViews = new List<ZNetView>(zNetViews);
										ZNetView parentZNetView = matchingPiece.GetComponent<ZNetView>();
										if (parentZNetView != null)
										{
											childZNetViews.Remove(parentZNetView);
										}
										foreach (ZNetView zNetView in childZNetViews)
										{
											// Do something with each child ZNetView
											Vector3 position = zNetView.transform.position;
											Quaternion rotation = zNetView.transform.rotation;
											zNetView.m_zdo.SetPosition(position);
											zNetView.m_zdo.SetRotation(rotation);
										}
									}
								}
							}
						}
					}
				}

				if (DotTracker.activeSelf && DotTracker.GetComponent<Rigidbody>())
				{
					Quaternion deltaRotation2 = Quaternion.Euler(m_EulerAngleVelocity2 * 10 * Time.fixedDeltaTime);
					DotTracker.gameObject.GetComponent<Rigidbody>().MoveRotation(DotTracker.GetComponent<Rigidbody>().rotation * deltaRotation2);
				}

				//Debug.Log("Yvelo: " + Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y + " Difference " + ValueChangeChecker.IsYVelocityIncreasingBy(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y, 50f));
				if (ValueChangeChecker.IsYVelocityIncreasingBy(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y, 50f) && IsBouncing == false &&
				   (HasTouchedSurf == true || HasTouchedSurf2 == true))

				{
					Debug.Log("Error34");
					Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, 0, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);

				}
			}
		}
	}


	[HarmonyPatch(typeof(Player), "RemovePiece")]
	public static class RemovePieces
	{

		private static void Prefix()
		{
			int mask3 = LayerMask.GetMask("piece");
			if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out var hitInfo, 50f, mask3))
			{
				WearNTear piece = hitInfo.collider.GetComponentInParent<WearNTear>();
				WallCheck.AllPieces.Remove(piece);
			}
		}
	}

	[HarmonyPatch(typeof(WearNTear), "UpdateWear")]
	public static class WearOff2
	{

		private static void Prefix(ref bool ___m_noSupportWear)
		{

			___m_noSupportWear = false;

		}
	}

	[HarmonyPatch(typeof(Hud), "UpdateCrosshair")]
	public static class RedDot
	{
		public static Vector3 CrossPos;

		private static void Postfix(ref Image ___m_crosshair, ref Image ___m_crosshairBow, ref TextMeshProUGUI ___m_hoverName)
		{
			CrossPos = ___m_crosshair.transform.position;


			if (Player.m_localPlayer.GetRightItem() != null)
			{
				int mask3 = LayerMask.GetMask("piece");
				if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out var hitInfo, 80f, mask3) && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin")
				{
					if (Vector3.Distance(Handpoint.rightHand, hitInfo.transform.position) < 50.2)
					{
						float HookGradient = Vector3.Distance(Handpoint.rightHand, hitInfo.transform.position);
						HookGradient /= 50;
						HookGradient = Mathf.Clamp(HookGradient, 0, 1);
						___m_crosshair.color = Color.Lerp(Color.cyan, Color.magenta, HookGradient);
					}
					if (Vector3.Distance(Handpoint.rightHand, hitInfo.transform.position) >= 50.2)
					{
						___m_crosshair.color = new Color(1f, 1f, 1f, 0.5f);
					}

				}

				if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out var hitInfo2, 80f, mask3) &&
					Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer")
				{
					if (hitInfo.collider != null)
					{				
						Piece piece = hitInfo.collider.GetComponentInParent<Piece>();
						if (piece != null && piece.gameObject != null && piece.gameObject.GetComponent<ZNetView>() != null && piece.m_name != null &&
							piece.m_name.ToString() == "$piece_blackmarble2x2x2" && ___m_hoverName != null)
						{
							ZNetView pieceZNetView = piece.gameObject.GetComponent<ZNetView>();
							if (pieceZNetView != null && pieceZNetView.GetZDO() != null)
							{
								___m_hoverName.text = "        Group: " + pieceZNetView.GetZDO().GetString("CustomStringValueKey", "");
							}
						}
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(ZNetScene), "Awake")]
	public static class ResetBlock
	{

		public static Collider[] colpiecefromAllPiece = new Collider[2000];
		public static Transform ChildPiecce2;
		public static void Prefix()
		{
			foreach (WearNTear allPiece in WallCheck.AllPieces)
			{

				if (allPiece != null)
				{
					int mask3 = LayerMask.GetMask("character", "character_ghost");
					colpiecefromAllPiece = Physics.OverlapSphere(allPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()), 20, mask3);

					if (Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer" && colpiecefromAllPiece.Length <= 0f)
					{
						Debug.Log("HELLOOOO?");
						allPiece.gameObject.GetComponent<Rigidbody>().transform.position = allPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3());
					}
				}
			}

		}
	}



	[HarmonyPatch(typeof(FootStep), "DoEffect")]
	public static class FootCloud
	{


		public static bool Prefix()
		{
			if (PlayerPatcher.piecename == "$piece_blackmarble_floor" && PlayerPatcher.TrackTerrain == null)
			{

				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(VisEquipment), "UpdateVisuals")]
	public static class Handpoint
	{

		public static Vector3 rightHand;

		public static void Prefix()
		{
			if (Player.m_localPlayer != null)
			{
				rightHand = Player.m_localPlayer.GetComponent<VisEquipment>().m_rightHand.position;
			}
		}
	}

	[HarmonyPatch(typeof(Player), "CheckPlacementGhostVSPlayers")]
	public static class NoInvalid
	{


		private static bool Prefix()
		{
			if (PrefabString != "")
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Player), "TestGhostClipping")]
	public static class NoInvalid2
	{


		private static bool Prefix()
		{
			if (PrefabString != "")
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Player), "SetPlacementGhostValid")]
	public static class NoInvalid3
	{


		private static bool Prefix()
		{
			if (PrefabString != "")
			{
				return false;
			}
			return true;
		}
	}



	[HarmonyPatch(typeof(Player), "PlacePiece")]
	private static class SaveZDO
	{
		public static GameObject TempBuildingPiece = new GameObject();


		[HarmonyPrefix]
		private static bool Prefix(Piece piece, PieceTable ___m_buildPieces, GameObject ___m_placementGhost, Player __instance)
		{

			if (___m_placementGhost != null && __instance == Player.m_localPlayer)
			{
				Vector3 position = ___m_placementGhost.transform.position;
				Quaternion rotation = ___m_placementGhost.transform.rotation;
				GameObject gameObject = piece.gameObject;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, position, rotation);



				Piece selectedPiece = gameObject2.GetComponent<Piece>();
				if (selectedPiece.m_nview.IsOwner())
				{
					selectedPiece.m_placeEffect?.Create(position, rotation);

					TempBuildingPiece = gameObject2;

					//xxx
					selectedPiece.GetComponent<ZNetView>().m_syncInitialScale = true;
					selectedPiece.transform.localScale = new Vector3(UpdatePlacement_Patch.GhostScaleX, UpdatePlacement_Patch.GhostScaleY, UpdatePlacement_Patch.GhostScaleZ);
					selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("scale1", selectedPiece.transform.localScale);
					Debug.Log(WallCheck.collisionOccurred);
					selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("AttachedTrue", WallCheck.collisionOccurred);
					selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("ZDOPos", selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetPosition());
					selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("AttachRadius", RadiusValue);
					//selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("Radius", WallCheck.sphereRadius2);
					//Debug.Log(selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("scale1", new Vector3()));
					selectedPiece.gameObject.GetComponent<Piece>().SetCreator(Game.instance.GetPlayerProfile().GetPlayerID());



					if (PrefabString == "Reset")
					{
						PrefabString = "";
					}
					if (TimeOfDay == "Reset")
					{
						TimeOfDay = "";
					}
					if (GroupingValue == "Reset")
					{
						GroupingValue = "";
					}
					if (TimeOfDay != "" && selectedPiece.m_name.ToString() == "$piece_maypole")
                    {
						selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("TimeOfDayString", TimeOfDay);
					}
					if (WeatherCondition != "" && selectedPiece.m_name.ToString() == "$piece_maypole")
					{
						selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("WeatherConditionString", WeatherCondition);
					}
					

					//XXX maybe check if PrefabString Leads to a Prefab
					if (PrefabString != "")
					{
						GameObject prefab = ZNetScene.instance.GetPrefab(PrefabString);
						if (prefab != null)
						{
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("PrefabString", PrefabString);
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("MatNumberInt", MatNumber);
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("ChildNumberInt", ChildNumber);
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("SkinnedBool", SkinnedToggle);

						}
					}
					if (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_blackmarble2x2x2" ||
						selectedPiece.m_name.ToString() == "$piece_blackmarble2x1x1" ||
						selectedPiece.m_name.ToString() == "$piece_ironwall" ||
						selectedPiece.m_name.ToString() == "$piece_dvergr_metal_wall" ||
						selectedPiece.m_name.ToString() == "$piece_blackmarble_column_1" ||
						selectedPiece.m_name.ToString() == "$piece_blackmarble_floor" ||
						selectedPiece.m_name.ToString() == "$piece_crystalwall1x1" ||
						selectedPiece.m_name.ToString() == "$piece_blackmarble_floor_triangle" ||
						selectedPiece.m_name.ToString() == "$piece_stonefloor2x2")
					{
						if (GroupingValue == "")
						{
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("CustomStringValueKey", "0");
						}
						else
						{
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("CustomStringValueKey", GroupingValue);
						}
						selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("floatDirectionValueKey", PlatformDirection);
						selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("floatValueKey", PlatformDistanceValue);
						selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("SpeedFloatValueKey", SpeedValue);

						if (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_blackmarble2x2x2")
						{
							//XXX recalculate start end pos
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("Vector3Position", selectedPiece.gameObject.GetComponent<Piece>().transform.position - (selectedPiece.gameObject.transform.forward * 0.125f));
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("Vector3Position2", selectedPiece.gameObject.GetComponent<Piece>().transform.position + (selectedPiece.gameObject.transform.forward * selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("floatValueKey")));
						}
						else if (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_blackmarble2x1x1")
						{
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("Vector3Position", selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetPosition() - selectedPiece.gameObject.transform.forward * 1);
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("Vector3Position2", selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetPosition() + selectedPiece.gameObject.transform.forward * selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("floatValueKey"));
						}
						else if (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_blackmarble_column_1")
						{
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("TurnDirectionFloatValueKey", TurnDirectionValue);
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("Vector3Position", selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetPosition());
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("Vector3Position2", selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetPosition() + selectedPiece.gameObject.transform.forward * selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("floatValueKey"));
						}


						else if ((selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_ironwall") || (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_dvergr_metal_wall"))
						{
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("TrampolinFloatValueKey", TrampolinValue);
						}
						else if (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_blackmarble_floor")
						{

							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("IceFloatValueKey", IcePlatformValue);
						}
						else if (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_crystalwall1x1")
						{
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("StickyFloatValueKey", StickyPlatformValue);
						}
						else if (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_stonefloor2x2")
						{
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("BhopFloatValueKey", BhopValue);
						}
						else if (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_blackmarble_floor_triangle")
						{
							selectedPiece.gameObject.GetComponent<ZNetView>().GetZDO().Set("SurfFloatValueKey", SurfPlatformValue);
						}
						return false;
					}
					return false;
				}
			}
			return true;
		}
	}

	// [HarmonyPatch(typeof(LineConnect), "SetEndpoint")]
	// private static class DeleteGrapple
	// {
	//     [HarmonyPrefix]
	//     private static void Prefix()
	//     {

	//TrackGrapple.block = null;
	//     }
	// }

	/// <summary>
	/// ////////////////////////////////////////////////////////////////Patch in Jump Mechanics
	/// </summary>
	/// 



	[HarmonyPatch(typeof(Player), "Update")]
	public static class WallCheck
	{
		public static bool RayFrontTest;
		public static bool RayBackTest;
		public static bool SphereTest;


		public static float RayFrontLength;
		public static float RayBackLength;
		public static float RayDownLength;

		public static HashSet<WearNTear> AllPieces = new HashSet<WearNTear>();

		//xxxx Collider Lenght Performance issue later?
		public static Collider[] colpiecefromplayer = new Collider[20000];
		public static Collider[] colpiecefromplayer2 = new Collider[20000];
		public static Collider[] colpiecefromplayer123 = new Collider[20000];
		public static Collider[] colpiecefromplayerNonSolid = new Collider[20000];
		public static Collider[] colpiecefromplayerforGhost = new Collider[20000];
		public static Collider[] colpiecefromAllPiece = new Collider[20000];


		public static Collider[] colpieceFromMovePlat = new Collider[20000];
		public static Collider[] colpieceFromMovePlat2 = new Collider[20000];
		public static float MovePlatcounter = 0;

		public static float DistanceGrapple;

		public static Rigidbody RBpiece3;
		public static ZSyncTransform Zpiece3;

		public static string thenull;
		public static List<Rigidbody> rigidbodiesMovePlat = new List<Rigidbody>();
		public static List<float> rigidbodiesMovePlatDirection = new List<float>();
		public static List<float> rigidbodiesMovePlatStoredSinVar = new List<float>();
		public static List<float> rigidbodiesMovePlatStoredSinDir = new List<float>();

		public static List<bool> rigidbodiesMovePlatStarting = new List<bool>();
		public static List<bool> rigidbodiesMovePlatCanMove = new List<bool>();
		public static List<bool> rigidbodiesMovePlatCanMove2 = new List<bool>();
		public static List<Vector3> rigidbodiesMovePlatPosition = new List<Vector3>();
		public static List<Vector3> rigidbodiesMovePlatPosition2 = new List<Vector3>();
		public static bool CanMove;
		public static bool CanMove2;
		public static bool IsGrappling;
		public static bool IsSwinging;
		public static float countcollider = 0;
		public static float countcollider2 = 0;

		public static int lengthOfLineRenderer = 20;
		public static Color startColorHook = Color.black;
		public static Color endColorHook = Color.black;
		public static Color startColor2 = Color.red;
		public static Color endColor2 = Color.green;
		public static LineRenderer ln;
		public static LineRenderer ln22;
		public static LineRenderer ln222;
		public static LineRenderer ln2222;
		public static GameObject[] horizontalSegments = new GameObject[2000];

		public static Gradient gradient = new Gradient();
		public static float alpha = 1.0f;

		public static ParticleSystemRenderer rn;
		public static ParticleSystem ps;

		public static float nearestDistance = 2.5f;

		public static float hSliderValueR = 0.5F;
		public static float hSliderValueG = 0.5F;
		public static float hSliderValueB = 0.5F;
		public static float hSliderValueA = 0.5F;

		public static float ResetCounter = 0;
		public static float JointCounter = 0;
		public static Vector3 RotationParticle = new Vector3(90, 0, 0);

		public static float SoundCounter = 0;

		public static Vector3 test23;

		public static Joint jointSwing;

		public static Collider[] colpiecefromAllPiecePlayers = new Collider[200];
		public static Collider[] colpieceFromMovePlat22 = new Collider[20000];
		public static Collider[] colpieceforAttachRadius = new Collider[200];
		public static Transform ChildPiecce22;
		public static GameObject NextBoundObj;
		public static GameObject NearestOjbect;
		public static Transform CurrentBoundPiece;
		public static List<Transform> AllBoundPieces = new List<Transform>();

		public static bool TouchingIce;
		public static GameObject TouchingIceObj = new GameObject();

		public static bool TouchingSurf;
		public static GameObject TouchingSurfObj = new GameObject();

		public static float maxIceRayDistance = 0f;
		public static bool OnSlope;
		public static Vector3 IceRayPushUp = new Vector3(0, 0, 0);

		public static bool BackupSurf = false;
		public static bool SurfEject = false;
		public static bool DisableSurf = false;
		public static bool TipVelocityLimit = false;
		public static bool boost = false;
		public static float boostDuration = 0.2f;
		public static float boostTimer = 0f;

		public static Vector3 initialZoneCornerPosition = new Vector3(0, 0, 0);

		public static bool hasGhostCollision = false;
		public static float SaveCount = 0f;
		public static float ResetTimerZoneTransition = 0f;
		public static float ResetTimerZoneTransition2 = 0f;
		public static float BufferTimer = 0f;
		public static Vector2i oldZoneID = new Vector2i(0, 0);
		public static Vector2i oldZoneID2 = new Vector2i(0, 0);
		public static float DotTrackerParentingDistanceCount = 0f;
		public static bool collisionOccurred = false;
		

		public static bool hasSwitchedZones = false;

		public static bool within0_5fRange = false; // Initialize a flag to false
		public static Vector3 directionToPlayer = Vector3.zero;
		public static Vector3 OlddirectionToPlayer = Vector3.zero;
		public static Vector3 oldhitInfo3 = Vector3.zero;
		public static Vector3 oldhitPoint = Vector3.zero;
		public static Vector3 currentPoint = Vector3.zero;
		public static Vector3 oldPoint = Vector3.zero;
		public static Vector3 hitInfo3Ynorm = Vector3.zero;
		public static Vector3 oldhitInfo3Ynorm = Vector3.zero;
		public static bool hasChangedWall = false;
		public static bool WallSlide = false;
		public static float WallJumpAngle = 0f;
		public static float BhopTimeFactor = 0f;
		public static float BhopTimeValue = 0f;
		public static float sphereRadius2 = 0f;
		public static HashSet<Collider> processedColliders = new HashSet<Collider>();
		public static List<Collider> collidersToRemove = new List<Collider>();
		public static bool AllowlnAdd = false;
		public static float KoyoteTimer = 0.25f;

		public static float timer = 0f;
		public static float updateInterval = 1f; // Set your desired interval in seconds
		public static bool OwnerChange = false;
		public static bool OverrideMove = false;
		public static Dictionary<string, List<WearNTear>> groupedPieces = new Dictionary<string, List<WearNTear>>();
		public static Text numberText;
		private static void Prefix(Player __instance)
		{



			if (__instance.IsPlayer() && Player.m_localPlayer != null)
			{


				if (Player.m_localPlayer.GetRightItem() == null || ZoneToggle == false)
				{
					LineRenderer TempLN = DotTrackerParentingDistance.GetComponent<LineRenderer>();
					LineRenderer TempLN2 = DotTrackerParentingDistance2.GetComponent<LineRenderer>();
					Destroy(TempLN);
					Destroy(TempLN2);
					DotTrackerParentingDistance.SetActive(false);
					DotTrackerParentingDistance2.SetActive(false);
					DotTrackerParentingDistanceCount = 0;


				}

				if (ZoneCorner != null && ZoneCorner2 != null && ZoneCorner3 != null && ZoneCorner4 != null && ZoneCorners != null && ZoneCenter != null)
				{

					Vector2i zoneID = ZoneSystem.m_instance.GetZone(ZNet.instance.GetReferencePosition());
					Vector3 zoneIDCorner = new Vector3(ZoneSystem.m_instance.GetZonePos(zoneID).x, 0f, ZoneSystem.m_instance.GetZonePos(zoneID).z);

					if (zoneID != oldZoneID && Player.m_localPlayer.GetRightItem() == null)
					{
						ResetTimerZoneTransition += Time.deltaTime;
						if (ResetTimerZoneTransition >= 1f)
						{
							//Debug.Log("ZoneReset");
							AllPieces.Clear();
							oldZoneID = zoneID;
							ResetTimerZoneTransition = 0f;

						}

					}

					//Debug.Log("Timer " + ResetTimerZoneTransition2 + "  " + zoneID + "  " + oldZoneID2);
					
					if (zoneID != oldZoneID2 && Player.m_localPlayer.GetRightItem() == null)
					{
						ResetTimerZoneTransition2 = 0f;
						oldZoneID2 = zoneID;
					}
					

					ZoneCorner.SetActive(true);
					ZoneCorner2.SetActive(true);
					ZoneCorner3.SetActive(true);
					ZoneCorner4.SetActive(true);
					ZoneCenter.SetActive(true);



					ZoneCorner.transform.position = new Vector3(zoneIDCorner.x + 32f, Player.m_localPlayer.transform.position.y - 20f, zoneIDCorner.z + 32f);
					ZoneCorner2.transform.position = new Vector3(zoneIDCorner.x - 32f, Player.m_localPlayer.transform.position.y - 20f, zoneIDCorner.z - 32f);
					ZoneCorner3.transform.position = new Vector3(zoneIDCorner.x + 32f, Player.m_localPlayer.transform.position.y - 20f, zoneIDCorner.z - 32f);
					ZoneCorner4.transform.position = new Vector3(zoneIDCorner.x - 32f, Player.m_localPlayer.transform.position.y - 20f, zoneIDCorner.z + 32f);

				


					Vector3 newCenterPosition = new Vector3(zoneIDCorner.x, Player.m_localPlayer.transform.position.y, zoneIDCorner.z);
					ZoneCenter.transform.position = newCenterPosition;

					//Debug.Log("ZC " + ZoneCenter.transform.position + " " + Player.m_localPlayer.transform.position);
					BoxCollider boxCollider = ZoneCenter.GetComponent<BoxCollider>();

					if (boxCollider == null)
					{
						boxCollider = ZoneCenter.gameObject.AddComponent<BoxCollider>();
						boxCollider.isTrigger = true;
						boxCollider.size = new Vector3(32f, 100f, 32f);
						boxCollider.center = new Vector3(ZoneCenter.transform.position.x, 0, ZoneCenter.transform.position.z);
						ZoneCenter.layer = LayerMask.NameToLayer("piece_nonsolid");
					}

					



					hasGhostCollision = false;
					Collider[] colliders = Physics.OverlapBox(ZoneCenter.transform.position, ZoneCenter.GetComponent<BoxCollider>().size / 1.2f);
					collisionOccurred = false;
					Vector3 totalExtents = Vector3.zero; // Initialize the sum of extents
					int totalColliders = 0;


					if (colliders != null)
					{

						foreach (Collider collider in colliders)
						{
							if (collider.gameObject.GetComponentInParent<Piece>() != null && GhostObject.gameObject.GetComponent<Piece>() != null)
							{

								//Debug.Log("collider " + collider.gameObject.GetComponentInParent<Piece>().m_name + " Ghost " + GhostObject.gameObject.GetComponent<Piece>().m_name);
								if (collider.gameObject.GetComponentInParent<ZNetView>() == null && collider.gameObject.GetComponentInParent<Piece>().m_name == GhostObject.gameObject.GetComponent<Piece>().m_name)
								{

									if (DotTrackerParentingDistance != null && DotTrackerParentingDistance.activeSelf && DotTrackerParentingDistance2 != null && DotTrackerParentingDistance2.activeSelf)
									{

										if (currentGhostObject != null && currentGhostObject != GhostObject.gameObject && DotTrackerParentingDistance.GetComponent<LineRenderer>() && DotTrackerParentingDistance2.GetComponent<LineRenderer>())
										{

											LineRenderer TempLN = DotTrackerParentingDistance.GetComponent<LineRenderer>();
											LineRenderer TempLN2 = DotTrackerParentingDistance2.GetComponent<LineRenderer>();
											Destroy(TempLN);
											Destroy(TempLN2);

											DotTrackerParentingDistance.SetActive(false);
											DotTrackerParentingDistance2.SetActive(false);
											DotTrackerParentingDistanceCount = 0;
										}

										// Update the currentGhostObject
										currentGhostObject = GhostObject.gameObject;
									}

									GameObject parentGameObject = null; // Initialize as null

									if (collider.transform.parent != null)
									{
										parentGameObject = collider.transform.parent.gameObject;
									}
									else
									{
										parentGameObject = collider.gameObject;
									}

									hasGhostCollision = true; // Set the collision status to true

									DotTrackerParentingDistance.SetActive(true);
									DotTrackerParentingDistance2.SetActive(true);

									if (parentGameObject != null)
									{

										DotTrackerParentingDistance.transform.position = parentGameObject.transform.position;
										DotTrackerParentingDistance2.transform.position = parentGameObject.transform.position;
									}

									collisionOccurred = true;
									totalExtents += collider.bounds.extents;
									totalColliders++;
								}
							}

						}

						DotTrackerParentingDistanceCount = 1;

						if (DotTrackerParentingDistance2.transform.rotation.eulerAngles.x != 90f)
						{
							// Execute this code when the X rotation is not 90 degrees
							DotTrackerParentingDistance2.transform.Rotate(Vector3.right, 90f);
						}

						if (collisionOccurred == true && totalColliders > 0)
						{
							if (ZoneToggle == true && DotTrackerParentingDistance != null && DotTrackerParentingDistance.activeSelf && !DotTrackerParentingDistance.GetComponent<LineRenderer>() && DotTrackerParentingDistanceCount == 1 && DotTrackerParentingDistance2 != null && DotTrackerParentingDistance2.activeSelf && !DotTrackerParentingDistance2.GetComponent<LineRenderer>())
							{
								Vector3 averagedExtents = totalExtents / totalColliders;

								//sphereRadius2 = Mathf.Max(averagedExtents.x * 2f, averagedExtents.y * 2f, averagedExtents.z * 2f) * 1.2f;
								//Debug.Log("SR " + sphereRadius2);
								//Debug.Log(sphereRadius2);
								DotTrackerParentingDistance.DrawCircle(RadiusValue, 0.1f, new Color(1f, 0f, 0f, 1f));


								DotTrackerParentingDistance2.DrawCircle(RadiusValue, 0.1f, new Color(1f, 0f, 0f, 1f));
								DotTrackerParentingDistanceCount += 1;

							}
						}
						if (collisionOccurred == false)
						{
							//Debug.Log("Delete Border");
							if (DotTrackerParentingDistance != null && DotTrackerParentingDistance.activeSelf && DotTrackerParentingDistance.GetComponent<LineRenderer>() && DotTrackerParentingDistance2 != null && DotTrackerParentingDistance2.activeSelf && DotTrackerParentingDistance2.GetComponent<LineRenderer>())
							{
								LineRenderer TempLN = DotTrackerParentingDistance.GetComponent<LineRenderer>();
								LineRenderer TempLN2 = DotTrackerParentingDistance2.GetComponent<LineRenderer>();
								Destroy(TempLN);
								Destroy(TempLN2);

								DotTrackerParentingDistance.SetActive(false);
								DotTrackerParentingDistance2.SetActive(false);
								DotTrackerParentingDistanceCount = 0;
							}
						}
					}
					//Debug.Log("Collision detected: " + hasGhostCollision);

					if (SaveCount == 0)
					{
						initialZoneCornerPosition = new Vector3(0, Player.m_localPlayer.transform.position.y, 0);
						SaveCount += 1;


					}

					ZoneCorners[0] = ZoneCorner;
					ZoneCorners[1] = ZoneCorner2;
					ZoneCorners[2] = ZoneCorner3;
					ZoneCorners[3] = ZoneCorner4;


					foreach (GameObject zoneCorner in ZoneCorners)
					{
						if (zoneCorner != null)
						{

							if (!zoneCorner.GetComponent<LineRenderer>())
							{
								ln2222 = zoneCorner.gameObject.AddComponent<LineRenderer>();
								ln2222 = zoneCorner.gameObject.GetComponent<LineRenderer>();
								zoneCorner.gameObject.GetComponent<LineRenderer>().SetPosition(0, zoneCorner.transform.position + Vector3.down * 50f);
								zoneCorner.gameObject.GetComponent<LineRenderer>().SetPosition(1, zoneCorner.transform.position + Vector3.up * 50f);

								if (ln2222.material == null || ln2222.material.shader != Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"))
								{
									ln2222.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
								}
								if (zoneCorner == ZoneCorners[1])
								{
									//Debug.Log("CreateHorizSeg");

									horizontalSegments = LineRendererUtility.CreateHorizontalSegments(ln2222, 20);
								}


							}



							ln2222.positionCount = 2;
							ln2222.startColor = startColor2;
							ln2222.endColor = startColor2;

							ln2222.startWidth = 0.1f;
							ln2222.endWidth = 0.1f;

							if (zoneCorner.GetComponent<LineRenderer>() != null && zoneCorner.GetComponent<LineRenderer>())
							{
								var CharYpos = new Vector3(0, Player.m_localPlayer.transform.position.y, 0);
								var cornerpos = new Vector3(zoneCorner.transform.position.x, 0, zoneCorner.transform.position.z);
								var linePos = new Vector3(zoneCorner.gameObject.GetComponent<LineRenderer>().GetPosition(0).x, 0, zoneCorner.gameObject.GetComponent<LineRenderer>().GetPosition(0).z);
								//Debug.Log(CharYpos.y - initialZoneCornerPosition.y);

								if ((zoneCorner.activeSelf && cornerpos != linePos) || (CharYpos.y - initialZoneCornerPosition.y < -20 || CharYpos.y - initialZoneCornerPosition.y > 20))
								{
									//Debug.Log("DestroHorizSeg");
									LineRendererUtility.DestroyHorizontalSegments();
									LineRenderer TempLN = zoneCorner.gameObject.GetComponent<LineRenderer>();
									Destroy(TempLN);
									SaveCount = 0;
								}
							}
						}

						if ((Player.m_localPlayer != null && Player.m_localPlayer.GetRightItem() == null && ZoneToggle == false) ||
							(Player.m_localPlayer != null && Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer" && ZoneToggle == false))
						{
							if (zoneCorner != null)
							{
								LineRenderer zoneCornerLineRenderer = zoneCorner.GetComponent<LineRenderer>();
								if (zoneCornerLineRenderer != null)
								{
									zoneCornerLineRenderer.enabled = false;
								}

								if (zoneCorner == ZoneCorners[1])
								{
									foreach (GameObject horizontalSegment in horizontalSegments)
									{
										if (horizontalSegment != null)
										{
											LineRenderer horizontalSegmentLineRenderer = horizontalSegment.GetComponent<LineRenderer>();
											if (horizontalSegmentLineRenderer != null)
											{
												horizontalSegmentLineRenderer.enabled = false;
											}
										}
									}
								}
							}
						}

						//make it happen with destroy care for zonecorner[1]
						if ((Player.m_localPlayer != null && Player.m_localPlayer.GetRightItem() == null && ZoneToggle == true) ||
							(Player.m_localPlayer != null && Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer" && ZoneToggle == true))
						{
							if (zoneCorner != null)
							{
								LineRenderer zoneCornerLineRenderer = zoneCorner.GetComponent<LineRenderer>();
								if (zoneCornerLineRenderer != null)
								{
									zoneCornerLineRenderer.enabled = true;
								}

								if (zoneCorner == ZoneCorners[1])
								{
									foreach (GameObject horizontalSegment in horizontalSegments)
									{
										if (horizontalSegment != null)
										{
											LineRenderer horizontalSegmentLineRenderer = horizontalSegment.GetComponent<LineRenderer>();
											if (horizontalSegmentLineRenderer != null)
											{
												horizontalSegmentLineRenderer.enabled = true;
											}
										}
									}
								}
							}
						}
					}

				}

				//Debug.Log("AnyContact  " + BounceII.ScaleFactor);
				//Debug.Log(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.magnitude + " " + Player.m_localPlayer.GetComponent<Character>().m_currentVel.magnitude);
				//Debug.Log(ScaleFactor);
				if (!ZNetScene.instance)
				{
					Debug.LogError("can't load prefab due to ZNetScene.instance is null");
				}
				else
				{

					int mask1337 = LayerMask.GetMask("piece");

					//If zonetransition
					List<WearNTear> instances2 = WearNTear.GetAllInstances();

					EnvMan.instance.m_debugTimeOfDay = false;
					EnvMan.instance.m_debugEnv = "Clear";
					foreach (WearNTear ResetList in instances2)
					{


						if (ResetList != null && Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer")
						{
							//xxx rausgenommen 06.11.2024 fix slide gravity slope
							//ResetList.gameObject.GetComponent<WearNTear>().ResetHighlight();
						}

						if (ResetList.transform.parent == null)
						{
							if (ResetList.transform != null && ResetList.GetComponentInParent<ZNetView>().GetZDO() != null)
							{
								if (ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("TimeOfDayString") != "")
								{
									string formattedTimeOfDay = ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("TimeOfDayString").Replace(',', '.');
									
									if (float.TryParse(formattedTimeOfDay, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
									{									
										EnvMan.instance.m_debugTimeOfDay = true;
										EnvMan.instance.m_debugTime = Mathf.Clamp01(floatValue);
									}
								}
								if (ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("WeatherConditionString") != "")
								{
									string Weather = ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("WeatherConditionString");
									EnvMan.instance.m_debugEnv = Weather;					
								}


								if (ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("scale1", new Vector3()) != null)
								{


									ResetList.transform.localScale = new Vector3(ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("scale1", new Vector3(1, 1, 1)).x, ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("scale1", new Vector3(1, 1, 1)).y, ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("scale1", new Vector3(1, 1, 1)).z);
								}

								if (ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("PrefabString") != "")
								{
									string @string = ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("PrefabString");

									if (@string.Length > 0 && @string != "")
									{

										var components = ResetList.transform.root.GetComponentsInChildren<MeshRenderer>();
										foreach (MeshRenderer r in components)
										{
											if (!r.gameObject.GetComponentInParent<Rigidbody>())
											{
												var TempBool = ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetBool("SkinnedBool");

												if (ZNetScene.instance.GetPrefab(ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("PrefabString")).GetComponentInChildren<MeshRenderer>() != null && TempBool == false)
												{
													var TempString = ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("PrefabString", "");
													var TempInt = ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetInt("MatNumberInt");
													var TempChildInt = ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetInt("ChildNumberInt");

													if (ZNetScene.instance.GetPrefab(TempString).GetComponentInChildren<MeshRenderer>().materials.Length > 1)
													{


														if (r.material.name.ToString() != ZNetScene.instance.GetPrefab(TempString).GetComponentsInChildren<MeshRenderer>()[TempChildInt].materials[TempInt].name.ToString())
														{

															r.material = ZNetScene.instance.GetPrefab(TempString)?.GetComponentsInChildren<MeshRenderer>()[TempChildInt].materials[TempInt];
															r.material.name = ZNetScene.instance.GetPrefab(TempString)?.GetComponentsInChildren<MeshRenderer>()[TempChildInt].materials[TempInt].name;
															//r.materials = ZNetScene.instance.GetPrefab("piece_jackoturnip")?.GetComponentInChildren<MeshRenderer>().materials;

														}
													}
													if (ZNetScene.instance.GetPrefab(TempString).GetComponentInChildren<MeshRenderer>().materials.Length <= 1)
													{

														if (r.material.name.ToString() != ZNetScene.instance.GetPrefab(TempString).GetComponentsInChildren<MeshRenderer>()[TempChildInt].materials[TempInt].name.ToString())
														{

															r.material = ZNetScene.instance.GetPrefab(TempString)?.GetComponentsInChildren<MeshRenderer>()[TempChildInt].materials[TempInt];
															r.material.name = ZNetScene.instance.GetPrefab(TempString)?.GetComponentsInChildren<MeshRenderer>()[TempChildInt].materials[TempInt].name;
															//r.materials = ZNetScene.instance.GetPrefab("piece_jackoturnip")?.GetComponentInChildren<MeshRenderer>().materials;
														}
													}
												}

												else if (ZNetScene.instance.GetPrefab(ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("PrefabString")).GetComponentInChildren<SkinnedMeshRenderer>() != null && TempBool == true)
												{

													var TempString = ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("PrefabString", "");
													var TempInt = ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetInt("MatNumberInt");
													var TempChildInt = ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetInt("ChildNumberInt");

													if (ZNetScene.instance.GetPrefab(TempString).GetComponentInChildren<SkinnedMeshRenderer>().materials.Length > 1)
													{

														if (r.material.name.ToString() != ZNetScene.instance.GetPrefab(TempString).GetComponentsInChildren<SkinnedMeshRenderer>()[TempChildInt].materials[TempInt].name.ToString())
														{

															r.material = ZNetScene.instance.GetPrefab(TempString)?.GetComponentsInChildren<SkinnedMeshRenderer>()[TempChildInt].materials[TempInt];
															r.material.name = ZNetScene.instance.GetPrefab(TempString)?.GetComponentsInChildren<SkinnedMeshRenderer>()[TempChildInt].materials[TempInt].name;
															//r.materials = ZNetScene.instance.GetPrefab("piece_jackoturnip")?.GetComponentInChildren<MeshRenderer>().materials;
														}
													}
													if (ZNetScene.instance.GetPrefab(TempString).GetComponentInChildren<SkinnedMeshRenderer>().materials.Length <= 1)
													{

														if (r.material.name.ToString() != ZNetScene.instance.GetPrefab(TempString).GetComponentsInChildren<SkinnedMeshRenderer>()[TempChildInt].materials[TempInt].name.ToString())
														{

															r.material = ZNetScene.instance.GetPrefab(TempString)?.GetComponentsInChildren<SkinnedMeshRenderer>()[TempChildInt].materials[TempInt];
															r.material.name = ZNetScene.instance.GetPrefab(TempString)?.GetComponentsInChildren<SkinnedMeshRenderer>()[TempChildInt].materials[TempInt].name;
															//r.materials = ZNetScene.instance.GetPrefab("piece_jackoturnip")?.GetComponentInChildren<MeshRenderer>().materials;
														}
													}
												}
											}
										}

									}
								}
							}
						}
					}


					

					var Platform2 = instances2.Where(WearNTear => { var pieceComponent = WearNTear.GetComponent<Piece>(); return pieceComponent != null && (pieceComponent.m_name.ToString() == "$piece_blackmarble2x2x2" || pieceComponent.m_name.ToString() == "$piece_blackmarble2x1x1" || pieceComponent.m_name.ToString() == "$piece_blackmarble_column_1"); }).ToList();

				
					groupedPieces.Clear();
					foreach (WearNTear allPiece2 in Platform2)
					{
						if (allPiece2 != null)
						{			
							// Check if the ZDO has a string tag you're looking for
							if (allPiece2.gameObject.GetComponent<ZNetView>() != null && allPiece2.gameObject.GetComponent<ZNetView>().GetZDO() != null)
							{							
								string customStringValue = allPiece2.gameObject.GetComponent<ZNetView>().GetZDO().GetString("CustomStringValueKey", "");						
								// Adjust the conditions as needed
								if (!string.IsNullOrEmpty(customStringValue))
								{							
									if (!groupedPieces.ContainsKey(customStringValue))
									{								
										groupedPieces[customStringValue] = new List<WearNTear>();
									}							
									groupedPieces[customStringValue].Add(allPiece2);
								}
							}
						}
					}

					


					


					//var Platform2 = instances2.Where(WearNTear => WearNTear.GetComponent<Piece>().m_name.ToString() == "$piece_blackmarble2x2x2" || WearNTear.GetComponent<Piece>().m_name.ToString() == "$piece_blackmarble2x1x1" || WearNTear.GetComponent<Piece>().m_name.ToString() == "$piece_blackmarble_column_1").ToList();
					foreach (WearNTear allPiece2 in Platform2)
					{



						OverrideMove = false;
						if (allPiece2.TryGetComponent(out Piece piece3) && piece3.m_name.ToString() == "$piece_blackmarble2x2x2" && Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer" && allPiece2.gameObject.GetComponent<Rigidbody>() && AllPieces.Contains(allPiece2) || (AllPieces.Count == 0 && allPiece2.TryGetComponent(out Piece piece4) && piece4.m_name.ToString() == "$piece_blackmarble2x2x2" && Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer" && allPiece2.gameObject.GetComponent<Rigidbody>()))
						{
							//XXX Continue Here
							int mask33 = LayerMask.GetMask("character", "character_net");
							Collider[] colpiecefromAllPiece = Physics.OverlapSphere(allPiece2.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()), 29f, mask33);
							var counter = 0f;
							foreach (Collider collider in colpiecefromAllPiece)
							{
								if (collider != null)
								{


									// Check if the collider belongs to a character or character_ghost layer and is not the player
									if (collider.gameObject.layer == LayerMask.NameToLayer("character") ||
										collider.gameObject.layer == LayerMask.NameToLayer("character_net"))
									{
										Player playerComponent = collider.gameObject.GetComponent<Player>();
										Player localPlayer = Player.m_localPlayer;

										if (playerComponent != null && playerComponent != localPlayer)
										{
											counter += 1f;
											OverrideMove = true;
											//XXX set a bool to false and the reset only happens when true
											Debug.Log("Detected another character or character_ghost collider that is not the player.");
										}
									}
								}
							}

							if (counter <= 0)
							{

								var ResetVector = allPiece2.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3());
								allPiece2.gameObject.GetComponent<Rigidbody>().transform.position = ResetVector;
								allPiece2.gameObject.GetComponent<Piece>().GetComponent<ZNetView>().GetZDO().Set("floatDirectionValueKey", 1f);
								Debug.Log("reset: " + allPiece2.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + "  " + allPiece2.gameObject.GetComponent<Rigidbody>().position);
								//allPiece.gameObject.GetComponent<Rigidbody>().GetComponent<ZNetView>().GetZDO().GetVec3("Vector3Position2", new Vector3())

								int mask3 = LayerMask.GetMask("piece");



								colpieceFromMovePlat22 = Physics.OverlapBox(allPiece2.gameObject.GetComponent<BoxCollider>().bounds.center, allPiece2.gameObject.GetComponent<BoxCollider>().bounds.extents * 1.05f, allPiece2.transform.rotation, mask3);

								//colpieceFromMovePlat22 = Physics.OverlapSphere(allPiece2.gameObject.GetComponent<Rigidbody>().position, 2f, mask3);
								foreach (Collider colliderfromplayer in colpieceFromMovePlat22)
								{
									ChildPiecce22 = colliderfromplayer.GetComponentInParent<Piece>().transform;

									if (ChildPiecce22 != null && ChildPiecce22.parent != null)
									{
										Debug.Log("Unparent222");
										allPiece2.gameObject.UnparentChildren();
									}
								}
								Rigidbody TempRB = allPiece2.GetComponentInParent<Rigidbody>();
								ZSyncTransform TempZT = allPiece2.GetComponentInParent<ZSyncTransform>();
								AllPieces.Clear();
								Destroy(TempRB);
								Destroy(TempZT);
								NextBoundObj = null;
								CurrentBoundPiece = null;
							}
						}
						else if (allPiece2.TryGetComponent(out Piece piece5) && piece5.m_name.ToString() == "$piece_blackmarble2x1x1" && Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer" && allPiece2.gameObject.GetComponent<Rigidbody>() && AllPieces.Contains(allPiece2) || (AllPieces.Count == 0 && allPiece2.TryGetComponent(out Piece piece6) && piece6.m_name.ToString() == "$piece_blackmarble2x1x1" && Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer" && allPiece2.gameObject.GetComponent<Rigidbody>()))
						{

							int mask33 = LayerMask.GetMask("character", "character_net");
							Collider[] colpiecefromAllPiece = Physics.OverlapSphere(allPiece2.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()), 29f, mask33);
							var counter2 = 0f;
							foreach (Collider collider in colpiecefromAllPiece)
							{
								if (collider != null)
								{


									// Check if the collider belongs to a character or character_ghost layer and is not the player
									if (collider.gameObject.layer == LayerMask.NameToLayer("character") ||
										collider.gameObject.layer == LayerMask.NameToLayer("character_net"))
									{
										Player playerComponent = collider.gameObject.GetComponent<Player>();
										Player localPlayer = Player.m_localPlayer;

										if (playerComponent != null && playerComponent != localPlayer)
										{
											counter2 += 1f;
											OverrideMove = true;
											//XXX set a bool to false and the reset only happens when true
											Debug.Log("Detected another character or character_ghost collider that is not the player.");
										}
									}
								}
							}

							if (counter2 <= 0)
							{
								var ResetVector = allPiece2.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3());
								allPiece2.gameObject.GetComponent<Rigidbody>().transform.position = ResetVector;

								Debug.Log("reset: " + allPiece2.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + "  " + allPiece2.gameObject.GetComponent<Rigidbody>().position);
								//allPiece.gameObject.GetComponent<Rigidbody>().GetComponent<ZNetView>().GetZDO().GetVec3("Vector3Position2", new Vector3())

								int mask3 = LayerMask.GetMask("piece");


								colpieceFromMovePlat22 = Physics.OverlapBox(allPiece2.gameObject.GetComponent<BoxCollider>().bounds.center, allPiece2.gameObject.GetComponent<BoxCollider>().bounds.extents * 1.05f, allPiece2.transform.rotation, mask3);

								//colpieceFromMovePlat22 = Physics.OverlapSphere(allPiece2.gameObject.GetComponent<Rigidbody>().position, 2f, mask3);
								foreach (Collider colliderfromplayer in colpieceFromMovePlat22)
								{
									ChildPiecce22 = colliderfromplayer.GetComponentInParent<Piece>().transform;

									if (ChildPiecce22 != null && ChildPiecce22.parent != null)
									{
										Debug.Log("Unparent211");
										allPiece2.gameObject.UnparentChildren();
									}
								}


								Rigidbody TempRB = allPiece2.GetComponentInParent<Rigidbody>();
								ZSyncTransform TempZT = allPiece2.GetComponentInParent<ZSyncTransform>();
								AllPieces.Clear();
								Destroy(TempRB);
								Destroy(TempZT);
								NextBoundObj = null;
								CurrentBoundPiece = null;
							}
						}

						//XXX
						else if (allPiece2.TryGetComponent(out Piece piece10) && piece10.m_name.ToString() == "$piece_blackmarble_column_1" && Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer" && allPiece2.gameObject.GetComponent<Rigidbody>() && AllPieces.Contains(allPiece2) || (AllPieces.Count == 0 && allPiece2.TryGetComponent(out Piece piece11) && piece11.m_name.ToString() == "$piece_blackmarble_column_1" && Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer" && allPiece2.gameObject.GetComponent<Rigidbody>()))
						{

							var ResetVector = allPiece2.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3());
							allPiece2.gameObject.GetComponent<Rigidbody>().transform.position = ResetVector;

							int mask3 = LayerMask.GetMask("piece");
							colpieceFromMovePlat22 = Physics.OverlapBox(allPiece2.gameObject.GetComponent<BoxCollider>().bounds.center, allPiece2.gameObject.GetComponent<BoxCollider>().bounds.extents * 1.05f, allPiece2.transform.rotation, mask3);

							//colpieceFromMovePlat22 = Physics.OverlapSphere(allPiece2.gameObject.GetComponent<Rigidbody>().position, 2f, mask3);
							foreach (Collider colliderfromplayer in colpieceFromMovePlat22)
							{
								ChildPiecce22 = colliderfromplayer.GetComponentInParent<Piece>().transform;

								if (ChildPiecce22 != null)
								{

									if (ChildPiecce22.parent != null)
									{

										allPiece2.gameObject.UnparentChildren();
									}
								}
							}

							Rigidbody TempRB = allPiece2.GetComponentInParent<Rigidbody>();
							ZSyncTransform TempZT = allPiece2.GetComponentInParent<ZSyncTransform>();
							AllPieces.Clear();
							Destroy(TempRB);
							Destroy(TempZT);
							NextBoundObj = null;
							CurrentBoundPiece = null;
						}

						AllBoundPieces.Clear();

						if (allPiece2 != null && allPiece2.GetComponent<Piece>() != null)
						{

							if (allPiece2.TryGetComponent(out Piece piece2) && piece2.m_name.ToString() == "$piece_blackmarble2x2x2" && (Player.m_localPlayer.GetRightItem() == null || Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin") || allPiece2.TryGetComponent(out Piece piece7) && piece7.m_name.ToString() == "$piece_blackmarble2x1x1" && (Player.m_localPlayer.GetRightItem() == null || Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin") || allPiece2.TryGetComponent(out Piece piece12) && piece12.m_name.ToString() == "$piece_blackmarble_column_1" && (Player.m_localPlayer.GetRightItem() == null || Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin"))
							{


								
								int mask33 = LayerMask.GetMask("character", "character_net");
								colpiecefromAllPiecePlayers = Physics.OverlapSphere(allPiece2.transform.position, 30f, mask33);
								
								//Debug.Log("ColL " + colpiecefromAllPiecePlayers.Length);
								Dictionary<Player, float> playerDistances = new Dictionary<Player, float>();
								
								foreach (Collider collider in colpiecefromAllPiecePlayers)
								{
									if (collider != null)
									{
										
										if (collider.gameObject.layer == LayerMask.NameToLayer("character") ||
											collider.gameObject.layer == LayerMask.NameToLayer("character_net"))
										{
											Player playerComponent = collider.gameObject.GetComponent<Player>();
											Player localPlayer = Player.m_localPlayer;
											
											if (playerComponent != null)
											{
												float distance = Vector3.Distance(allPiece2.transform.position, playerComponent.transform.position);
												playerDistances[playerComponent] = distance;
												
											}
										}
									}
								}
								
								float closestDistance = float.MaxValue; // Set to a large value initially
								Player closestPlayer = null;

								foreach (var kvp in playerDistances)
								{
									//Nepomuk 19.11.2023: 0
									if (kvp.Value < closestDistance)
									{
										closestDistance = kvp.Value;
										closestPlayer = kvp.Key;

									}
								}
							
								foreach (var group in groupedPieces)
								{
									bool isAnyPieceInRange = true;

									// Check if any piece in the group is within range and owned by the local player
									foreach (WearNTear matchingPiece in group.Value)
									{
										if (!matchingPiece.GetComponent<Piece>().m_nview.IsOwner() || Vector3.Distance(matchingPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()), Player.m_localPlayer.transform.position) < 29f)
										{
											isAnyPieceInRange = false;
											break; // Exit the loop as soon as we find one piece within range
										}
									}
								
									// If at least one piece in the group is within range, set the owner for all pieces in the group
									if (isAnyPieceInRange)
									{
										foreach (WearNTear matchingPiece in group.Value)
										{
											
											if (matchingPiece != null)
											{
												if (closestPlayer != null)
												{
													
													if (matchingPiece.gameObject.GetComponent<Piece>().m_nview != null && matchingPiece.gameObject.GetComponent<Piece>().m_nview.IsOwner())
													{
														if (matchingPiece.gameObject.GetComponent<ZNetView>() != null && matchingPiece.gameObject.GetComponent<Piece>().m_nview.IsValid() && playerDistances.Count > 1f && Vector3.Distance(Player.m_localPlayer.transform.position, matchingPiece.transform.position) >= 29f)
														{

															matchingPiece.gameObject.GetComponent<Piece>().m_nview.GetZDO().SetOwner(closestPlayer.m_nview.GetZDO().GetOwner());
															
														}
													}
												}
												
												if (closestPlayer == null)
												{
													if (matchingPiece.gameObject.GetComponent<Piece>().m_nview != null && matchingPiece.gameObject.GetComponent<Piece>().m_nview.IsOwner())
													{
														if (matchingPiece.gameObject.GetComponent<ZNetView>() != null && matchingPiece.gameObject.GetComponent<Piece>().m_nview.IsValid() && Vector3.Distance(matchingPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()), matchingPiece.transform.position) > 0f && Vector3.Distance(Player.m_localPlayer.transform.position, matchingPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3())) >= 29f)
														{

															var ResetVector = matchingPiece.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3());
															matchingPiece.transform.position = ResetVector;
															matchingPiece.gameObject.GetComponent<Piece>().GetComponent<ZNetView>().GetZDO().Set("floatDirectionValueKey", 1f);


														}
													}
												}
											}
											
										}
									}
								}








                                if (!AllPieces.Contains(allPiece2))
								{

									if (!allPiece2.gameObject.GetComponent<Rigidbody>())
									{

										allPiece2.gameObject.AddComponent<Rigidbody>();
										allPiece2.gameObject.GetComponent<Rigidbody>().isKinematic = true;
										allPiece2.gameObject.GetComponent<Rigidbody>().detectCollisions = true;
										allPiece2.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;


									}

									if (!allPiece2.gameObject.GetComponent<ZSyncTransform>())
									{
										allPiece2.gameObject.AddComponent<ZSyncTransform>();
										allPiece2.gameObject.GetComponent<ZSyncTransform>().m_syncPosition = true;
										allPiece2.gameObject.GetComponent<ZSyncTransform>().m_syncRotation = true;
										allPiece2.gameObject.GetComponent<ZSyncTransform>().m_syncBodyVelocity = true;
										allPiece2.gameObject.GetComponent<ZSyncTransform>().m_isKinematicBody = true;
									}
									// AllPieces.Add(allPiece2);
									
										AllPieces.Add(allPiece2);
									

									if (allPiece2 != null)
									{
										int mask3 = LayerMask.GetMask("piece");
										colpieceFromMovePlat = Physics.OverlapBox(allPiece2.gameObject.GetComponent<BoxCollider>().bounds.center, allPiece2.gameObject.GetComponent<BoxCollider>().bounds.extents * 1.2f, allPiece2.transform.rotation, mask3);

										int processedObjectCount = 1;
										;
										foreach (Collider colliderfromplayer in colpieceFromMovePlat)
										{
											NextBoundObj = null;
											CurrentBoundPiece = null;
											//colliderfromplayer.gameObject.GetComponent<WearNTear>().ResetHighlight();
											//colliderfromplayer.gameObject.GetComponent<WearNTear>().Invoke("ResetHighlight", 0.2f);
											if (colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x2x2" || colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x1x1" || colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_column_1")
											{
												continue;
											}
											else
											{

												CurrentBoundPiece = colliderfromplayer.GetComponentInParent<Piece>().transform;


												if (CurrentBoundPiece != null)
												{

													if (CurrentBoundPiece.parent == null && CurrentBoundPiece.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetBool("AttachedTrue") == true)
													{
														CurrentBoundPiece.SetParent(allPiece2.gameObject.GetComponent<Piece>().transform);

														//CurrentBoundPiece.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader = ZNetScene.instance.GetPrefab("Amber")?.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader;

														CurrentBoundPiece.gameObject.FindChildBound(allPiece2.gameObject, AllBoundPieces);

														processedObjectCount++;



													}
												}
											}



										}
										if (processedObjectCount == colpieceFromMovePlat.Length)
										{

										}
									}
								}

							}
						}
						else
						{
							continue;
						}
					}
				
				}
				
				//int mask22 = LayerMask.GetMask("Default", "static_solid", "terrain", "vehicle", "piece", "viewblock");




				RayFrontTest = false;
				RayBackTest = true;
				SphereTest = false;
				RayFrontLength = 2f;
				RayBackLength = 0f;
				RayDownLength = 1f;

				int mask22 = LayerMask.GetMask("Default", "static_solid", "terrain", "vehicle", "piece", "viewblock");


				Debug.Log("HCW " + hasChangedWall);

				int mask2 = LayerMask.GetMask("Default", "static_solid", "terrain", "vehicle", "piece", "viewblock");
				if (!__instance.IsOnGround())
				{
					KoyoteTimer -= Time.deltaTime;
					if (KoyoteTimer <= 0f)
					{
						UpdateGroundContact_Patch.KoyoteJump = false;
						
					}
					UpdateGroundContact_Patch.isonground = false;
					if (Physics.Raycast(__instance.transform.position, __instance.transform.forward, out var hitInfo3, RayFrontLength, mask2) && !Physics.Raycast(__instance.transform.position, __instance.transform.up * -1f, out var hitInfo4, 1f, mask2))
					{
						currentPoint.Set(hitInfo3.point.x, 0f, hitInfo3.point.z);
						hitInfo3Ynorm.Set(hitInfo3.normal.x, 0f, hitInfo3.normal.z);
						

						if (oldhitInfo3 == Vector3.zero && oldhitPoint == Vector3.zero && hasChangedWall != true)
						{

							oldhitPoint = hitInfo3.point;
							oldhitInfo3 = hitInfo3.normal;
							oldhitInfo3Ynorm.Set(oldhitInfo3.x, 0f, oldhitInfo3.z);
							oldPoint.Set(oldhitPoint.x, 0f, oldhitPoint.z);
							Debug.Log("0");
							hasChangedWall = true;
						}




						//Debug.Log("Angles " + Vector3.Angle(hitInfo3Ynorm, oldhitInfo3Ynorm));

						//if (Vector3.Angle(hitInfo3.normal, oldhitInfo3) >= 30f && hasChangedWall != true)
						//{

						//	Vector3 intersectionPoint;
						//	bool linesIntersect = AreLinesIntersectingClass.AreLinesIntersecting(currentPoint, hitInfo3Ynorm * 2.5f, oldPoint, oldhitInfo3Ynorm * 2.5f, out intersectionPoint);
						//	float distance = Vector3.Distance(currentPoint, oldPoint);

						//	if (linesIntersect == false)
						//	{
								
						//		Debug.Log("1");
						//		oldhitInfo3 = hitInfo3.normal;
						//		oldhitPoint = hitInfo3.point;
						//		oldhitInfo3Ynorm.Set(oldhitInfo3.x, 0f, oldhitInfo3.z);
						//		oldPoint.Set(oldhitPoint.x, 0f, oldhitPoint.z);
						//		hasChangedWall = true;
						//	}
						//	if (linesIntersect == true && distance >= 1.25f)
						//	{
								
						//		Debug.Log("2");
						//		oldhitInfo3 = hitInfo3.normal;
						//		oldhitPoint = hitInfo3.point;
						//		oldhitInfo3Ynorm.Set(oldhitInfo3.x, 0f, oldhitInfo3.z);
						//		oldPoint.Set(oldhitPoint.x, 0f, oldhitPoint.z);
						//		hasChangedWall = true;
						//	}

						//}
						
							Vector3 movementVector = currentPoint - oldPoint;

							// Calculate the distance along the hitInfo3.normal direction
							float distanceAlongNormal = Vector3.Project(movementVector, oldhitInfo3Ynorm).magnitude;
							
							
							if (distanceAlongNormal >= 0.1f)
							{
								Debug.Log("3");
								hasChangedWall = true;
								oldhitInfo3 = hitInfo3.normal;
								oldhitPoint = hitInfo3.point;
								oldhitInfo3Ynorm.Set(oldhitInfo3.x, 0f, oldhitInfo3.z);
								oldPoint.Set(oldhitPoint.x, 0f, oldhitPoint.z);
							}
							
						

						WallJumpAngle = Vector3.Angle(hitInfo3.normal * -1f, Player.m_localPlayer.GetComponent<Character>().m_moveDir.normalized);
						if (WallJumpAngle <= 45f)
						{

							WallSlide = true;
						}
						else
						{
							WallSlide = false;

						}

					}
				}
				if (__instance.IsOnGround() && oldhitInfo3 != Vector3.zero && oldhitPoint != Vector3.zero)
				{
					oldhitInfo3 = Vector3.zero;
					oldhitPoint = Vector3.zero;
				}

				if (WallCheck.TouchingIce != true)
				{
					maxIceRayDistance = 0f;

				}
				if (!__instance.IsOnGround())
				{
					if (!Physics.Raycast(__instance.transform.position, __instance.transform.up * -1, out var hitInfo4, RayDownLength, mask2))
					{

						WallGround.bouncenumber = 0;
					}
				}
			}





			if (Player.m_localPlayer != null)
			{
				int mask2 = LayerMask.GetMask("piece");
				int mask3 = LayerMask.GetMask("character", "character_ghost", "character_net");

				colpiecefromplayerforGhost = Physics.OverlapSphere(Player.m_localPlayer.transform.position, 1, mask3);

				foreach (Collider colliderfromplayerforGhost in colpiecefromplayerforGhost)
				{
					if (colliderfromplayerforGhost.gameObject.GetComponent<Character>() != Player.m_localPlayer)
					{
						Player.m_localPlayer.GetComponent<Character>().gameObject.layer = LayerMask.NameToLayer("character_ghost");
					}
				}
				if (colpiecefromplayerforGhost.Length == 1)
				{
					Player.m_localPlayer.GetComponent<Character>().gameObject.layer = LayerMask.NameToLayer("character");
				}


				if (Player.m_localPlayer.GetRightItem() == null || Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin")
				{
					CanMove = true;
					KeyBindString = "Dick\nDick";
					//secondPanel.SetActive(false);

				}
				if (Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer")
				{

					if (OverrideMove == false)
					{
						CanMove = false;
					}
					KeyBindString =
					"Keybindings\n" +
					config.PlatformIncrease.Value.ToUpper() + " = Distance+\n" +
					config.PlatformDecrease.Value.ToUpper() + " = Distance-\n" +
					config.PlatformSpeedIncrease.BoxedValue.ToString() + " = Speed+\n" +
					config.PlatformSpeedDecrease.BoxedValue.ToString() + " = Speed-\n" +
					config.commandModifier1.BoxedValue.ToString() + " = Size+\n" +
					config.commandModifier2.BoxedValue.ToString() + " = Size-\n"


					;
					//secondPanel.SetActive(true);
				}


				colpiecefromplayer = Physics.OverlapSphere(Player.m_localPlayer.transform.position, 100, mask2);


				Player component = Player.m_localPlayer.gameObject.GetComponent<Player>();


				if (component != null && (Player.m_localPlayer == component))
				{
					if (!component.GetComponent<LineRenderer>())
					{
						ln22 = component.gameObject.AddComponent<LineRenderer>();
						ln22 = component.gameObject.GetComponent<LineRenderer>();

					}

					//ln22.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));



					ln22.textureMode = LineTextureMode.Tile;
					ln22.sharedMaterial = ZNetScene.instance.GetPrefab("projectile_chitinharpoon")?.GetComponentInChildren<MeshRenderer>().sharedMaterial;
					ln22.sharedMaterial.shader = ZNetScene.instance.GetPrefab("projectile_chitinharpoon")?.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader;

					ln22.positionCount = 2;
					ln22.startColor = startColorHook;
					ln22.endColor = endColorHook;

					ln22.startWidth = 0.1f;
					ln22.endWidth = 0.1f;
					ln22.SetPosition(0, Handpoint.rightHand);

					if (TrackGrapple.block != null)
					{

						ln22.SetPosition(1, TrackGrapple.block.transform.position);
						Vector3 differenceDirection = Vector3.forward;
						//DistanceGrapple = Vector3.Dot(differenceDirection, TrackGrapple.HitVectorGrapple- Handpoint.rightHand);
					}
					else
					{
						ln22.SetPosition(1, GameCamera.instance.transform.position + GameCamera.instance.transform.forward * 25f);
					}





					if (Player.m_localPlayer.GetRightItem() == null)
					{

						Player.m_localPlayer.gameObject.gameObject.GetComponent<LineRenderer>().enabled = false;
						//Destroy(colliderfromplayer.GetComponent<LineRenderer>());
					}
					if ((Player.m_localPlayer.GetRightItem() != null && TrackGrapple.block == null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin") || (Player.m_localPlayer.GetRightItem() != null && TrackGrapple.block == null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer"))
					{

						Player.m_localPlayer.gameObject.gameObject.GetComponent<LineRenderer>().enabled = false;


					}
					if (Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin" && TrackGrapple.block != null && Vector3.Distance(Handpoint.rightHand, TrackGrapple.HitVectorGrapple) > 3f && Vector3.Distance(Handpoint.rightHand, TrackGrapple.HitVectorGrapple) <= 50f && IsSwinging == false)

					{

						Player.m_localPlayer.gameObject.gameObject.GetComponent<LineRenderer>().enabled = true;

						if (SoundCounter <= 0 && Vector3.Distance(Handpoint.rightHand, TrackGrapple.HitVectorGrapple) > 5f)
						{
							GameObject prefab = ZNetScene.instance.GetPrefab("sfx_rock_hit");
							GameObject gameObject = UnityEngine.Object.Instantiate(prefab, Handpoint.rightHand + (TrackGrapple.HitVectorGrapple - Handpoint.rightHand) / 4, Quaternion.identity);
							//Destroy(gameObject);
							SoundCounter = 1;
						}


						IsGrappling = true;
						//IsSwinging = false;

					}

					if (Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin" && TrackGrapple.block != null && Vector3.Distance(Handpoint.rightHand, TrackGrapple.HitVectorGrapple) > 3f && Vector3.Distance(Handpoint.rightHand, TrackGrapple.HitVectorGrapple) <= 50f && IsSwinging == true)
					{

						if (SoundCounter <= 0 && Vector3.Distance(Handpoint.rightHand, TrackGrapple.HitVectorGrapple) > 5f)
						{
							GameObject prefab = ZNetScene.instance.GetPrefab("sfx_rock_hit");
							GameObject gameObject = UnityEngine.Object.Instantiate(prefab, Handpoint.rightHand + (TrackGrapple.HitVectorGrapple - Handpoint.rightHand) / 4, Quaternion.identity);
							//Destroy(gameObject);
							SoundCounter = 1;
						}

						Player.m_localPlayer.gameObject.gameObject.GetComponent<LineRenderer>().enabled = true;
						//IsGrappling = true;
						//IsSwinging = false;

						if (JointCounter <= 0)
						{
							jointSwing = Player.m_localPlayer.gameObject.AddComponent<SpringJoint>();
							JointCounter = 1;
						}

						jointSwing.autoConfigureConnectedAnchor = false;
						jointSwing.connectedAnchor = TrackGrapple.block.transform.position;
						float distanceFromPoint = Vector3.Distance(Handpoint.rightHand, TrackGrapple.HitVectorGrapple);

						jointSwing.GetComponent<SpringJoint>().maxDistance = distanceFromPoint * 0.8f;
						jointSwing.GetComponent<SpringJoint>().minDistance = distanceFromPoint * 0.25f;
						jointSwing.GetComponent<SpringJoint>().spring = 1.5f;
						jointSwing.GetComponent<SpringJoint>().damper = 7f;
						jointSwing.GetComponent<SpringJoint>().massScale = 6.5f;
						PlayerPatcher.HookTimer = 0f;


					}

					if (Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin" && TrackGrapple.block != null && Vector3.Distance(Handpoint.rightHand, TrackGrapple.HitVectorGrapple) > 50f)
					{

						Player.m_localPlayer.gameObject.GetComponent<LineRenderer>().enabled = false;
						TrackGrapple.block = null;
						IsGrappling = false;
						IsSwinging = false;
						//Walk.Resetcounter = 0;
						Player.m_localPlayer.Message(MessageHud.MessageType.Center, "To Far Away");
						TrackGrapple.HitVectorGrapple = Vector3.zero;
						Destroy(jointSwing);
						JointCounter = 0;
						PlayerPatcher.GrappleCounter = 0;
					}
				}



				colpiecefromplayer2 = Physics.OverlapSphere(Player.m_localPlayer.transform.position + Vector3.up, 2.5f, mask2);
				TouchingIce = false;
				TouchingSurf = false;

				nearestDistance = 2.5f;

				
				HashSet<string> colliderTypes = new HashSet<string>();
				//Debug.Log("ColLength " + colpiecefromplayer2.Length);
				bool anyColliderWithin0_5fRange = false;
				foreach (Collider colliderfromplayer in colpiecefromplayer2)
				{
					if (colliderfromplayer.gameObject != null)
					{
						Vector3 closestPoint = colliderfromplayer.ClosestPoint(Player.m_localPlayer.transform.position);



						var distance = Vector3.Distance(Player.m_localPlayer.transform.position, closestPoint);
						//Debug.Log(distance);
						if (distance < nearestDistance)
						{
							nearestDistance = distance;
							NearestOjbect = colliderfromplayer.gameObject;
							directionToPlayer = (Player.m_localPlayer.transform.position - closestPoint).normalized;
							
							directionToPlayer *= 8.0f;
						}
						if (distance <= 0.5f)
						{
							anyColliderWithin0_5fRange = true;
						}







						//READ HERE, disable only if not touching surf

						string colliderType = colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString();

						colliderTypes.Add(colliderType);

					}

				}
				within0_5fRange = anyColliderWithin0_5fRange;

				if (colliderTypes.Count == 1 && !colliderTypes.Contains("$piece_blackmarble_floor_triangle") && WallGround.AnyContact)
				{

					DisableSurf = true;

				}
				if (colliderTypes.Count >= 2 && NearestOjbect.GetComponentInParent<Piece>().m_name.ToString() != "$piece_blackmarble_floor_triangle" && within0_5fRange == true)
				{
					DisableSurf = true;
				}



				if (colliderTypes.Count == 1 && colliderTypes.Contains("$piece_blackmarble_floor_triangle"))
				{
					WallGround.AnyContact = false;
					DisableSurf = false;
				}


				foreach (Collider colliderfromplayer in colpiecefromplayer2)
				{
					if (colliderfromplayer.gameObject != null && NearestOjbect != null && NearestOjbect.GetComponentInParent<Piece>().m_name.ToString() == "$piece_stonefloor2x2" && colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_stonefloor2x2" && colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("BhopFloatValueKey") > 0)
					{
						BhopTimeValue = colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey");
						float originalMin = 0f;   // Minimum value of the original range
						float originalMax = 4f;     // Maximum value of the original range

						float targetMin = 0f;       // Minimum value of the target range
						float targetMax = 1f;     // Maximum value of the target range

						// Use Mathf.Lerp to map the original value to the target range


						BhopTimeFactor = Mathf.Lerp(targetMin, targetMax, Mathf.InverseLerp(originalMin, originalMax, BhopTimeValue));

					}
					if (colliderfromplayer.gameObject != null && NearestOjbect != null && NearestOjbect.GetComponentInParent<Piece>().m_name.ToString() == "$piece_crystalwall1x1" && colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_crystalwall1x1" && colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("StickyFloatValueKey") > 0)
					{
						TouchingIce = true;
						HasTouchedIce = true;

						TouchingIceObj = colliderfromplayer.gameObject;
						return;
					}
				}

				if (HasTouchedIce == true && TouchingIce == false)
				{
					HasTouchedIce2 = true;
				}
				if (TouchingIce == false)
				{
					BounceII.TouchingStickyRoof = false;
				}

				int mask2234 = LayerMask.GetMask("piece");
				if (Physics.Raycast(Player.m_localPlayer.GetComponent<Character>().m_body.position + Vector3.up * 0.1f, Vector3.up * -1, out var hitInfo6, 5f, mask2234))
				{
					if (hitInfo6.collider.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_floor_triangle" && hitInfo6.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") >= 0)
					{
						//Debug.Log(hitInfo6.distance);
						if (hitInfo6.distance >= 0f)
						{
							BounceII.AllowSurfing = true;
						}
						if (hitInfo6.distance >= 1.5f)
						{
							Jump_Patch.hasjumped = true;
						}
					}

				}

				int maskNonSolide = LayerMask.GetMask("piece_nonsolid");
				colpiecefromplayerNonSolid = Physics.OverlapSphere(Player.m_localPlayer.transform.position + Vector3.up, 2.5f, maskNonSolide);

				foreach (Collider colliderfromplayerNonSolid in colpiecefromplayerNonSolid)
				{
					// XXX Is ZoneCenter Triggering this?

					if (colliderfromplayerNonSolid != null && colliderfromplayerNonSolid.gameObject.name != "ZoneCenter")
					{
						if (HasTouchedSurf == true || HasTouchedSurf2 == true)
						{
							boost = true;
							boostTimer = 0f;
							GameObject prefab = ZNetScene.instance.GetPrefab("sfx_WishbonePing_far");
							GameObject gameObject3 = UnityEngine.Object.Instantiate(prefab, Player.m_localPlayer.gameObject.GetComponent<Character>().transform.position, Quaternion.identity);
						}
					}

				}

				if (boost)
				{
					// Increase the timer
					boostTimer += Time.deltaTime;

					// Check if the timer has reached the boost duration
					if (boostTimer >= boostDuration)
					{
						// Timer has reached the duration, reset it and deactivate the boost
						boostTimer = 0f;
						boost = false;
					}
				}



				foreach (Collider colliderfromplayer in colpiecefromplayer2)
				{



					if (NearestOjbect != null && NearestOjbect.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_floor_triangle" && colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_floor_triangle" && colliderfromplayer.gameObject != null && colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SurfFloatValueKey") > 0)
					{

						//XXXX Make raycast check

						if (Physics.Raycast(Player.m_localPlayer.transform.position + Vector3.up, Vector3.down, out var hitinfor123, 3f, mask2) && colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") >= 0)
						{



							if (OldSlopeNorm == Vector3.zero)
							{
								OldSlopeNorm = hitinfor123.normal;
							}


							if ((OldSlopeNorm != hitinfor123.normal && PlayerPatcher.GraceSurfSlopeTimer <= 0.1f) || (OldSlopeNorm != hitinfor123.normal && Player.m_localPlayer.GetComponent<Character>().m_body.velocity.magnitude <= 10f))
							{
								Debug.Log("ResetSlope");
								OldSlopeNorm = hitinfor123.normal;
							}

							//Debug.Log("Angle " + Vector3.Angle(OldSlopeNorm, hitinfor123.normal));
							if (Vector3.Angle(OldSlopeNorm, hitinfor123.normal) >= 50f)
							{

								TouchingSurf = false;
								return;
							}


							if (Vector3.Angle(OldSlopeNorm, hitinfor123.normal) < 50f)
							{
								//Player.m_localPlayer.GetComponent<Character>().m_body.velocity = Vector3.ClampMagnitude(Player.m_localPlayer.GetComponent<Character>().m_body.velocity, 70f + Player.m_localPlayer.GetComponent<Character>().m_body.velocity.magnitude * 0.1f);
								TouchingSurf = true;
								HasTouchedSurf = true;
								HasTouchedSurf2 = false;
								PlayerPatcher.HasSwung = false;
								BounceII.IsBouncing = false;

								TouchingSurfObj = colliderfromplayer.gameObject;

								var ScaleVector = hitinfor123.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("scale1", new Vector3());
								var ScaleSize = ScaleVector.x;

								Vector3 localPeak = hitinfor123.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetPosition() + (hitinfor123.collider.gameObject.transform.up * -1 * ScaleSize) + (hitinfor123.collider.gameObject.transform.right * ScaleSize);







							}

							BounceII.projectedSurfaceNormal = hitinfor123.normal - Vector3.Dot(hitinfor123.normal, Vector3.up) * Vector3.up;

							// Calculate the angle between the projected surface normal and move direction
							if (Player.m_localPlayer.GetComponent<Character>().m_moveDir.magnitude > 0)
							{

								float angle2 = Vector3.Angle(BounceII.projectedSurfaceNormal, Player.m_localPlayer.GetComponent<Character>().m_moveDir);

								var CameraXYVector = GameCamera.instance.transform.forward;
								CameraXYVector = new Vector3(CameraXYVector.x, 0, CameraXYVector.z);
								float angle3 = Vector3.Angle(BounceII.projectedSurfaceNormal, CameraXYVector);

								var MoveXYVector = Player.m_localPlayer.GetComponent<Character>().m_body.velocity;
								MoveXYVector = new Vector3(MoveXYVector.x, 0, MoveXYVector.z);
								float angle4 = Vector3.Angle(BounceII.projectedSurfaceNormal, MoveXYVector);
								//if (ValueChecker.IsDecreasing(BounceII.yPos) == false && ValueChecker.IsDecreasing(angle3) == true)
								//{
								//	EjectValue += (Time.deltaTime / 2f);
								//	EjectValue = Mathf.Clamp(EjectValue, 0, 1.5f);
								//}

								//Debug.Log(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.magnitude);
								if (angle2 <= 60f && angle3 <= 75f && Player.m_localPlayer.GetComponent<Character>().m_body.velocity.magnitude >= 20f && PlayerPatcher.GraceSurfSlopeTimer >= 0.5f)
								{
									Debug.Log("STOP");
									BounceII.AllowSurfing = false;
									SurfEject = true;
									//Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y + 0.05f , Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);


									//Vector3 smoothEjectionVelocity = Vector3.Lerp(new Vector3(0f, 0f, 0f), hitInfo5.normal * 50, Time.fixedDeltaTime * 10f);
									//Player.m_localPlayer.GetComponent<Character>().m_body.velocity = smoothEjectionVelocity;

									//Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, 25f, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);
								}

								else
								{
									SurfEject = false;
								}

								//                        if (angle4 >= 160f && MoveXYVector.magnitude >= 70f && angle3 >= 105f)
								//                        {
								//	Debug.Log("STOP  2");
								//	BounceII.AllowSurfing = false;
								//}
							}
						}
					}

				}



				if (HasTouchedSurf == true && TouchingSurf == false)
				{

					HasTouchedSurf2 = true;
					BounceII.AllowSurfing = true;
					PlayerPatcher.GraceSurfSlopeTimer = 0f;
					SurfEject = false;

				}

				if (HasTouchedSurf2 == true)
				{
					Vector3 velocityWithoutY = new Vector3(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.x, 0, Player.m_localPlayer.GetComponent<Character>().m_body.velocity.z);
					float magnitudeWithoutY = velocityWithoutY.magnitude;
					float normalizedVelocity = Mathf.Clamp01(magnitudeWithoutY / 100f);
					BounceII.ScaleFactor = Mathf.Lerp(1, 1.2f, normalizedVelocity);


				}

				if (TouchingSurf == false)
				{
					OldSlopeNorm = Vector3.zero;
					BounceII.maxYPos = 0f;

				}

				PlayerPatcher.IceCooldown += Time.deltaTime;
				if (PlayerPatcher.IceCooldown >= 0.51f)
				{
					PlayerPatcher.IceCooldown = 0f;
				}

				foreach (Collider colliderfromplayer in colpiecefromplayer)
				{
					//XXX

					if (colliderfromplayer.gameObject.GetComponentInParent<Piece>() != null)
					{

						if (colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_floor" ||
							colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_crystalwall1x1" ||
							colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_stonefloor2x2" ||
							colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_floor_triangle")
						{


							if (PlayerPatcher.IceCooldown >= 0.5f)
							{
								if (PlayerPatcher.TrackPiece != null)
								{
									if (PlayerPatcher.piecename == "$piece_blackmarble_floor" && PlayerPatcher.TrackTerrain == null && PlayerPatcher.TrackPiece.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("IceFloatValueKey") > 0 && Player.m_localPlayer.gameObject.GetComponent<Character>().GetMoveDir().magnitude > 0)
									{

										Quaternion deltaRotation = Quaternion.LookRotation(Player.m_localPlayer.gameObject.GetComponent<Character>().GetMoveDir() * -1);
										GameObject prefab = ZNetScene.instance.GetPrefab("vfx_ColdBall_launch");
										if (prefab != null)
										{
											GameObject gameObject3 = UnityEngine.Object.Instantiate(prefab, Player.m_localPlayer.gameObject.GetComponent<Character>().transform.position, deltaRotation);
											Destroy(gameObject3.transform.GetChild(0).gameObject);
											gameObject3.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
											PlayerPatcher.IceCooldown = 0f;
										}
									}
								}
							}

							alpha = 1.0f;
							if (!colliderfromplayer.gameObject.GetComponent<LineRenderer>())
							{

								var ScaleVector = colliderfromplayer.GetComponentInParent<ZNetView>().GetZDO().GetVec3("scale1", new Vector3());
								var ScaleSize = ScaleVector.x;


								colliderfromplayer.gameObject.AddComponent<LineRenderer>();
								ln222 = colliderfromplayer.gameObject.GetComponent<LineRenderer>();
								ln222.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
								ln222.positionCount = 2;





								if (colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("IceFloatValueKey") > 0)
								{
									ln222.SetPosition(0, colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.up) * ((ScaleSize - 1) * 0.5f));
								}
								else if (colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("StickyFloatValueKey") > 0)
								{

									ln222.SetPosition(0, colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.up * (ScaleSize * 0.5f)));
								}
								else if (colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("BhopFloatValueKey") > 0)
								{

									ln222.SetPosition(0, colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.up * (ScaleSize * 0.5f)));
								}
								else if (colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SurfFloatValueKey") > 0)
								{

									ln222.SetPosition(0, colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.up * -1 * ScaleSize) + (colliderfromplayer.gameObject.transform.right * ScaleSize));
								}


								if (colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("IceFloatValueKey") > 0)
								{
									ln222.SetPosition(1, colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.up) * colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("IceFloatValueKey") + (colliderfromplayer.gameObject.transform.up * (ScaleSize - 0.5f)));
								}
								else if (colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("StickyFloatValueKey") > 0)
								{
									ln222.SetPosition(1, colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.up * (ScaleSize * 0.5f)) + (colliderfromplayer.gameObject.transform.forward * colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("StickyFloatValueKey")) + (colliderfromplayer.gameObject.transform.forward * (ScaleSize * 0.66f * colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("StickyFloatValueKey"))));
								}
								else if (colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("BhopFloatValueKey") > 0)
								{
									ln222.SetPosition(1, colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.up * (ScaleSize * 0.5f)) + (colliderfromplayer.gameObject.transform.up * colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("BhopFloatValueKey")) + (colliderfromplayer.gameObject.transform.up * (ScaleSize * 0.66f * colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("BhopFloatValueKey"))));
								}
								else if (colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SurfFloatValueKey") > 0)
								{
									ln222.SetPosition(1, colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.up * -1 * 2f * ScaleSize * colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SurfFloatValueKey")) + (colliderfromplayer.gameObject.transform.right * colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SurfFloatValueKey") * 2f * ScaleSize));
								}
								else
								{
									colliderfromplayer.gameObject.GetComponent<LineRenderer>().enabled = false;
								}

								ln222.startWidth = 0.1f + (colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") * 0.4f);
								ln222.endWidth = 0;

								gradient.SetKeys(
									new GradientColorKey[] { new GradientColorKey(startColor2, 0.0f), new GradientColorKey(endColor2, 1.0f) },
									new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
									);
								ln222.colorGradient = gradient;

							}

							if (Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer")
							{
								colliderfromplayer.gameObject.GetComponent<LineRenderer>().enabled = true;
							}
							if (Player.m_localPlayer.GetRightItem() == null || (Player.m_localPlayer.GetRightItem().m_shared.m_name != "$item_hammer") && Player.m_localPlayer.GetRightItem() != null)
							{
								colliderfromplayer.gameObject.GetComponent<LineRenderer>().enabled = false;
							}


						}


						if (colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x2x2" || colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x1x1" || colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_column_1")
						{

							if (Biomes == 4)
							{
								//XXX
								//colliderfromplayer.GetComponentInChildren<MeshRenderer>().material.shader = ZNetScene.instance.GetPrefab("Amber")?.GetComponentInChildren<MeshRenderer>().material.shader;
							}
							else
							{
								//colliderfromplayer.GetComponentInChildren<MeshRenderer>().material.shader = ZNetScene.instance.GetPrefab("Amber")?.GetComponentInChildren<MeshRenderer>().material.shader;
							}

							alpha = 1.0f;
							if (!colliderfromplayer.GetComponent<LineRenderer>())
							{
								var ScaleVector2 = colliderfromplayer.GetComponent<ZNetView>().GetZDO().GetVec3("scale1", new Vector3());
								var ScaleSize2 = ScaleVector2.x;
								colliderfromplayer.gameObject.AddComponent<LineRenderer>();
								ln = colliderfromplayer.gameObject.GetComponent<LineRenderer>();
								ln.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
								ln.positionCount = 2;



								if (colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_column_1")
								{
									ln.SetPosition(0, colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.up) * (colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TurnDirectionFloatValueKey")) * (ScaleSize2 * 0.5f));
								}
								else if (colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x1x1")
								{
									ln.SetPosition(0, colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.forward * (ScaleSize2 - 1f) * 0.5f));
								}
								else
								{
									ln.SetPosition(0, colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.forward * (ScaleSize2 - 1)));
								}

								if (colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_column_1")
								{

									ln.SetPosition(1, colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.up) * colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TurnDirectionFloatValueKey") + (colliderfromplayer.gameObject.transform.up * TurnDirectionValue * (ScaleSize2 * 0.5f)));
								}
								else
								{
									ln.SetPosition(1, colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetVec3("ZDOPos", new Vector3()) + (colliderfromplayer.gameObject.transform.forward * colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("floatValueKey")) + (colliderfromplayer.gameObject.transform.forward * (ScaleSize2 - 1)));
								}





								//ln.alignment = LineAlignment.Local;

								ln.startWidth = 0.1f + (colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("SpeedFloatValueKey") * 0.4f);
								ln.endWidth = 0;

								gradient.SetKeys(
									new GradientColorKey[] { new GradientColorKey(startColor2, 0.0f), new GradientColorKey(endColor2, 1.0f) },
									new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
									);
								ln.colorGradient = gradient;
							}

							if (Player.m_localPlayer.GetRightItem() == null || Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin")
							{

								colliderfromplayer.gameObject.GetComponent<LineRenderer>().enabled = false;
								//Destroy(colliderfromplayer.GetComponent<LineRenderer>());
							}
							if (Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer")
							{


								colliderfromplayer.gameObject.GetComponent<LineRenderer>().enabled = true;

							}

						}


						if (colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_ironwall" || colliderfromplayer.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_dvergr_metal_wall")
						{

							if (!colliderfromplayer.GetComponent<ParticleSystemRenderer>() && !colliderfromplayer.GetComponent<ParticleSystem>())
							{

								if (colliderfromplayer.transform == colliderfromplayer.gameObject.GetComponentInParent<Piece>().transform)
								{
									colliderfromplayer.gameObject.AddComponent<ParticleSystem>();
									colliderfromplayer.gameObject.AddComponent<ParticleSystemRenderer>();
									rn = colliderfromplayer.gameObject.GetComponent<ParticleSystemRenderer>();
									ps = colliderfromplayer.gameObject.GetComponent<ParticleSystem>();



									rn.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

									//ParticleMat = ZNetScene.instance.GetPrefab("Dandelion")?.GetComponentInChildren<MeshRenderer>().sharedMaterial;
									//rn.material = ParticleMat;
									//rn.material.EnableKeyword("_ALPHATEST_ON");
									//rn.material.SetFloat("_AlphaCutoffEnable", 1.0f); //needs alpha cutoff for cutouts
									//rn.material.SetFloat("_AlphaCutoff", 0.9f); // Settings this property is for HDRP
									//rn.material.SetFloat("_Cutoff", 0.9f); // Setting this property is for the GI baking syst

									//ps.Emit(1);

									ParticleSystem.MainModule psMain = ps.main;
									psMain.loop = true;

									if ((colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") > 0f && colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") <= 5f) || colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") < 0f)
									{
										psMain.startLifetime = 3f;
									}
									if (colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") > 5)
									{

										psMain.startLifetime = 50f;
									}



									psMain.startSpeed = (colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") * 0.35f) * (5f / colliderfromplayer.transform.localScale.x);
									psMain.startColor = new Color(hSliderValueR, hSliderValueG, hSliderValueB, hSliderValueA);
									psMain.startSize = 0.1f;




									psEmission2 = ps.emission;
									if (colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") == 0)
									{

										psEmission2.enabled = false;
										ps.Stop();
									}
									if (psEmission2.enabled == false && (colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") > 0 || colliderfromplayer.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") < 0))
									{
										ps.Play();

										psEmission2.enabled = true;
										psEmission2.rateOverTime = 13f;
									}


									ParticleSystem.ShapeModule psShape = ps.shape;
									psShape.shapeType = ParticleSystemShapeType.Cone;
									psShape.angle = 2f;

									ParticleSystem.LimitVelocityOverLifetimeModule psVelocity = ps.limitVelocityOverLifetime;
									psVelocity.enabled = true;
									psVelocity.dampen = 0.01f;

									psVelocity.limit = 0.05f;
								}
							}
						}

						



					}

				}
				int maskAttachRadius = LayerMask.GetMask("piece");
				colpieceforAttachRadius = Physics.OverlapSphere(Player.m_localPlayer.transform.position + Vector3.up, 10f, maskAttachRadius);
				
				foreach (Collider processedCollider2 in colpieceforAttachRadius)
				{
					if (processedCollider2.gameObject.GetComponentInParent<Piece>() != null)
					{
						if (processedColliders.Contains(processedCollider2))
						{
							continue;
						}
						AllowlnAdd = true;

						
						if (processedCollider2.gameObject.transform.parent != null)
						{
							foreach (Transform parent in processedCollider2.gameObject.transform.parent)
							{
								if (parent.name == "LineRendererObject" || parent.name == "LineRendererObject2")
								{
									AllowlnAdd = false;
								}
							}
						}
						
						else
						{
							Transform NoParent = processedCollider2.gameObject.transform;
							Transform child = NoParent.Find("LineRendererObject");
							Transform child2 = NoParent.Find("LineRendererObject2");
							if (child != null)
							{
								AllowlnAdd = false;
							}
							
						}
						
						Transform rootParent = processedCollider2.gameObject.transform.parent;
			
						// Check if any child of the root parent has a child named "LineRendererObject" or "LineRendererObject2"
						if (rootParent != null)
						{
							
							bool hasLineRendererObject = ChildChecker.HasChildWithName(rootParent, "LineRendererObject");
							bool hasLineRendererObject2 = ChildChecker.HasChildWithName(rootParent, "LineRendererObject2");
							

							if (hasLineRendererObject || hasLineRendererObject2)
							{
								AllowlnAdd = false;
							}
						}
				
						if (rootParent != null)
						{
							Transform rootParent2 = processedCollider2.gameObject.transform.parent.parent;

							if (rootParent2 != null)
							{
								bool hasLineRendererObject = ChildChecker.HasChildWithName(rootParent2, "LineRendererObject");
								bool hasLineRendererObject2 = ChildChecker.HasChildWithName(rootParent2, "LineRendererObject2");
								Debug.Log("1.3");

								if (hasLineRendererObject || hasLineRendererObject2)
								{
									AllowlnAdd = false;
								}
							}
						}

						if (AllowlnAdd == true)
						{
							GameObject LineObject = new GameObject("LineRendererObject");
							GameObject LineObject2 = new GameObject("LineRendererObject2");
							if (processedCollider2.gameObject.transform.parent != null)
							{
								LineObject.transform.SetParent(processedCollider2.gameObject.transform.parent, false); // Parent the LineObject
								LineObject2.transform.SetParent(processedCollider2.gameObject.transform.parent, false); // Parent the LineObject
								LineObject.SetActive(true);
								LineObject2.SetActive(true);

								LineObject.transform.position = processedCollider2.transform.parent.position;
								LineObject2.transform.position = processedCollider2.transform.parent.position;
							}
							else
							{
								
								LineObject.transform.SetParent(processedCollider2.gameObject.transform, false); // Parent the LineObject
								LineObject2.transform.SetParent(processedCollider2.gameObject.transform, false); // Parent the LineObject
								LineObject.SetActive(true);
								LineObject2.SetActive(true);

								LineObject.transform.position = processedCollider2.transform.position;
								LineObject2.transform.position = processedCollider2.transform.position;

							}

							
							if (LineObject2.transform.rotation.eulerAngles.x != 90f)
							{
								// Execute this code when the X rotation is not 90 degrees
								LineObject2.transform.Rotate(Vector3.right, 90f);
							}
							
							LineObject.DrawCircle(processedCollider2.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("AttachRadius"), 0.1f, new Color(1f, 0f, 0f, 1f));
							LineObject2.DrawCircle(processedCollider2.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("AttachRadius"), 0.1f, new Color(1f, 0f, 0f, 1f));
							
						}
						processedColliders.Add(processedCollider2);
						
					}
				}

				

				foreach (Collider processedCollider in processedColliders)
				{

					if (processedCollider != null && processedCollider.gameObject.GetComponentInParent<Piece>() != null)
					{
						if (RadiusToggle == false)
						{
							if (processedCollider.gameObject.transform.parent != null)
							{
								foreach (Transform parent in processedCollider.gameObject.transform.parent)
								{
									
									if (parent.name == "LineRendererObject" || parent.name == "LineRendererObject2")
									{
										// Deactivate the LineRendererObject
										parent.gameObject.SetActive(false);
									}
								}
							}
							else
							{
								Transform NoParent = processedCollider.gameObject.transform;
								Transform child = NoParent.Find("LineRendererObject");
								Transform child2 = NoParent.Find("LineRendererObject2");
								if (child != null)
								{
									child.gameObject.SetActive(false);
									child2.gameObject.SetActive(false);
								}
							}

						}
					
						if (RadiusToggle == true)
						{
							if (processedCollider.gameObject.transform.parent != null)
							{
								foreach (Transform parent in processedCollider.gameObject.transform.parent)
								{
								
									if (parent.name == "LineRendererObject" || parent.name == "LineRendererObject2")
									{
										// Deactivate the LineRendererObject
										parent.gameObject.SetActive(true);
									}
								}
							}
							else
							{
								Transform NoParent = processedCollider.gameObject.transform;
								Transform child = NoParent.Find("LineRendererObject");
								Transform child2 = NoParent.Find("LineRendererObject2");
								if (child != null)
								{
									child.gameObject.SetActive(true);
									child2.gameObject.SetActive(true);
								}
							}
						}
						
						if (!colpieceforAttachRadius.Contains(processedCollider))
						{
							if (processedCollider.gameObject.transform.parent != null)
							{
								
								foreach (Transform parent in processedCollider.gameObject.transform.parent)
								{

									if (parent.name == "LineRendererObject" || parent.name == "LineRendererObject2")
									{
										// Deactivate the LineRendererObject
										parent.gameObject.SetActive(false);
									}

								}
							}
							else
							{
								Transform NoParent = processedCollider.gameObject.transform;
								Transform child = NoParent.Find("LineRendererObject");
								Transform child2 = NoParent.Find("LineRendererObject2");
								if (child != null)
								{
									child.gameObject.SetActive(false);
									child2.gameObject.SetActive(false);
								}
							}
							// Add the collider to the list for removal
							collidersToRemove.Add(processedCollider);
						}
						
					}
				}
				
				foreach (Collider colliderToRemove in collidersToRemove)
				{
					if (colliderToRemove != null && colliderToRemove.gameObject.GetComponentInParent<Piece>() != null)
					{
						processedColliders.Remove(colliderToRemove);
					}
				}

				// Clear the list of colliders to be removed
				collidersToRemove.Clear();
			}
		}
	}

	private static bool Prefix()
	{
		if (PrefabString != "")
		{
			return false;
		}
		return true;
	}

	[HarmonyPatch(typeof(Player), "UpdatePlacementGhost")]
	private static class UpdatePlacementGhostTabCycle_Patch
	{
		private static bool Prefix(ref Player __instance)
		{
			//if (ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer)
			if (Console.IsVisible() && Player.m_localPlayer)
			{
				return false;
			}

			return true;
		}
	}

	[HarmonyPatch(typeof(GameCamera), "UpdateCamera")]
	private static class MaxCamera
	{
		private static void Postfix(ref float ___m_maxDistance)
		{
			if (Player.m_localPlayer != null)
			{
				var rightItem = Player.m_localPlayer.GetRightItem();
				if (rightItem != null)
				{
					if (rightItem.m_shared.m_name == "$item_hammer")
					{
						___m_maxDistance = 20f;
					}
				}
				else
				{
					___m_maxDistance = 8f;
				}
			}
		}
	}


	[HarmonyPatch(typeof(Player), "UpdatePlacementGhost")]
	public static class UpdatePlacementGhostPrism
	{


		public static Vector3 GhostPos = new Vector3(0f, 0f, 0f);
		public static float RotCounter = 0f;
		public static Quaternion rotation;



		private static void Postfix(ref Player __instance, bool flashGuardStone)
		{
			if (__instance.m_placementMarkerInstance && __instance.m_placementGhost)
			{
                if (Input.GetKey(config.UprightPrism.Value) && GhostObject.gameObject.GetComponent<Piece>().m_name.ToString() == "$piece_blackmarble_floor_triangle")
                {
                    GhostPos = __instance.m_placementGhost.transform.position;
                    Quaternion newRotation = Quaternion.Euler(315f, __instance.m_placementGhost.transform.rotation.eulerAngles.y, 90f);
                    __instance.m_placementGhost.transform.rotation = newRotation;
                    UpdatePlacementGhostPrism.UpdatePlacementGhost(__instance, flashGuardStone);
                }
                else if (Input.GetKey(config.UprightPrism.Value) && GhostObject.gameObject.GetComponent<Piece>().m_name.ToString() != "$piece_blackmarble_floor_triangle")
				{
					GhostPos = __instance.m_placementGhost.transform.position;
					Quaternion newRotation = Quaternion.Euler(90f, __instance.m_placementGhost.transform.rotation.eulerAngles.y, 0f);
					__instance.m_placementGhost.transform.rotation = newRotation;
					UpdatePlacementGhostPrism.UpdatePlacementGhost(__instance, flashGuardStone);
				}
			}
		}

		private static void UpdatePlacementGhost(Player __instance, bool flashGuardStone)
		{
			if (!(__instance.m_placementGhost == null))
			{
				bool flag = ZInput.GetButton("AltPlace") || ZInput.GetButton("JoyAltPlace");
				Piece component = __instance.m_placementGhost.GetComponent<Piece>();
				bool water = component.m_waterPiece || component.m_noInWater;
				Vector3 vector;
				Vector3 up;
				Piece piece;
				Heightmap heightmap;
				Collider x;
				if (__instance.PieceRayTest(out vector, out up, out piece, out heightmap, out x, water))
				{
					__instance.m_placementStatus = Player.PlacementStatus.Valid;
					if (__instance.m_placementMarkerInstance == null)
					{
						__instance.m_placementMarkerInstance = UnityEngine.Object.Instantiate<GameObject>(__instance.m_placeMarker, vector, Quaternion.identity);
					}
					__instance.m_placementMarkerInstance.SetActive(true);
					__instance.m_placementMarkerInstance.transform.position = vector;
					__instance.m_placementMarkerInstance.transform.rotation = Quaternion.LookRotation(up);
					if (component.m_groundOnly || component.m_groundPiece || component.m_cultivatedGroundOnly)
					{
						__instance.m_placementMarkerInstance.SetActive(false);
					}
					WearNTear wearNTear = (piece != null) ? piece.GetComponent<WearNTear>() : null;
					StationExtension component2 = component.GetComponent<StationExtension>();
					if (component2 != null)
					{
						CraftingStation craftingStation = component2.FindClosestStationInRange(vector);
						if (craftingStation)
						{
							component2.StartConnectionEffect(craftingStation, 1f);
						}
						else
						{
							component2.StopConnectionEffect();
							__instance.m_placementStatus = Player.PlacementStatus.ExtensionMissingStation;
						}
						if (component2.OtherExtensionInRange(component.m_spaceRequirement))
						{
							__instance.m_placementStatus = Player.PlacementStatus.MoreSpace;
						}
					}
					if (wearNTear && !wearNTear.m_supports)
					{
						__instance.m_placementStatus = Player.PlacementStatus.Invalid;
					}
					if (component.m_waterPiece && x == null && !flag)
					{
						__instance.m_placementStatus = Player.PlacementStatus.Invalid;
					}
					if (component.m_noInWater && x != null)
					{
						__instance.m_placementStatus = Player.PlacementStatus.Invalid;
					}
					if (component.m_groundPiece && heightmap == null)
					{
						__instance.m_placementGhost.SetActive(false);
						__instance.m_placementStatus = Player.PlacementStatus.Invalid;
						return;
					}
					if (component.m_groundOnly && heightmap == null)
					{
						__instance.m_placementStatus = Player.PlacementStatus.Invalid;
					}
					if (component.m_cultivatedGroundOnly && (heightmap == null || !heightmap.IsCultivated(vector)))
					{
						__instance.m_placementStatus = Player.PlacementStatus.NeedCultivated;
					}
					if (component.m_notOnWood && piece && wearNTear && (wearNTear.m_materialType == WearNTear.MaterialType.Wood || wearNTear.m_materialType == WearNTear.MaterialType.HardWood))
					{
						__instance.m_placementStatus = Player.PlacementStatus.Invalid;
					}
					if (component.m_notOnTiltingSurface && (double)up.y < 0.800000011920929)
					{
						__instance.m_placementStatus = Player.PlacementStatus.Invalid;
					}
					if (component.m_inCeilingOnly && (double)up.y > -0.5)
					{
						__instance.m_placementStatus = Player.PlacementStatus.Invalid;
					}
					if (component.m_notOnFloor && (double)up.y > 0.100000001490116)
					{
						__instance.m_placementStatus = Player.PlacementStatus.Invalid;
					}
					if (component.m_onlyInTeleportArea && !EffectArea.IsPointInsideArea(vector, EffectArea.Type.Teleport, 0f))
					{
						__instance.m_placementStatus = Player.PlacementStatus.NoTeleportArea;
					}
					if (!component.m_allowedInDungeons && __instance.InInterior())
					{
						__instance.m_placementStatus = Player.PlacementStatus.NotInDungeon;
					}
					if (heightmap)
					{
						up = Vector3.up;
					}
					__instance.m_placementGhost.SetActive(true);

					if (GhostObject.gameObject.GetComponent<Piece>().m_name.ToString() != "$piece_blackmarble_floor_triangle")
					{
						rotation = Quaternion.Euler(90f, __instance.m_placementGhost.transform.rotation.eulerAngles.y, 0f);
					}
					else if (GhostObject.gameObject.GetComponent<Piece>().m_name.ToString() == "$piece_blackmarble_floor_triangle")
					{
						rotation = Quaternion.Euler(315f, __instance.m_placementGhost.transform.rotation.eulerAngles.y, 90f);
					}
					if (((component.m_groundPiece || component.m_clipGround) && heightmap) || component.m_clipEverything)
					{
						if (__instance.m_buildPieces.GetSelectedPrefab().GetComponent<TerrainModifier>() && component.m_allowAltGroundPlacement && component.m_groundPiece && !ZInput.GetButton("AltPlace") && !ZInput.GetButton("JoyAltPlace"))
						{
							float groundHeight = ZoneSystem.instance.GetGroundHeight(__instance.transform.position);
							vector.y = groundHeight;
						}
						__instance.m_placementGhost.transform.position = vector;
						__instance.m_placementGhost.transform.rotation = rotation;
					}
					else
					{
						Collider[] componentsInChildren = __instance.m_placementGhost.GetComponentsInChildren<Collider>();
						if (componentsInChildren.Length != 0)
						{
							__instance.m_placementGhost.transform.position = vector + up * 50f;
							__instance.m_placementGhost.transform.rotation = rotation;
							Vector3 b = Vector3.zero;
							float num = 999999f;
							foreach (Collider collider in componentsInChildren)
							{
								if (!collider.isTrigger && collider.enabled)
								{
									MeshCollider meshCollider = collider as MeshCollider;
									if (!(meshCollider != null) || meshCollider.convex)
									{
										Vector3 vector2 = collider.ClosestPoint(vector);
										float num2 = Vector3.Distance(vector2, vector);
										if ((double)num2 < (double)num)
										{
											b = vector2;
											num = num2;
										}
									}
								}
							}
							Vector3 b2 = __instance.m_placementGhost.transform.position - b;
							if (component.m_waterPiece)
							{
								b2.y = 3f;
							}
							__instance.m_placementGhost.transform.position = vector + b2;
							__instance.m_placementGhost.transform.rotation = rotation;
						}
					}
					if (!flag)
					{
						__instance.m_tempPieces.Clear();
						Transform transform;
						Transform transform2;
						if (__instance.FindClosestSnapPoints(__instance.m_placementGhost.transform, 0.5f, out transform, out transform2, __instance.m_tempPieces))
						{
							Vector3 position = transform2.parent.position;
							Vector3 vector3 = transform2.position - (transform.position - __instance.m_placementGhost.transform.position);
							if (!__instance.IsOverlappingOtherPiece(vector3, __instance.m_placementGhost.transform.rotation, __instance.m_placementGhost.name, __instance.m_tempPieces, component.m_allowRotatedOverlap))
							{
								__instance.m_placementGhost.transform.position = vector3;
							}
						}
					}
					if (Location.IsInsideNoBuildLocation(__instance.m_placementGhost.transform.position))
					{
						__instance.m_placementStatus = Player.PlacementStatus.NoBuildZone;
					}
					if (!PrivateArea.CheckAccess(__instance.m_placementGhost.transform.position, component.GetComponent<PrivateArea>() ? component.GetComponent<PrivateArea>().m_radius : 0f, flashGuardStone, false))
					{
						__instance.m_placementStatus = Player.PlacementStatus.PrivateZone;
					}


					if (__instance.CheckPlacementGhostVSPlayers())
					{
						__instance.m_placementStatus = Player.PlacementStatus.BlockedbyPlayer;
					}
					if (component.m_onlyInBiome != Heightmap.Biome.None && (Heightmap.FindBiome(__instance.m_placementGhost.transform.position) & component.m_onlyInBiome) == Heightmap.Biome.None)
					{
						__instance.m_placementStatus = Player.PlacementStatus.WrongBiome;
					}
					if (component.m_noClipping && __instance.TestGhostClipping(__instance.m_placementGhost, 0.2f))
					{
						__instance.m_placementStatus = Player.PlacementStatus.Invalid;
					}
				}
				else
				{
					if (__instance.m_placementMarkerInstance)
					{
						__instance.m_placementMarkerInstance.SetActive(false);
					}
					__instance.m_placementGhost.SetActive(false);
					__instance.m_placementStatus = Player.PlacementStatus.Invalid;
				}
				__instance.SetPlacementGhostValid(__instance.m_placementStatus == Player.PlacementStatus.Valid);
				return;
			}
			if (!__instance.m_placementMarkerInstance)
			{
				return;
			}
			__instance.m_placementMarkerInstance.SetActive(false);
		}
	}


	[HarmonyPatch(typeof(Player), "CheckCanRemovePiece")]
	private static class NoRemove
	{
		private static void Postfix(Piece piece, ref bool __result)
		{
			if (!__result)
			{
				return;
			}
			if (!piece || !piece.m_nview.IsValid())
			{
				return;
			}
			//Debug.Log(Game.instance.GetPlayerProfile().GetPlayerID() + " " + piece.GetCreator());
			if (Game.instance.GetPlayerProfile().GetPlayerID() == piece.GetCreator())
			{
				return;
			}
			__result = false;
		}
	}

	[HarmonyPatch(typeof(WearNTear), "RPC_Damage")]
	private static class NoDmg
	{
		private static bool Prefix(WearNTear __instance)
		{
			return false;
		}
	}

	//[HarmonyPatch(typeof(Character), "RPC_Damage")]
	//private static class NoDmgChar
	//{
	//	private static bool Prefix(Character __instance)
	//	{
	//		return false;
	//	}
	//}

	[HarmonyPatch(typeof(Player), "UpdatePlacement")]
	private static class UpdatePlacement_Patch
	{

		public static Color startColor = Color.green;
		public static Color endColor = Color.red;
		public static LineRenderer lineRenderer;
		public static Collider[] colpiece22222 = new Collider[2000];

		public static float countcollider2 = 0;



		public static ParticleSystemRenderer rnGhost;
		public static ParticleSystem psGhost;


		public static float hSliderValueRGhost = 0.5F;
		public static float hSliderValueGGhost = 0.5F;
		public static float hSliderValueBGhost = 0.5F;
		public static float hSliderValueAGhost = 0.5F;

		public static float maxMaterial = 0f;
		public static float maxChild = 0f;

		public static float GhostScaleX;
		public static float GhostScaleY;
		public static float GhostScaleZ;

		public static float PrefabGhostCounter = 0;
		public static float ToDTimer = 1f;

		private static void Prefix(Player __instance, PieceTable ___m_buildPieces, GameObject ___m_placementGhost)
		{
			if (Player.m_localPlayer != null && __instance == Player.m_localPlayer)
			{
				//Player.m_localPlayer.m_maxPlaceDistance = 200f;

				if (__instance != Player.m_localPlayer)
				{
					return;
				}


				if (___m_placementGhost == null)
				{
					return;
				}


				int mask2 = LayerMask.GetMask("piece");
				if (___m_placementGhost != null)
				{

					colpiece22222 = Physics.OverlapSphere(___m_placementGhost.transform.position, 5f, mask2);
				}
				if (___m_placementGhost == null)
				{
					return;
				}



				if (___m_placementGhost != null)
				{

					Piece selectedPiece = ___m_buildPieces.GetSelectedPiece();

					GhostObject = selectedPiece.gameObject;
					string prefab = Utils.GetPrefabName(selectedPiece.gameObject);


					if (selectedPiece != null)
					{
						GhostScaleX = SizeValue;
						GhostScaleY = SizeValue;
						GhostScaleZ = SizeValue;
						___m_placementGhost.transform.localScale = new Vector3(GhostScaleX, GhostScaleY, GhostScaleZ);




						if (DotTrackerParentingDistance.activeSelf && DotTrackerParentingDistance2.activeSelf)
						{
							if (currentSizeValue != SizeValue || currentRadiusValue != RadiusValue)
							{
								//Debug.Log("ErrorDot");
								LineRenderer TempLN = DotTrackerParentingDistance.GetComponent<LineRenderer>();
								LineRenderer TempLN2 = DotTrackerParentingDistance2.GetComponent<LineRenderer>();
								Destroy(TempLN);
								Destroy(TempLN2);
								DotTrackerParentingDistance.SetActive(false);
								DotTrackerParentingDistance2.SetActive(false);
								WallCheck.DotTrackerParentingDistanceCount = 0;
							}

							currentSizeValue = SizeValue;
							currentRadiusValue = RadiusValue;
						}
						

						if (PrefabString == "Reset" || PrefabString == "Size")
						{
							SizeValue = 0;

						}
						if (PrefabString == "Reset")
						{

							OldPrefabString = prefab;
							PrefabString = OldPrefabString;

							var components = ___m_placementGhost.transform.root.GetComponentsInChildren<MeshRenderer>();

							foreach (MeshRenderer r in components)
							{


								//r.materials = ZNetScene.instance.GetPrefab("piece_jackoturnip")?.GetComponentInChildren<MeshRenderer>().materials;
								if (r.material != ZNetScene.instance.GetPrefab(PrefabString)?.GetComponentInChildren<MeshRenderer>().material)
								{

									r.materials = ZNetScene.instance.GetPrefab(PrefabString)?.GetComponentInChildren<MeshRenderer>().materials;
									r.material.name = ZNetScene.instance.GetPrefab(PrefabString)?.GetComponentInChildren<MeshRenderer>().material.name;
								}


							}

							selectedPiece.transform.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader = ZNetScene.instance.GetPrefab(PrefabString)?.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader;

							PrefabString = "";
						}

						//PrefabString = ZNetScene.instance.GetPrefab("Amber").name.ToString();
						//m_biomeNameLarge.text = text;
						//	MessageHud.instance.ShowBiomeFoundMsg(text, playStinger: true);
						//Debug.Log("TOD2");
						if (PrefabString != "" && PrefabString != prefab)
						{
							//Debug.Log(colliderfromplayer.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("PrefabString", ""));
							//var counter = 0f;

							var components = ___m_placementGhost.transform.root.GetComponentsInChildren<MeshRenderer>();

							var counter = 0;

							foreach (MeshRenderer r in components)
							{

								if (ZNetScene.instance.GetPrefab(PrefabString) == null)
								{

									continue;

								}

								if (ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<MeshRenderer>() == null && ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<SkinnedMeshRenderer>() != null)
								{
									SkinnedToggle = true;

								}

								if (ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<MeshRenderer>() != null && SkinnedToggle == false)
								{
									maxMaterial = ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<MeshRenderer>().materials.Length;
									maxChild = ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<MeshRenderer>().Length;
									MatNumber = Mathf.Clamp(MatNumber, 0, (ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<MeshRenderer>().materials.Length - 1));

									if (MatNumber > 0)
									{
										ChildNumber = 0;
										maxChild = 1;
									}
									else
									{
										ChildNumber = Mathf.Clamp(ChildNumber, 0, (ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<MeshRenderer>().Length - 1));
									}

									Debug.Log("MeshMaterial " + (MatNumber + 1) + "/" + maxMaterial + " AdditionalMaterial " + (ChildNumber + 1) + "/" + maxChild + "   " + ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<MeshRenderer>()[ChildNumber].material.name);
									if (ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<MeshRenderer>().materials.Length > 1)
									{

										if (r.material.name.ToString() != ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<MeshRenderer>()[ChildNumber].materials[MatNumber].name.ToString())
										{
											if (counter == 0)
											{
												GameObject prefab2 = ZNetScene.instance.GetPrefab("sfx_equip");
												GameObject gameObject3 = UnityEngine.Object.Instantiate(prefab2, ___m_placementGhost.transform.position, Quaternion.identity);
												counter = 1;
											}


											r.material = ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<MeshRenderer>()[ChildNumber].materials[MatNumber];
											r.material.name = ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<MeshRenderer>()[ChildNumber].materials[MatNumber].name;

										}

									}
									if (ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<MeshRenderer>().materials.Length <= 1)
									{

										if (r.material.name.ToString() != ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<MeshRenderer>()[ChildNumber].material.name.ToString())
										{

											OldPrefabString = prefab;
											r.material = ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<MeshRenderer>()[ChildNumber].materials[MatNumber];
											r.material.name = ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<MeshRenderer>()[ChildNumber].materials[MatNumber].name;

											//OldPrefabString = prefab;
											//r.materials = materials;
											//r.material.name = materials[0].name;

											if (counter == 0)
											{
												GameObject prefab2 = ZNetScene.instance.GetPrefab("sfx_equip");
												GameObject gameObject3 = UnityEngine.Object.Instantiate(prefab2, ___m_placementGhost.transform.position, Quaternion.identity);
												counter = 1;
											}
										}
									}
								}

								else if (ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<SkinnedMeshRenderer>() != null && SkinnedToggle == true)
								{
									maxMaterial = ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<SkinnedMeshRenderer>().materials.Length;
									maxChild = ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<SkinnedMeshRenderer>().Length;
									MatNumber = Mathf.Clamp(MatNumber, 0, (ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<SkinnedMeshRenderer>().materials.Length - 1));

									if (MatNumber > 0)
									{
										ChildNumber = 0;
										maxChild = 1;
									}
									else
									{
										ChildNumber = Mathf.Clamp(ChildNumber, 0, (ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<SkinnedMeshRenderer>().Length - 1));
									}
									Debug.Log("SkinnedMesh " + (MatNumber + 1) + "/" + maxMaterial + " AdditionalMaterial " + (ChildNumber + 1) + "/" + maxChild + "   " + ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<SkinnedMeshRenderer>()[ChildNumber].material.name);


									if (ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<SkinnedMeshRenderer>().materials.Length > 1)
									{
										if (r.material.name.ToString() != ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<SkinnedMeshRenderer>()[ChildNumber].materials[MatNumber].name.ToString())
										{

											if (counter == 0)
											{
												GameObject prefab2 = ZNetScene.instance.GetPrefab("sfx_equip");
												GameObject gameObject3 = UnityEngine.Object.Instantiate(prefab2, ___m_placementGhost.transform.position, Quaternion.identity);
												counter = 1;
											}


											r.material = ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<SkinnedMeshRenderer>()[ChildNumber].materials[MatNumber];
											r.material.name = ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<SkinnedMeshRenderer>()[ChildNumber].materials[MatNumber].name;

										}

									}

									if (ZNetScene.instance.GetPrefab(PrefabString).GetComponentInChildren<SkinnedMeshRenderer>().materials.Length <= 1)
									{

										if (r.material.name.ToString() != ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<SkinnedMeshRenderer>()[ChildNumber].material.name.ToString())
										{

											if (counter == 0)
											{
												GameObject prefab2 = ZNetScene.instance.GetPrefab("sfx_equip");
												GameObject gameObject3 = UnityEngine.Object.Instantiate(prefab2, ___m_placementGhost.transform.position, Quaternion.identity);
												counter = 1;
											}


											OldPrefabString = prefab;
											r.material = ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<SkinnedMeshRenderer>()[ChildNumber].materials[MatNumber];
											r.material.name = ZNetScene.instance.GetPrefab(PrefabString).GetComponentsInChildren<SkinnedMeshRenderer>()[ChildNumber].materials[MatNumber].name;

											//OldPrefabString = prefab;
											//r.materials = materials;
											//r.material.name = materials[0].name;


										}
									}
								}
							}

						}

					}

					if (selectedPiece != null && !selectedPiece.gameObject.GetComponent<LineRenderer>() && selectedPiece.m_name.ToString() == "$piece_blackmarble2x2x2" ||
						selectedPiece != null && !selectedPiece.gameObject.GetComponent<LineRenderer>() && selectedPiece.m_name.ToString() == "$piece_blackmarble2x1x1" ||
						selectedPiece != null && !selectedPiece.gameObject.GetComponent<LineRenderer>() && selectedPiece.m_name.ToString() == "$piece_blackmarble_column_1" ||
						selectedPiece != null && !selectedPiece.gameObject.GetComponent<LineRenderer>() && selectedPiece.m_name.ToString() == "$piece_blackmarble_floor" ||
						selectedPiece != null && !selectedPiece.gameObject.GetComponent<LineRenderer>() && selectedPiece.m_name.ToString() == "$piece_crystalwall1x1" ||
						selectedPiece != null && !selectedPiece.gameObject.GetComponent<LineRenderer>() && selectedPiece.m_name.ToString() == "$piece_stonefloor2x2" ||
						selectedPiece != null && !selectedPiece.gameObject.GetComponent<LineRenderer>() && selectedPiece.m_name.ToString() == "$piece_blackmarble_floor_triangle")
					{





						if (!___m_placementGhost.GetComponent<LineRenderer>())
						{
							lineRenderer = ___m_placementGhost.AddComponent<LineRenderer>();
							lineRenderer = ___m_placementGhost.GetComponent<LineRenderer>();
							lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

						}
						
						lineRenderer.positionCount = 2;
						//YYY
						if (selectedPiece.m_name.ToString() == "$piece_blackmarble_column_1")
						{
							lineRenderer.SetPosition(0, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.up * TurnDirectionValue * (SizeValue * 0.5f)));
						}
						else if (selectedPiece.m_name.ToString() == "$piece_blackmarble_floor")
						{
							lineRenderer.SetPosition(0, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.up * ((SizeValue - 1) * 0.5f)));
						}
						else if (selectedPiece.m_name.ToString() == "$piece_blackmarble2x1x1")
						{
							lineRenderer.SetPosition(0, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.forward * ((SizeValue - 1f) * 0.5f)));
						}
						else if (selectedPiece.m_name.ToString() == "$piece_crystalwall1x1")
						{
							lineRenderer.SetPosition(0, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.up * (SizeValue * 0.5f * StickyPlatformValue)));
						}
						else if (selectedPiece.m_name.ToString() == "$piece_stonefloor2x2")
						{
							lineRenderer.SetPosition(0, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.up * (SizeValue * 0.5f * BhopValue)));
						}
						else if (selectedPiece.m_name.ToString() == "$piece_blackmarble_floor_triangle")
						{
							lineRenderer.SetPosition(0, ___m_placementGhost.transform.position + ___m_placementGhost.gameObject.transform.forward * SizeValue * SurfPlatformValue + ___m_placementGhost.gameObject.transform.right * SizeValue * SurfPlatformValue);
						}
						else
						{
							lineRenderer.SetPosition(0, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.forward * (SizeValue - 1)));
						}

						if (selectedPiece.m_name.ToString() == "$piece_blackmarble_column_1")
						{
							lineRenderer.SetPosition(1, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.up * TurnDirectionValue) + (___m_placementGhost.gameObject.transform.up * TurnDirectionValue * (SizeValue * 0.5f)));
						}
						else if (selectedPiece.m_name.ToString() == "$piece_blackmarble_floor")
						{
							lineRenderer.SetPosition(1, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.up * IcePlatformValue) + (___m_placementGhost.gameObject.transform.up * (SizeValue - 1) * IcePlatformValue));
						}
						else if (selectedPiece.m_name.ToString() == "$piece_crystalwall1x1")
						{
							lineRenderer.SetPosition(1, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.up * (SizeValue * 0.5f * StickyPlatformValue)) + (___m_placementGhost.gameObject.transform.forward * StickyPlatformValue) + (___m_placementGhost.gameObject.transform.forward * (SizeValue * 0.66f * StickyPlatformValue)));
						}
						else if (selectedPiece.m_name.ToString() == "$piece_stonefloor2x2")
						{
							lineRenderer.SetPosition(1, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.up * (SizeValue * 0.5f * BhopValue)) + (___m_placementGhost.gameObject.transform.up * BhopValue) + (___m_placementGhost.gameObject.transform.up * (SizeValue * 0.66f * BhopValue)));
						}
						else if (selectedPiece.m_name.ToString() == "$piece_blackmarble_floor_triangle")
						{

							lineRenderer.SetPosition(1, ___m_placementGhost.transform.position + ___m_placementGhost.gameObject.transform.forward * 2f * SizeValue * SurfPlatformValue + ___m_placementGhost.gameObject.transform.right * 2f * SizeValue * SurfPlatformValue);
						}
						else
						{
							lineRenderer.SetPosition(1, ___m_placementGhost.transform.position + (___m_placementGhost.gameObject.transform.forward * PlatformDistanceValue) + (___m_placementGhost.gameObject.transform.forward * (SizeValue - 1)));
						}
						//lineRenderer.alignment = LineAlignment.Local;

						lineRenderer.startWidth = 0.1f + (SpeedValue * 0.4f);
						lineRenderer.endWidth = 0;
						float alpha = 1.0f;
						Gradient gradient = new Gradient();
						gradient.SetKeys(
							new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
							new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
						);
						lineRenderer.colorGradient = gradient;
						//lineRenderer.startWidth = 1f;
						//lineRenderer.endWidth = 0.5f;
						//___m_placementGhost.transform.localScale = new Vector3(2, 2, 2);



					}
					if (selectedPiece == null)
					{
						return;
					}
					if (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_maypole")
					{
						
						if (TimeOfDay != "")
						{
							string formattedTimeOfDay = TimeOfDay.Replace(',', '.');

							if (float.TryParse(formattedTimeOfDay, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
							{
								EnvMan.instance.m_debugTimeOfDay = true;
								EnvMan.instance.m_debugTime = Mathf.Clamp01(floatValue);
								
							}
						}
						if (TimeOfDay == "Reset")
						{
							EnvMan.instance.m_debugTimeOfDay = false;
							TimeOfDay = "";
						}
						if (GroupingValue == "Reset")
						{
							GroupingValue = "";
						}

						if (WeatherCondition != "")
                        {
							EnvMan.instance.m_debugEnv = WeatherCondition;
						}
						if (WeatherCondition == "Reset")
						{
							EnvMan.instance.m_debugEnv = "Clear";
						}
					}

					if ((selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_ironwall") || (selectedPiece != null && selectedPiece.m_name.ToString() == "$piece_dvergr_metal_wall"))
					{

						if (!___m_placementGhost.GetComponent<ParticleSystem>() && !___m_placementGhost.GetComponent<ParticleSystemRenderer>())
						{
							rnGhost = ___m_placementGhost.AddComponent<ParticleSystemRenderer>();
							rnGhost = ___m_placementGhost.GetComponent<ParticleSystemRenderer>();
							psGhost = ___m_placementGhost.AddComponent<ParticleSystem>();
							psGhost = ___m_placementGhost.GetComponent<ParticleSystem>();
							rnGhost.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
						}

						

						//ParticleMat = ZNetScene.instance.GetPrefab("Dandelion")?.GetComponentInChildren<MeshRenderer>().sharedMaterial;
						//rn.material = ParticleMat;
						//rn.material.EnableKeyword("_ALPHATEST_ON");
						//rn.material.SetFloat("_AlphaCutoffEnable", 1.0f); //needs alpha cutoff for cutouts
						//rn.material.SetFloat("_AlphaCutoff", 0.9f); // Settings this property is for HDRP
						//rn.material.SetFloat("_Cutoff", 0.9f); // Setting this property is for the GI baking syst

						//ps.Emit(1);




						ParticleSystem.MainModule psMain = psGhost.main;
						psMain.loop = true;
						psMain.startLifetime = 60f;
						psMain.startSpeed = TrampolinValue * 0.5f;
						psMain.startColor = new Color(hSliderValueRGhost, hSliderValueGGhost, hSliderValueBGhost, hSliderValueAGhost);
						psMain.startSize = 0.1f;

						psEmission = psGhost.emission;
						if (TrampolinValue == 0)
						{


							psEmission.enabled = false;
							psGhost.Stop();
						}
						if (psEmission.enabled == false && (TrampolinValue > 0 || TrampolinValue < 0))
						{
							psGhost.Play();


							psEmission.enabled = true;
							psEmission.rateOverTime = 13f;
						}



						ParticleSystem.ShapeModule psShape = psGhost.shape;
						psShape.shapeType = ParticleSystemShapeType.Cone;
						psShape.angle = 2f;

						ParticleSystem.LimitVelocityOverLifetimeModule psVelocity = psGhost.limitVelocityOverLifetime;
						psVelocity.enabled = true;
						psVelocity.dampen = 0.01f;

						psVelocity.limit = 0.05f;


						//ParticleMat = ZNetScene.instance.GetPrefab("Dandelion")?.GetComponentInChildren<MeshRenderer>().sharedMaterial;
						//if (ParticleMat != null)
						//{
						//	rend.material = gameObject.GetComponent<MeshRenderer>().material;
						//	rn.material = Resources.GetBuiltinResource<Material>("Default-Particle");
						//	rn.material.shader = ParticleMat.shader;
						//	//rn.material = new Material(Shader.Find("ship_water (Instance)"));
						//}


					}


				}



			}
		}
	}



	[HarmonyPatch(typeof(Character), "UpdateGroundContact")]
	private static class UpdateGroundContact_Patch
	{
		public static float WaterJump;
		public static Vector3 dir2;
		public static Vector3 dir3;
		public static Vector3 jumpvector;
		public static Vector3 groundpoint;
		public static Vector3 Nuller;

		public static bool OnIce;

		public static Quaternion groundpoint2;
		public static float angle;
		public static float windfactor;
		public static float jumpfactor = 10f;
		public static float jumpvelocity;
		public static float jumpvelocity2;

		public static bool isonground;
		public static bool KoyoteJump;

		public static float RunFactor = 20f;
		public static float RunTimeFactor = 0.15f;
		public static float IceFactor = 0f;
		public static float IceNumber = 0f;
		public static float IceJumpCounter = 0f;
		public static float ClampFactor = 0f;

		public static Vector3 MoveVector = Vector3.zero;
		public static Vector3 OldMoveVector = Vector3.zero;
		public static float LeavePlatformJumpForce = 0f;

		public static Quaternion CameraRotFloor = Quaternion.identity;
		public static float CameraPitchFloor = 0f; 
		public static Vector3 CameraPosFloor = Vector3.zero;
		public static Quaternion CameraRot2Floor = Quaternion.identity;



		private static void Prefix(ZNetView ___m_nview, Character __instance, ref float ___m_jumpForceTiredFactor, ref float ___m_flySlowSpeed, ref bool ___m_run, ref bool ___m_walk, ref float ___m_flyFastSpeed, ref Transform ___m_eye, ref float ___m_airControl, ref float ___m_speed, ref float ___m_acceleration, ref float ___m_runSpeed, ref float ___m_walkSpeed, ref Rigidbody ___m_body, ref Vector3 ___m_currentVel, ref float ___m_maxAirAltitude, ref float ___m_waterLevel, ref float ___m_jumpForce, ref float ___m_jumpForceForward, ref float ___m_runTurnSpeed, ref float ___m_turnSpeed, ref float ___m_flyTurnSpeed, ref Vector3 ___m_moveDir, ref Vector3 ___m_lastGroundPoint, ref ZSyncAnimation ___m_zanim)
		{




			if (__instance.IsPlayer() && MoveVector.magnitude <= 0f)
			{

				if (MoveVector.magnitude < ___m_currentVel.magnitude)
				{
					MoveVector.x = ___m_currentVel.x;
					MoveVector.z = ___m_currentVel.z;

				}


			}


			if (__instance.IsPlayer() && Player.m_localPlayer != null)
			{

				Vector3 vector = Vector3.zero;


				if (WallCheck.IsGrappling == true && PlayerPatcher.GrappleCounter <= 13 /*&& WallCheck.IsSwinging == false*/)
				{

					Player.m_localPlayer.GetComponent<Character>().m_body.velocity = TrackGrapple.velocityY + TrackGrapple.velocityXZ;
					PlayerPatcher.GrappleCounter += 1;
				}

				if (PlayerPatcher.HasSwung == true)
				{
					Player.m_localPlayer.GetComponent<Character>().m_airControl = 0.035f;
				}

				if (PlayerPatcher.piecename3 != null && PlayerPatcher.piecename3 == "$piece_blackmarble_arch" && PlayerPatcher.TrackTerrain == null)
				{
					if (!__instance.IsOnGround() && Jump_Patch.hasjumped == true)
					{

						int mask22 = LayerMask.GetMask("piece");

						if (!Physics.Raycast(__instance.transform.position, __instance.transform.up * -1, out var hitInfo4, 2.5f, mask22) && Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y <= 0)
						{


							___m_airControl = 2f;
							Player.m_localPlayer.GetComponent<Character>().m_body.velocity = Vector3.Lerp(Player.m_localPlayer.GetComponent<Character>().m_body.velocity, Vector3.zero, 4 * Time.deltaTime);



							//___m_nview.InvokeRPC(ZNetView.Everybody, "Step", 2, __instance.transform.position);

							//__instance.GetComponent<FootStep>().Invoke(ZNetView.Everybody, "Step", 1, __instance.transform.position);


						}

					}



					if (!__instance.IsOnGround() && Jump_Patch.hasjumped != true)
					{
						Player.m_localPlayer.GetComponent<Character>().m_body.velocity = Vector3.Lerp(Player.m_localPlayer.GetComponent<Character>().m_body.velocity, Vector3.zero, 4 * Time.deltaTime);
					}
				}
			}


			if (PlayerPatcher.piecename3 != null && PlayerPatcher.piecename3 == "$piece_blackmarble_floor" && PlayerPatcher.TrackTerrain == null && __instance.IsPlayer() && Player.m_localPlayer != null)
			{

				if (___m_moveDir.magnitude > 0)
				{
					//IceNumber = Mathf.Lerp(IceNumber, 5f, 0.5f * Time.deltaTime);
					if (___m_run == true)
					{
						RunFactor = 40f;
						RunTimeFactor = 0.075f;
					}
					if (___m_run != true)
					{
						RunFactor = 20f;
						RunTimeFactor = 0.15f;
					}
					if (Jump_Patch.hasjumped == true)
					{
						if (MoveVector.magnitude > OldMoveVector.magnitude)
						{
							OldMoveVector = MoveVector;

						}
						ClampFactor = 0f;
					}
					if (__instance.IsOnGround() && Jump_Patch.hasjumped != true)
					{
						ClampFactor = 19f;
						if (MoveVector.magnitude <= OldMoveVector.magnitude)
						{
							MoveVector = OldMoveVector;
							OldMoveVector = Vector3.zero;
						}
					}
					if (!__instance.IsOnGround() && Jump_Patch.hasjumped != true)
					{
						if (MoveVector.magnitude > OldMoveVector.magnitude)
						{
							OldMoveVector = MoveVector;
						}
						ClampFactor = 0f;
					}

					if (PlayerPatcher.piecename3 != null && PlayerPatcher.piecename == "$piece_blackmarble_floor" && PlayerPatcher.TrackPiece != null  && PlayerPatcher.TrackPiece.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("IceFloatValueKey") > 0)
					{
						OnIce = true;
						MoveVector = Vector3.Lerp(MoveVector, ___m_moveDir * RunFactor, RunTimeFactor * Time.deltaTime);
						MoveVector = Vector3.ClampMagnitude(MoveVector, 1f + ClampFactor);

						___m_acceleration = 0.045f;
						___m_acceleration = Mathf.Clamp(___m_acceleration, 0f, 0.1015f);
						___m_runSpeed = 10f;
						___m_runSpeed = Mathf.Clamp(___m_runSpeed, 10f, 20f);
						___m_walkSpeed = 5f;
						___m_walkSpeed = Mathf.Clamp(___m_walkSpeed, 5f, 20f);
						___m_flySlowSpeed = 10;
						___m_flyFastSpeed = 10;
						___m_turnSpeed = 300f;
						___m_runTurnSpeed = 300f;
						___m_jumpForce = 10;
						___m_jumpForceForward = 0f;
						___m_jumpForceTiredFactor = 0f;
					}

					if (PlayerPatcher.piecename3 != null && PlayerPatcher.piecename3 != "$piece_ironwall" && PlayerPatcher.piecename3 != "$piece_dvergr_metal_wall")
					{
						Jump_Patch.JumpLoadUpFactor = 0.8f;

					}
					if (PlayerPatcher.piecename != null && PlayerPatcher.TrackPiece != null)
					{
						if (PlayerPatcher.piecename != "$piece_blackmarble_floor" || (PlayerPatcher.piecename == "$piece_blackmarble_floor" && PlayerPatcher.TrackPiece.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("IceFloatValueKey") <= 0))
						{
							//MoveVector = Vector3.Lerp(MoveVector, ___m_moveDir, Time.deltaTime);
							//MoveVector = Vector3.ClampMagnitude(MoveVector, 1f + ClampFactor);
							___m_acceleration = 0.045f;
							___m_acceleration = Mathf.Clamp(___m_acceleration, 0f, 0.1015f);
							___m_runSpeed = 10f;
							___m_runSpeed = Mathf.Clamp(___m_runSpeed, 10f, 20f);
							___m_walkSpeed = 5f;
							___m_walkSpeed = Mathf.Clamp(___m_walkSpeed, 5f, 20f);
							___m_flySlowSpeed = 10;
							___m_flyFastSpeed = 10;
							___m_turnSpeed = 300f;
							___m_runTurnSpeed = 300f;
							___m_jumpForce = 10;
							___m_jumpForceForward = 0f;
							___m_jumpForceTiredFactor = 0f;
						}
					}
					//___m_running = false;
					//___m_walk = true;
					//___m_run = false;
					//___m_slippage = 0f;
					//___m_lookDir = MoveVector;

				}

				if (___m_moveDir.magnitude <= 0)
				{
					if (___m_run == true)
					{
						RunFactor = 40f;
						RunTimeFactor = 0.075f;
					}
					if (___m_run != true)
					{
						RunFactor = 20f;
						RunTimeFactor = 0.15f;
					}
					if (Jump_Patch.hasjumped == true)
					{
						if (MoveVector.magnitude > OldMoveVector.magnitude)
						{
							OldMoveVector = MoveVector;
						}
						ClampFactor = 0f;
					}
					if (__instance.IsOnGround() && Jump_Patch.hasjumped != true)
					{
						ClampFactor = 19f;
						if (MoveVector.magnitude <= OldMoveVector.magnitude)
						{
							MoveVector = OldMoveVector;
							OldMoveVector = Vector3.zero;
						}
					}

					if (!__instance.IsOnGround() && Jump_Patch.hasjumped != true)
					{
						if (MoveVector.magnitude > OldMoveVector.magnitude)
						{
							OldMoveVector = MoveVector;
						}
						ClampFactor = 0f;
					}

					//if (ResetList.transform != null && ResetList.GetComponentInParent<ZNetView>().GetZDO() != null)
					//	if (ResetList.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetString("PrefabString") != "")

					if (PlayerPatcher.TrackPiece == null)
					{
						return;
					}

					if (PlayerPatcher.TrackPiece != null)
					{

						if (PlayerPatcher.piecename != null && PlayerPatcher.piecename == "$piece_blackmarble_floor" && PlayerPatcher.TrackPiece.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("IceFloatValueKey") > 0)
						{

							OnIce = true;

							MoveVector = Vector3.Lerp(MoveVector, Vector3.zero, 0.25f * Time.deltaTime);

							___m_acceleration = 0.045f;
							___m_acceleration = Mathf.Clamp(___m_acceleration, 0f, 0.1015f);
							___m_runSpeed = 10f;
							___m_runSpeed = Mathf.Clamp(___m_runSpeed, 10f, 20f);
							___m_walkSpeed = 5f;
							___m_walkSpeed = Mathf.Clamp(___m_walkSpeed, 5f, 20f);
							___m_flySlowSpeed = 10;
							___m_flyFastSpeed = 10;
							___m_turnSpeed = 300f;
							___m_runTurnSpeed = 300f;
							___m_jumpForce = 10;
							___m_jumpForceForward = 0f;
							___m_jumpForceTiredFactor = 0f;
						}
					}

					if (PlayerPatcher.TrackPiece != null)
					{
						if (PlayerPatcher.piecename != null && PlayerPatcher.piecename != "$piece_blackmarble_floor" || (PlayerPatcher.piecename != null && PlayerPatcher.piecename == "$piece_blackmarble_floor" && PlayerPatcher.TrackPiece.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("IceFloatValueKey") <= 0))
						{

							MoveVector = Vector3.Lerp(MoveVector, Vector3.zero, Time.deltaTime);
							//MoveVector = Vector3.zero;
							___m_acceleration = 0.045f;
							___m_acceleration = Mathf.Clamp(___m_acceleration, 0f, 0.1015f);
							___m_runSpeed = 10f;
							___m_runSpeed = Mathf.Clamp(___m_runSpeed, 10f, 20f);
							___m_walkSpeed = 5f;
							___m_walkSpeed = Mathf.Clamp(___m_walkSpeed, 5f, 20f);
							___m_flySlowSpeed = 10;
							___m_flyFastSpeed = 10;
							___m_turnSpeed = 300f;
							___m_runTurnSpeed = 300f;
							___m_jumpForce = 10;
							___m_jumpForceForward = 0f;
							___m_jumpForceTiredFactor = 0f;
						}
					}
				}
			}

			if (__instance == null && __instance.IsPlayer() && Player.m_localPlayer != null)
			{
				// Handle the case where __instance is null
				return;
			}
			if (PlayerPatcher.piecename == null)
			{

				return;
			}
			if (PlayerPatcher.piecename != null && __instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename == "$piece_stonefloor2x2")
			{
				return;
			}

			if (PlayerPatcher.piecename != null && __instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename != "$piece_stonefloor2x2")
			{
				groundpoint = ___m_lastGroundPoint;
				groundpoint2 = Quaternion.LookRotation(__instance.transform.forward);

				if (PlayerPatcher.hasBhopTele == false)
				{
					CameraRotFloor = Player.m_localPlayer.gameObject.GetComponent<Character>().m_lookYaw;
					CameraPitchFloor = Player.m_localPlayer.m_lookPitch;
					CameraPosFloor = GameCamera.instance.transform.position;
					CameraRot2Floor = GameCamera.instance.transform.rotation;
				}
			}


			if (Player.m_localPlayer.GetComponent<Character>().IsPlayer() && WallCheck.TouchingIce == true)
			{

				___m_jumpForceForward = 5f;
				___m_jumpForce = 15f;
				___m_runTurnSpeed = 300f;
				___m_turnSpeed = 300f;
				___m_flyTurnSpeed = 300f;
			}

			if (PlayerPatcher.piecename != null && __instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename == "$piece_table_round")
			{
				___m_jumpForce = 20f;
			}
			if (PlayerPatcher.piecename != null && __instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename == "$piece_ironwall")
			{
				___m_jumpForce = 15f;
			}

			if (PlayerPatcher.piecename != null && PlayerPatcher.TrackPiece != null)
			{
				if (__instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename != "$piece_table_oak" && PlayerPatcher.piecename != "$piece_blackmarble_floor" || (PlayerPatcher.piecename == "$piece_blackmarble_floor" && PlayerPatcher.TrackPiece.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("IceFloatValueKey") <= 0))
				{
					WallGround.shootnumber = 0;
					JumpNumber = 0;
					___m_runSpeed = 7;
					___m_walkSpeed = 5;
					___m_acceleration = 1;



					WallGround.ShotVector = Vector3.zero;

				}
			}
			if (PlayerPatcher.piecename != null)
			{

				if (__instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecenameMovingPiece == "$piece_blackmarble2x2x2" && WallCheck.IsGrappling == false && WallCheck.IsSwinging == false)
				{

					float Yvelo = Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity.y;

					if (Yvelo <= 0f && WallCheck.WallSlide == false)
					{
						Debug.Log("Yvelo " + Yvelo);
						float originalMin = -4f;   // Minimum value of the original range
						float originalMax = 0f;     // Maximum value of the original range

						float targetMin = 13f;       // Minimum value of the target range
						float targetMax = 10f;     // Maximum value of the target range

						LeavePlatformJumpForce = Mathf.Lerp(targetMin, targetMax, Mathf.InverseLerp(originalMin, originalMax, Yvelo));
						Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity.x, -4f, Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity.z);
						Debug.Log("JumpForceLeave " + LeavePlatformJumpForce);
						___m_jumpForce = LeavePlatformJumpForce;
					}
					//XXX make scaling for up movement
					else if (Yvelo > 0f && WallCheck.WallSlide == false)
					{
						Debug.Log("Yvelo " + Yvelo);
						float originalMin = 0f;   // Minimum value of the original range
						float originalMax = 4f;     // Maximum value of the original range

						float targetMin = 10f;       // Minimum value of the target range
						float targetMax = 13f;     // Maximum value of the target range

						LeavePlatformJumpForce = Mathf.Lerp(targetMin, targetMax, Mathf.InverseLerp(originalMin, originalMax, Yvelo));
						Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity.x, -4f, Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity.z);
						Debug.Log("JumpForceLeave " + LeavePlatformJumpForce);
						___m_jumpForce = LeavePlatformJumpForce;
					}

				}



			}
			if (__instance.IsPlayer() && !__instance.IsOnGround() && PlayerPatcher.maxYVelo > 1)
			{
				___m_maxAirAltitude = __instance.transform.position.y + (___m_maxAirAltitude - __instance.transform.position.y) * 0f;
				//jumpvelocity2 = jumpvelocity;
			}



			if (__instance.IsPlayer() && __instance.IsOnGround())
			{
				Jump_Patch.hasjumped = false;
				Jump_Patch.HasWallJumped4 = false;
				BounceII.ActivateJump = false;
				___m_speed = 4f;
				WallCheck.KoyoteTimer = 0.25f;
				KoyoteJump = true;

				if (PlayerPatcher.HookTimer >= 0.5f && PlayerPatcher.haspressed2 == false)
				{
					Debug.Log("reste");
					Player.m_localPlayer.gameObject.GetComponent<LineRenderer>().enabled = false;
					TrackGrapple.block = null;
					WallCheck.IsGrappling = false;
					TrackGrapple.HitVectorGrapple = Vector3.zero;
					Walk.Resetcounter = 0;
					PlayerPatcher.GrappleCounter = 0;
					WallCheck.IsSwinging = false;
					WallCheck.JointCounter = 0;
					Destroy(WallCheck.jointSwing);
					PlayerPatcher.HasSwung = false;
					PlayerPatcher.HookTimerOn = false;
					PlayerPatcher.HookTimer = 0;
					PlayerPatcher.haspressed = false;
					StopWalkWhileSwing.TransitionCounter = 0f;

				}
			}

			if (PlayerPatcher.piecename != null && __instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename == "$piece_blackmarble_floor_triangle")
			{


				BounceII.ScaleFactor = 0f;
				BounceII.AllowSurfing = false;


			}

			if (PlayerPatcher.piecename != null && PlayerPatcher.TrackPiece != null)
			{
				if (__instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename != "$piece_ironwall" && PlayerPatcher.piecename != "$piece_dvergr_metal_wall" && PlayerPatcher.piecename != "$piece_crystalwall1x1" && PlayerPatcher.piecename != "$piece_blackmarble_floor_triangle" && PlayerPatcher.piecename != "$piece_table_round" && PlayerPatcher.piecename != "$piece_blackmarble2x2x2" && PlayerPatcher.piecename != "$piece_blackmarble_floor" || __instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.TrackTerrain != null || (__instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename == "$piece_blackmarble_floor" && PlayerPatcher.TrackPiece.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("IceFloatValueKey") <= 0))
				{


					BounceII.ScaleFactor = 0f;
					PlayerPatcher.maxYVelo = 10f;

					BounceII.maxYPos = 0f;
					WallCheck.BackupSurf = false;
					BounceII.AllowSurfing = true;
					BounceII.IsBouncing = false;

					OldSlopeNorm = Vector3.zero;
					

					___m_jumpForce = 10f;
					___m_jumpForceForward = 1f;
					___m_runTurnSpeed = 300f;
					___m_turnSpeed = 300f;
					___m_walkSpeed = 5f;
					___m_runSpeed = 7f;
					___m_flyTurnSpeed = 12f;

					OnIce = false;
					MoveVector = Vector3.zero;
					OldMoveVector = Vector3.zero;

					HasTouchedIce = false;
					HasTouchedIce2 = false;

					HasTouchedSurf = false;
					HasTouchedSurf2 = false;

					___m_maxAirAltitude = __instance.transform.position.y + (___m_maxAirAltitude - __instance.transform.position.y) * 0f;
					Jump_Patch.hasjumped = false;
					isonground = true;

					if (GroundType != 3)
					{
						WallGround.bouncenumber = 0;
						___m_airControl = 0.1f;
					}
					if (GroundType == 3)
					{
						Jump_Patch.hasjumped = true;
						___m_airControl = 0.1f;
					}
				}
			}


			if (PlayerPatcher.piecename != null && PlayerPatcher.piecename3 != null && __instance.IsPlayer() && !__instance.IsOnGround() && PlayerPatcher.piecename != "$piece_blackmarble_floor" && PlayerPatcher.piecename3 == "$piece_blackmarble_floor")

			{
				UpdateGroundContact_Patch.ClampFactor = 0f;

			}

			if (__instance.IsPlayer() && Biomes == 256)
			{
				WaterJump = ___m_waterLevel - __instance.transform.position.y;
			}

			//Swamp Wind
			//if (Biomes == 2 && __instance.IsPlayer() && __instance.IsOnGround())
			//{
			//	//Set Env
			//	//EnvMan.instance.m_debugEnv = "GDKing";
			//	dir2 = ___m_moveDir;
			//	dir2.y = 0f;
			//	dir2 = dir2.normalized;
			//	dir3 = EnvMan.instance.GetWindDir();
			//	dir3 = dir3.normalized;
			//	angle = Vector3.Angle(dir2, dir3);
			//	float factor = angle / 180f;
			//	windfactor = Mathf.Lerp(20f, 0f, factor);
			//	___m_jumpForceForward = windfactor;
			//}



			


			if (__instance.IsPlayer() && BounceII.hasBounced == true)
			{
				___m_runSpeed = 14f;
				___m_speed = 11f;
			}
		}
	}

	[HarmonyPatch(typeof(Character), "Jump")]
	private static class Jump_Patch
	{
		public static bool hasjumped;
		public static bool HasWallJumped4;
		public static Collider[] RemoveNull = new Collider[2000];
		public static float countcollider3 = 0;
		public static float JumpLoadUpDistance = 0f;
		public static float JumpLoadUpFactor = 0.8f;
		public static float counter = 0f;
		public static float WallJumpLoadUpFactor = 0f;
		public static float KoyoteJumpNumber = 0f;



		private static void Prefix(Character __instance, ref Vector3 ___m_lastGroundNormal, ref Vector3 ___m_currentVel, ref Rigidbody ___m_body, ref float ___m_jumpForce, ref float ___m_jumpForceForward, ref float ___m_lastGroundTouch, ref float ___m_maxAirAltitude, ref float ___m_jumpStaminaUsage, ref float ___m_airControl, ref Vector3 ___m_moveDir)
		{

			//Debug.Log("Jump");
			___m_jumpStaminaUsage = 0f;
			PlayerPatcher.cooldown = 0;


			//countcollider3 = 0;
			//UpdateGroundContact_Patch.IceNumber = 0f;
			//if(__instance.IsPlayer())
			//         {
			//	___m_body.velocity = Vector3.zero;

			//}
			if (__instance.IsOnGround() && Player.m_localPlayer == __instance && __instance.IsPlayer())
			{
				hasjumped = true;
			}

			int maskTrampoline2 = LayerMask.GetMask("piece");
			if (Player.m_localPlayer == __instance && __instance.IsPlayer())
			{
			
				if (Physics.Raycast(Player.m_localPlayer.GetComponent<Character>().m_body.position + Vector3.up * 0.2f, Vector3.up * -1, out var hitInfoTramp, 6f, maskTrampoline2))
				{
					//Logic && IsDecreasing false || hitinfotramp <= 0.5f and make noise when touching
					isJumping = true;
					JumpLoadUpFactor = 0.8f;

					counter = 0f;
					if (counter == 0 && hitInfoTramp.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") > 0f && Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y > 0f && hitInfoTramp.distance <= 6f)
					{
						JumpLoadUpDistance = hitInfoTramp.distance;


						float originalMin = 0.5f;   // Minimum value of the original range
						float originalMax = 6f;     // Maximum value of the original range

						float targetMin = 1f;       // Minimum value of the target range
						float targetMax = 1.5f;     // Maximum value of the target range

						// Use Mathf.Lerp to map the original value to the target range
						JumpLoadUpFactor = Mathf.Lerp(targetMin, targetMax, Mathf.InverseLerp(originalMin, originalMax, JumpLoadUpDistance));


						GameObject prefab = ZNetScene.instance.GetPrefab("sfx_bow_fire_silent");
						GameObject gameObject3 = UnityEngine.Object.Instantiate(prefab, Player.m_localPlayer.GetComponent<Character>().m_body.position, Quaternion.identity);
						counter += 1f;
						Debug.Log("UPWARD");
					}
					if (counter == 0 && hitInfoTramp.collider.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetFloat("TrampolinFloatValueKey") > 0f && Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y <= 0f && hitInfoTramp.distance <= 2f)
					{
						JumpLoadUpDistance = hitInfoTramp.distance;


						float originalMin = 0.5f;   // Minimum value of the original range
						float originalMax = 2f;     // Maximum value of the original range

						float targetMin = 0.95f;       // Minimum value of the target range
						float targetMax = 1f;     // Maximum value of the target range

						// Use Mathf.Lerp to map the original value to the target range
						JumpLoadUpFactor = Mathf.Lerp(targetMin, targetMax, Mathf.InverseLerp(originalMin, originalMax, JumpLoadUpDistance));


						GameObject prefab = ZNetScene.instance.GetPrefab("sfx_jump");
						GameObject gameObject3 = UnityEngine.Object.Instantiate(prefab, Player.m_localPlayer.GetComponent<Character>().m_body.position, Quaternion.identity);
						counter += 1f;
						Debug.Log("downward");


					}

				}
			}


			if (PlayerPatcher.piecename3 == "$piece_blackmarble_floor" && __instance.IsOnGround() && __instance.IsPlayer() && UpdateGroundContact_Patch.OnIce == true)
			{



				Vector3 IceVelo = Player.m_localPlayer.GetComponent<Character>().m_body.velocity;
				Mathf.Acos(Mathf.Clamp01(Player.m_localPlayer.GetComponent<Character>().m_lastGroundNormal.y));
				Vector3 normalized = (Player.m_localPlayer.GetComponent<Character>().m_lastGroundNormal + Vector3.up).normalized;

				float num3 = Player.m_localPlayer.GetComponent<Character>().m_jumpForce;
				float num4 = Vector3.Dot(normalized, IceVelo);
				if (num4 < num3)
				{
					IceVelo += normalized * (num3 - num4);
				}

				//UpdateGroundContact_Patch.MoveVector.Normalize();
				Player.m_localPlayer.GetComponent<Character>().m_jumpForceForward = 0f;

				IceVelo += UpdateGroundContact_Patch.MoveVector.normalized * Player.m_localPlayer.GetComponent<Character>().m_jumpForceForward;

				//___m_body.velocity = Vector3.zero;



				Player.m_localPlayer.GetComponent<Character>().m_body.velocity = IceVelo;

			}

			//YYY






			//Dash
			if (chest == true && Player.m_localPlayer.GetComponent<Character>().IsPlayer() && PlayerPatcher.piecename3 != "$piece_blackmarble_floor" && UpdateGroundContact_Patch.OnIce == false)

			{
				if (Player.m_localPlayer.GetComponent<Character>().IsOnGround())
				{
					hasjumped = true;
				}
				if (DashJumpNumber > 0 && Player.m_localPlayer.GetComponent<Character>().IsOnGround() && WallGround.number1 < 0.1f)
				{
					DashJumpNumber = 0;
					Player.m_localPlayer.GetComponent<Character>().m_airControl = 0.1f;

				}

				if (hasjumped == true && DashJumpNumber < 2 && !__instance.IsOnGround())
				{
					Player.m_localPlayer.GetComponent<Character>().m_airControl = 0.4f;
					Player.m_localPlayer.GetComponent<Character>().m_maxAirAltitude = Player.m_localPlayer.GetComponent<Character>().transform.position.y;
					Player.m_localPlayer.GetComponent<Character>().m_lastGroundTouch = 0.1f;
					Player.m_localPlayer.GetComponent<Character>().m_jumpForce = 5f;
					Player.m_localPlayer.GetComponent<Character>().m_jumpForceForward = 70f;
					DashJumpNumber++;
				}
			}



			//StickWall
			if ((WallCheck.TouchingIce == true || BounceII.TouchingStickyRoof == true && Player.m_localPlayer.GetComponent<Character>().IsPlayer()))
			{

				if (WallCheck.TouchingIce == true || BounceII.TouchingStickyRoof == true && StickyJumpNumber > 0)
				{
					StickyJumpNumber = 0;
				}
				if (StickyJumpNumber < 1 && !Player.m_localPlayer.GetComponent<Character>().IsOnGround() && HasWallJumped2 != true)
				{
					//YYY
					Player.m_localPlayer.GetComponent<Character>().m_maxAirAltitude = Player.m_localPlayer.GetComponent<Character>().transform.position.y;
					Player.m_localPlayer.GetComponent<Character>().m_lastGroundTouch = 0.1f;
					WallCheck.TouchingIce = false;
					HasWallJumped2 = true;
					HasWallJumped3 = false;
					StickyJumpNumber++;


				}
			}



			//WaterJump
			if (helmet == true && __instance.IsPlayer())
			{
				if (WaterJumpNumber > 0 && __instance.IsOnGround() && WallGround.number1 < 0.1f)
				{
					WaterJumpNumber = 0;
				}
				if (WaterJumpNumber < 3 && (double)UpdateGroundContact_Patch.WaterJump > -0.4 && (double)UpdateGroundContact_Patch.WaterJump < 1)
				{
					Player.m_localPlayer.GetComponent<Character>().m_maxAirAltitude = Player.m_localPlayer.GetComponent<Character>().transform.position.y;
					Player.m_localPlayer.GetComponent<Character>().m_lastGroundTouch = 0.1f;
					WaterJumpNumber++;
				}
				if (WaterJumpNumber < 3 && (double)UpdateGroundContact_Patch.WaterJump < -0.4 && !Player.m_localPlayer.GetComponent<Character>().IsOnGround())
				{
					WaterJumpNumber = 3;
				}
			}


			

			//WallJump
			if (cloak == true && Player.m_localPlayer.GetComponent<Character>().IsPlayer() && WallCheck.TouchingIce == false && BounceII.TouchingStickyRoof == false)
			{
				Debug.Log("WallJUmped!");
				if (JumpNumber > 0 && Player.m_localPlayer.GetComponent<Character>().IsOnGround() && WallGround.number1 < 0.1f)
				{
					JumpNumber = 0;
				}
				if (JumpNumber < 10 && !Player.m_localPlayer.GetComponent<Character>().IsOnGround() && WallCheck.within0_5fRange == true && WallCheck.hasChangedWall == true)
				{
					Player.m_localPlayer.GetComponent<Character>().m_jumpForce = 10f;
					if (WallCheck.WallJumpAngle <= 45f)
					{


						float originalMin = 0f;   // Minimum value of the original range
						float originalMax = 45f;     // Maximum value of the original range

						float targetMin = 15f;       // Minimum value of the target range
						float targetMax = 10f;     // Maximum value of the target range

						// Use Mathf.Lerp to map the original value to the target range
						WallJumpLoadUpFactor = Mathf.Lerp(targetMin, targetMax, Mathf.InverseLerp(originalMin, originalMax, WallCheck.WallJumpAngle));
						//Debug.Log("WallJumpLoadUpFactor " + WallJumpLoadUpFactor);

						Player.m_localPlayer.GetComponent<Character>().m_jumpForce = WallJumpLoadUpFactor;
					}
					if (Physics.Raycast(Player.m_localPlayer.GetComponent<Character>().m_body.position, Vector3.up * -1, out var hitInfoTramp, 1f, maskTrampoline2))
					{
						Player.m_localPlayer.GetComponent<Character>().m_jumpForce = 10f;
					}


					//Player.m_localPlayer.GetComponent<Character>().m_jumpForceForward = 3f;
					Player.m_localPlayer.GetComponent<Character>().m_lastGroundNormal = new Vector3(0f, 1f, 0f);
					Player.m_localPlayer.GetComponent<Character>().m_currentVel = WallCheck.directionToPlayer;

					//XXX Error
					HasTouchedSurf = false;
					HasTouchedSurf2 = false;
					Player.m_localPlayer.GetComponent<Character>().m_moveDir = Vector3.zero;
					Player.m_localPlayer.GetComponent<Character>().m_moveDir = WallCheck.directionToPlayer;
					Player.m_localPlayer.GetComponent<Character>().m_maxAirAltitude = Player.m_localPlayer.GetComponent<Character>().transform.position.y;
					Player.m_localPlayer.GetComponent<Character>().m_lastGroundTouch = 0.1f;
					Player.m_localPlayer.GetComponent<Character>().m_turnSpeed = 600f;
					Player.m_localPlayer.GetComponent<Character>().m_runTurnSpeed = 600f;
					Player.m_localPlayer.GetComponent<Character>().m_flyTurnSpeed = 600f;

					//xxx
					//Player.m_localPlayer.GetComponent<Character>().m_speed = 0f;
					//Player.m_localPlayer.GetComponent<Character>().m_runSpeed = 0f;
				
					HasWallJumped3 = false;
					HasWallJumped4 = true;
					//JumpNumber++;

				}
			}

			
			// Doublejump
			if (belt == true && Player.m_localPlayer.GetComponent<Character>().IsPlayer() && WallCheck.TouchingIce == false && BounceII.TouchingStickyRoof == false)
			{
				
				if ((DoubleJumpNumber > 0 && Player.m_localPlayer.GetComponent<Character>().IsOnGround() && WallGround.number1 < 0.1f) || (DoubleJumpNumber > 0 && HasWallJumped3 != true))
				{

					DoubleJumpNumber = 0;
				}
				if (DoubleJumpNumber < 1 && !Player.m_localPlayer.GetComponent<Character>().IsOnGround())
				{
					if (HasTouchedIce)
					{
						Player.m_localPlayer.GetComponent<Character>().m_jumpForce = 5f;
					}
					Player.m_localPlayer.GetComponent<Character>().m_maxAirAltitude = Player.m_localPlayer.GetComponent<Character>().transform.position.y;
					Player.m_localPlayer.GetComponent<Character>().m_lastGroundTouch = 0.1f;
					HasWallJumped3 = true;
					DoubleJumpNumber++;
				}
			}

			// KoyoteJump
			

			if (UpdateGroundContact_Patch.KoyoteJump == true && Player.m_localPlayer.GetComponent<Character>().IsPlayer() && WallCheck.TouchingIce == false && BounceII.TouchingStickyRoof == false)
			{

				if (KoyoteJumpNumber > 0 && Player.m_localPlayer.GetComponent<Character>().IsOnGround())
				{

					KoyoteJumpNumber = 0;
				}
				if (KoyoteJumpNumber < 1 && !Player.m_localPlayer.GetComponent<Character>().IsOnGround())
				{
					Player.m_localPlayer.GetComponent<Character>().m_maxAirAltitude = Player.m_localPlayer.GetComponent<Character>().transform.position.y;
					Player.m_localPlayer.GetComponent<Character>().m_lastGroundTouch = 0.1f;
					KoyoteJumpNumber++;
				}
			}
			UpdateGroundContact_Patch.KoyoteJump = false;
			WallCheck.hasChangedWall = false;
		}
	}

	/// <summary>
	/// ////////////////////////////////////////////////////////Patch Water, Wind, Stamina, Skill Lvl and Spikes
	/// </summary>
	[HarmonyPatch(typeof(Skills.Skill), "Raise")]
	private class ExpLimit
	{
		[HarmonyPrefix]
		internal static bool Prefix(ref float ___m_level, ref float factor)
		{
			if (___m_level >= 1f)
			{
				factor = 0f;
			}
			return true;
		}
	}



	[HarmonyPatch(typeof(EnvMan), "UpdateWind")]
	private static class Wind
	{
		public static Vector3 WindVector = Vector3.zero;
		public static Vector3 OldWindVector = Vector3.zero;
		[HarmonyPrefix]
		public static void Prefix(ref float ___m_debugWindIntensity, ref bool ___m_debugWind)
		{
			if (WallCheck.IsSwinging == true || WallCheck.IsGrappling == true)
			{
				___m_debugWind = true;
				WindVector = Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity;
				if (WindVector.magnitude > OldWindVector.magnitude)
				{
					OldWindVector = WindVector;
				}
				float WindSca = OldWindVector.magnitude + 0.01f / WindVector.magnitude + 0.01f;
				//WindSca = Mathf.Clamp(WindSca, 0, 1);
				//float WindScale = Mathf.Lerp(0, 10, WindMagni /** Time.deltaTime*/); 
				___m_debugWindIntensity = WindSca;

			}
			if (WallCheck.IsSwinging == false && WallCheck.IsGrappling == false)
			{
				___m_debugWindIntensity = 0;
				___m_debugWind = false;

			}

		}
	}

	[HarmonyPatch(typeof(Humanoid), "DrainEquipedItemDurability")]
	private class NoHarpoonDmg
	{
		private static void Prefix(ItemDrop.ItemData item)
		{
			if (item != null && Player.m_localPlayer.GetRightItem() != null)
			{


				if (Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin" || Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer")
				{
					item.m_durability += 10f;
				}

			}
		}
	}

	[HarmonyPatch(typeof(Aoe), "OnHit")]
	private class NoDmgSpike
	{
		private static bool Prefix(ref float ___m_damageSelf)
		{
			___m_damageSelf = 1;
			return true;
		}
	}

	[HarmonyPatch(typeof(WaterVolume), "Awake")]
	private static class NoWave
	{
		[HarmonyPrefix]
		public static void Prefix(ref bool ___m_useGlobalWind)
		{
			___m_useGlobalWind = false;
		}
	}

	[HarmonyPatch(typeof(Player), "CheckRun")]
	private static class RunStaminaDrain_Patch
	{
		[HarmonyPrefix]
		private static void Prefix(ref float ___m_runStaminaDrain)
		{
			___m_runStaminaDrain = 0f;
		}
	}

	/// <summary>
	/// ///////////////////////////////////////////////////////Patch Teleport
	/// </summary>
	[HarmonyPatch(typeof(Player), "UpdateTeleport")]
	private static class FastTele
	{
		[HarmonyPrefix]
		private static void Prefix(ref float ___m_teleportTimer, ref float ___m_teleportCooldown)
		{
			___m_teleportTimer *= 2f;
			___m_teleportCooldown *= 2f;
		}
	}

	[HarmonyPatch(typeof(WearNTear), "IsUnderWater")]
	private static class NoUnderWaterDamage
	{
		[HarmonyPrefix]
		private static void Postfix(ref bool __result)
		{
			__result = false;
		}
	}

	[HarmonyPatch(typeof(WearNTear), "UpdateWear")]
	private static class NoRain
	{
		[HarmonyPrefix]
		private static void Prefix(ref float time)
		{
			time = 0f;
		}
	}

	[HarmonyPatch(typeof(Game), "RequestRespawn")]
	private static class FastRespawn
	{
		public static string ripcount;
		public static float ripnumber;
		public static bool firstrun = false;
		[HarmonyPrefix]
		private static bool Prefix(ref float delay)
		{
			delay = 0f;
			if (firstrun == true)
			{
				ripnumber++;
			}
			ripcount = " RIP:" + ripnumber.ToString();
			firstrun = true;
			return true;
		}
	}

	[HarmonyPatch(typeof(Game), "FindSpawnPoint")]
	private static class SetSpawnPoint
	{
		[HarmonyPrefix]
		private static bool Prefix(out Vector3 point, out bool usedLogoutPoint, float dt, Game __instance, ref bool __result, ref float ___m_respawnWait)
		{
			point = Vector3.zero;
			usedLogoutPoint = false;

			if (spawnPos.y > 0)
			{
				point = spawnPos;
				ZNet.instance.SetReferencePosition(point);
				__result = ZNetScene.instance.IsAreaReady(point);
				return false;
			}
			___m_respawnWait *= 2f;
			return true;
		}
	}

	[HarmonyPatch(typeof(Player), "Awake")]
	private static class SetTele
	{
		[HarmonyPrefix]
		public static void Prefix(ref Player __instance)
		{

			if (PlayerPatcher.cpnumber < 1 && Player.m_localPlayer == __instance.GetComponentInParent<ZNetView>().IsOwner())
			{
				savedPos = new Vector3(__instance.transform.position.x, __instance.transform.position.y, __instance.transform.position.z);
				savedPos2 = new Vector3(__instance.transform.position.x, __instance.transform.position.y, __instance.transform.position.z);
			}
			spawnPos = Vector3.zero;
		}
	}



	[HarmonyPatch(typeof(Player), "Awake")]
	public static class Player_Awake_Patch
	{
		private static void Postfix(ref Player __instance)
		{
			__instance.m_maxPlaceDistance = 200f;
		}
	}

	[HarmonyPatch(typeof(Chat), "InputText")]
	private static class test
	{

		[HarmonyPrefix]
		public static void Prefix(InputField ___m_input)
		{
			if (Player.m_localPlayer != null)
			{
				if (Player.m_localPlayer == Player.m_localPlayer.GetComponentInParent<ZNetView>().IsOwner())
				{
					string text = ___m_input.text;
					if (text == "/die")
					{
						//HitData hitData = new HitData();
						//hitData.m_damage.m_damage = 99999f;
						//Player.m_localPlayer.Damage(hitData);
						spawnPos = Vector3.zero;

					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(Terminal), "InputText")]
	private static class test2
	{

		[HarmonyPrefix]
		public static void Prefix(GuiInputField ___m_input)
		{
			if (Player.m_localPlayer != null)
			{
				if (Player.m_localPlayer != null && Player.m_localPlayer.GetComponentInParent<ZNetView>().IsOwner())
				{ 
					ChildNumber = 0;
					MatNumber = 0;
					SkinnedToggle = false;

					string input = ___m_input.text;
					string[] parts = input.Split(' ');

					if (parts.Length == 2)
					{
						string numberString = parts[0];
						string wordString = parts[1];

						// Now, numberString contains "0.5" and wordString contains "tree"

						// You can attempt to parse numberString as a float if needed
						if (float.TryParse(numberString, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
						{
							TimeOfDay = numberString;
							GroupingValue = numberString;
							// Successfully parsed the number as a float
						}
						if (!float.TryParse(wordString, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue2))
						{
							WeatherCondition = wordString;
							WeatherCondition = char.ToUpper(WeatherCondition[0]) + WeatherCondition.Substring(1).ToLower();
							// Successfully parsed the number as a float
						}
					}
					if (parts.Length == 1)
					{
						string numberString = parts[0];

						// Now, numberString contains "0.5" and wordString contains "tree"

						// You can attempt to parse numberString as a float if needed
						if (float.TryParse(numberString, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
						{
							TimeOfDay = numberString;
							GroupingValue = numberString;
							WeatherCondition = "Clear";
							// Successfully parsed the number as a float
						}
						if (!float.TryParse(numberString, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue4))
						{
							WeatherCondition = numberString;
							WeatherCondition = char.ToUpper(WeatherCondition[0]) + WeatherCondition.Substring(1).ToLower();
						}
						if (!float.TryParse(numberString, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue3))
						{
							PrefabString = ___m_input.text;
							MatPrefabString = ___m_input.text;
						}
					}

					
					

					

					Debug.Log(PrefabString);
					Debug.Log(MatPrefabString);
					Debug.Log(TimeOfDay);
				}
			}
		}
	}

	[HarmonyPatch(typeof(Chat), "Update")]
	private static class ChatCheck
	{
		public static bool checking;

		[HarmonyPrefix]
		public static void Prefix(ref bool ___m_wasFocused)
		{
			if (___m_wasFocused)
			{
				checking = true;
			}
			if (!___m_wasFocused)
			{
				checking = false;
			}
		}
	}

	[HarmonyPatch(typeof(VisEquipment), "UpdateVisuals")]
	private static class ItemPower

	{
		public static void Prefix(ZNetView ___m_nview)
		{
			if (Player.m_localPlayer != null && ___m_nview.IsOwner())
			{
				//Check Helmet
				if (Player.m_localPlayer.GetComponent<VisEquipment>().m_helmetItem == "HelmetBronze")
				{
					helmet = true;

				}
				else
				{
					helmet = false;

				}

				//Check Cloak
				if (Player.m_localPlayer.GetComponent<VisEquipment>().m_shoulderItem == "CapeWolf")
				{
					cloak = true;

				}
				else
				{
					cloak = false;

				}

				//Check Shoe
				if (Player.m_localPlayer.GetComponent<VisEquipment>().m_legItem == "ArmorBronzeLegs")
				{
					shoes = true;

				}
				else
				{
					shoes = false;

				}

				//Check Chest
				if (Player.m_localPlayer.GetComponent<VisEquipment>().m_chestItem == "ArmorBronzeChest")
				{
					chest = true;

				}
				else
				{
					chest = false;

				}

				//Check Utility
				if (Player.m_localPlayer.GetComponent<VisEquipment>().m_utilityItem != null)
				{

					if (Player.m_localPlayer.GetComponent<VisEquipment>().m_utilityItem == "Demister")
					{
						demister = true;

					}

					else
					{
						demister = false;

					}

					if (Player.m_localPlayer.GetComponent<VisEquipment>().m_utilityItem == "BeltStrength")
					{
						belt = true;

					}

					else
					{
						belt = false;

					}
				}
			}
		}
	}

	/// <summary>
	/// Move this somerwhere better
	/// </summary>


	[HarmonyPatch(typeof(Projectile), "FixedUpdate")]
	internal static class ProjectileSpeed
	{
		private static void Prefix(ref Vector3 ___m_vel)
		{

			___m_vel *= 2.1f;
			Vector3.ClampMagnitude(___m_vel, 4f);
		}
	}

	[HarmonyPatch(typeof(Player), "UseStamina")]
	internal static class NoStaminaCostPatch
	{
		private static void Prefix(Player __instance, ref float v)
		{
			if (__instance.GetRightItem() != null)
			{
				if (__instance.GetRightItem().m_shared.m_name == "$item_hammer")
				{
					v = 0f;
				}
				if (__instance.GetRightItem().m_shared.m_name == "$item_spear_chitin")
				{
					v = 0f;
				}
			}
		}
	}

	[HarmonyPatch(typeof(Game), "FindSpawnPoint")]
	private static class SetSpawnPoints
	{
		[HarmonyPrefix]
		private static bool Prefix(out Vector3 point, out bool usedLogoutPoint, float dt, Game __instance, ref bool __result)
		{
			point = Vector3.zero;
			usedLogoutPoint = false;

			if (spawnPos.y > 0)
			{
				point = spawnPos;
				ZNet.instance.SetReferencePosition(point);
				__result = ZNetScene.instance.IsAreaReady(point);
				return false;
			}
			return true;
		}
	}

	//   [HarmonyPatch(typeof(Player), "PieceRayTest")]
	//   private static class HammerSign
	//   {


	//	[HarmonyPrefix]
	//       public static bool Prefix(out Vector3 normal, out Vector3 point, out Piece piece, out Heightmap heightmap, out Collider waterSurface, ref int ___m_placeRayMask)
	//       {
	//		int mask22 = ___m_placeRayMask;


	//		RaycastHit val = default(RaycastHit);

	//		if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out val, 50f, mask22) && val.collider.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x2x2")
	//           {
	//				point = val.point;
	//				normal = val.normal;
	//				piece = val.collider.GetComponent<Piece>();
	//				heightmap = val.collider.GetComponent<Heightmap>();

	//			
	//				if (val.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
	//				{
	//					waterSurface = val.collider;
	//				}
	//				else
	//				{
	//					waterSurface = null;
	//				}
	//				return true;
	//		}
	//		point = val.point;
	//		normal = val.normal;
	//		piece = val.collider.GetComponent<Piece>();
	//		heightmap = val.collider.GetComponent<Heightmap>();
	//		waterSurface = null;
	//		return false;

	//	}
	//}



	/// <summary>
	/// //////////////////////////////////////////////////////Updateing TP/CP, Stopwatch, WallChecker
	/// </summary>
	[HarmonyPatch(typeof(Player), "Update")]
	internal class PlayerPatcher
	{
		public static Quaternion rotation;
		public static Quaternion rotation2;
		public static string timing;
		public static string tpcount;
		public static string cpcount;
		public static string text3;

		public static string piecename;
		public static string piecename3;
		public static string piecename4;
		public static string piecenameMovingPiece;
		//public static string piecename5;
		//public static string signname;
		//public static float signnumber;



		public static int tpnumber;
		public static int cpnumber;
		public static float cooldown = 0;
		public static float SlowFallCooldown = 0;
		public static float IceCooldown = 0;
		public static float counter1 = 0;
		public static float counter2 = 0;

		public static float HookTimer;
		public static bool HookTimerOn;

		public static float GrappleCounter;
		public static float counterSP;

		public static Collider[] colpiece222 = new Collider[2000];
		public static Collider[] colpiece2222 = new Collider[2000];
		public static Collider[] array = new Collider[2000];

		public static string[] piecename2 = new string[2000];
		public static Vector3[] MovePosPiece = new Vector3[2000];

		public static Vector3 realHitPoint;

		public static Vector3[] spawnPosPiece = new Vector3[2000];
		public static Quaternion[] spawnRotPiece = new Quaternion[2000];
		public static Vector3 m_EulerAngleVelocity2 = new Vector3(0, 10, 0);

		public static float JumpLoadUp;
		public static bool[] touched = new bool[2000];
		public static float OldSin = 0f;

		public static Piece TrackPiece = new Piece();
		public static Piece TrackPiece2 = new Piece();
		public static Heightmap TrackTerrain = new Heightmap();

		public static GameObject[] GOpiece2 = new GameObject[2000];
		public static GameObject[] GOpiece22 = new GameObject[2000];


		public static Ground CurrentGround { get; private set; } = Ground.None;

		public static bool haspressed = false;
		public static bool haspressed2 = false;

		public static bool HasSwung;

		public static RaycastHit[] sphereCastHitUpdate;
		public static RaycastHit rayCastHitUpdate;
		public static RaycastHit SpCastHitUpdate;
		public static Color AimAssistColor = Color.red;
		public static Vector3 targetAngle = new Vector3(0f, 345f, 0f);
		public static Vector3 currentAngle;
		public static float DotCounter;
		public static Vector3 CharPosition;

		public static bool KeyModifier;
		public static Vector3 m_EulerAngleVelocityIce = new Vector3(0, 10, 0);

		public static float GraceSurfSlopeTimer = 0f;
		public static bool InvalidRun = false;
		public static float maxYVelo = 10f;
		public static float TrampTreckNumber = 0f;
		public static float FloatYVelo = 10f;

		public static Vector3 CameraPos = Vector3.zero;
		public static Vector3 CameraPosPrev = Vector3.zero;
		public static Quaternion CameraRot = Quaternion.identity;
		public static Quaternion CameraRotPrev = Quaternion.identity;
		public static Quaternion CameraRot2 = Quaternion.identity;
		public static Quaternion CameraRot2Prev = Quaternion.identity;
		public static float CameraPitch = 0f;
		public static float CameraPitchPrev = 0f;
		public static bool hasTeleported = false;
		public static bool hasTeleported2 = false;
		public static float TeleTimer = 0.1f;
		public static bool hasBhopTele = false;
		public static float BhopeTeleTimer = 0.1f;
		public static float ZoneResetCounter = 0f;
		public static float ZoneResetCounter2 = 0f;
		public static float RadiusResetCounter = 0f;
		public static float RadiusResetCounter2 = 0f;
		


		public static void Postfix(ref Player __instance)
		{

			if (__instance == Player.m_localPlayer && Player.m_localPlayer != null)
			{
				


				FloatYVelo = Mathf.Abs(Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y);

				float originalMin = 1f;   // Minimum value of the original range
				float originalMax = 15f;     // Maximum value of the original range

				float targetMin = 20f;       // Minimum value of the target range
				float targetMax = 45f;     // Maximum value of the target range

				// Use Mathf.Lerp to map the original value to the target range
				var scalingFactor = targetMin + (BounceII.trampolineValue - originalMin) * (targetMax - targetMin) / (originalMax - originalMin);








				FloatYVelo = Mathf.Clamp(FloatYVelo, 10f, scalingFactor);

				if (TrampTreckNumber != BounceII.trampolineValue)
				{
					TrampTreckNumber = BounceII.trampolineValue;
					maxYVelo = FloatYVelo;
				}

				if (FloatYVelo > maxYVelo)
				{
					maxYVelo = FloatYVelo;
				}
				if (BounceII.hasBounced == true)
				{
					maxYVelo = FloatYVelo;
				}
				//Debug.Log("FloatY "  + FloatYVelo + " MaxVelo " + maxYVelo + " bool " + Player.m_localPlayer.GetComponent<Character>().m_body.velocity.y);


				SpeedValue = Mathf.Clamp(SpeedValue, 0f, 4f);
				RadiusValue = Mathf.Clamp(RadiusValue, 0f, 5f);
				PlatformDistanceValue = Mathf.Clamp(PlatformDistanceValue, 0f, 15f);
				TrampolinValue = Mathf.Clamp(TrampolinValue, -1f, 15f);
				SizeValue = Mathf.Clamp(SizeValue, 1f, 19f);
				TurnDirectionValue = Mathf.Clamp(TurnDirectionValue, -1f, 1f);
				IcePlatformValue = Mathf.Clamp(IcePlatformValue, 0f, 1f);
				StickyPlatformValue = Mathf.Clamp(StickyPlatformValue, 0f, 1f);
				BhopValue = Mathf.Clamp(BhopValue, 0f, 1f);
				SurfPlatformValue = Mathf.Clamp(SurfPlatformValue, 0f, 1f);

				
				if (Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_hammer")
				{
					if (ZoneToggle == false && ZoneResetCounter2 < 1f)
					{
						
						ZoneToggle = true;
						ZoneResetCounter2++;
					}
					
				}
				//Debug.Log("Rbool " + RadiusToggle);

				CharPosition = Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().position;
				//__instance.SetMouseLook(Vector2.zero);
				if (config.IfCheckpointkeyPressed() && UpdateGroundContact_Patch.isonground == true && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer)
				{
					savedPos2 = savedPos;
					savedPos = new Vector3(__instance.transform.position.x, __instance.transform.position.y, __instance.transform.position.z);
					rotation2 = rotation;
					rotation = Quaternion.LookRotation(__instance.transform.forward);

					CameraRotPrev = CameraRot;
					CameraRot = Player.m_localPlayer.gameObject.GetComponent<Character>().m_lookYaw;

					CameraPitchPrev = CameraPitch;
					CameraPitch = __instance.m_lookPitch;

					CameraPosPrev = CameraPos;
					CameraPos = GameCamera.instance.transform.position;

					CameraRot2Prev = CameraRot2;
					CameraRot2 = GameCamera.instance.transform.rotation;
					spawnPos = savedPos;
					cpnumber++;
					Debug.Log($"Saved pos : {savedPos}");
					Debug.Log($"Saved pos : {savedPos2}");
				}
				else if (config.IfTeleportingkeyPressed() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer)
				{

					__instance.TeleportTo(savedPos, rotation, distantTeleport: true);
					hasTeleported = true;

					//BounceII.AllowSurfing = true;
					WallCheck.TouchingSurf = false;
					HasTouchedSurf = false;
					HasTouchedSurf2 = false;
					Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(0, 0, 0);
					Player.m_localPlayer.GetComponent<Character>().m_currentVel = new Vector3(0, 0, 0);
					Debug.Log($"Teleported to : {savedPos}");
					tpnumber++;
				}
				else if (config.IfTeleportingkey2Pressed() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer)
				{

					__instance.TeleportTo(savedPos2, rotation2, distantTeleport: true);
					hasTeleported2 = true;

					//BounceII.AllowSurfing = true;
					WallCheck.TouchingSurf = false;
					HasTouchedSurf = false;
					HasTouchedSurf2 = false;
					Player.m_localPlayer.GetComponent<Character>().m_body.velocity = new Vector3(0, 0, 0);
					Player.m_localPlayer.GetComponent<Character>().m_currentVel = new Vector3(0, 0, 0);
					Debug.Log($"Teleported to : {savedPos2}");
					tpnumber++;
				}
				else if (config.IfKillMePressed() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer)
				{
					spawnPos = Vector3.zero;
					HitData hitData = new HitData();
					hitData.m_damage.m_damage = 99999f;
					Player.m_localPlayer.Damage(hitData);
				}
				else if (config.IfPlatformIncreasePressed() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer && !FindModifyer.IsDown(config.commandModifier1.Value) && !FindModifyer.IsDown(config.commandModifier3.Value) && !FindModifyer.IsDown(config.commandModifier5.Value))
				{



					ChildNumber += 1;
				}

				else if (FindModifyer.IsDown(config.commandModifier1.Value) && Player.m_localPlayer && !FindModifyer.IsDown(config.PlatformSpeedIncrease.Value) && !FindModifyer.IsDown(config.commandModifier5.Value) && !FindModifyer.IsDown(config.RadiusIncrease.Value))
				{

					SizeValue += 1;


				}

				else if (FindModifyer.IsDown(config.commandModifier3.Value) && Player.m_localPlayer && !FindModifyer.IsDown(config.commandModifier5.Value))
				{


					MatNumber += 1;

				}

				else if (FindModifyer.IsDown(config.commandModifier5.Value) && !FindModifyer.IsDown(config.RadiusIncrease.Value) && Player.m_localPlayer)
				{
					PlatformDistanceValue += 1;
					TurnDirectionValue += 1;
					TrampolinValue += 1;
					IcePlatformValue += 1;
					StickyPlatformValue += 1;
					BhopValue += 1;
					SurfPlatformValue += 1;
				}
				else if (config.IfPlatformdDecreasePressed() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer && !FindModifyer.IsDown(config.commandModifier2.Value) && !FindModifyer.IsDown(config.commandModifier4.Value) && !FindModifyer.IsDown(config.commandModifier6.Value))
				{


					ChildNumber -= 1;
				}

				else if (FindModifyer.IsDown(config.RadiusIncrease.Value) && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer /*&& !FindModifyer.IsDown(config.PlatformSpeedIncrease.Value) && !FindModifyer.IsDown(config.commandModifier5.Value)*/)
				{
					Debug.Log("+1");
					RadiusValue += 1f;

				}
				else if (FindModifyer.IsDown(config.RadiusDecrease.Value) && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer /*&& !FindModifyer.IsDown(config.PlatformSpeedDecrease.Value) && !FindModifyer.IsDown(config.commandModifier6.Value)*/)
				{
					RadiusValue -= 1f;
					Debug.Log("-1");
				}

				else if (FindModifyer.IsDown(config.commandModifier2.Value) && Player.m_localPlayer && !FindModifyer.IsDown(config.PlatformSpeedDecrease.Value) && !FindModifyer.IsDown(config.commandModifier6.Value) && !FindModifyer.IsDown(config.RadiusDecrease.Value))
				{

					SizeValue -= 1;

				}
				else if (FindModifyer.IsDown(config.commandModifier4.Value) && Player.m_localPlayer && !FindModifyer.IsDown(config.commandModifier6.Value))
				{
					MatNumber -= 1;
				}
				else if (FindModifyer.IsDown(config.commandModifier6.Value) && !FindModifyer.IsDown(config.RadiusDecrease.Value) && Player.m_localPlayer)
				{
					PlatformDistanceValue -= 1;
					TurnDirectionValue += -1;
					TrampolinValue -= 1;
					IcePlatformValue -= 1;
					StickyPlatformValue -= 1;
					BhopValue -= 1;
					SurfPlatformValue -= 1;
				}

				else if (FindModifyer.IsDown(config.PlatformSpeedIncrease.Value) && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer)
				{
					SpeedValue += 1f;

				}
				else if (FindModifyer.IsDown(config.PlatformSpeedDecrease.Value) && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer)
				{
					SpeedValue -= 1f;
				}

				

				
				
				else if (config.IfSkinTogglePressed() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer && PrefabString != "")
				{
					GameObject prefab = ZNetScene.instance.GetPrefab(PrefabString);

					if (SkinnedToggle == true && prefab != null && prefab.GetComponentInChildren<MeshRenderer>() != null)
					{
						SkinnedToggle = false;
					}
					else if (SkinnedToggle == false && prefab != null && prefab.GetComponentInChildren<SkinnedMeshRenderer>() != null)
					{
						SkinnedToggle = true;
					}
				}

				if (FindModifyer.IsDown(config.ZoneGrid.Value) && !FindModifyer.IsDown(config.RadiusGrid.Value) && Player.m_localPlayer)
				{
					Debug.Log("F6");
					if (ZoneToggle == true)
					{
						ZoneToggle = false;
					}
					else if (ZoneToggle == false)
					{
						ZoneToggle = true;
					}
				}
				else if (FindModifyer.IsDown(config.RadiusGrid.Value) && Player.m_localPlayer)
				{
					if (RadiusToggle == true)
					{
						RadiusToggle = false;
					}
					else if (RadiusToggle == false)
					{
						RadiusToggle = true;
					}
				}

				if (Player.m_localPlayer.GetRightItem() != null)
				{
					if (config.IfGrapplingPressed() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin")
					{
						TrackGrapple.block = null;
						WallCheck.IsGrappling = false;
						TrackGrapple.HitVectorGrapple = Vector3.zero;
						//Walk.Resetcounter = 0;
						GrappleCounter = 0;
						//WallCheck.IsSwinging = false;
						WallCheck.JointCounter = 0;
						Destroy(WallCheck.jointSwing);
						HasSwung = false;
						TrackGrapple2.CounterSphere = 0;
						StopWalkWhileSwing.TransitionCounter = 0;
						haspressed2 = false;
						WallCheck.SoundCounter = 0;


					}
					else if (config.IfSwingingPressedDown() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer && TrackGrapple.block != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin")
					{

						//Player.m_localPlayer.gameObject.GetComponent<Character>().SetMoveDir(Vector3.zero);
						//Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

						if (!__instance.IsOnGround())
						{
							haspressed = true;
							HasSwung = false;
						}
						if (TrackGrapple.block != null)
						{
							WallCheck.IsGrappling = false;
							WallCheck.IsSwinging = true;
						}


					}
					else if (config.IfSwingingPressedDown() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer && WallCheck.IsGrappling == false && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin")
					{

						//Player.m_localPlayer.gameObject.GetComponent<Character>().SetMoveDir(Vector3.zero);
						//Player.m_localPlayer.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
						haspressed2 = true;
						WallCheck.IsSwinging = true;

						//WallCheck.IsGrappling = false;
					}

					else if (config.IfSwingingPressedUp() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer && WallCheck.IsSwinging == true && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin")
					{

						TrackGrapple.block = null;

						WallCheck.IsGrappling = false;
						TrackGrapple.HitVectorGrapple = Vector3.zero;
						//Walk.Resetcounter = 0;
						//GrappleCounter = 0;
						WallCheck.IsSwinging = false;
						WallCheck.JointCounter = 0;
						Destroy(WallCheck.jointSwing);
						haspressed = false;
						HasSwung = true;
						StopWalkWhileSwing.TransitionCounter = 0;

					}

					if (haspressed == true && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer && TrackGrapple.block != null && Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin")
					{
						WallCheck.IsGrappling = false;
						WallCheck.IsSwinging = true;
					}
				}

			}


			//XXX
			if (__instance == Player.m_localPlayer && Player.m_localPlayer.GetRightItem() != null)

			{



				if (Player.m_localPlayer.GetRightItem().m_shared.m_name == "$item_spear_chitin")
				{

					int mask3 = LayerMask.GetMask("piece");
					//currentHitObjects.Clear();
					sphereCastHitUpdate = Physics.SphereCastAll(GameCamera.instance.transform.position, 1f, GameCamera.instance.transform.forward, 80f, mask3);
					Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out rayCastHitUpdate, 80f, mask3);
					Physics.SphereCast(GameCamera.instance.transform.position, 1f, GameCamera.instance.transform.forward, out SpCastHitUpdate, 80f, mask3);
					float[] distances = new float[sphereCastHitUpdate.Length];
					for (int i = 0; i < sphereCastHitUpdate.Length; i++)
					{
						distances[i] = sphereCastHitUpdate[i].distance;
					}
					Array.Sort(distances, sphereCastHitUpdate);



					foreach (RaycastHit hit in sphereCastHitUpdate)
					{



						if (hit.collider != null && hit.collider.transform.gameObject != DotTracker.transform.gameObject && hit.collider.transform.gameObject.GetComponent<Rigidbody>())
						{
							//currentHitObjects.Add(hit.transform.gameObject);
							if (hit.point != Vector3.zero && rayCastHitUpdate.point == Vector3.zero && Vector3.Distance(Handpoint.rightHand, hit.transform.position) <= 50f && Vector3.Distance(Handpoint.rightHand, hit.transform.position) > 3f && hit.transform.gameObject.GetComponent<Rigidbody>())
							{


								DotTracker.SetActive(true);
								DotTracker.transform.position = hit.point - GameCamera.instance.transform.forward;
								DotCounter = 1;


							}
							else if (hit.point != Vector3.zero && rayCastHitUpdate.point != Vector3.zero && Vector3.Distance(Handpoint.rightHand, hit.transform.position) <= 50f && Vector3.Distance(Handpoint.rightHand, hit.transform.position) > 3f && rayCastHitUpdate.collider.transform.gameObject.GetComponent<Rigidbody>())
							{


								DotTracker.SetActive(true);
								DotTracker.transform.position = rayCastHitUpdate.point - GameCamera.instance.transform.forward;
								//DotTracker.transform.Rotate(15, 0, 0);
								DotCounter = 1;

							}
							else if (hit.point != Vector3.zero && rayCastHitUpdate.point != Vector3.zero && Vector3.Distance(Handpoint.rightHand, hit.transform.position) <= 50f && Vector3.Distance(Handpoint.rightHand, hit.transform.position) > 3f && !rayCastHitUpdate.transform.gameObject.GetComponent<Rigidbody>())
							{




								DotTracker.SetActive(true);
								RaycastHit hitt = hit;
								hitt.point = hit.collider.ClosestPointOnBounds(GameCamera.instance.transform.position + GameCamera.instance.transform.forward * hit.distance);
								DotTracker.transform.position = hitt.point - GameCamera.instance.transform.forward;
								//DotTracker.transform.Rotate(15, 0, 0);
								DotCounter = 1;

							}
						}
						else
						{
							continue;
						}

					}

					//Debug.Log(SpCastHitUpdate.collider.transform.gameObject.GetComponentInChildren<Piece>().m_name);


					if (DotTracker.activeSelf)
					{


						if (sphereCastHitUpdate.Length == 0 || (rayCastHitUpdate.collider != null && !rayCastHitUpdate.transform.gameObject.GetComponent<Rigidbody>()) || (rayCastHitUpdate.collider != null && !rayCastHitUpdate.collider.transform.gameObject.GetComponentInChildren<Rigidbody>()) || Vector3.Distance(Handpoint.rightHand, SpCastHitUpdate.collider.transform.position) < 3f || Vector3.Distance(Handpoint.rightHand, SpCastHitUpdate.collider.transform.position) > 50f)
						{
							Rigidbody TempRB = DotTracker.GetComponent<Rigidbody>();
							LineRenderer TempLN = DotTracker.GetComponent<LineRenderer>();
							Destroy(TempRB);
							Destroy(TempLN);
							DotTracker.SetActive(false);
							DotCounter = 0;
							CanHook = false;
							//Debug.Log("1");
						}

						if (sphereCastHitUpdate.Length >= 0 && SpCastHitUpdate.collider != null && !SpCastHitUpdate.collider.transform.gameObject.GetComponentInChildren<Rigidbody>() && rayCastHitUpdate.collider == null)
						{
							counterSP = 0;
							foreach (RaycastHit hit in sphereCastHitUpdate)
							{
								if (hit.collider != null && hit.collider.transform.gameObject != DotTracker.transform.gameObject && hit.collider.transform.gameObject.GetComponent<Rigidbody>())
								{
									counterSP += 1;
								}

							}

							if (counterSP <= 0)
							{

								Rigidbody TempRB2 = DotTracker.GetComponent<Rigidbody>();
								LineRenderer TempLN2 = DotTracker.GetComponent<LineRenderer>();
								Destroy(TempRB2);
								Destroy(TempLN2);
								DotTracker.SetActive(false);
								DotCounter = 0;
								Debug.Log("2");
								CanHook = false;
							}

						}


						if (!DotTracker.GetComponent<LineRenderer>() && !DotTracker.GetComponent<Rigidbody>() && DotCounter == 1)
						{
							DotTracker.AddComponent<Rigidbody>();
							DotTracker.GetComponent<Rigidbody>().isKinematic = false;
							DotTracker.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
							DotTracker.DrawCircle(0.3f, 0.1f, new Color(1, 0, 0, 1));
							currentAngle = DotTracker.transform.eulerAngles;
							DotCounter += 1;
							CanHook = false;
							Debug.Log("3");
						}

						if (DotTracker.activeSelf && DotTracker.gameObject.GetComponent<LineRenderer>())
						{

							float HookGradient = Vector3.Distance(Handpoint.rightHand, DotTracker.transform.position);
							HookGradient /= 50;
							HookGradient = Mathf.Clamp(HookGradient, 0, 1);
							DotTracker.gameObject.GetComponent<LineRenderer>().startColor = Color.Lerp(Color.cyan, Color.magenta, HookGradient);
							DotTracker.gameObject.GetComponent<LineRenderer>().endColor = Color.Lerp(Color.cyan, Color.magenta, HookGradient);
							CanHook = true;
							Debug.Log("4");
						}


					}


					if (WallCheck.IsSwinging == true && TrackGrapple.block != null && TrackGrapple.HitVectorGrapple != null && WallCheck.jointSwing != null)
					{
						if (Input.GetKey(KeyCode.Space))
						{

							float distanceFromPoint = Vector3.Distance(Handpoint.rightHand, TrackGrapple.HitVectorGrapple);
							WallCheck.jointSwing.GetComponent<SpringJoint>().maxDistance = distanceFromPoint * 0.8f;
							WallCheck.jointSwing.GetComponent<SpringJoint>().minDistance = distanceFromPoint * 0.25f;
						}
						if (Input.GetKey(KeyCode.S) && WallCheck.IsSwinging == true)
						{

							float extendeddistanceFromPoint = Vector3.Distance(Handpoint.rightHand, TrackGrapple.HitVectorGrapple) + 20;
							WallCheck.jointSwing.GetComponent<SpringJoint>().maxDistance = extendeddistanceFromPoint * 0.8f;
							WallCheck.jointSwing.GetComponent<SpringJoint>().minDistance = extendeddistanceFromPoint * 0.25f;

						}
					}

				}

			}

			if (__instance == Player.m_localPlayer)
			{
				if (WallCheck.TouchingSurf == true)
				{
					GraceSurfSlopeTimer += Time.deltaTime;
				}
				if (Player.m_localPlayer.GetRightItem() == null && DotTracker.activeSelf)
				{

					Rigidbody TempRB = DotTracker.GetComponent<Rigidbody>();
					LineRenderer TempLN = DotTracker.GetComponent<LineRenderer>();
					Destroy(TempRB);
					Destroy(TempLN);
					DotTracker.SetActive(false);
					DotCounter = 0;
				}
				if (Player.m_localPlayer.GetRightItem() == null && DotTrackerParentingDistance.activeSelf && DotTrackerParentingDistance2.activeSelf)
				{

					LineRenderer TempLN = DotTrackerParentingDistance.GetComponent<LineRenderer>();
					LineRenderer TempLN2 = DotTrackerParentingDistance2.GetComponent<LineRenderer>();
					Destroy(TempLN);
					Destroy(TempLN2);
					DotTrackerParentingDistance.SetActive(false);
					DotTrackerParentingDistance2.SetActive(false);
					WallCheck.DotTrackerParentingDistanceCount = 0;
				}
			}
			//Debug.Log("IsSwing " + WallCheck.IsSwinging + " IsGrap " + WallCheck.IsGrappling + "haspressed2 " + haspressed + " hooktimer " + HookTimer);



			if (HasWallJumped2 == true)
			{
				if (WallCheck.TouchingIce == true)
				{
					timer2 -= 1.0f * Time.deltaTime;
					if (timer2 <= 0f)
					{
						HasWallJumped2 = false;
						timer2 = wallJumpCooldown2;
					}
				}
				if (WallCheck.TouchingIce == false)
				{
					HasWallJumped2 = false;
					timer2 = wallJumpCooldown2;
				}
			}

			if (JumpWorld.Biomes == 32)
			{
				stopwatchActive = true;
			}

			if (JumpWorld.Biomes == 512)
			{
				stopwatchActive = false;
			}

			if (InvalidRun == false && stopwatchActive == true && (Player.m_localPlayer.GetRightItem() == null || (Player.m_localPlayer.GetRightItem() != null && Player.m_localPlayer.GetRightItem().m_shared.m_name != "$item_hammer")))
			{
				if (currentTime > 0)
				{
					currentTime = 0.01f;
				}
				currentTime = currentTime + Time.deltaTime;
			}
			else
			{
				currentTime = 1.337f;
				InvalidRun = true;
			}
			TimeSpan t = TimeSpan.FromSeconds(currentTime);
			timing = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
			tpcount = " TP:" + tpnumber.ToString();
			cpcount = " CP:" + cpnumber.ToString();



			if (Player.m_localPlayer != null)
			{
				Biomes = (int)Player.m_localPlayer.GetCurrentBiome();
				text3 = TerrainModifier.FindClosestModifierPieceInRange(Player.m_localPlayer.transform.position, 6f)?.m_name?.Replace("$piece_", string.Empty);

				Collider lastGroundCollider = __instance.GetLastGroundCollider();

				WearNTear wearNTear = null;
				TrackPiece = null;




				if (lastGroundCollider != null)
				{
					TrackTerrain = lastGroundCollider.GetComponentInParent<Heightmap>();
					wearNTear = lastGroundCollider.GetComponentInParent<WearNTear>();
					TrackPiece = lastGroundCollider.GetComponentInParent<Piece>();

					TrackPiece2 = null;

					// Check if the collider has a parent
					if (lastGroundCollider.transform.parent != null)
					{
						// If parent exists, try to get the Piece component from the parent
						TrackPiece2 = lastGroundCollider.transform.parent.GetComponent<Piece>();
					}

					// If the parent doesn't exist or doesn't have a Piece component, try to get it from the collider itself
					if (TrackPiece2 == null)
					{
						TrackPiece2 = lastGroundCollider.GetComponentInParent<Piece>();
					}

					if (TrackPiece2 != null)
					{
						piecenameMovingPiece = TrackPiece2.m_name.ToString();
					}
					else
					{
						piecenameMovingPiece = string.Empty;

					}


					if (TrackPiece != null)
					{
						piecename = TrackPiece.m_name.ToString();
						piecename3 = TrackPiece.m_name.ToString();

					}
					else
					{
						piecename = string.Empty;

					}
				}


				if (wearNTear != null && Enum.TryParse<Ground>(wearNTear.m_materialType.ToString(), ignoreCase: true, out var result))
				{
					CurrentGround = result;
				}
				else if (Enum.TryParse<Ground>(text3 ?? string.Empty, ignoreCase: true, out result))
				{
					CurrentGround = result;
				}
				else
				{
					CurrentGround = Ground.None;
				}
			}

			if (__instance.IsOnGround())
			{
				GroundType = (int)CurrentGround;
			}
			else
			{
				GroundType = 99;
				piecename = string.Empty;
				piecenameMovingPiece = string.Empty;
			}


			if (Player.m_localPlayer != null && __instance == Player.m_localPlayer)
			{
				if (piecename == "$piece_stonefloor2x2" || (WallCheck.NearestOjbect != null && WallCheck.NearestOjbect.GetComponentInParent<Piece>().m_name.ToString() == "$piece_stonefloor2x2" && WallCheck.within0_5fRange == true))
				{
					if (WallCheck.NearestOjbect.GetComponentInParent<ZNetView>().GetZDO().GetFloat("BhopFloatValueKey") > 0)
					{
						cooldown += Time.deltaTime;
						if (cooldown > (1f - WallCheck.BhopTimeFactor))
						{
							__instance.TeleportTo(UpdateGroundContact_Patch.groundpoint, UpdateGroundContact_Patch.groundpoint2, distantTeleport: true);
							BhopeTeleTimer = 0.1f;
							hasBhopTele = true;
						}
					}
				}
			}
			if (hasBhopTele == true)
			{
				BhopeTeleTimer -= Time.deltaTime;

				if (BhopeTeleTimer <= 0.0f)
				{
					Player.m_localPlayer.gameObject.GetComponent<Character>().m_lookYaw = UpdateGroundContact_Patch.CameraRotFloor;
					Player.m_localPlayer.m_lookPitch = UpdateGroundContact_Patch.CameraPitchFloor;
					GameCamera.instance.transform.position = UpdateGroundContact_Patch.CameraPosFloor;
					GameCamera.instance.transform.rotation = UpdateGroundContact_Patch.CameraRot2Floor;
					hasBhopTele = false;
					
				}
			}

			if (piecename3 == "$piece_blackmarble_arch" && TrackTerrain == null)
			{
				SlowFallCooldown += Time.deltaTime;
				if (SlowFallCooldown > 0.2f)
				{

					GameObject prefab = ZNetScene.instance.GetPrefab("sfx_WishbonePing_far");
					//GameObject prefab2 = ZNetScene.instance.GetPrefab("sfx_pick_wisp");
					GameObject gameObject3 = UnityEngine.Object.Instantiate(prefab, Player.m_localPlayer.gameObject.GetComponent<Character>().transform.position, Quaternion.identity);
					//GameObject gameObject4 = UnityEngine.Object.Instantiate(prefab2, Player.m_localPlayer.gameObject.GetComponent<Character>().transform.position, Quaternion.identity);
					//Destroy(gameObject);
					SlowFallCooldown = 0;
				}
			}


			if (HookTimerOn == true)
			{
				HookTimer += Time.deltaTime;


			}

			

			if (hasTeleported == true)
			{
				TeleTimer -= Time.deltaTime;

				if (TeleTimer <= 0.0f)
				{
					Player.m_localPlayer.gameObject.GetComponent<Character>().m_lookYaw = CameraRot;
					Player.m_localPlayer.m_lookPitch = CameraPitch;
					GameCamera.instance.transform.position = CameraPos;
					GameCamera.instance.transform.rotation = CameraRot2;
					hasTeleported = false;
					TeleTimer = 0.1f;

				}
			}

			if (hasTeleported2 == true)
			{
				TeleTimer -= Time.deltaTime;

				if (TeleTimer <= 0.0f)
				{
					Player.m_localPlayer.gameObject.GetComponent<Character>().m_lookYaw = CameraRotPrev;
					Player.m_localPlayer.m_lookPitch = CameraPitchPrev;
					GameCamera.instance.transform.position = CameraPosPrev;
					GameCamera.instance.transform.rotation = CameraRot2Prev;
					hasTeleported2 = false;
					TeleTimer = 0.2f;

				}
			}

			if (isJumping)
			{
				// Reduce the timer in every frame
				TrampolineJumpTimer -= Time.deltaTime;



				if (TrampolineJumpTimer <= 0.0f)
				{
					// Reset the boolean after the timer expires
					isJumping = false;
					BounceII.BounceLimiter = 0f;
					TrampolineJumpTimer = 1.0f; // Reset the timer

				}

			}

			if (BounceII.hasBounced == true)
			{

				if (BounceTimer == 1f)
				{
					GameObject prefab = ZNetScene.instance.GetPrefab("sfx_bow_draw");
					GameObject gameObject3 = UnityEngine.Object.Instantiate(prefab, BounceII.trampolineHitPoint, Quaternion.identity);
				}

				BounceTimer -= Time.deltaTime;




				if (BounceTimer <= 0.0f)
				{
					// Reset the boolean after the timer expires
					BounceII.hasBounced = false;
					BounceTimer = 1f; // Reset the timer
				}


			}
		}
	}




	/// <summary>
	/// /////////////////////////////////////Load Hud
	/// </summary>
	[HarmonyPatch(typeof(Hud), "Awake")]
	public static class Hud_Awake_Patch
	{
		public static void Postfix(Hud __instance)
		{
			CreatePanel(__instance);
			DotTracker = new GameObject("DotTracker");
			DotTrackerParentingDistance = new GameObject("DotTrackerParentingDistance");
			DotTrackerParentingDistance2 = new GameObject("DotTrackerParentingDistance2");
			DotTrackerParentingDistanceBuilt = new GameObject("DotTrackerParentingDistanceBuilt");
			ZoneCorner = new GameObject("ZoneCorner");
			ZoneCorner2 = new GameObject("ZoneCorner2");
			ZoneCorner3 = new GameObject("ZoneCorner3");
			ZoneCorner4 = new GameObject("ZoneCorner4");
			ZoneCenter = new GameObject("ZoneCenter");
			GhostObject = new GameObject("GhostObject");
			currentGhostObject = new GameObject("currentGhostObject");
			
		}
	}

	[HarmonyPatch(typeof(Character), "Awake")]
	public static class Character_Awake_Patch
	{
		//XXX
		public static void Postfix(ref Rigidbody ___m_body)
		{
			___m_body.maxDepenetrationVelocity = 10f;
		}
	}

	private void Awake()
	{
		//WallCheck.ln2222.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
		timer = wallJumpCooldown;
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		config.ConfigFile = base.Config;
		config.SetupConfig();
		currentTime = 0;
		SphereRadius = 5f;
	}
}
/// <summary>
/// /////////////////////////////////////////Patch in Config File
/// </summary>
/// 



public static class FindModifyer
{

	public static bool IsDown(KeyboardShortcut value)

	{
		if (Input.GetKeyDown(value.MainKey))
		{
			if (value.Modifiers != null)
			{
				foreach (var mod in value.Modifiers)
				{
					if (!Input.GetKey(mod))
					{
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}
}
public static class DestroyChild
{
	public static float counter;
	public static void UnparentChildren(this GameObject Parent)

	{
		Debug.Log(Parent.transform.childCount);
		int i = 0;
		counter = 0;

		//Array to hold all child obj
		GameObject[] allChildren = new GameObject[Parent.transform.childCount];

		//Find all child obj and store to that array
		foreach (Transform child in Parent.transform)
		{

			if (child.gameObject.GetComponentInChildren<Piece>() && child != null)
			{

				allChildren[i] = child.gameObject;
				i += 1;

			}
			else
			{
				allChildren[i] = null;
				i += 1;
			}


		}

		//Now Unparent them
		foreach (GameObject child in allChildren)
		{

			if (child != null)
			{
				child.gameObject.transform.parent = null;
			}



		}
	}
}

//if (foundChildren.Count > 0)
//{
//	for (int i = 0; i < foundChildren.Count; i++)
//	{
//		var child = foundChildren[i];
//		FindAttached(ref self, ref masterList, ref child);
//	}
//}



public static class FindChildBound_class
{
	public static Collider[] colpieceFromMovePlat2 = new Collider[2000];
	public static Transform piece22;
	public static void FindChildBound(this GameObject parent, GameObject ToBeParent, List<Transform> list)

	{

		int mask3 = LayerMask.GetMask("piece");
		piece22 = null;
		colpieceFromMovePlat2 = new Collider[2000];
		list.Clear();


		Collider collider = parent.gameObject.GetComponentInChildren<Collider>();
		if (collider != null)
		{
			//float sphereRadius = Mathf.Max(collider.bounds.extents.x * 2f, collider.bounds.extents.y * 2f, collider.bounds.extents.z * 2f) * 1.2f;
			Vector3 sphereCenter = collider.bounds.center;
			//Debug.Log("ColLength: " + sphereRadius);
			colpieceFromMovePlat2 = Physics.OverlapSphere(sphereCenter, parent.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("AttachRadius"), mask3);

		}


		if (colpieceFromMovePlat2 != null)
		{
			List<GameObject> foundChildren = new List<GameObject>();
			foreach (Collider colliderfromplayer2 in colpieceFromMovePlat2)
			{

				//Debug.Log("1 " + colliderfromplayer2.gameObject.GetComponentInParent<Piece>().m_name.ToString());
				if (colliderfromplayer2.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x2x2" || colliderfromplayer2.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble2x1x1" || colliderfromplayer2.gameObject.GetComponentInParent<Piece>().m_name.ToString() == "$piece_blackmarble_column_1" || parent == colliderfromplayer2.gameObject)
				{

					continue;
				}

				else
				{


					piece22 = colliderfromplayer2.GetComponentInParent<Piece>().transform;

					if (piece22 != null && piece22.parent == null && piece22.gameObject.GetComponentInParent<ZNetView>().GetZDO().GetBool("AttachedTrue") == true)
					{



						piece22.transform.SetParent(ToBeParent.gameObject.GetComponent<Piece>().transform);
						//piece22.transform.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader = ZNetScene.instance.GetPrefab("Amber")?.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader;
						list.Add(piece22);
						foundChildren.Add(piece22.gameObject);
					}
				}
			}

			if (foundChildren.Count > 0)
			{
				for (int i = 0; i < foundChildren.Count; i++)
				{
					var child = foundChildren[i];


					FindChildBound(child, ToBeParent, list);
				}
			}
			if (foundChildren.Count <= 0)
			{
				return;

			}
		}
	}
}





public static class DrawCircle_Class
{
	public static void DrawCircle(this GameObject container, float radius, float lineWidth, Color lineColor)

	{
		var segments = 360;
		var line = container.AddComponent<LineRenderer>();
		line.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

		line.useWorldSpace = false;
		line.startWidth = lineWidth;
		line.endWidth = lineWidth;
		line.positionCount = segments + 1;
		line.startColor = lineColor;
		line.endColor = lineColor;

		var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
		var points = new Vector3[pointCount];

		for (int i = 0; i < pointCount; i++)
		{
			var rad = Mathf.Deg2Rad * (i * 360f / segments);
			points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
		}

		line.SetPositions(points);
	}
}

public class JumpworldConfig
{
	public ConfigEntry<string> checkpoint;
	public ConfigEntry<string> teleporting;
	public ConfigEntry<string> teleporting2;
	public ConfigEntry<string> killme;
	public ConfigEntry<string> PlatformIncrease;
	public ConfigEntry<string> PlatformDecrease;
	public ConfigEntry<string> UprightPrism;
	public ConfigEntry<KeyboardShortcut> PlatformSpeedIncrease;
	public ConfigEntry<KeyboardShortcut> PlatformSpeedDecrease;
	public ConfigEntry<KeyboardShortcut> RadiusIncrease;
	public ConfigEntry<KeyboardShortcut> RadiusDecrease;
	public ConfigEntry<string> Grappling;
	public ConfigEntry<string> Swinging;
	public ConfigEntry<string> SkinToggle;
	public ConfigEntry<KeyboardShortcut> ZoneGrid;
	public ConfigEntry<KeyboardShortcut> RadiusGrid;
	


	public ConfigEntry<KeyboardShortcut> commandModifier1;
	public ConfigEntry<KeyboardShortcut> commandModifier2;
	public ConfigEntry<KeyboardShortcut> commandModifier3;
	public ConfigEntry<KeyboardShortcut> commandModifier4;
	public ConfigEntry<KeyboardShortcut> commandModifier5;
	public ConfigEntry<KeyboardShortcut> commandModifier6;

	public ConfigFile ConfigFile { get; set; }

	public void SetupConfig()
	{
		UprightPrism = ConfigFile.Bind("Hotkeys", "UprightPrism", "F", "Flip");
		checkpoint = ConfigFile.Bind("Hotkeys", "Checkpoint", "Insert", "blaaa");
		teleporting = ConfigFile.Bind("Hotkeys", "Teleporting", "Delete", "nraaa");
		teleporting2 = ConfigFile.Bind("Hotkeys", "Teleporting2", "END", "graaaa");
		killme = ConfigFile.Bind("Hotkeys", "Killme", "HOME", "kraaaa");
		PlatformIncrease = ConfigFile.Bind("Hotkeys", "Platform Increase", "F9", "ChildUp");
		PlatformDecrease = ConfigFile.Bind("Hotkeys", "Platform Decrease", "F10", "ChildDown");
		PlatformSpeedIncrease = ConfigFile.Bind("Hotkeys", "Platform Speed Increase", new KeyboardShortcut(KeyCode.F7, KeyCode.LeftAlt), "SpeedInc");
		PlatformSpeedDecrease = ConfigFile.Bind("Hotkeys", "Platform Speed Decrease", new KeyboardShortcut(KeyCode.F8, KeyCode.LeftAlt), "SpeedDec");

		RadiusIncrease = ConfigFile.Bind("Hotkeys", "Platform Radius Increase", new KeyboardShortcut(KeyCode.F7, KeyCode.LeftAlt, KeyCode.LeftControl), "RadiusInc");
		RadiusDecrease = ConfigFile.Bind("Hotkeys", "Platform Radius Decrease", new KeyboardShortcut(KeyCode.F8, KeyCode.LeftAlt, KeyCode.LeftControl), "RadiusDec");

		SkinToggle = ConfigFile.Bind("Hotkeys", "SkinToggle", "F12", "SkinToggle");
		ZoneGrid = ConfigFile.Bind("Hotkeys", "ZoneGrid", new KeyboardShortcut(KeyCode.F6), "ZoneGrid");
		RadiusGrid = ConfigFile.Bind("Hotkeys", "RadiusGrid", new KeyboardShortcut(KeyCode.F6, KeyCode.LeftAlt), "RadiusGrid");

		commandModifier1 = ConfigFile.Bind("Hotkeys", "Modifier 1", new KeyboardShortcut(KeyCode.F7), "SizeUp");
		commandModifier2 = ConfigFile.Bind("Hotkeys", "Modifier 2", new KeyboardShortcut(KeyCode.F8), "SizeDown");
		commandModifier3 = ConfigFile.Bind("Hotkeys", "Modifier 3", new KeyboardShortcut(KeyCode.F9, KeyCode.LeftAlt), "MaterialUp");
		commandModifier4 = ConfigFile.Bind("Hotkeys", "Modifier 4", new KeyboardShortcut(KeyCode.F10, KeyCode.LeftAlt), "MaterialDown");
		commandModifier5 = ConfigFile.Bind("Hotkeys", "Modifier 5", new KeyboardShortcut(KeyCode.F7, KeyCode.LeftControl), "Platform Increase");
		commandModifier6 = ConfigFile.Bind("Hotkeys", "Modifier 6", new KeyboardShortcut(KeyCode.F8, KeyCode.LeftControl), "Platform Decrease");


		Grappling = ConfigFile.Bind("Hotkeys", "Grappling", "0", "GrapplingActivation");
		Swinging = ConfigFile.Bind("Hotkeys", "Grappling2", "1", "GrapplingActivation2");

		UprightPrism.Value = UprightPrism.Value.ToLower();
		checkpoint.Value = checkpoint.Value.ToLower();
		teleporting.Value = teleporting.Value.ToLower();
		teleporting2.Value = teleporting2.Value.ToLower();
		killme.Value = killme.Value.ToLower();
		PlatformIncrease.Value = PlatformIncrease.Value.ToLower();
		PlatformDecrease.Value = PlatformDecrease.Value.ToLower();


		SkinToggle.Value = SkinToggle.Value.ToLower();
		


	}
	public bool IfUprightPrismPressed()
	{
		return Input.GetKeyDown(UprightPrism.Value);
	}
	public bool IfCheckpointkeyPressed()
	{
		return Input.GetKeyDown(checkpoint.Value);
	}

	public bool IfSkinTogglePressed()
	{
		return Input.GetKeyDown(SkinToggle.Value);
	}

	
	public bool IfTeleportingkeyPressed()
	{
		return Input.GetKeyDown(teleporting.Value);
	}

	public bool IfTeleportingkey2Pressed()
	{
		return Input.GetKeyDown(teleporting2.Value);
	}

	public bool IfKillMePressed()
	{
		return Input.GetKeyDown(killme.Value);
	}

	public bool IfPlatformIncreasePressed()
	{
		return Input.GetKeyDown(PlatformIncrease.Value);
	}

	public bool IfPlatformdDecreasePressed()
	{
		return Input.GetKeyDown(PlatformDecrease.Value);
	}



	public bool IfGrapplingPressed()
	{
		int number;
		bool isParsable = Int32.TryParse(Grappling.Value, out number);

		if (isParsable)
		{
			return Input.GetMouseButtonDown(number);
		}
		return Input.GetMouseButtonDown(0);
	}

	public bool IfSwingingPressedDown()
	{
		int number;
		bool isParsable = Int32.TryParse(Swinging.Value, out number);

		if (isParsable)
		{
			return Input.GetMouseButtonDown(number);

		}
		return Input.GetMouseButtonDown(1);
	}
	public bool IfSwingingPressedUp()
	{
		int number;
		bool isParsable = Int32.TryParse(Swinging.Value, out number);

		if (isParsable)
		{
			return Input.GetMouseButtonUp(number);

		}
		return Input.GetMouseButtonUp(1);
	}
}

public enum Ground
{
	None,
	Wood,
	Stone,
	Iron,
	HardWood,
	PavedRoad
}




public static class AreLinesIntersectingClass
{
	public static bool AreLinesIntersecting(Vector3 P1_0, Vector3 V1, Vector3 P2_0, Vector3 V2, out Vector3 intersection)
	{
		Vector3 cross = Vector3.Cross(V1, V2);
		if (cross.magnitude < float.Epsilon)
		{
			// The lines are parallel or collinear, no intersection
			intersection = Vector3.zero;
			return false;
		}

		Vector3 delta = P2_0 - P1_0;
		float t1 = Vector3.Dot(Vector3.Cross(delta, V2), cross) / cross.sqrMagnitude;
		float t2 = Vector3.Dot(Vector3.Cross(delta, V1), cross) / cross.sqrMagnitude;

		if (t1 >= 0f && t1 <= 1f && t2 >= 0f && t2 <= 1f)
		{
			// Lines intersect
			intersection = P1_0 + t1 * V1;
			return true;
		}

		// Lines don't intersect
		intersection = Vector3.zero;
		return false;
	}
}

public static class ValueChecker
{
	private static float previousValue = float.MinValue; // Initialize to a minimum value.

	public static bool IsDecreasing(float currentValue)
	{
		// Check if the current value is less than the previous value.
		bool isDecreasing = currentValue < previousValue;

		// Update the previous value.
		previousValue = currentValue;

		return isDecreasing;
	}
}

public static class ValueChangeChecker
{
	private static float previousVelocityY = float.MinValue; // Initialize to a minimum value.
	private static bool isFirstCheck = true; // Flag to handle the first check.

	public static bool IsYVelocityIncreasingBy(float currentVelocityY, float threshold)
	{
		// Handle the first check separately
		if (isFirstCheck)
		{
			isFirstCheck = false;
			previousVelocityY = currentVelocityY;
			return false; // Skip the first check
		}

		// Check if the current Y velocity is greater than the previous Y velocity.
		bool isIncreasing = currentVelocityY > previousVelocityY;

		// Calculate the difference between the current and previous Y velocities.
		float difference = Mathf.Abs(currentVelocityY - previousVelocityY);

		// Check if the difference is greater than or equal to the specified threshold.
		bool isIncreasingByThreshold = difference >= threshold;

		// Update the previous Y velocity.
		previousVelocityY = currentVelocityY;

		// Return true if both conditions are met.
		return isIncreasing && isIncreasingByThreshold;
	}
}

public static class SigmoidCalculator
{
	public static float Sigmoid(float x)
	{
		// Adjust the curve parameters as needed for your desired progression
		float alpha = 5f;
		return 1.0f / (1.0f + Mathf.Exp(-alpha * (x - 0.5f)));
	}
}


public class ChildChecker : MonoBehaviour
{
	public static bool HasChildWithName(Transform parentTransform, string childName)
	{
		// Check if the parent itself has the desired name
		if (parentTransform.gameObject.name == childName)
		{
			return true;
		}

		// Check each child for the desired name
		foreach (Transform child in parentTransform)
		{
			if (child.gameObject.name == childName)
			{
				return true;
			}

			// Recursively check the child's children
			if (HasChildWithName(child, childName))
			{
				return true;
			}
		}

		return false;
	}
}



public class LineRendererUtility : MonoBehaviour
{
	private static List<GameObject> horizontalSegments = new List<GameObject>();

	public static GameObject[] CreateHorizontalSegments(LineRenderer verticalLine, int numSegments)
	{
		// Calculate the total length of the vertical LineRenderer
		float verticalLength = verticalLine.GetPosition(verticalLine.positionCount - 1).y - verticalLine.GetPosition(0).y;

		// Calculate the spacing between horizontal segments
		float segmentSpacing = verticalLength / (numSegments - 1);

		for (int i = 0; i < numSegments; i++)
		{
			// Calculate the y position of the horizontal segment
			float yPos = verticalLine.GetPosition(0).y + (i * segmentSpacing);

			// Create a new horizontal LineRenderer segment
			GameObject horizontalSegment = new GameObject("HorizontalSegment_" + i);
			LineRenderer horizontalLine = horizontalSegment.AddComponent<LineRenderer>();
			horizontalLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
			horizontalLine.startWidth = 0.1f; // Use the same width
			horizontalLine.endWidth = 0.1f;
			horizontalLine.startColor = JumpWorld.WallCheck.startColor2; // Use the same width
			horizontalLine.endColor = JumpWorld.WallCheck.startColor2;
			// Set the positions of the horizontal segment
			horizontalLine.positionCount = 5;
			horizontalLine.SetPosition(0, new Vector3(verticalLine.GetPosition(0).x, yPos, verticalLine.GetPosition(0).z));
			horizontalLine.SetPosition(1, new Vector3(verticalLine.GetPosition(0).x + 64f, yPos, verticalLine.GetPosition(0).z));
			horizontalLine.SetPosition(2, new Vector3(verticalLine.GetPosition(0).x + 64f, yPos, verticalLine.GetPosition(0).z + 64f));
			horizontalLine.SetPosition(3, new Vector3(verticalLine.GetPosition(0).x, yPos, verticalLine.GetPosition(0).z + 64f));
			horizontalLine.SetPosition(4, new Vector3(verticalLine.GetPosition(0).x, yPos, verticalLine.GetPosition(0).z));
			// Store the created horizontal LineRenderer segment in the list
			horizontalSegments.Add(horizontalSegment);
		}

		return horizontalSegments.ToArray();
	}

	public static void DestroyHorizontalSegments()
	{
		foreach (GameObject horizontalSegment in horizontalSegments)
		{
			if (horizontalSegment != null)
			{
				Destroy(horizontalSegment);
			}
		}

		// Clear the list
		horizontalSegments.Clear();
	}
}