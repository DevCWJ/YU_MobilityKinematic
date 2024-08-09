using CWJ.YU.Mobility;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;

namespace CWJ
{
    /// <summary>
    /// CWJ - 24.08.08
    /// <para/>점들을 선으로 이어주기위해 만든 녀석. 캐싱기능도 추가해놓음.
    /// <para/>target지점을 이동하면 점, 선, text 도 함께 움직임.
    /// <para/>target지점의 transform이 비활성화 되면 알아서 사라짐. 
    /// </summary>
    public class LinePoint_Generator : CWJ.Singleton.SingletonBehaviour<LinePoint_Generator>, INeedSceneObj, CWJ.Singleton.IDontAutoCreatedWhenNull
    {
        #region Field
        [VisualizeProperty] public Camera playerCamera { get; set; }
        [VisualizeProperty] public Transform playerCamTrf { get; set; }
        [VisualizeProperty] public Canvas worldCanvas { get; set; }
        [VisualizeProperty] public RectTransform worldCanvasRectTrf { get; set; }

#if UNITY_EDITOR
        [SerializeField] CWJ.Serializable.DictionaryVisualized<int, LinePointIns[]>
#else
        Dictionary<int, LinePointIns[]>
#endif
            _CacheData = 
            
#if UNITY_EDITOR
            new CWJ.Serializable.DictionaryVisualized<int, LinePointIns[]>();
#else
            new Dictionary<int, LinePointIns[]>();
#endif


        [SerializeField] private List<PointData> testPointDatas;

        [SerializeField] GameObject linePrefab;
        [SerializeField] MeshRenderer pointPrefab;
        [SerializeField] TMPro.TextMeshPro prefab_text;


        public float defaultArrowHeight = 0.025f;
        public float defaultArrowWidth = 0.025f;

        [Serializable]
        public struct PointData
        {
            public Transform pointTrf;
            [ColorUsage(true, true)]
            public Color color;

            public PointData(Transform pointTrf, Color color)
            {
                this.pointTrf = pointTrf;
                this.color = color;
            }
        }

        static int GetCacheID(bool isLineLoop, PointData[] _points)
        {
            var instanceIDs = _points.Select(p => p.pointTrf.GetInstanceID()).ToArray();
            int hash = 17;
            hash = hash * 31 + isLineLoop.GetHashCode();
            foreach (int id in instanceIDs)
            {
                hash = hash * 31 + id;
            }
            return hash;
        }


        [Serializable]
        public class LinePointIns
        {
#if UNITY_EDITOR
            [SerializeField] string insName;
#endif
            public bool isLineLoop;
            public Transform fromTrf;
            public Transform toTrf;
            public bool isLineRndrSingle;
            public bool isDotOnly;
            public LineRenderer lineRndr;
            public LineRenderer capRndr;

            public float arrowHeight, arrowWidth;

            public bool isGenerateOrAnimDone;
            public Vector3 lastFromPos;
            public Vector3 lastToPos;

            public LinePointIns(bool isLineLoop, Transform fromTrf, Transform toTrf, bool isLineRndrSingle, LineRenderer lineRndr, LineRenderer capRndr, float arrowWidth, float arrowHeight)
            {
                this.fromTrf = fromTrf;
                this.toTrf = toTrf;
                this.lineRndr = lineRndr;
                this.capRndr = capRndr;
                this.isLineRndrSingle = isLineRndrSingle;
                isDotOnly = fromTrf == toTrf;
                this.isLineLoop = isLineLoop;

                lastFromPos = Vector3.zero;
                lastToPos = Vector3.zero;
                this.arrowWidth = arrowWidth;
                this.arrowHeight = arrowHeight;

                bool isVerified = CheckIsVerified();
                isGenerateOrAnimDone = !isVerified;
#if UNITY_EDITOR
                this.insName = isVerified ? $"{fromTrf.name}->{toTrf.name}" : "VerifiedError";
#endif
            }

            public bool CheckIsVerified()
            {
                if (isDotOnly)
                {
                    return fromTrf.gameObject.activeInHierarchy;
                }
                else
                {
                    if (fromTrf == null || toTrf == null || lineRndr == null)
                        return false;
                    if (!fromTrf.gameObject.activeInHierarchy || !toTrf.gameObject.activeInHierarchy)
                        return false;
                    if (!isLineRndrSingle && capRndr == null)
                        return false;
                }
                return true;
            }

            public bool EqualsCurTrfPos(out Vector3 curFromPos , out Vector3 curToPos)
            {
                curFromPos = fromTrf.position;
                curToPos = toTrf.position;
                if (!curToPos.Equals(lastToPos))
                {
                    return false;
                }
                return isDotOnly || curFromPos.Equals(lastFromPos);
            }
        }

        static MeshRenderer _PointPrefab;
        static string _PointPrefabName;
        static string PointPrefabName
        {
            get
            {
                if (_PointPrefabName == null)
                {
                    _PointPrefab = Instance.pointPrefab;
                    _PointPrefabName = _PointPrefab.name;
                }
                return _PointPrefabName;
            }
        }

        static TextMeshPro _TextPrefab;
        static string _TextPrefabName;
        static string TextPrefabName
        {
            get
            {
                if (_TextPrefabName == null)
                {
                    _TextPrefab = Instance.prefab_text;
                    _TextPrefabName = _TextPrefab.name;
                }
                return _TextPrefabName;
            }
        }

        static string[] _PointChildPrefabNames;
        #endregion Field

        [InvokeButton]
        public static int Generate(PointData[] pointDatas, bool isLineLoop, float animTime, float arrowWidth = -1, float arrowHeight = -1)
        {
            int pointDataLength = pointDatas.Length;
            bool hasAnimation = animTime > 0.0f && pointDataLength > 1;
            isLineLoop = isLineLoop && pointDataLength > 2;
            return _Generate(pointDatas, isLineLoop, hasAnimation, animTime, arrowWidth, arrowHeight);
        }

        [InvokeButton]
        void TestGenerate(bool isLoop, bool hasAnim)
        {
            Generate(testPointDatas.ToArray(), isLoop, hasAnim ? 1 : 0);
        }

        public bool DestroyLineObj(int cacheID)
        {
            if (!_CacheData.TryGetValue(cacheID, out LinePointIns[] insArr))
            {
                return false;
            }
            insArr.ForEach(ins =>
            {
                if (ins.fromTrf != null && ins.fromTrf.childCount > 0)
                {
                    int childCnt = ins.fromTrf.childCount;
                    for (int i = 0; i < childCnt; i++)
                    {
                        var child = ins.fromTrf.GetChild(i);
                        if (child == null)
                        {
                            continue;
                        }
                        string childName = child.gameObject.name;
                        if (_PointChildPrefabNames.IsExists(n => childName.Contains(n)))
                        {
                            Destroy(child.gameObject);
                        }
                    }
                }
                if (ins.lineRndr != null)
                {
                    if (ins.lineRndr.transform.parent == Instance.transform)
                        Destroy(ins.lineRndr.gameObject);
                    else
                        Destroy(ins.lineRndr.transform.parent.gameObject);
                }
            });
            return _CacheData.Remove(cacheID);
        }

        protected override void _Awake()
        {
            willRemoveList = null;
            _PointChildPrefabNames = new string[]
            {
                PointPrefabName,
                TextPrefabName
            };
        }

        List<int> willRemoveList;
        private void LateUpdate()
        {
            if (_CacheData.Count > 0)
            {
                foreach (var kv in _CacheData)
                {
                    foreach (var lpIns in kv.Value)
                    {
                        if (!lpIns.CheckIsVerified())
                        {
                            if (willRemoveList == null)
                                willRemoveList = new List<int>();
                            willRemoveList.Add(kv.Key);
                            break;
                        }

                        if (!lpIns.isGenerateOrAnimDone)
                        {
                            continue;
                        }

                        if (!lpIns.EqualsCurTrfPos(out var curFromPos, out var curToPos))
                        {
                            UpdateLinePos(lpIns, curFromPos, curToPos);
                        }
                    }
                }
            }

            if (willRemoveList != null)
            {
                int[] removeKeys = willRemoveList.ToArray();
                willRemoveList.Clear();
                willRemoveList = null;
                foreach (var key in removeKeys)
                {
                    DestroyLineObj(key);
                }
            }
        }

        #region Generate

        static int _Generate(PointData[] pointDatas, bool isLineLoop, bool hasAnimation, float animTime, float arrowWidth, float arrowHeight)
        {
            var cacheID = GetCacheID(isLineLoop, pointDatas);
            var helper = LinePoint_Generator.Instance;
            arrowWidth = arrowWidth == -1 ? helper.defaultArrowWidth : arrowWidth;
            arrowHeight = arrowHeight == -1 ? helper.defaultArrowHeight : arrowHeight;

            int pointDataLength = pointDatas.Length;

            if (!helper._CacheData.TryGetValue(cacheID, out var instanceList))
            {
                var newInsList = new List<LinePointIns>();

                for (int i = 0; i < pointDataLength; i++)
                {
                    var next = i + 1;
                    if (next >= pointDataLength)
                    {
                        if (isLineLoop)
                        {
                            next = 0;
                        }
                        else
                        {
                            break;
                        }
                    }

                    var fromTrf = pointDatas[i].pointTrf;
                    var toTrf = pointDatas[next].pointTrf;

                    if (fromTrf == null || toTrf == null)
                    {
                        Debug.LogError("LinePoint_Generator.Create : from, to 어쨋음?");
                        continue;
                    }

                    bool isLineSingle = true;
                    LineRenderer lineRenderer = null;
                    LineRenderer capRenderer = null;
                    if (fromTrf != toTrf)
                    {
                        var lrObj = Instantiate(helper.linePrefab, helper.transform);

                        lineRenderer = lrObj.GetComponent<LineRenderer>();

                        if (lineRenderer == null)
                        {
                            lineRenderer = lrObj.transform.GetChild(0).GetComponent<LineRenderer>();
                            capRenderer = lrObj.transform.GetChild(1).GetComponent<LineRenderer>();
                            if (lineRenderer == null)
                            {
                                Debug.LogError("No line renderer found in the arrow object");
                                instanceList[i] = null;
                                continue;
                            }
                            if (capRenderer != null)
                            {
                                isLineSingle = false;
                            }
                        }
                    }

                    var lpIns = new LinePointIns(isLineLoop, fromTrf, toTrf, isLineSingle, lineRenderer, capRenderer, arrowWidth, arrowHeight);
                    if (lpIns.CheckIsVerified() == false)
                    {
#if UNITY_EDITOR
                        Debug.LogError("제작중 문제가 있어 취소");
#endif
                        return -1;
                    }
                    newInsList.Add(lpIns);
                }

                instanceList = newInsList.ToArray();
                helper._CacheData.Add(cacheID, instanceList);
            }

            int insLength = instanceList.Length;

            bool isNeedLastToObj = !isLineLoop && pointDataLength >= 2;
            int lastCreateChildIndex = insLength - 1;

            for (int i = 0; i < insLength; ++i)
            {
                var lpIns = instanceList[i];
                if (lpIns == null)
                {
                    continue;
                }
                lpIns.arrowHeight = arrowHeight;
                lpIns.arrowWidth = arrowWidth;
                if (lpIns.CheckIsVerified())
                {
                    var color = pointDatas[i].color;
                    if (color.a == 0) //실수일것
                        color = new Color(color.r, color.g, color.b, 1f);

                    CreateChildObj(lpIns.fromTrf, arrowWidth, color);

                    if (isNeedLastToObj && i == lastCreateChildIndex)
                        CreateChildObj(lpIns.toTrf, arrowWidth, color);

                    helper.CreateLineAndPoint(lpIns, color, hasAnimation, animTime);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("제작중 문제가 있어 취소");
#endif
                }
            }

            return cacheID;
        }


        static void CreateChildObj(Transform parentTrf, float width, Color color)
        {
            if (parentTrf == null) return;
            string parentTrfName = parentTrf.gameObject.name.Trim();
            Debug.LogError(parentTrfName);

            int childCnt = parentTrf.childCount;

            int pointChildIndex = -1;
            int textChildIndex = -1;

            for (int j = 0; j < childCnt; j++)
            {
                var child = parentTrf.GetChild(j);
                if (child != null)
                {
                    string childName = child.gameObject.name;
                    if (childName.Contains(PointPrefabName))
                        pointChildIndex = j;
                    else if (childName.Contains(TextPrefabName))
                        textChildIndex = j;
                }
            }

            if (!parentTrfName.ToUpper().Contains("[X:P]"))
            {
                MeshRenderer pointObjRndr = null;
                if (pointChildIndex != -1)
                    pointObjRndr = parentTrf.GetChild(pointChildIndex).GetComponent<MeshRenderer>();
                if (pointObjRndr == null)
                {
                    pointObjRndr = Instantiate(_PointPrefab, null);
                    pointObjRndr.transform.localScale = Vector3.one * width * 2;
                    pointObjRndr.transform.SetParent(parentTrf);
                    pointObjRndr.transform.localPosition = Vector3.zero;
                    var pointColor = new Color(color.r, color.g, color.b, 0.9f);
                    pointObjRndr.material.SetColor("_BaseColor", pointColor);
                }
            }

            if (!parentTrfName.ToUpper().Contains("[X:T]"))
            {
                string textContent = parentTrfName;
                Vector3? pivot = null;
                if (parentTrfName.Contains("//"))
                {
                    var splits = parentTrfName.Split("//");
                    textContent = splits[0].Trim();
                    string comment = splits[1].Trim();
                    if (comment.StartsWith("("))
                        pivot = StringUtil.ConvertToVector3(comment);
                }
                TextMeshPro textObj = null;
                if (pointChildIndex != -1)
                    textObj = parentTrf.GetChild(textChildIndex).GetComponent<TextMeshPro>();
                if (textObj == null)
                {
                    textObj = Instantiate(_TextPrefab, null);
                    textObj.transform.localScale = Vector3.one * width * 10;
                    textObj.transform.SetParent(parentTrf);
                    textObj.transform.localRotation = Quaternion.identity;
                    var textColor = new Color(color.r, color.g, color.b, 1);
                    textObj.color = textColor;
                }
                if (pivot != null)
                    textObj.rectTransform.pivot = pivot.Value;
                textObj.transform.localPosition = Vector3.zero;
                textObj.SetText(textContent);
            }
        }
  

        void CreateLineAndPoint(LinePointIns linePointIns, Color color, bool hasAnimation, float animationTime)
        {
            if (linePointIns.isDotOnly)
            {
                return;
            }

            linePointIns.isGenerateOrAnimDone = false;
            
            Vector3 curFromPos = linePointIns.fromTrf.position;
            Vector3 curToPos = linePointIns.toTrf.position;

            var color1 = new Color(color.r, color.g, color.b, 1);
            linePointIns.lastFromPos = curFromPos;
            linePointIns.lastToPos = curToPos;
            if (linePointIns.capRndr != null)
            {
                linePointIns.capRndr.material.color = color1;
                linePointIns.capRndr.material.SetColor("_BaseColor", color1);
                linePointIns.capRndr.startColor = color1;
                linePointIns.capRndr.endColor = color1;
            }
            linePointIns.lineRndr.material.color = color1;
            linePointIns.lineRndr.material.SetColor("_BaseColor", color1);
            linePointIns.lineRndr.startColor = color1;
            linePointIns.lineRndr.endColor = color1;

            if (hasAnimation)
            {
#if UNITY_2023_1_OR_NEWER
                _= _GenerateLinePointWithAnimation(linePointIns, animationTime);
#else
                StartCoroutine(IE_CreateLinePointWithAnimation(linePointIns, animationTime));
#endif
                return;
            }
            else // or immediately
            {
                UpdateLinePos(linePointIns, curFromPos, curToPos);
            }
            return;
        }

        void UpdateLinePos(LinePointIns linePointIns, Vector3 _from, Vector3 _to)
        {
            if (linePointIns.isDotOnly)
            {
                return;
            }

            linePointIns.lastFromPos = _from;
            linePointIns.lastToPos = _to;
            //linePointIns.isGenerateOrAnimDone = false;
            var lineRenderer = linePointIns.lineRndr;
            var capRenderer = linePointIns.capRndr;
            bool single = linePointIns.isLineRndrSingle;
            float arrowHeight = linePointIns.arrowHeight;
            float arrowWidth = linePointIns.arrowWidth;


            var minLen = Math.Sqrt(arrowHeight * arrowHeight + arrowWidth * arrowWidth);

            var distance = Vector3.Distance(_from, _to);

            if (distance < minLen)
            {
                arrowHeight = distance;
            }
            if (distance == 0)
            {
                distance = 0.000001f;
            }
            var pointC = _to + arrowHeight * (_from - _to) / distance;

            var camPoint = (playerCamTrf ?? Camera.main.transform).position;
            var camToFrom = _from - camPoint;
            var camToTo = _to - camPoint;

            var normal = Vector3.Cross(camToFrom, camToTo).normalized;

            var pointD = pointC + (arrowWidth * 1.2f) * normal;
            var pointE = pointC - (arrowWidth * 1.2f) * normal;

            lineRenderer.SetPosition(0, _from);
            lineRenderer.SetPosition(1, pointC);

            if (single)
            {
                lineRenderer.SetPosition(2, pointD);
                lineRenderer.SetPosition(3, _to);
                lineRenderer.SetPosition(4, pointE);
                lineRenderer.SetPosition(5, pointD);
            }
            else
            {
                capRenderer.SetPosition(0, pointD);
                capRenderer.SetPosition(1, _to);
                capRenderer.SetPosition(2, pointE);
                //capRenderer.SetPosition(3, pointD);
            }
            linePointIns.isGenerateOrAnimDone = true;
        }



#if UNITY_2023_1_OR_NEWER
        async Awaitable _GenerateLinePointWithAnimation(LinePointIns linePointIns, float animationTime)
#else
        IEnumerator IE_CreateLinePointWithAnimation(LinePointIns linePointIns, float animationTime)
#endif
        {
            linePointIns.isGenerateOrAnimDone = false;
            Vector3 _from = linePointIns.lastFromPos;
            Vector3 _to = linePointIns.lastToPos;

            var lineRenderer = linePointIns.lineRndr;
            var capRenderer = linePointIns.capRndr;
            bool single = linePointIns.isLineRndrSingle;
            var arrowHeight = linePointIns.arrowHeight;
            var arrowWidth = linePointIns.arrowWidth;

            var maxWidth = 0f;
            foreach (var k in lineRenderer.widthCurve.keys)
            {
                if (k.value > maxWidth)
                {
                    maxWidth = k.value;
                }
            }
            maxWidth *= lineRenderer.widthMultiplier;

            var minLen = Math.Sqrt(arrowHeight * arrowHeight + arrowWidth * arrowWidth);
            var distance = Vector3.Distance(_from, _to);
            if (distance < arrowHeight)
            {
                arrowHeight = distance;
            }
            if (distance == 0)
            {
                distance = 0.000001f;
            }

            var camPoint = (playerCamTrf ?? Camera.main.transform).position;
            var camToFrom = _from - camPoint;
            var camToTo = _to - camPoint;

            var normal = Vector3.Cross(camToFrom, camToTo).normalized;

            Vector3 from() => linePointIns.fromTrf.position;
            Vector3 to() => linePointIns.toTrf.position;

            var totalTime = 0f;
            while (totalTime < animationTime)
            {
                if (!linePointIns.CheckIsVerified())
                {
                    yield break;
                }

                var timeRatio = totalTime / animationTime;

                var len = arrowHeight * timeRatio;

                var pointB2 = from() + timeRatio * (to() - from());
                var pointC = pointB2 + len * (from() - to()) / distance;

                var pointD = pointC + arrowWidth * normal;
                var pointE = pointC - arrowWidth * normal;

                if (Vector3.Distance(from(), pointC) >= maxWidth)
                {
                    lineRenderer.SetPosition(0, from());
                    lineRenderer.SetPosition(1, pointC);
                    if (single)
                    {
                        lineRenderer.SetPosition(2, pointD);
                        lineRenderer.SetPosition(3, pointB2);
                        lineRenderer.SetPosition(4, pointE);
                        lineRenderer.SetPosition(5, pointD);
                    }
                    else
                    {
                        capRenderer.SetPosition(0, pointD);
                        capRenderer.SetPosition(1, pointB2);
                        capRenderer.SetPosition(2, pointE);
                    }
                }

                totalTime += Time.deltaTime;
#if UNITY_2023_1_OR_NEWER
                await Awaitable.NextFrameAsync();
#else
                yield return null;
#endif
            }

            linePointIns.isGenerateOrAnimDone = true;
        }
        #endregion Generate
    }
}