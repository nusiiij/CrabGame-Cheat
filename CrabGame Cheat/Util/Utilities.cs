﻿using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace JNNJMods.CrabGameCheat.Util
{
    public static class Utilities
    {
        /// <summary>
        /// Downloads a File from an URL
        /// </summary>
        /// <param name="fileName">File Location</param>
        /// <param name="url">Download URL</param>
        public static void DownloadFile(string fileName, string url)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, fileName);
            }
        }

        public static string FormatAssemblyVersion(Assembly assembly, bool pretty = false)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetExecutingAssembly();
            }

            string version = assembly.GetName().Version.ToString();
            for (int i = 0; i < 100; i++)
            {
                if (version.EndsWith(".0"))
                    version = version.Trim().Substring(0, version.Length - 2);
                else
                    break;
            }

            //Readd a single ".0" to make it look nicer
            if (pretty && !version.Contains("."))
            {
                version += ".0";
            }

            return version;
        }

        public static byte[] GetResourceBytes(string embeddedResource)
        {
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource))
            {
                byte[] buffer = manifestResourceStream != null ? new byte[(int)manifestResourceStream.Length] :
                    throw new Exception(embeddedResource + " is not found in Embedded Resources.");
                manifestResourceStream.Read(buffer, 0, (int)manifestResourceStream.Length);
                if (buffer.Length > 1000)
                    return buffer;
            }
            return null;
        }

        public static string GetAssemblyLocation()
        {
            return GetAssemblyLocation(Assembly.GetExecutingAssembly());
        }

        public static string GetAssemblyLocation(Assembly assembly)
        {
            return Path.GetDirectoryName(assembly.Location);
        }

    }
}
