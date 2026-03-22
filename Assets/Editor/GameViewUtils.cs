using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class GameViewUtils
{
    private static readonly (string label, int width, int height)[] IOSPortraitResolutions =
    {
        ("01 - 640x1136", 640, 1136),
        ("02 - 750x1334", 750, 1334),
        ("03 - 768x1024", 768, 1024),
        ("04 - 828x1792", 828, 1792),
        ("05 - 1080x1920", 1080, 1920),
        ("06 - 1080x2340", 1080, 2340),
        ("07 - 1125x2436", 1125, 2436),
        ("08 - 1170x2532", 1170, 2532),
        ("09 - 1179x2556", 1179, 2556),
        ("10 - 1242x2688", 1242, 2688),
        ("11 - 1284x2778", 1284, 2778),
        ("12 - 1290x2796", 1290, 2796),
        ("13 - 1488x2266", 1488, 2266),
        ("14 - 1536x2048", 1536, 2048),
        ("15 - 1620x2160", 1620, 2160),
        ("16 - 1640x2360", 1640, 2360),
        ("17 - 1668x2224", 1668, 2224),
        ("18 - 1668x2388", 1668, 2388),
        ("19 - 2048x2732", 2048, 2732),
    };

    // Android categories
    private static readonly (string label, int width, int height)[] AndroidBudgetResolutions =
    {
        ("A01 - 720x1600", 720, 1600),
        ("A02 - 1080x2340", 1080, 2340),
        ("A03 - 1080x2388", 1080, 2388),
        ("A04 - 1080x2424", 1080, 2424),
        ("A05 - 1440x3120", 1440, 3120),
        ("B01 - 720x1600", 720, 1600),
        ("B02 - 720x1612", 720, 1612),
        ("B03 - 720x1650", 720, 1650),
        ("B04 - 720x1680", 720, 1680),
    };

    private static readonly (string label, int width, int height)[] AndroidMidrangeResolutions =
    {
        ("M01 - 1080x2340", 1080, 2340),
        ("M02 - 1080x2400", 1080, 2400),
        ("M03 - 1080x2388", 1080, 2388),
        ("M04 - 1080x2412", 1080, 2412),
    };

    private static readonly (string label, int width, int height)[] AndroidFlagshipResolutions =
    {
        ("F01 - 1080x2424", 1080, 2424),
        ("F02 - 1260x2800", 1260, 2800),
        ("F03 - 1440x3088", 1440, 3088),
        ("F04 - 1440x3120", 1440, 3120),
    };

    private static readonly (string label, int width, int height)[] AndroidFoldableResolutions =
    {
        ("D01 - Fold Cover 904x2316", 904, 2316),
        ("D02 - Fold Cover 968x2376", 968, 2376),
        ("D03 - Fold Inner 1812x2176", 1812, 2176),
        ("D04 - Fold Inner 1840x2208", 1840, 2208),
    };

    private static readonly (string label, int width, int height)[] AndroidAllResolutions =
        Concat(
            AndroidBudgetResolutions,
            AndroidMidrangeResolutions,
            AndroidFlagshipResolutions,
            AndroidFoldableResolutions
        );

    [MenuItem("Tools/Game View/Add iOS/19 Portrait Resolutions")]
    public static void AddAllIOSPortraitResolutions()
    {
        AddResolutionSet(GameViewSizeGroupType.Android, IOSPortraitResolutions, "iOS");
    }

    [MenuItem("Tools/Game View/Remove iOS/19 Portrait Resolutions")]
    public static void RemoveAllIOSPortraitResolutions()
    {
        RemoveResolutionSet(GameViewSizeGroupType.Android, IOSPortraitResolutions, "iOS");
    }

    [MenuItem("Tools/Game View/Add Android/Budget")]
    public static void AddAndroidBudgetResolutions()
    {
        AddResolutionSet(GameViewSizeGroupType.Android, AndroidBudgetResolutions, "Android Budget");
    }

    [MenuItem("Tools/Game View/Remove Android/Budget")]
    public static void RemoveAndroidBudgetResolutions()
    {
        RemoveResolutionSet(GameViewSizeGroupType.Android, AndroidBudgetResolutions, "Android Budget");
    }

    [MenuItem("Tools/Game View/Add Android/Midrange")]
    public static void AddAndroidMidrangeResolutions()
    {
        AddResolutionSet(GameViewSizeGroupType.Android, AndroidMidrangeResolutions, "Android Midrange");
    }

    [MenuItem("Tools/Game View/Remove Android/Midrange")]
    public static void RemoveAndroidMidrangeResolutions()
    {
        RemoveResolutionSet(GameViewSizeGroupType.Android, AndroidMidrangeResolutions, "Android Midrange");
    }

    [MenuItem("Tools/Game View/Add Android/Flagship")]
    public static void AddAndroidFlagshipResolutions()
    {
        AddResolutionSet(GameViewSizeGroupType.Android, AndroidFlagshipResolutions, "Android Flagship");
    }

    [MenuItem("Tools/Game View/Remove Android/Flagship")]
    public static void RemoveAndroidFlagshipResolutions()
    {
        RemoveResolutionSet(GameViewSizeGroupType.Android, AndroidFlagshipResolutions, "Android Flagship");
    }

    [MenuItem("Tools/Game View/Add Android/Foldable")]
    public static void AddAndroidFoldableResolutions()
    {
        AddResolutionSet(GameViewSizeGroupType.Android, AndroidFoldableResolutions, "Android Foldable");
    }

    [MenuItem("Tools/Game View/Remove Android/Foldable")]
    public static void RemoveAndroidFoldableResolutions()
    {
        RemoveResolutionSet(GameViewSizeGroupType.Android, AndroidFoldableResolutions, "Android Foldable");
    }

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

    [MenuItem("Tools/Game View/Add All/iOS + Android")]
    public static void AddAllPresets()
    {
        AddAllIOSPortraitResolutions();
        AddAllAndroidPortraitResolutions();
    }

    [MenuItem("Tools/Game View/Remove All/iOS + Android")]
    public static void RemoveAllPresets()
    {
        RemoveAllIOSPortraitResolutions();
        RemoveAllAndroidPortraitResolutions();
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
