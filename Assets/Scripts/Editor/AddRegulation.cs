using System.IO;
using System.Linq;
using AssetRegulationManager.Editor.Core.Data;
using AssetRegulationManager.Editor.Core.Model.AssetRegulations;
using AssetRegulationManager.Editor.Core.Model.AssetRegulations.AssetConstraintImpl;
using AssetRegulationManager.Editor.Core.Model.AssetRegulations.AssetFilterImpl;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

public static class AddRegulation
{
    // レギュレーション追加のイメージ
    [MenuItem("Assets/Polka/AddRegulation")]
    private static void AddSelectObjRegulation()
    {
        var obj = Selection.activeObject;
        var texture = obj as Texture;
        var path = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;

        var preset = new Preset(importer);
        var presetPath = Path.GetDirectoryName(path) + "/" + texture + "_Preset.preset";
        AssetDatabase.CreateAsset(preset, presetPath);

        var presetType = preset.GetPresetType();

        if (presetType.IsValid())
        {
            var tmp = Preset.GetDefaultPresetsForType(presetType).ToList();
            tmp.Add(new DefaultPreset("", preset));
            Preset.SetDefaultPresetsForType(presetType, tmp.ToArray());
        }

        AddAssetRegulation();
        ;
    }

    private static void AddAssetRegulation()
    {
        var regulationGUID = AssetDatabase.FindAssets("t:AssetRegulationSetStore").Single();

        var assetRegulationSetStore = AssetDatabase.LoadAssetAtPath<AssetRegulationSetStore>(AssetDatabase.GUIDToAssetPath(regulationGUID));

        // Create and add a regulation.
        var regulation = new AssetRegulation();
        assetRegulationSetStore.Set.Add(regulation);

        // Create targets.
        var assetGroup = regulation.AddAssetGroup();
        var regexBasedAssetFilter = assetGroup.AddFilter<RegexBasedAssetFilter>();
        regexBasedAssetFilter.AssetPathRegex.IsListMode = true;
        regexBasedAssetFilter.AssetPathRegex.AddValue("Assets/Texture");
        regexBasedAssetFilter.MatchWithFolders = true;

        var maxTextureSizeConstraint = regulation.AddConstraint<MaxTextureSizeConstraint>();
        maxTextureSizeConstraint.MaxSize = new Vector2(500, 500);

        EditorUtility.SetDirty(assetRegulationSetStore);
        AssetDatabase.SaveAssets();
    }
}
