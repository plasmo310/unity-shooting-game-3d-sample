using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

namespace Editor
{
    /// <summary>
    /// ビルド後処理
    /// </summary>
    public class PostBuildProcessor : IPostprocessBuildWithReport
    {
        /// <summary>
        /// 実行順
        /// </summary>
        public int callbackOrder => 100;

        public void OnPostprocessBuild(BuildReport report)
        {
#if UNITY_IOS
            // plistの読込
            var plist = new PlistDocument();
            var plistPath = report.summary.outputPath + "/Info.plist";
            plist.ReadFromString(File.ReadAllText(plistPath));
            
            // NSUserTrackingUsageDescription の追加
            var plistRoot = plist.root;
            plistRoot.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");
            File.WriteAllText(plistPath, plist.WriteToString());
            
            // pbxプロジェクトの読込
            var project = new PBXProject();
            var projectPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
            project.ReadFromString(File.ReadAllText(projectPath));
            
            // UnityFramework に AuthenticationServices.framework を追加
            var targetGuid = project.GetUnityFrameworkTargetGuid();
            project.AddFrameworkToProject(targetGuid, "AuthenticationServices.framework", false);
            File.WriteAllText(projectPath, project.WriteToString());
#endif
        }
    }
}
