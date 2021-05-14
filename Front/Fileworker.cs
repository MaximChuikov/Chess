using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using Newtonsoft.Json;
using System.IO;

namespace ChessWPF
{
    public enum TypeOfData
    {
        FigureStyle,
    }
    static class Fileworker
    { 
        private static string directory;
        private static string pathFStyle;
        public static void ShowMessage(string mess)
        {
            MessageBox.Show(mess);
        }
        public static void SaveData(TypeOfData data, object value)
        {
            CheckPaths();
            string jsonPath;
            switch (data)
            {
                case TypeOfData.FigureStyle:
                    jsonPath = pathFStyle;
                    break;
                default:
                    throw new Exception("You tricked enums");
            }
            using(var toWrite = new StreamWriter(jsonPath))
            {
                var json = JsonConvert.SerializeObject(value);
                toWrite.Write(json);
            }
        }
        public static void DeleteData(TypeOfData data)
        {
            CheckPaths();
            string jsonPath;
            switch (data)
            {
                case TypeOfData.FigureStyle:
                    jsonPath = pathFStyle;
                    break;
                default:
                    throw new Exception();
            }
            File.Delete(jsonPath);
        }
        public static int GetStyle()
        {
            CheckPaths();
            string json;
            using (var reader = new StreamReader(pathFStyle))
                json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<int>(json);
        }
        public static void SetPaths(string pathDirectory)
        {
            directory = pathDirectory + "\\SaveStyles";
            Directory.CreateDirectory(directory);
            pathFStyle = directory + "\\FigureStyle";

            if (!File.Exists(pathFStyle))
                SaveData(TypeOfData.FigureStyle, 0);
        }
        private static void CheckPaths()
        {
            if (directory == null)
                SetPaths(directory + pathFStyle);
        }
    }
}
