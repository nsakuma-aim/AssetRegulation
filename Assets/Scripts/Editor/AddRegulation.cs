using System.Linq;
using AssetRegulationManager.Editor.Core.Data;
using AssetRegulationManager.Editor.Core.Model.AssetRegulations;
using AssetRegulationManager.Editor.Core.Model.AssetRegulations.AssetConstraintImpl;
using AssetRegulationManager.Editor.Core.Model.AssetRegulations.AssetFilterImpl;
using UnityEditor;

public static class AddRegulation
{
    // レギュレーション追加のイメージ
    [MenuItem("Assets/Polka/AddRegulation")]
    private static void AddSelectObjRegulation()
    {
        var obj = Selection.activeObject;

        var regulationGUID = AssetDatabase.FindAssets("t:AssetRegulationSetStore").Single();

        var assetRegulationSetStore = AssetDatabase.LoadAssetAtPath<AssetRegulationSetStore>(AssetDatabase.GUIDToAssetPath(regulationGUID));

        // Create and add a regulation.
        var regulation = new AssetRegulation();
        assetRegulationSetStore.Set.Add(regulation);

        // Create targets.
        var assetGroup = regulation.AddAssetGroup();
        var extensionFilter = assetGroup.AddFilter<ExtensionBasedAssetFilter>();
        extensionFilter.Extension.IsListMode = true;
        extensionFilter.Extension.AddValue("png");
        extensionFilter.Extension.AddValue("jpg");

        // Create constraints.
        var fileSizeConstraint = regulation.AddConstraint<FileSizeConstraint>();
        fileSizeConstraint.Unit = FileSizeConstraint.SizeUnit.KB;
        fileSizeConstraint.MaxSize = 50;

        EditorUtility.SetDirty(assetRegulationSetStore);
        AssetDatabase.SaveAssets();
    }
}
