/*
Copyright (c) 2013 Mitch Thompson
Extended by Harald Lurger (2013) (Process to Sprites)

Standard MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;


public static class TexturePackerImport
{

	public static void ProcessToSprite(TextAsset txt)
	{
		string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(txt));
		TexturePacker.MetaData meta = TexturePacker.GetMetaData(txt.text);

		List<SpriteMetaData> newSprites = TexturePacker.ProcessToSprites(txt.text);

		string path = rootPath + "/" + meta.image;
		TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;

		if (texImp.spriteImportMode == SpriteImportMode.Multiple)
		{
			List<SpriteMetaData> oldSprites = new List<SpriteMetaData>(texImp.spritesheet);
			for (int i = 0; i < newSprites.Count; i++)
			{
				SpriteMetaData newSprite = newSprites[i];
				for (int j = 0; j < oldSprites.Count; j++)
				{
					SpriteMetaData oldSprite = oldSprites[j];
					if (newSprite.name == oldSprite.name)
					{
						if (newSprite.rect.width == oldSprite.rect.width && newSprite.rect.height == oldSprite.rect.height)
						{
							newSprite.alignment = oldSprite.alignment;
							newSprite.border = oldSprite.border;

							newSprites[i] = newSprite;
						}
						newSprite.pivot = oldSprite.pivot;
						break;
					}
				}
			}
		}

		texImp.textureType = TextureImporterType.Sprite;
		texImp.spriteImportMode = SpriteImportMode.Multiple;
		texImp.spritesheet = newSprites.ToArray();

		//MoleMole.UIPostprocessor.CreateSpriteSheetScriptObject(texImp);

		AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
	}
	[MenuItem("Assets/TexturePacker/Process to Sprites")]
	static void ProcessToSprite()
	{
		TextAsset txt = (TextAsset)Selection.activeObject;

		ProcessToSprite(txt);
	}

	[MenuItem("Assets/TexturePacker/Process to Meshes")]
	static Mesh[] ProcessToMeshes()
	{
		TextAsset txt = (TextAsset)Selection.activeObject;

		Quaternion rotation = Quaternion.identity;
		string pref = EditorPrefs.GetString("TexturePackerImporterFacing", "back");

		switch (pref)
		{
			case "back":
				rotation = Quaternion.identity;
				break;
			case "forward":
				rotation = Quaternion.LookRotation(Vector3.back);
				break;
			case "up":
				rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
				break;
			case "down":
				rotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
				break;
			case "right":
				rotation = Quaternion.LookRotation(Vector3.left);
				break;
			case "left":
				rotation = Quaternion.LookRotation(Vector3.right);
				break;
		}


		Mesh[] meshes = TexturePacker.ProcessToMeshes(txt.text, rotation);

		string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(txt));

		Directory.CreateDirectory(Application.dataPath + "/" + rootPath.Substring(7, rootPath.Length - 7) + "/Meshes");

		Mesh[] returnMeshes = new Mesh[meshes.Length];

		int i = 0;
		foreach (Mesh m in meshes)
		{
			string assetPath = rootPath + "/Meshes/" + Path.GetFileNameWithoutExtension(m.name) + ".asset";
			Mesh existingMesh = (Mesh)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Mesh));
			if (existingMesh == null)
			{
				AssetDatabase.CreateAsset(m, assetPath);
				existingMesh = (Mesh)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Mesh));
			}
			else
			{
				existingMesh.triangles = new int[0];
				existingMesh.colors32 = new Color32[0];
				existingMesh.uv = new Vector2[0];
				existingMesh.vertices = m.vertices;
				existingMesh.uv = m.uv;
				existingMesh.colors32 = m.colors32;
				existingMesh.triangles = m.triangles;
				existingMesh.RecalculateNormals();
				existingMesh.RecalculateBounds();
				EditorUtility.SetDirty(existingMesh);
				Mesh.DestroyImmediate(m);
			}

			returnMeshes[i] = existingMesh;
			i++;
		}

		return returnMeshes;
	}

	[MenuItem("Assets/TexturePacker/Process to Prefabs")]
	static void ProcessToPrefabs()
	{
		Mesh[] meshes = ProcessToMeshes();


		TextAsset txt = (TextAsset)Selection.activeObject;
		string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(txt));

		string prefabPath = rootPath.Substring(7, rootPath.Length - 7) + "/Prefabs";
		Directory.CreateDirectory(Application.dataPath + "/" + prefabPath);

		prefabPath = "Assets/" + prefabPath;


		//make material
		TexturePacker.MetaData meta = TexturePacker.GetMetaData(txt.text);

		string matPath = rootPath + "/" + (Path.GetFileNameWithoutExtension(meta.image) + ".mat");
		string texturePath = rootPath + "/" + meta.image;
		Material mat = (Material)AssetDatabase.LoadAssetAtPath(matPath, typeof(Material));
		if (mat == null)
		{
			mat = new Material(Shader.Find("Sprites/Transparent Unlit"));
			Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
			if (tex == null)
			{
				EditorUtility.DisplayDialog("Error!", "Texture " + meta.image + " not found!", "Ok");
			}
			mat.mainTexture = tex;
			AssetDatabase.CreateAsset(mat, matPath);
		}


		AssetDatabase.Refresh();

		for (int i = 0; i < meshes.Length; i++)
		{
			string prefabFilePath = prefabPath + "/" + meshes[i].name + ".prefab";

			bool createdNewPrefab = false;
			Object prefab = AssetDatabase.LoadAssetAtPath(prefabFilePath, typeof(Object));

			if (prefab == null)
			{
				prefab = PrefabUtility.CreateEmptyPrefab(prefabFilePath);
				createdNewPrefab = true;
			}

			if (createdNewPrefab)
			{
				GameObject go = new GameObject(meshes[i].name, typeof(MeshRenderer), typeof(MeshFilter));
				go.GetComponent<MeshFilter>().sharedMesh = meshes[i];
				go.GetComponent<Renderer>().sharedMaterial = mat;

				PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ConnectToPrefab);

				GameObject.DestroyImmediate(go);
			}
			else
			{
				GameObject pgo = (GameObject)prefab;
				pgo.GetComponent<Renderer>().sharedMaterial = mat;
				pgo.GetComponent<MeshFilter>().sharedMesh = meshes[i];
				EditorUtility.SetDirty(pgo);
			}
		}
	}

	//Validators
	[MenuItem("Assets/TexturePacker/Process to Prefabs", true)]
	[MenuItem("Assets/TexturePacker/Process to Meshes", true)]
	[MenuItem("Assets/TexturePacker/Process to Sprites", true)]
	static bool ValidateProcessTexturePacker()
	{
		Object o = Selection.activeObject;

		if (o == null)
			return false;

		if (o.GetType() == typeof(TextAsset))
		{
			return (((TextAsset)o).text.hashtableFromJson()).IsTexturePackerTable();
		}

		return false;

	}




	//Attach 90 degree "Shadow" meshes
	[MenuItem("Assets/TexturePacker/Attach Shadow Mesh")]
	static void AttachShadowMesh()
	{
		List<Mesh> meshes = new List<Mesh>();
		foreach (Object o in Selection.objects)
		{
			if (o is Mesh) meshes.Add(o as Mesh);
		}

		foreach (Mesh m in meshes)
		{
			Vector3[] verts = new Vector3[m.vertexCount * 2];
			Vector2[] uvs = new Vector2[m.vertexCount * 2];
			Color32[] colors = new Color32[m.vertexCount * 2];
			int[] triangles = new int[m.triangles.Length * 2];

			System.Array.Copy(m.vertices, 0, verts, m.vertexCount, m.vertexCount);
			System.Array.Copy(m.uv, 0, uvs, m.vertexCount, m.vertexCount);
			System.Array.Copy(m.colors32, 0, colors, m.vertexCount, m.vertexCount);
			System.Array.Copy(m.triangles, 0, triangles, m.triangles.Length, m.triangles.Length);

			for (int i = 0; i < m.vertexCount; i++)
			{
				verts[i].x = verts[i + m.vertexCount].x;
				verts[i].y = verts[i + m.vertexCount].z;
				verts[i].z = verts[i + m.vertexCount].y;

				uvs[i] = uvs[i + m.vertexCount];
				colors[i] = new Color32(0, 0, 0, 64);


			}

			for (int i = 0; i < m.triangles.Length; i++)
			{
				triangles[i] = triangles[i + m.triangles.Length];
				triangles[i + m.triangles.Length] += m.vertexCount;

			}

			m.vertices = verts;
			m.uv = uvs;
			m.colors32 = colors;
			m.triangles = triangles;

			m.RecalculateNormals();
			m.RecalculateBounds();
			EditorUtility.SetDirty(m);
		}
	}


	//Validators
	[MenuItem("Assets/TexturePacker/Attach Shadow Mesh", true)]
	static bool ValidateAttachShadowMesh()
	{
		Object[] objs = Selection.objects;
		foreach (Object o in objs)
		{
			if (!(o is Mesh))
			{
				return false;
			}
		}

		return true;
	}



	//Options
	[MenuItem("Assets/TexturePacker/Facing/Back")]
	static void SetFacingBack() { EditorPrefs.SetString("TexturePackerImporterFacing", "back"); }

	[MenuItem("Assets/TexturePacker/Facing/Forward")]
	static void SetFacingForward() { EditorPrefs.SetString("TexturePackerImporterFacing", "forward"); }

	[MenuItem("Assets/TexturePacker/Facing/Up")]
	static void SetFacingUp() { EditorPrefs.SetString("TexturePackerImporterFacing", "up"); }

	[MenuItem("Assets/TexturePacker/Facing/Down")]
	static void SetFacingDown() { EditorPrefs.SetString("TexturePackerImporterFacing", "down"); }

	[MenuItem("Assets/TexturePacker/Facing/Right")]
	static void SetFacingRight() { EditorPrefs.SetString("TexturePackerImporterFacing", "right"); }

	[MenuItem("Assets/TexturePacker/Facing/Left")]
	static void SetFacingLeft() { EditorPrefs.SetString("TexturePackerImporterFacing", "left"); }
}

public static class TexturePackerExtensions
{
	public static Rect TPHashtableToRect(this Hashtable table)
	{
		return new Rect((float)table["x"], (float)table["y"], (float)table["w"], (float)table["h"]);
	}

	public static Vector2 TPHashtableToVector2(this Hashtable table)
	{
		if (table.ContainsKey("x") && table.ContainsKey("y"))
		{
			return new Vector2((float)table["x"], (float)table["y"]);
		}
		else
		{
			return new Vector2((float)table["w"], (float)table["h"]);
		}
	}

	public static Vector2 TPVector3toVector2(this Vector3 vec)
	{
		return new Vector2(vec.x, vec.y);
	}

	public static bool IsTexturePackerTable(this Hashtable table)
	{
		if (table == null) return false;

		if (table.ContainsKey("meta"))
		{
			Hashtable metaTable = (Hashtable)table["meta"];
			if (metaTable.ContainsKey("app"))
			{
				return true;
				//				if((string)metaTable["app"] == "http://www.texturepacker.com"){
				//					return true;	
				//				}
			}
		}

		return false;
	}
}

public class TexturePacker
{

	public class PackedFrame
	{
		public string name;
		public Rect frame;
		public Rect spriteSourceSize;
		public Vector2 sourceSize;
		public bool rotated;
		public bool trimmed;
		Vector2 atlasSize;

		public PackedFrame(string name, Vector2 atlasSize, Hashtable table)
		{
			this.name = name;
			this.atlasSize = atlasSize;

			frame = ((Hashtable)table["frame"]).TPHashtableToRect();
			spriteSourceSize = ((Hashtable)table["spriteSourceSize"]).TPHashtableToRect();
			sourceSize = ((Hashtable)table["sourceSize"]).TPHashtableToVector2();
			rotated = (bool)table["rotated"];
			trimmed = (bool)table["trimmed"];
		}

		public Mesh BuildBasicMesh(float scale, Color32 defaultColor)
		{
			return BuildBasicMesh(scale, defaultColor, Quaternion.identity);
		}

		public Mesh BuildBasicMesh(float scale, Color32 defaultColor, Quaternion rotation)
		{
			Mesh m = new Mesh();
			Vector3[] verts = new Vector3[4];
			Vector2[] uvs = new Vector2[4];
			Color32[] colors = new Color32[4];


			if (!rotated)
			{
				verts[0] = new Vector3(frame.x, frame.y, 0);
				verts[1] = new Vector3(frame.x, frame.y + frame.height, 0);
				verts[2] = new Vector3(frame.x + frame.width, frame.y + frame.height, 0);
				verts[3] = new Vector3(frame.x + frame.width, frame.y, 0);
			}
			else
			{
				verts[0] = new Vector3(frame.x, frame.y, 0);
				verts[1] = new Vector3(frame.x, frame.y + frame.width, 0);
				verts[2] = new Vector3(frame.x + frame.height, frame.y + frame.width, 0);
				verts[3] = new Vector3(frame.x + frame.height, frame.y, 0);
			}




			uvs[0] = verts[0].TPVector3toVector2();
			uvs[1] = verts[1].TPVector3toVector2();
			uvs[2] = verts[2].TPVector3toVector2();
			uvs[3] = verts[3].TPVector3toVector2();

			for (int i = 0; i < uvs.Length; i++)
			{
				uvs[i].x /= atlasSize.x;
				uvs[i].y /= atlasSize.y;
				uvs[i].y = 1.0f - uvs[i].y;
			}

			if (rotated)
			{
				verts[3] = new Vector3(frame.x, frame.y, 0);
				verts[0] = new Vector3(frame.x, frame.y + frame.height, 0);
				verts[1] = new Vector3(frame.x + frame.width, frame.y + frame.height, 0);
				verts[2] = new Vector3(frame.x + frame.width, frame.y, 0);
			}


			//v-flip
			for (int i = 0; i < verts.Length; i++)
			{
				verts[i].y = atlasSize.y - verts[i].y;
			}

			//original origin
			for (int i = 0; i < verts.Length; i++)
			{
				verts[i].x -= frame.x - spriteSourceSize.x + (sourceSize.x / 2.0f);
				verts[i].y -= (atlasSize.y - frame.y) - (sourceSize.y - spriteSourceSize.y) + (sourceSize.y / 2.0f);
			}

			//scaler
			for (int i = 0; i < verts.Length; i++)
			{
				verts[i] *= scale;
			}

			//rotator
			if (rotation != Quaternion.identity)
			{
				for (int i = 0; i < verts.Length; i++)
				{
					verts[i] = rotation * verts[i];
				}
			}

			for (int i = 0; i < colors.Length; i++)
			{
				colors[i] = defaultColor;
			}


			m.vertices = verts;
			m.uv = uvs;
			m.colors32 = colors;
			m.triangles = new int[6] { 0, 3, 1, 1, 3, 2 };

			m.RecalculateNormals();
			m.RecalculateBounds();
			m.name = name;

			return m;
		}

		public SpriteMetaData BuildBasicSprite(float scale, Color32 defaultColor)
		{
			SpriteMetaData smd = new SpriteMetaData();
			Rect rect;

			if (!rotated)
			{
				rect = this.frame;
			}
			else
			{
				rect = new Rect(frame.x, frame.y, frame.height, frame.width);
			}


			/* Look if frame is outside from texture */

			if ((frame.x + frame.width) > atlasSize.x || (frame.y + frame.height) > atlasSize.y ||
				(frame.x < 0 || frame.y < 0))
			{
				Debug.Log(this.name + " is outside from texture! Sprite is ignored!");
				smd.name = "IGNORE_SPRITE";
				return smd;

			}
			//calculate Height 
			/* Example: Texture: 1000 Width x 500 height 
		 	 * Sprite.Recht(0,0,100,100) --> Sprite is on the bottom left
			 */

			rect.y = atlasSize.y - frame.y - rect.height;

			smd.rect = rect;
			smd.alignment = 1;
			smd.name = name;
			smd.pivot = this.frame.center;
			smd.alignment = (int)SpriteAlignment.Center;


			return smd;
		}
	}

	public class MetaData
	{
		public string image;
		public string format;
		public Vector2 size;
		public float scale;
		public string smartUpdate;

		public MetaData(Hashtable table)
		{
			image = (string)table["image"];
			format = (string)table["format"];
			size = ((Hashtable)table["size"]).TPHashtableToVector2();
			scale = float.Parse(table["scale"].ToString());
			smartUpdate = (string)table["smartUpdate"];
		}
	}

	public static List<SpriteMetaData> ProcessToSprites(string text)
	{
		Hashtable table = text.hashtableFromJson();

		MetaData meta = new MetaData((Hashtable)table["meta"]);

		List<PackedFrame> frames = new List<PackedFrame>();
		Hashtable frameTable = (Hashtable)table["frames"];

		foreach (DictionaryEntry entry in frameTable)
		{
			frames.Add(new PackedFrame((string)entry.Key, meta.size, (Hashtable)entry.Value));
		}

		SortFrames(frames);

		List<SpriteMetaData> sprites = new List<SpriteMetaData>();
		for (int i = 0; i < frames.Count; i++)
		{
			SpriteMetaData smd = frames[i].BuildBasicSprite(0.01f, new Color32(128, 128, 128, 128));
			if (!smd.name.Equals("IGNORE_SPRITE"))
				sprites.Add(smd);
		}

		return sprites;

	}

	private static List<PackedFrame> SortFrames(List<PackedFrame> frames)
	{
		for (int i = frames.Count - 1; i > 0; i--)
		{
			for (int j = 0; j < i; j++)
			{
				if (string.Compare(frames[j + 1].name, frames[j].name) < 0)
				{
					PackedFrame temp = frames[j + 1];
					frames[j + 1] = frames[j];
					frames[j] = temp;
				}
			}
		}
		return frames;
	}

	public static Mesh[] ProcessToMeshes(string text)
	{
		return ProcessToMeshes(text, Quaternion.identity);
	}

	public static Mesh[] ProcessToMeshes(string text, Quaternion rotation)
	{
		Hashtable table = text.hashtableFromJson();

		MetaData meta = new MetaData((Hashtable)table["meta"]);

		List<PackedFrame> frames = new List<PackedFrame>();
		Hashtable frameTable = (Hashtable)table["frames"];

		foreach (DictionaryEntry entry in frameTable)
		{
			frames.Add(new PackedFrame((string)entry.Key, meta.size, (Hashtable)entry.Value));
		}

		List<Mesh> meshes = new List<Mesh>();
		for (int i = 0; i < frames.Count; i++)
		{
			meshes.Add(frames[i].BuildBasicMesh(0.01f, new Color32(128, 128, 128, 128), rotation));
		}

		return meshes.ToArray();
	}

	public static MetaData GetMetaData(string text)
	{
		Hashtable table = text.hashtableFromJson();
		MetaData meta = new MetaData((Hashtable)table["meta"]);

		return meta;
	}
}