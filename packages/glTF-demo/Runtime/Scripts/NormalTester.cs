// Copyright 2020-2022 Andreas Atteneder
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using UnityEngine;

[ExecuteInEditMode]
public class NormalTester : MonoBehaviour {
	
	public bool showNormals = true;
	public Color color = new Color(0,1,0);
	public float length = 0.1f;
	public bool showTangents = false;
	public Color tangentsColor = new Color(1,0,1);
	private Mesh mesh;
	
	// Use this for initialization
	void Start () {
		MeshFilter mf = GetComponent<MeshFilter>();
		if(mf!=null) {
			mesh = mf.sharedMesh;
		}
		else {
			var smr = GetComponent<SkinnedMeshRenderer>();
			if (smr != null) {
				mesh = smr.sharedMesh;
			}
		}

		if (mesh != null) {
			Debug.Log("Mesh "+mesh.name+ " "+mesh.vertices.Length+" "+mesh.normals.Length+" "+mesh.tangents.Length);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(mesh==null) return;
		var tmpVerts = mesh.vertices;
		var tmpNorms = mesh.normals;
		var tmpTans = mesh.tangents;
		Matrix4x4 mat = transform.localToWorldMatrix; // Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);// 
		var rot = transform.rotation;
		for(int i=0;i<tmpVerts.Length;i++) {
			Vector3 start = tmpVerts[i];
			start = mat.MultiplyPoint3x4(start);
			Vector3 end;
			
			if(showNormals && tmpNorms!=null && tmpNorms.Length>0 ) {
				// end = tmpVerts[i] + tmpNorms[i]*length;
				// end = mat.MultiplyPoint3x4(end);
				end = start + (rot * tmpNorms[i])*length;
				UnityEngine.Debug.DrawLine( start, end, color);
			}
			
			if(showTangents && tmpTans!=null && tmpTans.Length>0 ) {
				Vector4 t = tmpTans[i];
				end = mat.MultiplyPoint3x4( tmpVerts[i] + (new Vector3(t.x,t.y,t.z)*(t.w*length) ) );
				end = start + (rot * new Vector3(t.x,t.y,t.z) )*(t.w*length);
				UnityEngine.Debug.DrawLine( start, end, tangentsColor);
			}
			
		}
	}
}
