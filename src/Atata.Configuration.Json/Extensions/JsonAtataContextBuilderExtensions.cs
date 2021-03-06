﻿using System;
using System.IO;
using System.Reflection;
using Atata.Configuration.Json;
using Newtonsoft.Json;

namespace Atata
{
    public static class JsonAtataContextBuilderExtensions
    {
        private const string DefaultConfigFileName = "Atata";

        private const string DefaultConfigFileExtension = ".json";

        public static AtataContextBuilder ApplyJsonConfig(this AtataContextBuilder builder, string filePath = null, string environmentAlias = null)
        {
            return builder.ApplyJsonConfig<JsonConfig>(filePath, environmentAlias);
        }

        public static AtataContextBuilder ApplyJsonConfig<TConfig>(this AtataContextBuilder builder, string filePath = null, string environmentAlias = null)
            where TConfig : JsonConfig<TConfig>, new()
        {
            string completeFilePath = BuildCompleteFilePath(filePath, environmentAlias);

            string jsonContent = File.ReadAllText(completeFilePath);

            PropertyInfo currentConfigProperty = GetCurrentConfigProperty<TConfig>();

            TConfig config = currentConfigProperty.GetValue(null, null) as TConfig ?? new TConfig();
            JsonConvert.PopulateObject(jsonContent, config);

            AtataContextBuilder resultBuilder = JsonConfigMapper.Map(config, builder);

            currentConfigProperty.SetValue(null, config, null);

            return resultBuilder;
        }

        private static string BuildCompleteFilePath(string filePath, string environmentAlias)
        {
            string completeFilePath = null;
            string environmentAliasInsertion = string.IsNullOrWhiteSpace(environmentAlias) ? null : $".{environmentAlias}";

            if (string.IsNullOrWhiteSpace(filePath))
            {
                completeFilePath = $"{DefaultConfigFileName}{environmentAliasInsertion}{DefaultConfigFileExtension}";
            }
            else
            {
                if (filePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    completeFilePath = $"{filePath}{DefaultConfigFileName}{environmentAliasInsertion}{DefaultConfigFileExtension}";
                }
                else if (Path.HasExtension(filePath))
                {
                    if (environmentAliasInsertion == null)
                        completeFilePath = filePath;
                    else
                        completeFilePath = $"{Path.GetFileNameWithoutExtension(filePath)}{environmentAliasInsertion}{Path.GetExtension(filePath)}";
                }
                else
                {
                    completeFilePath = $"{filePath}{environmentAliasInsertion}{DefaultConfigFileExtension}";
                }
            }

            if (!Path.IsPathRooted(completeFilePath))
                completeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, completeFilePath);

            return completeFilePath;
        }

        private static PropertyInfo GetCurrentConfigProperty<TConfig>()
            where TConfig : JsonConfig<TConfig>
        {
            Type type = typeof(TConfig);
            string currentPropertyName = nameof(JsonConfig.Current);

            PropertyInfo property = type.GetProperty(currentPropertyName, BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return property ?? throw new MissingMemberException(type.FullName, currentPropertyName);
        }
    }
}
