#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;

public class PostBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log( report.summary.outputPath );
        Debug.Log(Path.GetDirectoryName(report.summary.outputPath) + @"\MediaInfo.dll");
        var path = Path.GetDirectoryName(report.summary.outputPath) + @"\MediaInfo.dll";
        
        File.Copy(Application.dataPath + "/Add to build/MediaInfo.dll",path,true);
    }
}
#endif
