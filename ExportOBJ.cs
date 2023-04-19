using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class ExportOBJ : MonoBehaviour
{
    [MenuItem("Tools/Export OBJ")]
    static void ExportSelectedObjectsToOBJ()
    {
        StringBuilder sb = new StringBuilder();

        string objName = "exported_obj";
        string objPath = EditorUtility.SaveFilePanel("Export OBJ", "", objName, "obj");

        if (string.IsNullOrEmpty(objPath)) return;

        Transform[] transforms = Selection.GetTransforms(SelectionMode.Deep | SelectionMode.ExcludePrefab);

        foreach (Transform t in transforms)
        {
            MeshFilter mf = t.GetComponent<MeshFilter>();
            if (mf == null) continue;

            Mesh mesh = mf.sharedMesh;

            int vertexOffset = sb.Length;

            // Export vertices
            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 transformedVertex = t.TransformPoint(vertex);
                sb.AppendLine($"v {transformedVertex.x} {transformedVertex.y} {transformedVertex.z}");
            }

            // Export normals
            foreach (Vector3 normal in mesh.normals)
            {
                Vector3 transformedNormal = t.TransformDirection(normal).normalized;
                sb.AppendLine($"vn {transformedNormal.x} {transformedNormal.y} {transformedNormal.z}");
            }

            // Export UVs
            foreach (Vector2 uv in mesh.uv)
            {
                sb.AppendLine($"vt {uv.x} {uv.y}");
            }

            // Export triangles
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] triangles = mesh.GetTriangles(i);
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    sb.AppendLine($"f {triangles[j + 2] + 1 + vertexOffset} {triangles[j + 1] + 1 + vertexOffset} {triangles[j] + 1 + vertexOffset}");
                }
            }
        }

        File.WriteAllText(objPath, sb.ToString());
        Debug.Log($"OBJ exported to: {objPath}");
    }
}
