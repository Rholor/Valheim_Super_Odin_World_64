// JumpWorld
using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;




[BepInPlugin("Rholor.JumpWorld", "Jump World", "1.0.0")]
public class JumpWorld : BaseUnityPlugin
{
	public static bool HasWallJumped = false;
	public static float timer;
	public static float wallJumpCooldown = 0.1f;
	public static Vector3 savedPos;
	public static Vector3 savedPos2;
	public static Vector3 spawnPos;
	public static JumpworldConfig config = new JumpworldConfig();
	public static bool stopwatchActive = false;
	public static float currentTime;
	private static GameObject _panel;
	private static Text _timeText;
	private static Text _TPText;
	private static Text _CPText;
	private static Text _RIPText;
	public static int Biomes;
	public static int GroundType;
	public static bool helmet = false;
	public static int JumpNumber
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
			component.anchoredPosition = new Vector2(-140f, -265f);
			component.sizeDelta = new Vector2(200f, 50f);
			Image component2 = _panel.GetComponent<Image>();
			component2.enabled = true;
			component2.color = new Color(0f, 0f, 0f, 0.3921569f);
			_timeText.enabled = true;
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
		CreateStopwatch();
		CreateTP();
		CreateCP();
		CreateRIP();
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
		public static bool Shoot;
		public static float number1;
		public static float number2;
		public static float number3;
		public static float shootnumber = 0;
		public static Vector3 GrabCurrentVelo;
		public static Vector3 ShotVector;
		public static float bouncenumber = 0;
		public static GameObject Player;
		
		public static Rigidbody rb;
		public static float thrust = 60f;

		public static Collider collider1;

		private static void Prefix(Collision collision, Character __instance, ref Rigidbody ___m_body, ref float ___m_airControl, ref Vector3 ___m_currentVel, ref Vector3 ___m_moveDir, ref float ___m_runSpeed, ref float ___m_slippage, ref float ___m_acceleration, ref Vector3 ___m_lastGroundPoint)
		{
			
			int mask2 = LayerMask.GetMask("Default", "static_solid", "terrain", "vehicle", "character", "piece", "character_net", "viewblock");
			int mask3 = LayerMask.GetMask("piece");
			ContactPoint[] contacts1 = collision.contacts;
			
			for (int i = 0; i < contacts1.Length; i++)
			{
				ContactPoint contactPoint1 = contacts1[i];
				number2 = contactPoint1.point.y;
				number1 = contactPoint1.point.y - __instance.transform.position.y;


				if (PlayerPatcher.piecename == "$piece_table_oak")
				{
					if (Physics.Raycast(__instance.transform.position, __instance.transform.forward, out var hitInfo3, 1f, mask3))
					{
						shootnumber +=2;
						shootnumber = Mathf.Clamp(shootnumber, 0, 50);
						float factor = shootnumber / 50f;
						
						___m_runSpeed = Mathf.Lerp(7, 50, factor);
						___m_acceleration = Mathf.Lerp(1, 50, factor);
						float runspeeds = ___m_runSpeed;
                        ___m_airControl = 0.01f;
                        Shoot = true;
						ShotVector = ___m_currentVel;
						Debug.Log("speed" + runspeeds);
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
			if (PlayerPatcher.piecename != "$piece_ironfloor" || !(bouncenumber < 1f))
			{
				return;
			}
			if (__instance.IsOnGround() && PlayerPatcher.piecename == "$piece_ironfloor" && bouncenumber == 0 && UpdateGroundContact_Patch.jumpvelocity2 > 10)
			{
				rb = ___m_body;
				rb.AddExplosionForce(UpdateGroundContact_Patch.jumpvelocity2, ___m_lastGroundPoint, 5f, 1f, ForceMode.VelocityChange);
				bouncenumber++;
				//Debug.Log("bouncenr: " + bouncenumber + "velo: " + UpdateGroundContact_Patch.jumpvelocity2);
			}
			if (PlayerPatcher.piecename == "Guard stone")
            {
				
			}
			//else
   //         {
			//	collision.collider.transform.SetParent(null);
			//}
		}
	}



	[HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
	public static class Walk
	{
		private static bool Prefix()
		{
			if (PlayerPatcher.piecename == "$piece_table_oak" && WallGround.ShotVector.magnitude > 20f)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Character), "CanWallRun")]
	public static class BounceWallRun
	{
		private static bool Prefix(Character __instance)
		{
			if (PlayerPatcher.piecename == "$piece_ironwall")
			{
				return false;
			}
			return true;
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



	[HarmonyPatch(typeof(Character), "FixedUpdate")]
	public static class BounceII
	{
		public static Piece HitPiece;
		public static float RayDownLength2;
		
		public static float counter1 = 0;
		public static float counter2 = 0;
		public static Collider[] colpiece = new Collider[200];
		public static Collider[] array = new Collider[200];
		public static string[] destroname = new string[200];
		
		public static Vector3[] spawnPosPiece = new Vector3[200];
		public static Quaternion[] spawnRotPiece = new Quaternion[200];
		public static Vector3 m_EulerAngleVelocity = new Vector3(10,0,0);

		public static bool[] touched = new bool[200];

		//public static Rigidbody attachedRigidbody;

		private static void Prefix(ref Rigidbody ___m_body, ref float ___m_slippage, ref float ___m_jumpForce, Player __instance)
		{
			if (PlayerPatcher.piecename == "$piece_ironwall")
			{
				___m_body.velocity = Bounce.avgbouncevector*30;
			}
			if (PlayerPatcher.piecename == "$piece_crystalwall1x1")
			{
				___m_slippage = -8f;
				___m_jumpForce = 50f;
			}


			if (Player.m_localPlayer != null)
			{
                int mask2 = LayerMask.GetMask("Default", "static_solid", "vehicle", "piece", "viewblock");


                colpiece = Physics.OverlapSphere(__instance.transform.position, 20f, mask2);

                GameObject[] GOpiece = new GameObject[colpiece.Length];
                Rigidbody[] RBpiece = new Rigidbody[colpiece.Length];
                Destructible[] destro = new Destructible[colpiece.Length];
                Rigidbody[] attachedRigidbody = new Rigidbody[colpiece.Length];

                for (int i = 0; i < colpiece.Length; i++)
                {
                    GOpiece[i] = colpiece[i].gameObject;
                    destro[i] = colpiece[i].GetComponentInParent<Destructible>();


                    if (destro[i] != null)
                    {
                        destroname[i] = destro[i].ToString();
                        if (destroname[i] == "HeathRockPillar(Clone) (Destructible)" && !GOpiece[i].GetComponent<Rigidbody>())
                        {
                            RBpiece[i] = GOpiece[i].AddComponent<Rigidbody>();
                            RBpiece[i].isKinematic = true;
                            RBpiece[i].detectCollisions = true;
                        }
                        if (destroname[i] == "HeathRockPillar(Clone) (Destructible)")
                        {
                            attachedRigidbody[i] = colpiece[i].GetComponent<Rigidbody>();
                            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
                            attachedRigidbody[i].MoveRotation(attachedRigidbody[i].rotation * deltaRotation);
                        }
                    }
                }




                for (int i = 0; i < Bounce.rb.Count; i++)
				{

					Bounce.rb[i].MovePosition(Bounce.rb[i].position + Bounce.rb[i].transform.forward * Time.fixedDeltaTime);


					if (Vector3.Distance(__instance.transform.position, Bounce.rb[i].position) > 18f && Vector3.Distance(__instance.transform.position, Bounce.rb[i].position) < 21f)
					{

						Bounce.rb[i].position = Bounce.rbpos[i];
						Destroy(Bounce.rb[i]);
						Bounce.rb.RemoveAt(i);
						Bounce.rbpos.RemoveAt(i);
						i--;

					}
				}
			}	
        }
	}


	[HarmonyPatch(typeof(WearNTear), "UpdateWear")]
	public static class WearOff
	{

		private static void Prefix(ref bool ___m_noSupportWear)
		{

			___m_noSupportWear = false;
		}
	}




	[HarmonyPatch(typeof(Character), "OnCollisionStay")]
	public static class Bounce
	{
		public static Vector3[] bouncevector = new Vector3[10];
		public static Vector3 avgbouncevector = Vector3.zero;

		public static Rigidbody RBpiece2;
		public static List<Rigidbody> rb = new List<Rigidbody>();
		public static List<Vector3> rbpos = new List<Vector3>();
		public static bool switcher;
		public static bool countermove;



		private static void Prefix(Character __instance, ref Rigidbody ___m_body, ref Vector3 ___m_lastGroundPoint, Collision collision, ref float ___m_airControl)
		{
			ContactPoint[] contacts1 = collision.contacts;
			int mask3 = LayerMask.GetMask("piece");
			avgbouncevector = Vector3.zero;
			for (int i = 0; i < contacts1.Length; i++)
			{


				if (PlayerPatcher.piecename == "$piece_ironwall")
				{
					var point = collision.contacts[i].point;
					var dir = -collision.contacts[i].normal;
					point -= dir;

					if (Physics.Raycast(point, dir, out RaycastHit hitInfo, 10f, mask3))
					{

						bouncevector[i] = hitInfo.normal;
						//Vector3 reflect = bouncevector[i];
						//Debug.Log(reflect);
						//var angle = Vector3.Angle(-__instance.transform.forward, bouncevector);

					}
				}

				avgbouncevector += bouncevector[i];
				//avgbouncevector /= bouncevector.Length;
				break;
			}
			if (Player.m_localPlayer != null)
			{
				if (PlayerPatcher.piecename == "$piece_woodwall")
				{
					if (!collision.gameObject.GetComponent<Rigidbody>())
					{
						RBpiece2 = collision.gameObject.AddComponent<Rigidbody>();
						RBpiece2 = collision.gameObject.GetComponent<Rigidbody>();
						RBpiece2.isKinematic = true;
						RBpiece2.detectCollisions = true;
						rb.Add(RBpiece2.GetComponent<Rigidbody>());
						rbpos.Add(collision.transform.position);
					}
				}	
			}
		}
	}



	/// <summary>
	/// ////////////////////////////////////////////////////////////////Patch in Jump Mechanics
	/// </summary>
	[HarmonyPatch(typeof(Character), "UpdateGroundContact")]
	public static class WallCheck
	{
		public static bool RayFrontTest;
		public static bool RayBackTest;
		public static bool SphereTest;

		public static float RayFrontLength;
		public static float RayBackLength;
		public static float RayDownLength;
		public static Vector3 test2;

		private static void Prefix(Character __instance, ref float ___m_slippage)
		{
			
			test2 = new Vector3(0, 2f, 0);
			if (__instance.IsPlayer())
			{
				RayFrontTest = false;
				RayBackTest = true;
				SphereTest = false;
				RayFrontLength = 5f;
				RayBackLength = 0f;
				RayDownLength = 1f;

				int mask2 = LayerMask.GetMask("Default", "static_solid", "terrain", "vehicle", "piece", "viewblock");
				if (!__instance.IsOnGround())
				{
					UpdateGroundContact_Patch.isonground = false;
					if (Physics.Raycast(__instance.transform.position, __instance.transform.forward, out var hitInfo3, RayFrontLength, mask2))
					{
						RayFrontTest = true;
						RayBackLength = 0f;
					}
					else
					{
						RayFrontTest = false;
						RayBackLength = 5f;
					}

					if (Physics.Raycast(__instance.transform.position, __instance.transform.forward * -1, out var hitInfo2, RayBackLength, mask2))
					{
						RayBackTest = true;
						RayFrontLength = 0f;
					}
					else
					{
						RayBackTest = false;
						RayFrontLength = 5f;
					}

					if (Physics.CheckSphere(__instance.transform.position + Vector3.up, 1f, mask2))
					{
						SphereTest = true;
					}
					else
					{
						SphereTest = false;
					}
				}
				if (!__instance.IsOnGround())
                {
					if (!Physics.Raycast(__instance.transform.position, __instance.transform.up * -1, out var hitInfo4, RayDownLength, mask2))
					{
						WallGround.bouncenumber = 0;						
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
		public static Quaternion groundpoint2;
		public static float angle;
		public static float windfactor;
		public static float jumpfactor = 10f;
		public static float jumpvelocity;
		public static float jumpvelocity2;
		public static bool isonground;

		private static void Prefix(Character __instance, ref float ___m_airControl,  ref float ___m_acceleration, ref float ___m_runSpeed, ref float ___m_walkSpeed, ref Rigidbody ___m_body,  ref Vector3 ___m_currentVel, ref float ___m_maxAirAltitude, ref float ___m_waterLevel, ref float ___m_jumpForce, ref float ___m_jumpForceForward, ref float ___m_runTurnSpeed, ref float ___m_turnSpeed, ref float ___m_flyTurnSpeed, ref Vector3 ___m_moveDir, ref Vector3 ___m_lastGroundPoint)
		{
			jumpvector = Vector3.Project(___m_body.velocity, __instance.transform.up * -1);
			jumpvelocity = jumpvector.magnitude;
			jumpvelocity = Mathf.Clamp(jumpvelocity, 0f, 30f);
			Vector3 vector = Vector3.zero;

			if (__instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename == "$piece_stonefloor2x2")
			{
				return;
			}



			if (__instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename != "$piece_stonefloor2x2")
			{
				groundpoint = ___m_lastGroundPoint;
				groundpoint2 = Quaternion.LookRotation(__instance.transform.forward);
			}

			if (__instance.IsPlayer() && !__instance.IsOnGround() && WallGround.shootnumber > 0)
			{
				Jump_Patch.hasjumped = true;
				___m_airControl = 0.01f;
				WallGround.ShotVector = Vector3.zero;
			}

			if (__instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename == "$piece_crystalwall1x1")
			{
				___m_jumpForceForward = 0f;
				___m_jumpForce = 15f;
				___m_runTurnSpeed = 300f;
				___m_turnSpeed = 300f;
				___m_flyTurnSpeed = 30f;
			}

			if (__instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename == "$piece_table_round")
			{
				___m_jumpForce = 20f;
			}

			if (__instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename != "$piece_table_oak")
			{
				WallGround.shootnumber = 0;
				JumpNumber = 0;
				___m_runSpeed = 7;
				___m_walkSpeed = 5;
				___m_acceleration = 1;
				
				WallGround.ShotVector = Vector3.zero;

			}

			if (__instance.IsPlayer() && !__instance.IsOnGround() && jumpvelocity > 1)
			{
				___m_maxAirAltitude = __instance.transform.position.y + (___m_maxAirAltitude - __instance.transform.position.y) * 0f;
				jumpvelocity2 = jumpvelocity;
				
			}
			if (__instance.IsPlayer() && __instance.IsOnGround() && PlayerPatcher.piecename != "$piece_crystalwall1x1" && PlayerPatcher.piecename != "$piece_table_round")
			{
				___m_jumpForce = 10f;
				___m_jumpForceForward = 1f;
				___m_runTurnSpeed = 300f;
				___m_turnSpeed = 300f;
				___m_flyTurnSpeed = 12f;
				
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

			if (__instance.IsPlayer() && Biomes == 256)
			{
				WaterJump = ___m_waterLevel - __instance.transform.position.y;
			}

			//Swamp Wind
			if (Biomes == 2 && __instance.IsPlayer() && __instance.IsOnGround())
			{
				//Set Env
				//EnvMan.instance.m_debugEnv = "GDKing";
				dir2 = ___m_moveDir;
				dir2.y = 0f;
				dir2 = dir2.normalized;
				dir3 = EnvMan.instance.GetWindDir();
				dir3 = dir3.normalized;
				angle = Vector3.Angle(dir2, dir3);
				float factor = angle / 180f;
				windfactor = Mathf.Lerp(20f, 0f, factor);
				___m_jumpForceForward = windfactor;
			}

			//Mountain
			if (Biomes == 4 && __instance.IsPlayer() && WallGround.number1 > 0.1f)
			{
				___m_runTurnSpeed = 300f;
				___m_turnSpeed = 300f;
				___m_flyTurnSpeed = 30f;
			}
			//Blackforest
			if (Biomes == 8 && __instance.IsPlayer() && WallGround.number1 > 0.1f)
			{
				___m_jumpForce = 10f;
				___m_jumpForceForward = 1f;
				___m_runTurnSpeed = 300f;
				___m_turnSpeed = 300f;
				___m_flyTurnSpeed = 30f;
			}
			
			
		}
	}

	[HarmonyPatch(typeof(Character), "Jump")]
	private static class Jump_Patch
	{
		public static bool hasjumped;
		private static void Prefix(Character __instance, ref Vector3 ___m_currentVel, ref Rigidbody ___m_body, ref float ___m_jumpForce, ref float ___m_jumpForceForward, ref float ___m_lastGroundTouch, ref float ___m_maxAirAltitude, ref float ___m_jumpStaminaUsage, ref float ___m_airControl, ref Vector3 ___m_moveDir)
		{
			___m_jumpStaminaUsage = 0f;
			PlayerPatcher.cooldown = 0;
			//Plains Dash
			if (Biomes == 16 && __instance.IsPlayer())
			{
				if (__instance.IsOnGround())
				{
					hasjumped = true;
				}
				if (JumpNumber > 0 && __instance.IsOnGround() && WallGround.number1 < 0.1f)
				{
					JumpNumber = 0;
					___m_airControl = 0.1f;
					
				}

				if (hasjumped == true && JumpNumber < 2 && !__instance.IsOnGround())
				{
					___m_airControl = 0.4f;
					___m_maxAirAltitude = __instance.transform.position.y;
					___m_lastGroundTouch = 0.1f;
					___m_jumpForce = 5f;
					___m_jumpForceForward = 70f;
					JumpNumber++;
				}
			}

			//Blackforest Doublejump
			if (Biomes == 8 && __instance.IsPlayer())
			{
				if (JumpNumber > 0 && __instance.IsOnGround() && WallGround.number1 < 0.1f)
				{
					JumpNumber = 0;
				}
				if (JumpNumber < 1 && !__instance.IsOnGround())
				{
					___m_maxAirAltitude = __instance.transform.position.y;
					___m_lastGroundTouch = 0.1f;
					JumpNumber++;
				}
			}

			//Ashland Slowfall
			if (Biomes == 32 && __instance.IsPlayer())
			{
				if (!__instance.IsOnGround())
				{
					___m_body.useGravity = false;
					___m_body.velocity = ___m_currentVel;
				}
			}

			//Ocean WaterJump
			if (Biomes == 256 && __instance.IsPlayer())
			{
				if (JumpNumber > 0 && __instance.IsOnGround() && WallGround.number1 < 0.1f)
				{
					JumpNumber = 0;
				}
				if (JumpNumber < 3 && (double)UpdateGroundContact_Patch.WaterJump > -0.4 && (double)UpdateGroundContact_Patch.WaterJump < 1)
				{
					___m_maxAirAltitude = __instance.transform.position.y;
					___m_lastGroundTouch = 0.1f;
					JumpNumber++;
				}
				if (JumpNumber < 3 && (double)UpdateGroundContact_Patch.WaterJump < -0.4 && !__instance.IsOnGround())
				{
					JumpNumber = 3;
				}
			}

			//Mountain WallJump
			if (Biomes == 4 && __instance.IsPlayer())
			{
				if (JumpNumber > 0 && __instance.IsOnGround() && WallGround.number1 < 0.1f)
				{
					JumpNumber = 0;
				}
				if (JumpNumber < 10 && !__instance.IsOnGround() && WallGround.WallContact == true && HasWallJumped != true)
				{
					___m_jumpForce = 10f;
					___m_moveDir = WallGround.GrabCurrentVelo;
					___m_maxAirAltitude = __instance.transform.position.y;
					___m_lastGroundTouch = 0.1f;
					HasWallJumped = true;
					JumpNumber++;
				}
			}
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
		[HarmonyPrefix]
		public static void Prefix(ref float ___m_debugWindIntensity, ref bool ___m_debugWind)
		{
			if (Biomes == 2)
			{
				___m_debugWind = true;
				___m_debugWindIntensity = 1;
			}
			if (Biomes != 2)
			{
				___m_debugWind = false;	
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
			if (PlayerPatcher.cpnumber < 1)
			{
				savedPos = new Vector3(__instance.transform.position.x, __instance.transform.position.y, __instance.transform.position.z);
				savedPos2 = new Vector3(__instance.transform.position.x, __instance.transform.position.y, __instance.transform.position.z);
			}
			spawnPos = Vector3.zero;
		}
	}

	[HarmonyPatch(typeof(Chat), "InputText")]
	private static class test
	{

		[HarmonyPrefix]
		public static void Prefix(InputField ___m_input)
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

    [HarmonyPatch(typeof(VisEquipment), "Update")]
    private static class ItemPower

    {
        public static void Prefix(ref VisEquipment __instance, ref string ___m_helmetItem)
        {
			if(___m_helmetItem == "HelmetBronze")
            {
				helmet = true;
            }
			if (___m_helmetItem == "")
			{
				helmet = false;
			}
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



	/// <summary>
	/// //////////////////////////////////////////////////////Updateing TP/CP, Stopwatch, WallChecker
	/// </summary>
	[HarmonyPatch(typeof(Player), "Update")]
	internal class PlayerPatcher
	{
		public static Quaternion rotation;
		public static string timing;
		public static string tpcount;
		public static string cpcount;
		public static string text3;
		public static string piecename;
		

		public static int tpnumber;
		public static int cpnumber;
		public static float cooldown = 0;
		public static float counter1 = 0;
		public static float counter2 = 0;
		public static bool switcher = false;



		public static Ground CurrentGround { get; private set; } = Ground.None;


		public static void Postfix(ref Player __instance)
		{
			if (__instance == Player.m_localPlayer)
			{
				if (config.IfCheckpointkeyPressed() && UpdateGroundContact_Patch.isonground == true && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer)
				{
					savedPos2 = savedPos;
					savedPos = new Vector3(__instance.transform.position.x, __instance.transform.position.y, __instance.transform.position.z);
					rotation = Quaternion.LookRotation(__instance.transform.forward);
					spawnPos = savedPos;
					cpnumber++;
					Debug.Log($"Saved pos : {savedPos}");
					Debug.Log($"Saved pos : {savedPos2}");
				}
				else if (config.IfTeleportingkeyPressed() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer)
				{
					__instance.TeleportTo(savedPos, rotation, distantTeleport: true);
					Debug.Log($"Teleported to : {savedPos}");
					tpnumber++;
				}
				else if (config.IfTeleportingkey2Pressed() && ChatCheck.checking == false && !Console.IsVisible() && !Menu.IsVisible() && !Minimap.InTextInput() && !TextInput.IsVisible() && Player.m_localPlayer)
				{
					__instance.TeleportTo(savedPos2, rotation, distantTeleport: true);
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
			}

			if (HasWallJumped == true)			 
			{
				if(WallCheck.RayBackTest == true || WallCheck.RayFrontTest == false)
				{
					if (WallCheck.SphereTest == false)
					{
						timer -= 1.0f * Time.deltaTime;
						if (timer <= 0f)
						{
							HasWallJumped = false;
							timer = wallJumpCooldown;
						}
					}
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

			if (stopwatchActive == true)
			{
				currentTime = currentTime + Time.deltaTime;
			}
			TimeSpan t = TimeSpan.FromSeconds(currentTime);
			timing = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
			tpcount = " TP:" + tpnumber.ToString();
			cpcount = " CP:" + cpnumber.ToString();
			Biomes = (int)__instance.GetCurrentBiome();

			if (Player.m_localPlayer != null)
			{
				text3 = TerrainModifier.FindClosestModifierPieceInRange(Player.m_localPlayer.transform.position, 6f)?.m_name?.Replace("$piece_", string.Empty);
				
				Collider lastGroundCollider = __instance.GetLastGroundCollider();
				WearNTear wearNTear = null;
				Piece piece = null;
				
				if (lastGroundCollider != null)
				{
					wearNTear = lastGroundCollider.GetComponentInParent<WearNTear>();
					piece = lastGroundCollider.GetComponentInParent<Piece>();
					
					
					if ( piece != null)
					{
						piecename = piece.m_name.ToString();
					
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
			}



            if (piecename == "$piece_stonefloor2x2")
			{
				cooldown += Time.deltaTime;
				if (cooldown > 0.15)
				{
					__instance.TeleportTo(UpdateGroundContact_Patch.groundpoint, UpdateGroundContact_Patch.groundpoint2, distantTeleport: true);
					cooldown = 0;
				}
			}

			// Debug.Log("bouncenr: " + BounceIII.bouncenumber + "velo: " + UpdateGroundContact_Patch.jumpvelocity2);

			//Debug.Log("bounce: " + BounceII.touched);
			//Debug.Log("bounce: " + counter1);

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
		}
	}


	private void Awake()
	{
		timer = wallJumpCooldown;
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		config.ConfigFile = base.Config;
		config.SetupConfig();
		currentTime = 0;
	}
}
/// <summary>
/// /////////////////////////////////////////Patch in Config File
/// </summary>
public class JumpworldConfig
{
	public ConfigEntry<string> checkpoint;
	public ConfigEntry<string> teleporting;
	public ConfigEntry<string> teleporting2;
	public ConfigEntry<string> killme;

	public ConfigFile ConfigFile { get; set; }

	public void SetupConfig()
	{
		checkpoint = ConfigFile.Bind("Hotkeys", "Checkpoint", "Insert", "blaaa");
		teleporting = ConfigFile.Bind("Hotkeys", "Teleporting", "Delete", "nraaa");
		teleporting2 = ConfigFile.Bind("Hotkeys", "Teleporting2", "END", "graaaa");
		killme = ConfigFile.Bind("Hotkeys", "Killme", "HOME", "kraaaa");

		checkpoint.Value = checkpoint.Value.ToLower();
		teleporting.Value = teleporting.Value.ToLower();
		teleporting2.Value = teleporting2.Value.ToLower();
		killme.Value = killme.Value.ToLower();
	}

	public bool IfCheckpointkeyPressed()
    {
		return Input.GetKeyDown(checkpoint.Value);
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

