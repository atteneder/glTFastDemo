using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class MeshTest : MonoBehaviour
{
    public Transform[] bones;
    public Mesh mesh;
#if UNITY_EDITOR
    // Vertex with FP32 position, FP16 2D normal and a 4-byte tangent.
    // In some cases StructLayout attribute needs
    // to be used, to get the data layout match exactly what it needs to be.
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct ExampleVertex
    {
        public Vector3 pos;
        public Vector3 normal;

        // bone weight
        public float bw0;
        // bone index
        public uint bi0;
    }

    // [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    // struct ExampleVertex2
    // {
    //     public float m0;
    //     public float m1;
    //     public float m2;
    //     public float m3;
    //     public float m4;
    //     public float m5;

    //     // bone index
    //     public uint bi0;

    //     // bone weight
    //     public float bw0;
    // }

    void Start() {
        DoStuff();
    }

    unsafe void DoStuff() {
        
        mesh = new Mesh();
        mesh.subMeshCount = 1;

        // specify vertex count and layout
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.BlendWeight, VertexAttributeFormat.Float32, 1),
            new VertexAttributeDescriptor(VertexAttribute.BlendIndices, VertexAttributeFormat.UInt32, 1),
        };
        var vertexCount = 4;
        mesh.SetVertexBufferParams(vertexCount, layout);

        // set vertex data
        // var verts = new NativeArray<ExampleVertex2>(vertexCount, Allocator.Temp);

        // verts[0] = new ExampleVertex2() {m0=-1,m1=0,m2=0,m3=0,m4=1,m5=0, bi0=0, bi1=1, bi2=2, bi3=0, bw0=1f, bw1=0f, bw2=0f, bw3=0f };
        // verts[1] = new ExampleVertex2() {m0=0,m1=0,m2=1,m3=0,m4=1,m5=0, bi0=1, bi1=1, bi2=2, bi3=0, bw0=1f, bw1=0f, bw2=0f, bw3=0f };
        // verts[2] = new ExampleVertex2() {m0=1,m1=0,m2=0,m3=0,m4=1,m5=0, bi0=2, bi1=1, bi2=2, bi3=0, bw0=1f, bw1=0f, bw2=0f, bw3=0f };


        // var ptr = NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(verts);
        // var verts2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ExampleVertex2>(ptr,3,Allocator.None);
        // var sh = AtomicSafetyHandle.Create();
        // NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref verts2, sh);

        // mesh.SetVertexBufferData(verts2, 0, 0, vertexCount);


        var verts = new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);

        verts[0] = new ExampleVertex() { pos=new Vector3(-1,0,0), normal=Vector3.back, bi0=0, bw0=1f };
        verts[1] = new ExampleVertex() { pos=new Vector3(-1,2,0), normal=Vector3.back, bi0=1, bw0=1f };
        verts[2] = new ExampleVertex() { pos=new Vector3(1,2,0),  normal=Vector3.back, bi0=1, bw0=1f };
        verts[3] = new ExampleVertex() { pos=new Vector3(1,0,0),  normal=Vector3.back, bi0=0, bw0=1f };

        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);

        int indC = 6;
        var ind = new NativeArray<System.UInt16>(indC,Allocator.Temp);

        ind[0] = 0;
        ind[1] = 1;
        ind[2] = 2;
        ind[3] = 0;
        ind[4] = 2;
        ind[5] = 3;

        mesh.SetIndexBufferParams(indC,IndexFormat.UInt16);
        mesh.SetIndexBufferData(ind,0,0,indC);
        mesh.SetSubMesh(0,new SubMeshDescriptor(0,indC));

        var posesMalone = new Matrix4x4[bones.Length];
        for (int i = 0; i < bones.Length; i++) {
            var bone = bones[i];
            posesMalone[i] = bone.worldToLocalMatrix * transform.localToWorldMatrix;
        }
        mesh.bindposes = posesMalone;

        mesh.UploadMeshData(false);

        // AtomicSafetyHandle.Release(sh);
        verts.Dispose();
        ind.Dispose();

        // var mf = GetComponent<MeshFilter>();
        // mf.mesh = mesh;

        var smr = GetComponent<SkinnedMeshRenderer>();
        smr.bones = bones;
        smr.sharedMesh = mesh;
    }
#endif
}
