using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class GameViewUtils
{
   

    private static readonly (string label, int width, int height)[] AspectRatioResolutions =
    {
        ("22:9 - 900x2200", 900, 2200),
        ("21:9 - 900x2100", 900, 2100),
        ("20:9 - 900x2000", 900, 2000),
        ("19.5:9 - 900x1950", 900, 1950),
        ("19:9 - 900x1900", 900, 1900),
        ("18.5:9 - 900x1850", 900, 1850),
        ("18:9 - 900x1800", 900, 1800),
        ("16:9 - 1080x1920", 1080, 1920),
        ("5:3 - 900x1500", 900, 1500),
        ("16:10 - 1200x1920", 1200, 1920),
        ("3:2 - 1000x1500", 1000, 1500),
        ("10:7 - 700x1000", 700, 1000),
        ("4:3 - 1200x1600", 1200, 1600),

    };
 

    private static readonly (string label, int width, int height)[] AndroidAllResolutions =
        Concat(
            AspectRatioResolutions 
        );
 

 
 

    [MenuItem("Tools/Game View/Add Android/All Categories")]
    public static void AddAllAndroidPortraitResolutions()
    {
        AddResolutionSet(GameViewSizeGroupType.Android, AndroidAllResolutions, "Android All");
    }

    [MenuItem("Tools/Game View/Remove Android/All Categories")]
    public static void RemoveAllAndroidPortraitResolutions()
    {
        RemoveResolutionSet(GameViewSizeGroupType.Android, AndroidAllResolutions, "Android All");
    }
 
 

    private static void AddResolutionSet(GameViewSizeGroupType groupType, (string label, int width, int height)[] sizes, string groupName)
    {
        int added = 0;
        int skipped = 0;

        foreach (var size in sizes)
        {
            if (HasCustomSize(groupType, size.width, size.height, size.label))
            {
                skipped++;
                continue;
            }

            AddCustomSize(groupType,  size.width, size.height, size.label);
            added++;
        }

        Debug.Log($"GameViewUtils: {groupName} added {added} resolutions, skipped {skipped} existing ones.");
    }

    private static void RemoveResolutionSet(GameViewSizeGroupType groupType, (string label, int width, int height)[] sizes, string groupName)
    {
        int removed = 0;

        for (int i = sizes.Length - 1; i >= 0; i--)
        {
            var size = sizes[i];
            if (RemoveCustomSize(groupType, size.width, size.height, size.label))
            {
                removed++;
            }
        }

        Debug.Log($"GameViewUtils: {groupName} removed {removed} resolutions.");
    }

    private static void AddCustomSize(GameViewSizeGroupType groupType, int width, int height, string text)
    {
        object group = GetGroup(groupType);
        Type gameViewSizeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        Type gameViewSizeEnumType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");

        ConstructorInfo ctor = gameViewSizeType.GetConstructor(new[]
        {
            gameViewSizeEnumType,
            typeof(int),
            typeof(int),
            typeof(string)
        });

        object newSize = ctor.Invoke(new object[]
        {
            1,
            width,
            height,
            text
        });

        MethodInfo addCustomSize = group.GetType().GetMethod("AddCustomSize");
        addCustomSize.Invoke(group, new[] { newSize });

        SaveGameViewSizes();
    }

    private static bool RemoveCustomSize(GameViewSizeGroupType groupType, int width, int height, string text)
    {
        object group = GetGroup(groupType);
        int builtinCount = (int)group.GetType().GetMethod("GetBuiltinCount").Invoke(group, null);
        int totalCount = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        MethodInfo getGameViewSize = group.GetType().GetMethod("GetGameViewSize");
        MethodInfo removeCustomSize = group.GetType().GetMethod("RemoveCustomSize");

        for (int i = totalCount - 1; i >= builtinCount; i--)
        {
            object size = getGameViewSize.Invoke(group, new object[] { i });
            if (SizeMatches(size, width, height, text))
            {
                removeCustomSize.Invoke(group, new object[] { i });
                SaveGameViewSizes();
                return true;
            }
        }

        return false;
    }

    private static bool HasCustomSize(GameViewSizeGroupType groupType, int width, int height, string text)
    {
        object group = GetGroup(groupType);
        int builtinCount = (int)group.GetType().GetMethod("GetBuiltinCount").Invoke(group, null);
        int totalCount = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        MethodInfo getGameViewSize = group.GetType().GetMethod("GetGameViewSize");

        for (int i = builtinCount; i < totalCount; i++)
        {
            object size = getGameViewSize.Invoke(group, new object[] { i });
            if (SizeMatches(size, width, height, text))
            {
                return true;
            }
        }

        return false;
    }

    private static bool SizeMatches(object size, int width, int height, string text)
    {
        if (size == null)
            return false;

        Type sizeType = size.GetType();

        int existingWidth = (int)sizeType.GetProperty("width").GetValue(size, null);
        int existingHeight = (int)sizeType.GetProperty("height").GetValue(size, null);
        string existingText = (string)sizeType.GetProperty("baseText").GetValue(size, null);

        return existingWidth == width && existingHeight == height && existingText == text;
    }

    private static object GetGroup(GameViewSizeGroupType groupType)
    {
        Type sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        Type singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);

        PropertyInfo instanceProp = singleType.GetProperty("instance");
        object instance = instanceProp.GetValue(null, null);

        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        return getGroup.Invoke(instance, new object[] { (int)groupType });
    }

    private static void SaveGameViewSizes()
    {
        Type sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        Type singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);

        PropertyInfo instanceProp = singleType.GetProperty("instance");
        object instance = instanceProp.GetValue(null, null);

        MethodInfo saveToHdd = sizesType.GetMethod("SaveToHDD");
        saveToHdd.Invoke(instance, null);
    }

    private static (string label, int width, int height)[] Concat(
        params (string label, int width, int height)[][] arrays)
    {
        int total = 0;
        for (int i = 0; i < arrays.Length; i++)
            total += arrays[i].Length;

        var result = new (string label, int width, int height)[total];
        int index = 0;

        for (int i = 0; i < arrays.Length; i++)
        {
            for (int j = 0; j < arrays[i].Length; j++)
            {
                result[index++] = arrays[i][j];
            }
        }

        return result;
    }
}
