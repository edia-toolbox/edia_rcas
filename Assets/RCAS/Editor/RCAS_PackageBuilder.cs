using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEditor;

namespace RCAS_Editor
{
    public class RCAS_PackageBuilder
    {
        static PackRequest Request;

        [MenuItem("eDIA/RCAS/Build Tarballs", false, -10)]
        public static void BuildPackage()
        {
            BuildTarball("com.rcas.rcas");
        }

        static void BuildTarball(string source)
        {
            source = "Packages/" + source;
            string destination = Application.dataPath + "/../tarballs";

            Debug.Log("Started building " + source + "...");
            Request = Client.Pack(source, destination);
            EditorApplication.update += Progress;
        }

        static void Progress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                    Debug.Log("Tarball created: " + Request.Result.tarballPath);
                else if (Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                EditorApplication.update -= Progress;
            }
        }
    }
}