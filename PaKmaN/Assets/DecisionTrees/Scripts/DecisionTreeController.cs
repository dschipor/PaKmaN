using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System;

[AddComponentMenu("Decision Tree Toolkit/Decision Tree Controller")]
public class DecisionTreeController : MonoBehaviour
{
	#region Beliefs
	public int batchSize = 250;
	public string sqlLiteDbName = "DecisionTreeData.db";
	public string dataFolder = "DecisionTrees";
	public DecisionTreeInfo[] decisionTrees;
	public GameObject[] eventListeners;
	private DecisionTreeID3 id3 = new DecisionTreeID3();
	#endregion

	#region Behavior
	void Start()
	{
		InitDb();
		BuildTrees();
	}

	public DecisionTreeInfo GetDecisionTree(string decisionTreeName)
	{
		for(int i = 0; i < decisionTrees.Length; i++)
		{
			if (decisionTrees[i].decisionTreeName == decisionTreeName)
				return decisionTrees[i];
		}
		return null;
	}

	public bool? Decide(DecisionQuery query)
	{
		query.result = null;
		var info = GetDecisionTree(query.decisionTreeName);
		
		if (info == null)
			return null;
		

		query.info = info;

		var logic = info.GetOutcome(query.outcomeName);
		UnityEngine.Debug.Log("DECISION: ajunge aici" + logic == null);
		if (logic == null || logic.root == null)
			return null;
		


		query.result = logic.CachedResolve(query);
		
		if (query.result == null)
		{
			var node = id3.Resolve(logic.root, info.predictColumn, query.predicate);
			if (node == null)
				return null;

			if (node.attribute.IsValue)
			{
				query.root = logic.root;
				query.result = (bool)node.attribute.Value;
				logic.Cache(query, query.result.Value);
				return query.result;
			}
			
			return null;
		}
		return query.result;
	}

	public string PrintDecision (DecisionQuery query)
	{
		var info = GetDecisionTree(query.decisionTreeName);
		if (info == null)
			return null;
		
		var logic = info.GetOutcome(query.outcomeName);
		if (logic == null || logic.root == null)
			return null;


		return "<color=#ffe900>"+query.outcomeName+"</color>\n"
			+ id3.PrintResolve(logic.root, info.predictColumn, "o ", query.predicate);
	}

	public void BuildTrees()
	{
		StartCoroutine(LoadFromDb());
	}
	#endregion

	#region SqlLiteDatabase
	public static string GetDataPath(string section)
	{
		string path = System.IO.Path.Combine(Application.persistentDataPath, section);
		if (!System.IO.Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		return path;
	}

	private string GetDbPath ()
	{
		string path = GetDataPath (dataFolder);
		if (!Directory.Exists (path))
			Directory.CreateDirectory (path);
		path = Path.Combine (path, sqlLiteDbName);
		return path;
	}

	private string InitDb()
	{
		var path = GetDbPath ();

		if (!System.IO.File.Exists(path))
		{
			CreateDbFromResource(path, sqlLiteDbName);
		}
		Hawk.Log("DECISION TREE DATABASE READY: {0}", path);
		return path;
	}

	private string GetConnectionString(string path = null)
	{
		if (path == null)
			path = GetDbPath();
		return string.Format ("URI=file:{0}; Version=3", path);
	}

	// Uses the resource to create a copy of the database for our perusal
	public void CreateDbFromResource(string path, string assetName)
	{
		TextAsset asset =  (TextAsset)Resources.Load (assetName);
		if (asset == null)
			throw new InvalidOperationException(string.Format("There is not bytes resource with the name {0}",assetName));
		
		using(FileStream stream = new FileStream(path, FileMode.CreateNew))
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write (asset.bytes);
			writer.Flush();
			stream.Flush();
			writer.Close();
			stream.Close();
		}
	}

	// Use this for initialization
	IEnumerator LoadFromDb ()
	{
		using (SqliteConnection conn = new SqliteConnection(GetConnectionString()))
		{
			conn.Open();
			SqliteCommand command = new SqliteCommand(conn);
			id3 = new DecisionTreeID3();
			int iBatch = 0;
			foreach(var info in decisionTrees)
			{
				string query = string.Format("SELECT * FROM {0}", info.tableName);
				command.CommandText = query;
				foreach(var split in info.outcomes)
				{
					var reader = command.ExecuteReader ();
					
					DataTable table = new DataTable(info.tableName);
					for(int i = 0; i < reader.FieldCount; i++)
					{
						if (reader.GetName(i) == info.predictColumn)
						{
							table.Columns.Add( reader.GetName(i), typeof(bool) );
						}
						else
						{
							table.Columns.Add (reader.GetName(i), reader.GetFieldType(i));
						}
					}
					
					while(reader.Read())
					{
						var dr = table.NewRow();
						for(int i = 0; i < reader.FieldCount; i++)
						{
							if (table.Columns[i].ColumnName == info.predictColumn)
							{
								if (!reader.IsDBNull(i))
								{
									switch(split.type)
									{
									case DecisionTreeOutcome.OutcomeType.Int:
										dr[i] = (reader.GetInt32(i) == split.value);
										break;
									case DecisionTreeOutcome.OutcomeType.Text:
										dr[i] = (reader.GetString(i) == split.textValue);
										break;
									}
								}
							}
							else 
								dr[i] = reader[i];
						}
						table.Rows.Add(dr);
						if (++iBatch % batchSize == 0)
							yield return new WaitForEndOfFrame();
					}
					
					table.AcceptChanges();
					split.root = id3.MountTree(table, info.predictColumn, info.GetAttributes());
					Hawk.Log(string.Format("DECISION TREE {0}{1}\n{2}", info.decisionTreeName, split.name, split.root.PrintNode("--->")));

					foreach(var obj in eventListeners)
					{
						obj.SendMessage("OnDecisionTreeReady", split.root);
					}

					reader.Close ();
				}
			}
		}
	}
	#endregion

}

