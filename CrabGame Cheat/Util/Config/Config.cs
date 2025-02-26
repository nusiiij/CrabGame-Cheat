﻿using JNNJMods.CrabGameCheat.Modules;
using JNNJMods.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace JNNJMods.CrabGameCheat.Util
{
    public class Config
    {

        public static Config Instance { get; private set; }

        public List<ModuleBase> Modules { get; private set; }

        public KeyCode ClickGuiKeyBind = KeyCode.RightShift;

        public void ExecuteForModules(Action<ModuleBase> action)
        {
            foreach (ModuleBase module in Modules)
            {
                action.Invoke(module);
            }
        }

        public T GetModule<T>() where T : ModuleBase
        {
            return Modules.OfType<T>().FirstOrDefault();
        }

        private List<ModuleBase> GetModules(ClickGUI gui)
        {
            List<ModuleBase> modules = new List<ModuleBase>(CheatModuleAttribute.InstantiateAll(gui));

            return modules;
        }

        public Config(ClickGUI gui)
        {
            Instance = this;
            Modules = GetModules(gui);
        }

        public void WriteToFile(string file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file));

            File.WriteAllText(file, ToJson());
        }

        public static Config FromFile(string file, ClickGUI gui)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file));

            if (!File.Exists(file))
            {
                return CreateFile(file, gui);
            }

            try
            {
                return FromJson(File.ReadAllText(file), gui);
            } catch(JsonException)
            {
                File.Delete(file);
                return CreateFile(file, gui);
            }
        }

        private static Config CreateFile(string file, ClickGUI gui)
        {
            File.CreateText(file);

            Config config = new Config(gui);
            config.WriteToFile(file);

            return config;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static Config FromJson(string json, ClickGUI gui)
        {
            try
            {
                Config instance = JsonConvert.DeserializeObject<Config>(json);

                if(instance.Modules.Count <= 0 || instance.Modules.Count != CheatModuleAttribute.GetAllModules().Length)
                {
                    instance.Modules = instance.GetModules(gui);
                }

                return Instance = instance;
            } catch(Exception)
            {
                return new Config(gui);
            }
        }
    }
}
