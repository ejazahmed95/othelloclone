using System.Collections.Generic;
using UnityEngine;
using RangerRPG.Utility;

namespace RangerRPG.Core {
    public enum LogLevel {
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warn = 4,
        Error = 5,
        Fatal = 6,
    }

    public struct TagAttributes {
        public Color Colour;
        public bool Bold;
        public bool Italic;

        public TagAttributes(bool bold = false, bool italic = false){
            Colour = Color.white;
            Bold = bold;
            Italic = italic;
        }
        public TagAttributes(Color color, bool bold = false, bool italic = false){
            Colour = color;
            Bold = bold;
            Italic = italic;
        }
    }
    public static class Log {
        private static LogLevel _level = LogLevel.Debug;

        private static Dictionary<string, TagAttributes> _attributes = new Dictionary<string, TagAttributes>{
            {"EventRaised", new TagAttributes(Color.red, true)},
            {"EventListener", new TagAttributes(Color.cyan, true)}
        };

        public static void AddTagAttribute(string key, TagAttributes tagAttributes) {
            _attributes.Add(key, tagAttributes);
        }
        
        public static void SetLogLevel(LogLevel l) {
            _level = l;
        }
		
        public static void Trace(string message, string tag = "[Default]") {
            if (_level > LogLevel.Trace) return;
            UnityEngine.Debug.Log($"[T] [{tag}]: {message}");
        }
		
        public static void Debug(string message, string tag = "[Default]") {
            if (_level > LogLevel.Debug) return;
            UnityEngine.Debug.Log($"<color=green>[DEBUG] [{tag}]: {message}</color>");
        }
		
        public static void Info(string message, string tag = "Default") {
            if (_level > LogLevel.Info) return;
            UnityEngine.Debug.Log($"[INFO] {ApplyFormatting(tag, LogLevel.Info, message)}");
        }
		
        public static void Warn(string message, string tag = "[Default]") {
            if (_level > LogLevel.Warn) return;
            UnityEngine.Debug.LogWarning($"[WARN] [{tag}]: {message}");
        }
        public static void Err(string message, string tag = "[Default]") {
            if (_level > LogLevel.Error) return;
            UnityEngine.Debug.LogError($"[ERR] [{tag}]: {message}");
        }
        public static void Fatal(string tag, string message) {
            // if (level > LogLevel.INFO) return;
            UnityEngine.Debug.LogError($"[Fatal] [{tag}]: {message}");
        }

        private static string ApplyFormatting(string tag, LogLevel level, string message){
            var formattedString = $"[{tag}] {message}";
            if (!_attributes.TryGetValue(tag, out TagAttributes result)){
                return formattedString;
            }

            formattedString = formattedString.Color(result.Colour);
            if (result.Bold) formattedString = formattedString.Bold();
            if (result.Italic) formattedString = formattedString.Italic();
            return formattedString;
        }
        
        private static string GetColorForTag(string tag, LogLevel level){
            if (_attributes.TryGetValue(tag, out var result)){
                return $"#{ColorUtility.ToHtmlStringRGB(result.Colour)}";
            }

            if (level == LogLevel.Debug){
                return Color.yellow.ToString();
            } else if (level == LogLevel.Info){
                return Color.cyan.ToString();
            }

            return "white";
        }
    }
}