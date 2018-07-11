using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SakuraCrowdLib
{
    /// <summary>
    /// Anima2D のエディタ編集を補助するエディタ拡張の名前空間です。
    /// </summary>
    namespace SCAnima2DHelper
    {
        /// <summary>
        /// シーンに配置されている名前が一致する SpriteMeshInstance を指定された座標に配置します。
        /// トリミングデータの１行目の名前の SpriteMeshInstance が一番手前に表示されます。
        /// </summary>
        public class SCSpriteMeshInstanceLayout : EditorWindow
        {
            /// <summary>
            /// メニュー処理。
            /// ウィンドウを表示します。
            /// </summary>
            [MenuItem("GameObject/SCAnima2DHelper/SpriteMeshInstance Layout")]
            static void SayHelloFromMenuBar()
            {
                SCSpriteMeshInstanceLayout _editorWindow = UnityEditor.EditorWindow.GetWindow<SCSpriteMeshInstanceLayout>();
                _editorWindow.Show();

                return;
            }

            string textPartsPlicTrimInfo_;
            /// <summary>
            /// 画像の PixelPerUnit です。一致しない場合、配置がずれます。
            /// </summary>
            int intPixelPerUnit_ = 64;
            /// <summary>
            /// 行内のパラメータの区切り文字(例えば, や半角スペースや \t)を指定します。
            /// </summary>
            string textSeparator_ = "\\t";

            /// <summary>
            /// ウィンドウの GUI 処理。
            /// </summary>
            private void OnGUI()
            {

                // テキスト情報の設定
                GUILayout.Label("パーツの位置情報を設定してください。");
                GUILayout.Label("(座標はピクセル単位、手前のパーツから順に1行ずつ)");
                GUILayout.Label("SpriteMeshName Left Top Right Bottom Width Height");
                textPartsPlicTrimInfo_ = EditorGUILayout.TextArea(textPartsPlicTrimInfo_, GUILayout.Height(16 * 15), GUILayout.MaxHeight(16 * 15));

                // PixelPerUnit 設定
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("PixelPerUnit");
                intPixelPerUnit_ = EditorGUILayout.IntField(intPixelPerUnit_, GUILayout.Width(16 * 5));
                GUILayout.FlexibleSpace();  // 左寄せ
                EditorGUILayout.EndHorizontal();

                // 分割文字設定
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("SeparatorChar");
                textSeparator_ = EditorGUILayout.TextField(textSeparator_, GUILayout.Width(16 * 2));
                GUILayout.Label("(エスケープ文字は \\t のみ対応)");
                GUILayout.FlexibleSpace();  // 左寄せ
                EditorGUILayout.EndHorizontal();

                // Layout ボタン
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();  // 中央寄せのための左側スペース
                                            // ボタン(フォントサイズ変更)
                int _prevButtonFontSize = GUI.skin.button.fontSize;
                GUI.skin.button.fontSize = 35;
                bool _buttonLayout = GUILayout.Button("Layout!", GUILayout.Width(16 * 15), GUILayout.Height(16 * 5));
                GUI.skin.button.fontSize = _prevButtonFontSize;
                GUILayout.FlexibleSpace();  // 中央寄せのための右側スペース
                EditorGUILayout.EndHorizontal();

                if (_buttonLayout == true)
                {
                    bool _flgCall = true;

                    if (intPixelPerUnit_ < 0)
                    {
                        Debug.LogError(string.Format("SCAnima2DHelper: SCSpriteMeshInstanceLayout.cs :PixelPerUnit の値が 0 未満です。"));
                        _flgCall = false;
                    }

                    char charSeparator_ = '\0';
                    // テキストに入力できない特定のエスケープシーケンスは、TryParse では2文字扱いで失敗するため、手動で変換する。
                    if (textSeparator_ == "\\t")
                    {
                        charSeparator_ = '\t';
                    }
                    if (charSeparator_ == '\0') // まだ変換されていない場合は TryParse で変換を試みる。
                    {
                        if (char.TryParse(textSeparator_, out charSeparator_) == false)
                        {
                            Debug.LogError(string.Format("SCAnima2DHelper: SCSpriteMeshInstanceLayout.cs :セパレータを char に変換できませんでした。", textSeparator_));
                            _flgCall = false;
                        }
                    }

                    if (_flgCall == true)
                    {
                        LayoutSpriteMeshes(textPartsPlicTrimInfo_, intPixelPerUnit_, charSeparator_);
                    }

                }
            }

            /// <summary>
            /// トリミングデータ1行から1個の SpriteMesh の配置を行います。
            /// </summary>
            /// <param name="_textPartsPlicTrimInfo">SpriteMesh 1個分のトリミングデータ(1行)</param>
            /// <param name="_pixelPerUnit">画像の PixelPerUnit</param>
            /// <param name="_separator">区切り文字</param>
            static void LayoutSpriteMeshes(string _textPartsPlicTrimInfo, int _pixelPerUnit, char _separator)
            {
                int _cntLine = 0;
                StringReader _stringReader = new StringReader(_textPartsPlicTrimInfo);
                while (_stringReader.Peek() > -1)
                {
                    ++_cntLine;
                    string _strLine = _stringReader.ReadLine();
                    string[] _paramList = _strLine.Split(_separator);

                    if (_paramList.Length < 7)
                    {
                        Debug.LogError(string.Format("SCAnima2DHelper: SCSpriteMeshInstanceLayout.cs :{0}行目のパラメータの個数が少ないです。処理を中断します。", _cntLine));
                        return;
                    }

                    int _paramIndex = 0;
                    float _left, _top, _width, _height = 0f;

                    // left
                    _paramIndex = 1;
                    if (float.TryParse(_paramList[_paramIndex], out _left) == false)
                    {
                        Debug.LogError(string.Format("SCAnima2DHelper: SCSpriteMeshInstanceLayout.cs :{0}行目の{1}番目の文字列{2}を数値に変換できませんでした。処理を中断します。", _cntLine, _paramIndex + 1, _paramList[_paramIndex]));
                        return;
                    }

                    // top
                    _paramIndex = 2;
                    if (float.TryParse(_paramList[_paramIndex], out _top) == false)
                    {
                        Debug.LogError(string.Format("SCAnima2DHelper: SCSpriteMeshInstanceLayout.cs :{0}行目の{1}番目の文字列{2}を数値に変換できませんでした。処理を中断します。", _cntLine, _paramIndex + 1, _paramList[_paramIndex]));
                        return;
                    }

                    // width
                    _paramIndex = 5;
                    if (float.TryParse(_paramList[_paramIndex], out _width) == false)
                    {
                        Debug.LogError(string.Format("SCAnima2DHelper: SCSpriteMeshInstanceLayout.cs :{0}行目の{1}番目の文字列{2}を数値に変換できませんでした。処理を中断します。", _cntLine, _paramIndex + 1, _paramList[_paramIndex]));
                        return;
                    }

                    // height
                    _paramIndex = 6;
                    if (float.TryParse(_paramList[_paramIndex], out _height) == false)
                    {
                        Debug.LogError(string.Format("SCAnima2DHelper: SCSpriteMeshInstanceLayout.cs :{0}行目の{1}番目の文字列{2}を数値に変換できませんでした。処理を中断します。", _cntLine, _paramIndex + 1, _paramList[_paramIndex]));
                        return;
                    }

                    // オブジェクトを再配置
                    LayoutSpriteMesh(
                        _paramList[0],
                        int.Parse(_paramList[1]),
                        int.Parse(_paramList[2]),
                        int.Parse(_paramList[5]),
                        int.Parse(_paramList[6]),
                        -_cntLine,
                        _pixelPerUnit
                        );
                }

                if (_cntLine == 0)
                {
                    Debug.LogError(string.Format("SCAnima2DHelper: SCSpriteMeshInstanceLayout.cs :有効なデータがありませんでした。"));
                }

            }

            /// <summary>
            /// 指定された位置に SpriteMesh を配置し、表示順を設定します。
            /// </summary>
            /// <param name="_name">シーンの SpriteMeshInstance の名前</param>
            /// <param name="_left">ピクセル単位の座標</param>
            /// <param name="_top">ピクセル単位の座標</param>
            /// <param name="_width">ピクセル単位の幅</param>
            /// <param name="_height">ピクセル単位の幅</param>
            /// <param name="_sortingOrder">表示順。大きいほうが手前に表示されます。</param>
            /// <param name="_pixelPerUnit">画像の PixelPerUnit</param>
            static void LayoutSpriteMesh(string _name, float _left, float _top, float _width, float _height, int _sortingOrder, int _pixelPerUnit)
            {
                GameObject _go = GameObject.Find(_name);
                if (_go == null)
                {
                    Debug.LogError(string.Format("SCAnima2DHelper: SCSpriteMeshInstanceLayout.cs :{0} GameObject が見つかりませんでした。", _name));
                    return;
                }
                _go.transform.position = new Vector2((_left + _width / 2f) / (float)_pixelPerUnit, -(_top + _height / 2f) / (float)_pixelPerUnit);
                Anima2D.SpriteMeshInstance _spriteMeshInstance = _go.GetComponent<Anima2D.SpriteMeshInstance>();
                if (_spriteMeshInstance == null)
                {
                    Debug.LogError(string.Format("SCAnima2DHelper: SCSpriteMeshInstanceLayout.cs :{0} に Anima2D.SpriteMeshInstance コンポーネントがありません。", _name));
                }
                else
                {
                    _spriteMeshInstance.sortingOrder = _sortingOrder;
                }
            }
        }
    }
}
