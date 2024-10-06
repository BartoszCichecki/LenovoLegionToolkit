﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct RegistryKeyValuePackageRule : IPackageRule
{
    private string Key { get; init; }
    private string KeyName { get; init; }
    private Version Version { get; init; }

    public static bool TryCreate(XmlNode? node, out RegistryKeyValuePackageRule value)
    {
        var key = node?.SelectSingleNode("Key")?.InnerText;
        var keyName = node?.SelectSingleNode("KeyName")?.InnerText;
        var versionString = node?.SelectSingleNode("Version")?.InnerText;


        Version? version = null;
        if (Version.TryParse(RemoveNonVersionCharacters(versionString), out var v))
            version = v;

        if (key is null || keyName is null || version is null)
        {
            value = default;
            return false;
        }

        value = new RegistryKeyValuePackageRule { Key = key, KeyName = keyName, Version = version };
        return true;
    }

    public Task<bool> CheckDependenciesSatisfiedAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3)
    {
        var hive = Key.Split('\\').FirstOrDefault();
        var path = string.Join('\\', Key.Split('\\').Skip(1));

        if (hive is null || string.IsNullOrEmpty(path))
            return Task.FromResult(false);

        var keyExists = Registry.ValueExists(hive, path, KeyName);
        if (!keyExists)
            return Task.FromResult(true);

        var versionString = Registry.GetValue(hive, path, KeyName, string.Empty);

        if (!Version.TryParse(versionString, out var version))
            return Task.FromResult(false);

        var result = Version <= version;
        return Task.FromResult(result);
    }

    public Task<bool> DetectInstallNeededAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3)
    {
        var hive = Key.Split('\\').FirstOrDefault();
        var path = string.Join('\\', Key.Split('\\').Skip(1));

        if (hive is null || string.IsNullOrEmpty(path))
            return Task.FromResult(false);

        var keyExists = Registry.ValueExists(hive, path, KeyName);
        if (!keyExists)
            return Task.FromResult(false);

        var versionString = Registry.GetValue(hive, path, KeyName, string.Empty);

        if (versionString.Any(char.IsWhiteSpace))
            versionString = versionString.Split(null).FirstOrDefault(versionString);

        if (!Version.TryParse(versionString, out var version))
            return Task.FromResult(false);

        var result = Version > version;
        return Task.FromResult(result);
    }

    private static string RemoveNonVersionCharacters(string? versionString)
    {
        var arr = versionString?.ToCharArray() ?? [];
        arr = Array.FindAll(arr, c => char.IsDigit(c) || c == '.');
        return new string(arr);
    }
}
