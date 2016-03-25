using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class MeshInfo {
	public Mesh m;
	public Vector3[] verts;
	public int[] tris;
	public Vector2[] uv, uv2;
	public Vector3[] normals;
	public string name;

	public MeshInfo(Mesh me, Vector3[] ver, int[] tri) {
		m = me;
		verts = ver;
		tris = tri;
	}
}

public class ObjectInfo {
	public GameObject o;
	public Transform trans;
	public Matrix4x4 lTWM;

	public ObjectInfo(GameObject obj, Transform tran, Matrix4x4 LTWM) {
		o = obj;
		trans = tran;
		lTWM = LTWM;
	}
}

public class SpriteInfo {
	public Sprite spr;
	public Rect rect;
	public Texture2D texture;
	public float width, height;

	public SpriteInfo(Sprite _spr, Rect _rect, Texture2D _texture, float width, float height) {
		spr = _spr;
		rect = _rect;
		texture = _texture;
		this.width = width;
		this.height = height;
    }
}
public class DecalManager : MonoBehaviour {

	public Decal[] decals;
	GameObject[] affectedObjects;
	Mesh[] affectedMeshes;
	Mesh dummyMesh;
	MeshInfo dummyInfo;
	// Use this for initialization
	void Starts() {
		
	}


	

	public void Start() {

		decals = GetComponentsInChildren<Decal>();
		//affectedObjects = GetAffectedObjects(new Bounds(transform.position,Vector3.one * 5), decals[0].affectedLayers);
		List<Mesh> meshes = new List<Mesh>();

		//affectedObjects = new Dictionary<ObjectInfo, MeshInfo>();

		dummyMesh = new Mesh();

		dummyInfo = new MeshInfo(dummyMesh, dummyMesh.vertices, dummyMesh.triangles);
		dummyInfo.uv = dummyMesh.uv;
		dummyInfo.uv2 = dummyMesh.uv2;
		dummyInfo.normals = dummyMesh.normals;

		/*foreach(GameObject go in affectedGOs) {


			if (go.GetComponent<MeshFilter>() != null)
				affectedObjects.Add(new ObjectInfo( go, go.transform, go.transform.localToWorldMatrix), new MeshInfo( go.GetComponent<MeshFilter>().sharedMesh, go.GetComponent<MeshFilter>().sharedMesh.vertices, go.GetComponent<MeshFilter>().sharedMesh.triangles));
		}*/

		affectedMeshes = meshes.ToArray();

		foreach (Decal dc in GetComponentsInChildren<Decal>()) {
			BuildDecal(dc);
			//Thread t = new Thread((value) => BuildDecal((Decal)value));
			//t.Name = "DecalBUilder";
			//t.Start(dc);
			dc.mr.material = dc.material;
		}

		
    }

	private static bool IsLayerContains(LayerMask mask, int layer) {
		return (mask.value & 1 << layer) != 0;
	}


	private void BuildDecal(Decal decal) {
		MeshFilter filter = decal.mf;
		if (filter == null) filter = decal.gameObject.AddComponent<MeshFilter>();
		if (decal.mr == null) decal.gameObject.AddComponent<MeshRenderer>();

		Debug.Log(decal.material == null || decal.sprite == null);
		if (decal.material == null || decal.sprite == null) {
			filter.mesh = null;
			return;
		}

		foreach (GameObject go in GetAffectedObjects(decal.GetBounds(), decal.affectedLayers)) {
			DecalBuilder.BuildDecalForObject(decal, go); 

		}

		DecalBuilder.Push(decal.pushDistance);

		Mesh mesh = DecalBuilder.CreateMesh(dummyInfo);

		Debug.Log("Mesh created");

        if (mesh != null) {
			mesh.name = "DecalMesh";
			filter.mesh = mesh;
		}
	}

	private static GameObject[] GetAffectedObjects(Bounds bounds, LayerMask affectedLayers) {
		MeshRenderer[] renderers = (MeshRenderer[])GameObject.FindObjectsOfType<MeshRenderer>();
		List<GameObject> objects = new List<GameObject>();
		foreach (Renderer r in renderers) {
			if (!r.enabled) continue;
			if (!IsLayerContains(affectedLayers, r.gameObject.layer)) continue;
			if (Vector3.Distance(bounds.center, r.transform.position) > 10) continue;
			if (r.GetComponent<Decal>() != null) continue;

			if (bounds.Intersects(r.bounds)) {
				objects.Add(r.gameObject);
			}
		}
		return objects.ToArray();
	}
}


public class DecalPolygon {

	public List<Vector3> vertices = new List<Vector3>(9);

	public DecalPolygon(params Vector3[] vts) {
		vertices.AddRange(vts);
	}

	public static DecalPolygon ClipPolygon(DecalPolygon polygon, Plane plane) {
		bool[] positive = new bool[9];
		int positiveCount = 0;

		for (int i = 0; i < polygon.vertices.Count; i++) {
			positive[i] = !plane.GetSide(polygon.vertices[i]);
			if (positive[i]) positiveCount++;
		}

		if (positiveCount == 0) return null; // полностью за плоскостью
		if (positiveCount == polygon.vertices.Count) return polygon; // полностью перед плоскостью

		DecalPolygon tempPolygon = new DecalPolygon();

		for (int i = 0; i < polygon.vertices.Count; i++) {
			int next = i + 1;
			next %= polygon.vertices.Count;

			if (positive[i]) {
				tempPolygon.vertices.Add(polygon.vertices[i]);
			}

			if (positive[i] != positive[next]) {
				Vector3 v1 = polygon.vertices[next];
				Vector3 v2 = polygon.vertices[i];

				Vector3 v = LineCast(plane, v1, v2);
				tempPolygon.vertices.Add(v);
			}
		}

		return tempPolygon;
	}

	private static Vector3 LineCast(Plane plane, Vector3 a, Vector3 b) {
		float dis;
		Ray ray = new Ray(a, b - a);
		plane.Raycast(ray, out dis);
		return ray.GetPoint(dis);
	}

}
public class DecalBuilder {

	private static List<Vector3> bufVertices = new List<Vector3>();
	private static List<Vector3> bufNormals = new List<Vector3>();
	private static List<Vector2> bufTexCoords = new List<Vector2>();
	private static List<int> bufIndices = new List<int>();

	public static void BuildDecalForObjectAsync(Decal decal, ObjectInfo affectedObject, MeshInfo affectedMesh) {
		if (affectedMesh.m == null) return;

		Debug.Log("BuildDecalForObjectAsync");

		float maxAngle = decal.maxAngle;

		Plane right = new Plane(Vector3.right, Vector3.right / 2f);
		Plane left = new Plane(-Vector3.right, -Vector3.right / 2f);

		Plane top = new Plane(Vector3.up, Vector3.up / 2f);
		Plane bottom = new Plane(-Vector3.up, -Vector3.up / 2f);

		Plane front = new Plane(Vector3.forward, Vector3.forward / 2f);
		Plane back = new Plane(-Vector3.forward, -Vector3.forward / 2f);

		Vector3[] vertices = affectedMesh.verts;
		int[] triangles = affectedMesh.tris;
		int startVertexCount = bufVertices.Count;



		Matrix4x4 matrix = decal.wTLM * affectedObject.lTWM;

		for (int i = 0; i < triangles.Length; i += 3) {
			int i1 = triangles[i];
			int i2 = triangles[i + 1];
			int i3 = triangles[i + 2];

			Vector3 v1 = matrix.MultiplyPoint(vertices[i1]);
			Vector3 v2 = matrix.MultiplyPoint(vertices[i2]);
			Vector3 v3 = matrix.MultiplyPoint(vertices[i3]);



			Vector3 side1 = v2 - v1;
			Vector3 side2 = v3 - v1;
			Vector3 normal = Vector3.Cross(side1, side2).normalized;

			if (Vector3.Angle(-Vector3.forward, normal) >= maxAngle) continue;


			DecalPolygon poly = new DecalPolygon(v1, v2, v3);



			poly = DecalPolygon.ClipPolygon(poly, right);
			if (poly == null) continue;
			poly = DecalPolygon.ClipPolygon(poly, left);
			if (poly == null) continue;

			poly = DecalPolygon.ClipPolygon(poly, top);
			if (poly == null) continue;
			poly = DecalPolygon.ClipPolygon(poly, bottom);
			if (poly == null) continue;

			poly = DecalPolygon.ClipPolygon(poly, front);
			if (poly == null) continue;
			poly = DecalPolygon.ClipPolygon(poly, back);
			if (poly == null) continue;

			AddPolygon(poly, normal);
		}

		GenerateTexCoordsAsync(startVertexCount, decal.spriteInfo);
	}

	public static void BuildDecalForObject(Decal decal, GameObject affectedObject) {
		Mesh affectedMesh = affectedObject.GetComponent<MeshFilter>().sharedMesh;
		if (affectedMesh == null) return;

		float maxAngle = decal.maxAngle;

		Plane right = new Plane(Vector3.right, Vector3.right / 2f);
		Plane left = new Plane(-Vector3.right, -Vector3.right / 2f);

		Plane top = new Plane(Vector3.up, Vector3.up / 2f);
		Plane bottom = new Plane(-Vector3.up, -Vector3.up / 2f);

		Plane front = new Plane(Vector3.forward, Vector3.forward / 2f);
		Plane back = new Plane(-Vector3.forward, -Vector3.forward / 2f);

		Vector3[] vertices = affectedMesh.vertices;
		int[] triangles = affectedMesh.triangles;
		int startVertexCount = bufVertices.Count;

		Matrix4x4 matrix = decal.transform.worldToLocalMatrix * affectedObject.transform.localToWorldMatrix;

		for (int i = 0; i < triangles.Length; i += 3) {
			int i1 = triangles[i];
			int i2 = triangles[i + 1];
			int i3 = triangles[i + 2];

			Vector3 v1 = matrix.MultiplyPoint(vertices[i1]);
			Vector3 v2 = matrix.MultiplyPoint(vertices[i2]);
			Vector3 v3 = matrix.MultiplyPoint(vertices[i3]);

			Vector3 side1 = v2 - v1;
			Vector3 side2 = v3 - v1;
			Vector3 normal = Vector3.Cross(side1, side2).normalized;

			if (Vector3.Angle(-Vector3.forward, normal) >= maxAngle) continue;


			DecalPolygon poly = new DecalPolygon(v1, v2, v3);

			poly = DecalPolygon.ClipPolygon(poly, right);
			if (poly == null) continue;
			poly = DecalPolygon.ClipPolygon(poly, left);
			if (poly == null) continue;

			poly = DecalPolygon.ClipPolygon(poly, top);
			if (poly == null) continue;
			poly = DecalPolygon.ClipPolygon(poly, bottom);
			if (poly == null) continue;

			poly = DecalPolygon.ClipPolygon(poly, front);
			if (poly == null) continue;
			poly = DecalPolygon.ClipPolygon(poly, back);
			if (poly == null) continue;

			AddPolygon(poly, normal);
		}

		GenerateTexCoords(startVertexCount, decal.sprite);
	}

	private static void AddPolygon(DecalPolygon poly, Vector3 normal) {
		int ind1 = AddVertex(poly.vertices[0], normal);
		for (int i = 1; i < poly.vertices.Count - 1; i++) {
			int ind2 = AddVertex(poly.vertices[i], normal);
			int ind3 = AddVertex(poly.vertices[i + 1], normal);

			bufIndices.Add(ind1);
			bufIndices.Add(ind2);
			bufIndices.Add(ind3);
		}
	}

	private static int AddVertex(Vector3 vertex, Vector3 normal) {
		int index = FindVertex(vertex);
		if (index == -1) {
			bufVertices.Add(vertex);
			bufNormals.Add(normal);
			index = bufVertices.Count - 1;
		} else {
			Vector3 t = bufNormals[index] + normal;
			bufNormals[index] = t.normalized;
		}
		return (int)index;
	}

	private static int FindVertex(Vector3 vertex) {
		for (int i = 0; i < bufVertices.Count; i++) {
			if (Vector3.Distance(bufVertices[i], vertex) < 0.01f) {
				return i;
			}
		}
		return -1;
	}
	private static void GenerateTexCoordsAsync(int start, SpriteInfo sprite) {
		Rect rect = sprite.rect;
		rect.x /= sprite.width;
		rect.y /= sprite.height;
		rect.width /= sprite.width;
		rect.height /= sprite.height;

		for (int i = start; i < bufVertices.Count; i++) {
			Vector3 vertex = bufVertices[i];

			Vector2 uv = new Vector2(vertex.x + 0.5f, vertex.y + 0.5f);
			uv.x = Mathf.Lerp(rect.xMin, rect.xMax, uv.x);
			uv.y = Mathf.Lerp(rect.yMin, rect.yMax, uv.y);

			bufTexCoords.Add(uv);
		}
	}
	private static void GenerateTexCoords(int start, Sprite sprite) {
		Rect rect = sprite.rect;
		rect.x /= sprite.texture.width;
		rect.y /= sprite.texture.height;
		rect.width /= sprite.texture.width;
		rect.height /= sprite.texture.height;

		for (int i = start; i < bufVertices.Count; i++) {
			Vector3 vertex = bufVertices[i];

			Vector2 uv = new Vector2(vertex.x + 0.5f, vertex.y + 0.5f);
			uv.x = Mathf.Lerp(rect.xMin, rect.xMax, uv.x);
			uv.y = Mathf.Lerp(rect.yMin, rect.yMax, uv.y);

			bufTexCoords.Add(uv);
		}
	}

	public static void Push(float distance) {
		for (int i = 0; i < bufVertices.Count; i++) {
			Vector3 normal = bufNormals[i];
			bufVertices[i] += normal * distance;
		}
	}


	public static Mesh CreateMesh(MeshInfo m) {
		if (bufIndices.Count == 0) {
			return null;
		}
		Mesh mesh = new Mesh();

		mesh.vertices = bufVertices.ToArray();
		mesh.normals = bufNormals.ToArray();
		mesh.uv = bufTexCoords.ToArray();
		mesh.uv2 = bufTexCoords.ToArray();
		mesh.triangles = bufIndices.ToArray();

		bufVertices.Clear();
		bufNormals.Clear();
		bufTexCoords.Clear();
		bufIndices.Clear();

		return mesh;
	}

}
/*
public class DecalBuilder {

	private static List<Vector3> bufVertices = new List<Vector3>();
	private static List<Vector3> bufNormals = new List<Vector3>();
	private static List<Vector2> bufTexCoords = new List<Vector2>();
	private static List<int> bufIndices = new List<int>();


	public static void BuildDecalForObject(Decal decal, ObjectInfo affectedObject, MeshInfo affectedMesh) {
		if (affectedMesh.m == null) return;

		float maxAngle = decal.maxAngle;

		Plane right = new Plane(Vector3.right, Vector3.right / 2f);
		Plane left = new Plane(-Vector3.right, -Vector3.right / 2f);

		Plane top = new Plane(Vector3.up, Vector3.up / 2f);
		Plane bottom = new Plane(-Vector3.up, -Vector3.up / 2f);

		Plane front = new Plane(Vector3.forward, Vector3.forward / 2f);
		Plane back = new Plane(-Vector3.forward, -Vector3.forward / 2f);

		Vector3[] vertices = affectedMesh.verts;
		int[] triangles = affectedMesh.tris;
		int startVertexCount = bufVertices.Count;

		

		Matrix4x4 matrix =  decal.lTWM * affectedObject.lTWM;

		for (int i = 0; i < triangles.Length; i += 3) {
			int i1 = triangles[i];
			int i2 = triangles[i + 1];
			int i3 = triangles[i + 2];

			Vector3 v1 = matrix.MultiplyPoint(vertices[i1]);
			Vector3 v2 = matrix.MultiplyPoint(vertices[i2]);
			Vector3 v3 = matrix.MultiplyPoint(vertices[i3]);

			

			Vector3 side1 = v2 - v1;
			Vector3 side2 = v3 - v1;
			Vector3 normal = Vector3.Cross(side1, side2).normalized;

			if (Vector3.Angle(-Vector3.forward, normal) >= maxAngle) continue;


			DecalPolygon poly = new DecalPolygon(v1, v2, v3);

			

			poly = DecalPolygon.ClipPolygon(poly, right);
			Debug.Log(poly);
			if (poly == null) continue;
			poly = DecalPolygon.ClipPolygon(poly, left);
			Debug.Log(poly);
			if (poly == null) continue;

			poly = DecalPolygon.ClipPolygon(poly, top);
			Debug.Log(poly);
			if (poly == null) continue;
			poly = DecalPolygon.ClipPolygon(poly, bottom);
			Debug.Log(poly);
			if (poly == null) continue;

			poly = DecalPolygon.ClipPolygon(poly, front);
			Debug.Log(poly);
			if (poly == null) continue;
			poly = DecalPolygon.ClipPolygon(poly, back);
			Debug.Log(poly);
			if (poly == null) continue;

			AddPolygon(poly, normal);
		}

		GenerateTexCoords(startVertexCount, decal.spriteInfo);
	}

	private static void AddPolygon(DecalPolygon poly, Vector3 normal) {
		int ind1 = AddVertex(poly.vertices[0], normal);
		for (int i = 1; i < poly.vertices.Count - 1; i++) {
			int ind2 = AddVertex(poly.vertices[i], normal);
			int ind3 = AddVertex(poly.vertices[i + 1], normal);

			bufIndices.Add(ind1);
			bufIndices.Add(ind2);
			bufIndices.Add(ind3);
		}
	}

	private static int AddVertex(Vector3 vertex, Vector3 normal) {
		int index = FindVertex(vertex);
		if (index == -1) {
			bufVertices.Add(vertex);
			bufNormals.Add(normal);
			index = bufVertices.Count - 1;
		} else {
			Vector3 t = bufNormals[index] + normal;
			bufNormals[index] = t.normalized;
		}
		return (int)index;
	}

	private static int FindVertex(Vector3 vertex) {
		for (int i = 0; i < bufVertices.Count; i++) {
			if (Vector3.Distance(bufVertices[i], vertex) < 0.01f) {
				return i;
			}
		}
		return -1;
	}

	private static void GenerateTexCoords(int start, SpriteInfo sprite) {
		Rect rect = sprite.rect;
		rect.x /= sprite.width;
		rect.y /= sprite.height;
		rect.width /= sprite.width;
		rect.height /= sprite.height;

		for (int i = start; i < bufVertices.Count; i++) {
			Vector3 vertex = bufVertices[i];

			Vector2 uv = new Vector2(vertex.x + 0.5f, vertex.y + 0.5f);
			uv.x = Mathf.Lerp(rect.xMin, rect.xMax, uv.x);
			uv.y = Mathf.Lerp(rect.yMin, rect.yMax, uv.y);

			bufTexCoords.Add(uv);
		}
	}

	public static void Push(float distance) {
		for (int i = 0; i < bufVertices.Count; i++) {
			Vector3 normal = bufNormals[i];
			bufVertices[i] += normal * distance;
		}
	}


	public static Mesh CreateMesh() {
		Debug.Assert(bufIndices.Count != 0);
		if (bufIndices.Count == 0) {
			return null;
		}
		Mesh mesh = new Mesh();

		Debug.Assert(bufVertices.Count != 0);

		mesh.vertices = bufVertices.ToArray();
		mesh.normals = bufNormals.ToArray();
		mesh.uv = bufTexCoords.ToArray();
		mesh.uv2 = bufTexCoords.ToArray();
		mesh.triangles = bufIndices.ToArray();

		bufVertices.Clear();
		bufNormals.Clear();
		bufTexCoords.Clear();
		bufIndices.Clear();

		return mesh;
	}

}
*/
