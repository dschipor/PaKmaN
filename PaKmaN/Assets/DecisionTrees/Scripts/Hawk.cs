//#define DEBUG_LEVEL_VERBOSE
#define DEBUG_LEVEL_DRAWING
#define DEBUG_LEVEL_INFO
#define DEBUG_LEVEL_WARNING
#define DEBUG_LEVEL_ERROR
#define DEBUG_LEVEL_CONSOLEOUT

// NOTE: Defines must be on top of the file. Disable to take out debug code in levels. For MAX performance - Disable ALL OF IT before a release
using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using System.IO;

/// <summary>
/// Utility class: Our wrapper solution to do debugging properly and use .NET level power. This will ensure that we can control debug code output in the final game.
/// </summary>
public static class Hawk
{
	public const int ObjectLogLimit = 50;
	private static Dictionary<GameObject, List<string>> messages = new Dictionary<GameObject, List<string>>();

	private static void Record(Object context, string message)
	{
		GameObject go = context as GameObject;
		if (go == null)
		{
			var comp = context as Component;
			if (comp != null)
				go = comp.gameObject;
		}
		if (go == null)
			return;

		List<string> log = new List<string>();
		if (!messages.TryGetValue(go, out log))
		{
			messages.Add(go, log = new List<string>());
		}

		if (log.Count >= ObjectLogLimit)
			log.RemoveAt(0);

		log.Add(message);
	}

	public static List<string> GetLog(GameObject obj)
	{
		if (!messages.ContainsKey(obj))
			return null;

		return messages[obj];
	}


	[Conditional("DEBUG_LEVEL_VERBOSE")]
	public static void AssertLogVerbose (this Object context, bool condition, string message, params object[] args)
	{
		AssertLog(context, condition, message, args);
	}

	[Conditional("DEBUG_LEVEL_INFO")]
	public static void AssertLog (this Object context, bool condition, string message, params object[] args)
	{
		if (!condition)
			Log(message, args);
	}

	[Conditional("DEBUG_LEVEL_INFO")]
	public static void Log(string message, params object[] args)
	{
		Log (null, message, args);
	}

	[Conditional("DEBUG_LEVEL_VERBOSE")]
	public static void LogVerbose(this Object context, string message, params object[] args)
	{
		Log(context, message, args);
	}
	
	[Conditional("DEBUG_LEVEL_INFO")]
	public static void Log(this Object context, string message, params object[] args)
	{
		if (args.Length > 0)
			message = string.Format(message, args);
		
		if (context != null)
			Debug.Log (message, context);
		else
			Debug.Log (message);

		Record (context, message);
	}
	
	[Conditional("DEBUG_LEVEL_WARNING")]
	public static void LogWarning(string message, params object[] args)
	{
		LogWarning(null, message, args);
	}
	
	[Conditional("DEBUG_LEVEL_WARNING")]
	public static void LogWarning(Object context, string message, params object[] args)
	{
		if (args.Length > 0)
			message = string.Format(message, args);
		
		if (context != null)
			Debug.LogWarning (message, context);
		else 
			Debug.LogWarning(message);

		message = "WARNING: "+message;

		Record(context, message);
		//ConsoleOut(message);
	}
	
	[Conditional("DEBUG_LEVEL_WARNING")]
	public static void AssertWarning(bool condition, string message, params object[] args)
	{
		if (condition)
			return;


		LogWarning(message, args);
	}
	
	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void LogError(string message, params object[] args)
	{
		LogError(null, message, args);
	}

	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void LogError(System.Exception ex)
	{
		Debug.LogException (ex);
	}
	
	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void LogError(Object context, string message, params object[] args)
	{
		if (args.Length > 0)
			message = string.Format(message, args);

		message = string.Format("{0} {1}", context != null ? context.ToString() : null, message);
		
		if (context != null)
			Debug.LogError (message, context);
		else
			Debug.LogError(message);

		message = "ERROR: "+message;
		Record(context, message);
		//ConsoleOut(message);
	}

	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void AssertParameter(Object context, object parameter, string paramName, string msg = null)
	{
		AssertParameter(context, paramName, parameter != null, msg);
	}

	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void AssertParameter(Object context, string paramName, bool condition, string msg = null)
	{
		if (!condition)
			ParameterError(context, paramName, msg);
	}

	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void AssertError (bool condition, string message, params object[] args)
	{
		AssertError(null, condition, message, args);
	}

	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void ParameterError(Object context, string paramName, string msg = null)
	{
		LogError(context, "The {0} parameter '{1}' assertion failed. {2}", context, paramName, msg);
	}
	
	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void AssertNotNull(params object[] objs)
	{
		for(int i = 0; i < objs.Length; i++)
		{
			var obj = objs[i];
			if (obj == null)
			{
				AssertError(false, "Null Check Failed! Hint: Make sure your designer configuration is correct.");
				return;
			}
		}
	}

	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void AssertError (Object context, bool condition, string message, params object[] args)
	{
		if (condition)
			return;

		LogError(context, message, args);
	}
	
	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void LogException(System.Exception ex)
	{
		LogException(null, ex);
	}
	
	[Conditional("DEBUG_LEVEL_ERROR")]
	public static void LogException(Object context, System.Exception ex)
	{
		Debug.LogException (ex, context);
		//ConsoleOut(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
		Record(context, ex.Message);
	}

	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void AssertCube(bool condition, Bounds bounds, Color color = default(Color))
	{
		if (condition)
			DrawCube(bounds, color == default(Color) ? Color.green : color);
	}

	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void DrawCube(this Bounds bounds, Color color = default(Color))
	{
		DrawCube(bounds.center, bounds.size, color == default(Color) ? Color.green : color);
	}

	
	/// <summary>
	/// Draws the cube.
	/// </summary>
	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void DrawCube(Vector3 position, Vector3 size, Color color)
	{
		Vector3 leftFrontDown 	= new Vector3( -size.x * 0.5f, -size.y * 0.5f, -size.z * 0.5f );
		Vector3 rightFrontDown 	= new Vector3( 	size.x * 0.5f, -size.y * 0.5f, -size.z * 0.5f );
		Vector3 rightFrontUp 	= new Vector3( 	size.x * 0.5f, 	size.y * 0.5f, -size.z * 0.5f );
		Vector3 leftFrontUp 	= new Vector3( -size.x * 0.5f, 	size.y * 0.5f, -size.z * 0.5f );
 
		Vector3 leftBackDown 	= new Vector3( -size.x * 0.5f, -size.y * 0.5f, size.z * 0.5f );
		Vector3 rightBackDown 	= new Vector3( 	size.x * 0.5f, -size.y * 0.5f, size.z * 0.5f );
		Vector3 rightBackUp 	= new Vector3( 	size.x * 0.5f, 	size.y * 0.5f, size.z * 0.5f );
		Vector3 leftBackUp 		= new Vector3( -size.x * 0.5f, 	size.y * 0.5f, size.z * 0.5f );
 
		Vector3[] arr = new Vector3[8];
 
		arr[0] = leftFrontDown;
		arr[1] = rightFrontDown;
		arr[2] = rightFrontUp;
		arr[3] = leftFrontUp;
 
		arr[4] = leftBackDown;
		arr[5] = rightBackDown;
		arr[6] = rightBackUp;
		arr[7] = leftBackUp;
 
		for (int i = 0; i < arr.Length; i++)
			arr[i] += position;
 
		for (int i = 0; i < arr.Length; i++)
		{
			for (int j = 0; j < arr.Length; j++)
			{
				if (i != j)
				{
					UnityEngine.Debug.DrawLine(arr[i], arr[j], color);	
				}
			}
		}
	}

	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void AssertLine(bool condition, params Vector3[] vectors )
	{
		if (condition)
			DrawLine(Color.white, Color.white, vectors);
	}
	
	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void DrawLine(params Vector3[] vectors )
	{
		DrawLine(Color.white, Color.white, vectors);
	}

	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void AssertLine (bool condition, Color color, params Vector3[] vectors)
	{
		if (condition)
			DrawLine(color, color, vectors);
	}
	
	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void DrawLine (Color color, params Vector3[] vectors)
	{
		DrawLine(color, color, vectors);
	}
	
	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void DrawLine (Color lineColor, Color vertexColor, params Vector3[] vectors)
	{
		for(int i = 0; i < vectors.Length-1; i++)
		{	
			Vector3 p1 = vectors[i];
			Vector3 p2 = vectors[(i+1) % vectors.Length];
			UnityEngine.Debug.DrawLine (p1, p2, lineColor  );
			
			p2 = p1;
			p2.y += 1f;
			UnityEngine.Debug.DrawLine(p1, p2, vertexColor);
			
		}
	}

	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void DrawRay(Color lineColor, Vector3 pos, Vector3 dir )
	{
		Debug.DrawRay(pos, dir, lineColor);
	}

	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void AssertRay(bool condition, Color lineColor, Vector3 pos, Vector3 dir )
	{
		if (condition)
			DrawRay(lineColor, pos, dir);
	}


	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void DrawTriangles(Vector3[] vectors, int[] triangles)
	{
		Color[] colors = new Color[] { Color.white, Color.grey, Color.yellow };
		int c = 0;
		for(int i = 0; i < triangles.Length; i = i+3)
		{	
			Color color = colors[c % colors.Length];
			DrawLine(color, color, vectors[triangles[i]], vectors[triangles[i+1]], vectors[triangles[i+2]]);
			c++;
		}
	}

	[Conditional("DEBUG_LEVEL_DRAWING")]
	public static void DumpPng(Texture2D texture, string name)
	{
		if (texture == null)
			return;

		try
		{
			var path = Path.Combine(Application.persistentDataPath, string.Format("Debug{0}{1}.png", Path.DirectorySeparatorChar, name));
			var dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			File.WriteAllBytes(path, texture.EncodeToPNG());
		}
		catch(System.Exception ex)
		{
			LogError(ex);
		}
	}
}


