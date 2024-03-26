using MessagePack;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AssetStudio
{
    public static class ResourceMap
    {
        private static AssetMap Instance = new() { GameType = GameType.Normal, AssetEntries = new List<AssetEntry>() };
        public static List<AssetEntry> GetEntries() => Instance.AssetEntries;
        public static void FromFile(string path, Game game)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Logger.Info(string.Format("Parsing...."));
                try
                {
                    if (path.EndsWith(".json")) {
                        using var stream = File.OpenRead(path);
                        using var file = new StreamReader(stream);
                        using var reader = new JsonTextReader(file);

                        var serializer = new JsonSerializer() { Formatting = Newtonsoft.Json.Formatting.Indented };
                        serializer.Converters.Add(new StringEnumConverter());

                        List<AssetEntry> entries = serializer.Deserialize<List<AssetEntry>>(reader);
                        Instance = new() { GameType = game.Type, AssetEntries = entries };
                    } else {
                        using var stream = File.OpenRead(path);
                        Instance = MessagePackSerializer.Deserialize<AssetMap>(stream, MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray));
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("AssetMap was not loaded");
                    Console.WriteLine(e.ToString());
                    return;
                }
                Logger.Info("Loaded !!");
            }
        }

        public static void Clear()
        {
            Instance.GameType = GameType.Normal;
            Instance.AssetEntries = new List<AssetEntry>();
        }
    }
}
