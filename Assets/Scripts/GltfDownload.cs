// using UnityEngine;
// using UnityEngine.Networking; 
// using System.Collections;
// using UnityEngine.Assertions;
// using Unity.Collections;
// using Unity.Collections.LowLevel.Unsafe;
// using System.Collections.Generic;

// public class GltfDownload : MonoBehaviour
// {
//     public string url;
//     // Start is called before the first frame update
//     IEnumerator Start()
//     {
//         var buf = new byte[1<<20];
//         var dl = new UnityWebRequest(url);
//         dl.downloadHandler = new GltfDownloadHandler(buf);
//         yield return dl.SendWebRequest();

//         if(dl.isNetworkError || dl.isHttpError) {
//             Debug.LogError(dl.error);
//         } else {
//             // Show results as text
//             Debug.Log(dl.downloadHandler.text);
 
//             // Or retrieve results as binary data
//             byte[] results = dl.downloadHandler.data;
//         }
//     }
// }

// public class GltfDownloadHandler : DownloadHandlerScript {

//     const uint GLB_MAGIC = 0x46546c67;

//     enum ChunkFormat : uint {
//         JSON = 0x4e4f534a,
//         BIN = 0x004e4942
//     }

//     ulong contentLength;
//     ulong loaded;
//     bool isBinary;
//     bool loadingJson;
//     ulong nextChunk;
//     string json;
//     List<NativeArray<byte>> binChunks;
//     int currChunkIndex;
//     uint currChunkLength;

//     public GltfDownloadHandler(): base() {}

//     public GltfDownloadHandler(byte[] buffer): base(buffer) {}

//     protected override byte[] GetData() { return null; }

//     protected override bool ReceiveData(byte[] data, int dataLength) {
//         if(data == null || data.Length < 1) {
//             Debug.LogError("LoggingDownloadHandler :: ReceiveData - received a null/empty buffer");
//             return false;
//         }

//         Debug.Log(string.Format("LoggingDownloadHandler :: ReceiveData - received {0} bytes", dataLength));

//         ulong index = loaded;

//         if(loaded<=0) {
//             uint magic = System.BitConverter.ToUInt32( data, 0 );
//             isBinary = magic == GLB_MAGIC;

//             if(isBinary) {
//                 uint version = System.BitConverter.ToUInt32( data, 4 );
//                 if (version!=2) {
//                     Debug.LogErrorFormat("Unsupported glTF version {0}",version);
//                     return false;
//                 }
//                 uint length = System.BitConverter.ToUInt32( data, 8 );
//                 if(length != contentLength) {
//                     Debug.LogErrorFormat("Length mismatch {0} != {1}",length,contentLength);
//                 }
//                 nextChunk = 12;
//                 index = 12;
//             }
//         }

//         if(isBinary) {
//             while(index<(loaded+(ulong)dataLength)) {
//                 if(index==nextChunk) {
//                     currChunkLength = System.BitConverter.ToUInt32( data, (int)(index-loaded) );
//                     index += 4;
//                     uint chType = System.BitConverter.ToUInt32( data, (int)(index-loaded) );
//                     index += 4;
//                     nextChunk += currChunkLength + 8;

//                     if (chType == (uint)ChunkFormat.BIN) {
//                         Assert.IsFalse(loadingJson);
//                         var arr = new NativeArray<byte>((int)currChunkLength,Allocator.Persistent,NativeArrayOptions.UninitializedMemory);
//                         currChunkIndex = binChunks.Count;
//                         binChunks.Add(arr);
//                     }
//                     // else if (chType == (uint)ChunkFormat.JSON) {
//                     // }
//                 }
//                 if(loadingJson) {
//                     if ( currChunkLength < (dataLength-index) ) {
//                         json = System.Text.Encoding.UTF8.GetString(data, index, (int)currChunkLength );
//                         index += (int)currChunkLength;
//                     } else {
//                         // TODO: zizerlweis
//                     }
//                 } else {
//                     if ( currChunkLength < (dataLength-index) ) {
//                         Copy(data,arr,index,currChunkLength);
//                     } else {
//                         Copy(data,arr,index,dataLength-index);
//                         // TODO: zizerlweis
//                     }
//                 }
//             }
//         } else {

//         }

//         loaded += (ulong) dataLength;
//         nextChunk -= dataLength;

//         return true;
//     }

//     unsafe void Copy(byte[] src, NativeArray<byte> dest, int startIndex, long length) {
//         var destPtr = NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(dest);
//         fixed( void* srcPtr = &(src[startIndex]) ) {
//             System.Buffer.MemoryCopy(
//                 srcPtr,
//                 destPtr,
//                 dest.Length,
//                 length
//             );
//         }
//     }

//     // Called when all data has been received from the server and delivered via ReceiveData.
    
//     protected override void CompleteContent() {
//         Debug.Log("LoggingDownloadHandler :: CompleteContent - DOWNLOAD COMPLETE!");
//     }

//     protected override void ReceiveContentLengthHeader(ulong contentLength) {
//         // base.ReceiveContentLengthHeader(contentLength);
//         Debug.Log(string.Format("LoggingDownloadHandler :: ReceiveContentLength - length {0}", contentLength));
//         this.contentLength = contentLength;
//         loaded = 0;
//         binChunks = new List<NativeArray<byte>>(4);
//         loadingJson = true;
//         currChunkIndex = 0;
//     }
// }
