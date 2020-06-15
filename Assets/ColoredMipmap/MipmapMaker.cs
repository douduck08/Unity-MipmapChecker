using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class MipmapMaker : ScriptableObject {

    // [SerializeField] Texture2D target;
    // [SerializeField] List<Texture2D> mipmaps;

    [SerializeField] List<Texture2D> sources;

#if UNITY_EDITOR
    bool ValidateSources () {
        if (sources == null || sources.Count < 1) {
            Debug.Log ("`sources` list is empty");
            return false;
        }

        var width = sources[0].width;
        var height = sources[0].height;
        for (int i = 0; i < sources.Count; i++) {
            if (width != sources[i].width || height != sources[i].height) {
                Debug.LogFormat ("`sources` item {0} has invalidated resolution", i);
                return false;
            }
            width /= 2;
            height /= 2;
        }
        return true;
    }

    string GetFileName () {
        var path = AssetDatabase.GetAssetPath (this);
        return Path.Combine (Path.GetDirectoryName (path), this.name + "_texture.asset");
    }


    [ContextMenu ("Create Texture")]
    void CreateMipMap () {
        if (!ValidateSources ()) {
            return;
        }

        var texture = new Texture2D (sources[0].width, sources[0].height);
        texture.name = sources[0].name;
        texture.SetPixels (sources[0].GetPixels ());
        texture.Apply ();

        for (int i = 1; i < sources.Count; i++) {
            var pixels = sources[i].GetPixels ();
            texture.SetPixels (pixels, i);
        }
        texture.Apply (false, true);

        AssetDatabase.CreateAsset (texture, GetFileName ());
        // AssetDatabase.SaveAssets ();
        // AssetDatabase.Refresh ();
    }

    // void SetupMipMap () {
    //     Debug.Log (target.format);
    //     for (int i = 0; i < mipmaps.Count; i++) {
    //         target.SetPixels (sources[i].GetPixels (), i + 1);
    //     }
    //     target.Apply (false, true);
    // }
#endif
}
