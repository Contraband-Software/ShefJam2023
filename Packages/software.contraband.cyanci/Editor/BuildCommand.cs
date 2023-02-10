using UnityEditor;
using UnityEngine;

using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#pragma warning disable IDE0079

namespace CyanCI
{
    public class BuildCommand : MonoBehaviour
    {
        #region CLIENT
        private static void RunBatchCommand(string cmd)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + cmd;
            process.StartInfo = startInfo;
            process.Start();
        }

        [MenuItem("CyanCI/Debug/Client/Build Current", priority = 2)]
        public static void BuildThis()
        {
            RunBatchCommand(
                "cd \"" + Application.dataPath + "\\.. \"" +
                " && git add ." +
                " && git commit --allow-empty -m \"[Build]\"" +
                " && git push"
            );
        }

        [MenuItem("CyanCI/Build Last Commit", priority = 1)]
        public static void InvokeCICommit()
        {
            RunBatchCommand(
                "cd \"" + Application.dataPath + "\\.. \"" +
                " && git commit --allow-empty -m \"[Build]\"" +
                " && git push"
            );
        }
        #endregion

        #region SERVER_ONLY
        static BuildPlayerOptions GetBuildPlayerOptions(
            bool askForLocation = false,
            BuildPlayerOptions defaultOptions = new BuildPlayerOptions())
        {
            // Get static internal "GetBuildPlayerOptionsInternal" method
#pragma warning disable S3011
            MethodInfo method = typeof(BuildPlayerWindow.DefaultBuildMethods).GetMethod(
                "GetBuildPlayerOptionsInternal",
                BindingFlags.NonPublic | BindingFlags.Static
            );
#pragma warning restore S3011

            // invoke internal method
            return (BuildPlayerOptions)method.Invoke(
                null,
                new object[] { askForLocation, defaultOptions }
            );
        }

        [MenuItem("CyanCI/Debug/Server/Build Current Windows Config", priority = 3)]
        public static void BuildWin32()
        {
            BuildPlayerOptions options = GetBuildPlayerOptions();

            string[] args = Environment.GetCommandLineArgs();

            string buildPath = "";

            for (uint i = 0; i < args.Length; i++)
            {
                if (args[i] == "-CyanCIBuildPath")
                {
                    buildPath = args[i + 1];
                }
            }

            //If no path is set or if the directory doesnt exist, build to the project's "Build" directory
            if (buildPath.Length == 0 || !Directory.Exists(buildPath))
            {
                buildPath = Application.dataPath + "/../Build";
                Directory.CreateDirectory(buildPath);
            }

            options.locationPathName = buildPath + '/' + GetNewDirectoryNumber(buildPath) + "/" + Application.productName + ".exe";

            var report = BuildPipeline.BuildPlayer(options);
            print("Build Result: " + report.summary.result.ToString());

            if (
                args.ToList<string>().IndexOf("-quit") == -1 &&
                args.ToList<string>().IndexOf("-batchMode") != -1
            )
            {
                EditorApplication.Exit(0);
            }
        }

        private static string GetNewDirectoryNumber(string path)
        {
            List<string> directories;
            try
            {
                directories = Directory.GetDirectories(path).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                return "!";
            }

            if (directories.Count != 0)
            {
                List<int> dirIDs = new List<int>();

                foreach (string dir in directories)
                {
                    dirIDs.Add(int.Parse(Path.GetFileName(dir)));
                }

                Debug.Log(dirIDs[0].ToString());

                return (dirIDs.Max() + 1).ToString();
            }
            else
            {
                return "1";
            }
        }
        #endregion
    }
}