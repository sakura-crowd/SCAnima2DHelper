using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SakuraCrowdLib
{
    namespace SCAnima2DHelper
    {
        /// <summary>
        /// プロジェクトウィンドウで選択された1個以上の SpriteMesh アセットを現在のシーンに配置します。
        /// </summary>
        public class SCMultiSpriteMeshInstantiate
        {
            /// <summary>
            /// メニュー処理。
            /// </summary>
            [MenuItem("Assets/Create/SCAnima2DHelper/Multi SpriteMesh Instantiate")]
            static void SayHelloFromMenuBar2()
            {
                if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length == 0)
                {
                    return;
                }

                foreach (string _assetGuid in Selection.assetGUIDs)
                {
                    string _assetPath = AssetDatabase.GUIDToAssetPath(_assetGuid);
                    //Debug.Log("assetGUID:" + _assetGuid + ", assetPath" + _assetPath);

                    Anima2D.SpriteMesh _spriteMesh = AssetDatabase.LoadAssetAtPath<Anima2D.SpriteMesh>(_assetPath);    // Object で取得した際にデバッグで型を確認した。
                    if (_spriteMesh == null)
                    {
                        Debug.LogError(string.Format("SCAnima2DHelper: SCMultiSpriteMeshInstantiate.cs :LoadMainAssetAtPath failed. assetGUID = {0}, assetPath = {1}", _assetGuid, _assetPath));
                        return;
                    }
                    Object _spriteMeshInstance = Anima2D.SpriteMeshUtils.CreateSpriteMeshInstance(_spriteMesh, false);
                    if (_spriteMeshInstance == null)
                    {
                        Debug.LogError("SCAnima2DHelper: SCMultiSpriteMeshInstantiate.cs :Instantiate failed.");
                    }
                }
            }
        }
    }
}
