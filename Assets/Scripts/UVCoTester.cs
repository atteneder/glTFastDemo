using UnityEngine;

[ExecuteInEditMode]
public class UVCoTester : MonoBehaviour {
	
	[SerializeField]
	Vector2 offset = Vector2.zero;
	
	[SerializeField]
	float rotation = 0;

	[SerializeField]
	Vector2 scale = Vector2.one;

	Material material;

	// Update is called once per frame
	void Update () {

		if(material==null) {
			var r = GetComponent<Renderer>();
			if(r==null) return;
			material = r.sharedMaterial;
		}

		if(material==null) return;

		var sin = Mathf.Sin(rotation);
		var cos = Mathf.Cos(rotation);
		material.mainTextureOffset = new Vector2(
			offset.x + scale.y * sin,
			1 - offset.y - scale.y * cos
		);
		material.mainTextureScale = scale;
		material.SetVector(
			GLTFast.Materials.StandardShaderHelper.mainTexRotatePropId,
			new Vector4(cos,sin,-sin,cos)
			);
	}
}
