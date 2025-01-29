using UnityEngine;

public class TestMatrixTranslate : MonoBehaviour
{
// Translate, rotate and scale a mesh. Try varying
// the parameters in the inspector while running
// to see the effect they have.

    public Vector3 translation;
    private MeshFilter mf;
    private Vector3[] origVerts;
    private Vector3[] newVerts;

    void Start() {
        mf = GetComponent<MeshFilter>();
        origVerts = mf.mesh.vertices;
        newVerts = new Vector3[origVerts.Length];
    }

    void Update() {
        Matrix4x4 m = Matrix4x4.Translate(translation);
        int i = 0;
        while (i < origVerts.Length) {
            newVerts[i] = m.MultiplyPoint3x4(origVerts[i]);
            i++;
        }
        mf.mesh.vertices = newVerts;
    }
}
