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
    /// CWJ
    /// <para/>
    /// 점을 선으로 이어주는 녀석. 캐싱기능도 추가해놓음
    /// </summary>
    public class LinePointGenerator : CWJ.Singleton.SingletonBehaviour<LinePointGenerator>, INeedSceneObj, CWJ.Singleton.IDontAutoCreatedWhenNull
    {
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
            public bool isVerified;
            public bool isLineLoop;
            public Transform fromTrf;
            public Transform toTrf;
            public bool isLineRndrSingle;
            public LineRenderer lineRndr;
            public LineRenderer capRndr;

            public float arrowHeight, arrowWidth;

            public bool isGenerateOrAnimDone;
            public Vector3 lastFromPos;
            public Vector3 lastToPos;

            public LinePointIns(bool isLineLoop, Transform fromTrf, Transform toTrf, bool isLineRndrSingle, LineRenderer lineRndr, LineRenderer capRndr, float arrowHeight, float arrowWidth)
            {
                this.fromTrf = fromTrf;
                this.toTrf = toTrf;
                this.lineRndr = lineRndr;
                this.capRndr = capRndr;
                this.isLineRndrSingle = isLineRndrSingle;

                this.isVerified = true;
                this.isVerified = CheckIsVerified();
                isGenerateOrAnimDone = !isVerified;
#if UNITY_EDITOR
                this.insName = isVerified ? $"{fromTrf.name}->{toTrf.name}" : "VerifiedError";
#endif
                this.isLineLoop = isLineLoop;

                lastFromPos = Vector3.zero;
                lastToPos = Vector3.zero;
                this.arrowHeight = arrowHeight;
                this.arrowWidth = arrowWidth;
            }

            public bool CheckIsVerified()
            {
                if (!isVerified)
                {
                    return false;
                }
                if (fromTrf == null || toTrf == null || lineRndr == null)
                {
                    return false;
                }
                if (!fromTrf.gameObject.activeInHierarchy || !toTrf.gameObject.activeInHierarchy)
                {
                    return false;
                }
                return true;
            }

            public bool EqualsCurTrfPos(out Vector3 curFromPos , out Vector3 curToPos)
            {
                curFromPos = fromTrf.position;
                curToPos = toTrf.position;
                return (curFromPos.Equals(lastFromPos) && curToPos.Equals(lastToPos));
            }
        }

        public void SetCamera(Camera camera)
        {
            targetCameraTrf = (camera == null) ? Camera.main.transform : camera.transform;
        }

        public void SetCanvas(Canvas canvas) { }

        [SerializeField] private List<PointData> testPointDatas;

        public Transform targetCameraTrf;
        [SerializeField] GameObject linePrefab;
        [SerializeField] MeshRenderer pointPrefab;
        [SerializeField] TMPro.TextMeshPro prefab_text;

        string[] pointChildPrefabNames;

        public float arrowHeight = 0.25f;
        public float arrowWidth = 0.25f;
        public float animationTime = 1.0f;

        /// <summary>
        /// 포인트지점 Transform은 모두 활성화 상태여야함
        /// </summary>
        /// <param name="pointDatas"></param>
        /// <param name="isLineLoop"></param>
        /// <param name="hasAnimation"></param>
        /// <param name="arrowHeight"></param>
        /// <param name="arrowWidth"></param>
        /// <param name="animTime"></param>
        /// <returns></returns>
        [InvokeButton]
        public static int Create(PointData[] pointDatas, bool isLineLoop, bool hasAnimation, float arrowHeight = -1, float arrowWidth=-1, float animTime=-1)
        {
            var cacheID = GetCacheID(isLineLoop, pointDatas);
            var helper = LinePointGenerator.Instance;
            arrowHeight = arrowHeight == -1 ? helper.arrowHeight : arrowHeight;
            arrowWidth = arrowWidth == -1 ? helper.arrowWidth : arrowWidth;

            if (!helper._CacheData.TryGetValue(cacheID, out var instanceList))
            {
                var newInsList = new List<LinePointIns>();

                for (int i = 0; i < pointDatas.Length; i++)
                {
                    var next = i + 1;
                    if (next >= pointDatas.Length)
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
                        Debug.LogError("LinePointGenerator.Create : from to 어쨋음?");
                        continue;
                    }

                    var color = pointDatas[i].color;
                    if (color.a == 0) //실수일것
                        color = new Color(color.r, color.g, color.b, 1f);

                    MeshRenderer pointObjRndr = Instantiate(helper.pointPrefab);
                    pointObjRndr.transform.SetParent(null, true);
                    pointObjRndr.transform.localScale = Vector3.one * arrowWidth * 2;
                    pointObjRndr.transform.SetParent(fromTrf, true);
                    pointObjRndr.transform.localPosition = Vector3.zero;
                    var pointColor = new Color(color.r, color.g, color.b, 0.9f);
                    pointObjRndr.material.SetColor("_BaseColor", pointColor);

                    TextMeshPro text = Instantiate(helper.prefab_text);
                    text.transform.SetParent(null, true);
                    text.transform.localScale = Vector3.one * arrowWidth * 10;
                    text.transform.SetParent(fromTrf, true);
                    text.transform.localPosition = Vector3.zero;
                    text.transform.localRotation = Quaternion.identity;
                    text.SetText(fromTrf.gameObject.name);
                    var textColor = new Color(color.r, color.g, color.b, 1);
                    text.color = textColor;

                    var lrObj = Instantiate(helper.linePrefab, helper.transform);

                    bool isSingle = true;
                    LineRenderer lineRenderer = lrObj.GetComponent<LineRenderer>();
                    LineRenderer capRenderer = null;

                    if (lineRenderer == null)
                    {
                        lineRenderer = lrObj.transform.GetChild(0).GetComponent<LineRenderer>();
                        capRenderer = lrObj.transform.GetChild(1).GetComponent<LineRenderer>();
                        if (lineRenderer == null)
                        {
                            Debug.LogError("No line renderer found in the arrow object");
                            instanceList[i] = default(LinePointIns);
                            continue;
                        }
                        if (capRenderer != null)
                        {
                            isSingle = false;
                        }
                    }

                    var lpIns = new LinePointIns( isLineLoop, fromTrf, toTrf, isSingle, lineRenderer, capRenderer, arrowHeight, arrowWidth);
                    if (!lpIns.isVerified)
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

            float animationTime = animTime == -1 ? helper.animationTime : animTime;

            for (int i = 0; i < instanceList.Length; ++i)
            {
                var lpIns = instanceList[i];
                lpIns.arrowHeight = arrowHeight;
                lpIns.arrowWidth = arrowWidth;
                if (lpIns.CheckIsVerified())
                {
                    helper.GenerateLineAndPoint(lpIns, pointDatas[i].color, hasAnimation, animationTime);
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

        [InvokeButton]
        void TestCreate(bool isLoop, bool hasAnim)
        {
            Create(testPointDatas.ToArray(), isLoop, hasAnim);
        }

        List<int> willRemoveList = null;

        protected override void _Awake()
        {
            pointChildPrefabNames = new string[]
            {
                pointPrefab.gameObject.name,
                prefab_text.gameObject.name
            };
        }

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

        public void DestroyLineObj(int cacheID)
        {
            if (!_CacheData.TryGetValue(cacheID, out var insArr))
            {
                return;
            }
            insArr.Do(ins =>
            {
                if (ins.fromTrf != null && ins.fromTrf.childCount > 0)
                {
                    for (int i = 0; i < pointChildPrefabNames.Length; i++)
                    {
                        var child = ins.fromTrf.Find(pointChildPrefabNames[i]);
                        Destroy(child);
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
            _CacheData.Remove(cacheID);
        }

        void GenerateLineAndPoint(LinePointIns linePointIns, Color color, bool hasAnimation, float animationTime)
        {
            linePointIns.isGenerateOrAnimDone = false;
            
            Vector3 curFromPos = linePointIns.fromTrf.position;
            Vector3 curToPos = linePointIns.toTrf.position;

            var color1 = new Color(color.r, color.g, color.b, 1);
            linePointIns.lastFromPos = curFromPos;
            linePointIns.lastToPos = curToPos;
            if (linePointIns.capRndr != null)
            {
                linePointIns.capRndr.material.SetColor("_BaseColor", color1);
                linePointIns.capRndr.startColor = color1;
                linePointIns.capRndr.endColor = color1;
            }
            linePointIns.lineRndr.material.SetColor("_BaseColor", color1);
            linePointIns.lineRndr.startColor = color1;
            linePointIns.lineRndr.endColor = color1;

            if (hasAnimation)
            {
#if UNITY_2023_1_OR_NEWER
                _= _GenerateLinePointWithAnimation(linePointIns, animationTime);
#else
                StartCoroutine(IE_GenerateLinePointWithAnimation(linePointIns, animationTime));
#endif
                return;
            }
            else // or immediately
            {
                UpdateLinePos(linePointIns, curFromPos, curToPos);
            }
            return;
        }

        void UpdateLinePos(LinePointIns linePointIns, Vector3 from, Vector3 to)
        {
            linePointIns.lastFromPos = from;
            linePointIns.lastToPos = to;
            //linePointIns.isGenerateOrAnimDone = false;
            var lineRenderer = linePointIns.lineRndr;
            var capRenderer = linePointIns.capRndr;
            bool single = linePointIns.isLineRndrSingle;
            float arrowHeight = linePointIns.arrowHeight;
            float arrowWidth = linePointIns.arrowWidth;

            var distance = Vector3.Distance(from, to);
            var minLen = Math.Sqrt(arrowHeight * arrowHeight + arrowWidth * arrowWidth);

            if (distance < minLen)
            {
                arrowHeight = distance;
            }

            var pointC = to + arrowHeight * (from - to) / distance;

            var camPoint = (Instance.targetCameraTrf ?? Camera.main.transform).position;
            var s1 = from - camPoint;
            var s2 = to - camPoint;

            var normal = Vector3.Cross(s1, s2).normalized;

            var pointD = pointC + (arrowWidth * 1.2f) * normal;
            var pointE = pointC - (arrowWidth * 1.2f) * normal;

            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, pointC);

            if (single)
            {
                lineRenderer.SetPosition(2, pointD);
                lineRenderer.SetPosition(3, to);
                lineRenderer.SetPosition(4, pointE);
                lineRenderer.SetPosition(5, pointD);
            }
            else
            {
                capRenderer.SetPosition(0, pointD);
                capRenderer.SetPosition(1, to);
                capRenderer.SetPosition(2, pointE);
                //capRenderer.SetPosition(3, pointD);
            }
            linePointIns.isGenerateOrAnimDone = true;
        }



#if UNITY_2023_1_OR_NEWER
        async Awaitable _GenerateLinePointWithAnimation(LinePointIns linePointIns, float animationTime)
#else
        IEnumerator IE_GenerateLinePointWithAnimation(LinePointIns linePointIns, float animationTime)
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

            var dir = _to - _from;
            var camPoint = (Instance.targetCameraTrf ?? Camera.main.transform).position;
            var s1 = _from - camPoint;
            var s2 = _to - camPoint;

            var normal = Vector3.Cross(s1, s2).normalized;

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
    }
}