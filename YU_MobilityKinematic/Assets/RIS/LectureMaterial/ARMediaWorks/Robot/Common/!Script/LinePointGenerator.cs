using CWJ.YU.Mobility;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CWJ
{
  

    public class LinePointGenerator : CWJ.Singleton.SingletonBehaviour<LinePointGenerator>, INeedSceneObj, CWJ.Singleton.IDontAutoCreatedWhenNull
    {
        //public CWJ.Serializable.DictionaryVisualized<int, LinePointIns[]> _Cache;
        static Dictionary<int, LinePointIns[]> _Cache = new Dictionary<int, LinePointIns[]>();

#if UNITY_EDITOR
        [SerializeField] List<VisualizeCache> _VisualizeCaches = new List<VisualizeCache>();
        [Serializable]
        public struct VisualizeCache
        {
            public Transform[] points;
            public LineRenderer[] LineRenderers;
            public LineRenderer[] capRenderers;

            public VisualizeCache(LinePointIns[] linePointIns)
            {
                points = new Transform[linePointIns.Length];
                LineRenderers = new LineRenderer[linePointIns.Length];
                capRenderers = new LineRenderer[linePointIns.Length];
                int i = 0;
                foreach (var item in linePointIns)
                {
                    points[i] = item.fromTrf;
                    LineRenderers[i] = item.lineRndr;
                    capRenderers[i] = item.capRndr;
                    i++;
                }

            }
        }
#endif

        [Serializable]
        public struct PointData
        {
            public Transform pointTrf;
            [ColorUsage(true, true)]
            public Color color;
        }

        static int GetCacheKey(bool isLineLoop, PointData[] _points)
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
        public struct LinePointIns
        {
            public bool isVerified;
            public bool isLineLoop;
            public Transform fromTrf;
            public Transform toTrf;
            public bool isLineRndrSingle;
            public LineRenderer lineRndr;
            public LineRenderer capRndr;

            public LinePointIns(LinePointIns src)
            {
                this.isVerified = src.isVerified;
                this.isLineLoop = src.isLineLoop;
                this.fromTrf = src.fromTrf;
                this.toTrf = src.toTrf;
                this.isLineRndrSingle = src.isLineRndrSingle;
                this.lineRndr = src.lineRndr;
                this.capRndr = src.capRndr;
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
                if (!isLineRndrSingle && capRndr == null)
                {
                    return false;
                }
                return true;
            }
        }

        protected override void _Awake()
        {
            _Cache = new Dictionary<int, LinePointIns[]>();
        }

        public void SetCamera(Camera camera)
        {
            if(camera != null)
            targetCameraTrf = camera.transform;
        }

        public void SetCanvas(Canvas canvas) { }

        [SerializeField] private List<PointData> testPointDatas;

        public LineRenderer[] arrowLines;

        public Transform targetCameraTrf;
        public GameObject linePrefab;
        public float arrowLength = 0.1f;
        public float arrowWidth = 0.1f;
        public float animationTime = 1.0f;

        public static LinePointIns[] Create(PointData pointData, bool isLineLoop, bool hasAnimation)
        {
            return Create(new PointData[1] { pointData }, isLineLoop, hasAnimation);
        }

        public struct GenerateParams
        {
            public Transform cameraTrf;
            public float arrowLength;
            public float arrowWidth;
            public float animationTime;
        }

        [InvokeButton]
        public static LinePointIns[] Create(PointData[] pointDatas, bool isLineLoop, bool hasAnimation)
        {
            var cacheKeyID = GetCacheKey(isLineLoop, pointDatas);
            var helper = LinePointGenerator.Instance;

            if (!_Cache.TryGetValue(cacheKeyID, out var instanceList))
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

                    var lrObj = Instantiate(helper.linePrefab, helper.transform);
                    var from = pointDatas[i].pointTrf;
                    var to = pointDatas[next].pointTrf;
                    var color = pointDatas[i].color;

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

                    var lpIns = new LinePointIns
                    {
                        isVerified = true,
                        isLineLoop= isLineLoop,
                        fromTrf = from,
                        toTrf = to,
                        isLineRndrSingle = isSingle,
                        lineRndr = lineRenderer,
                        capRndr = capRenderer
                    };

                    newInsList.Add(lpIns);

                }

                instanceList = newInsList.ToArray();
                _Cache.Add(cacheKeyID, instanceList);
#if UNITY_EDITOR
                Instance._VisualizeCaches.Add(new VisualizeCache(instanceList));
#endif
            }

            var genParams = new GenerateParams
            {
                cameraTrf = helper.targetCameraTrf,
                arrowLength = helper.arrowLength,
                arrowWidth = helper.arrowWidth,
                animationTime = helper.animationTime
            };

            for (int i = 0; i < instanceList.Length; ++i)
            {
                var lpIns = instanceList[i];
                if (lpIns.CheckIsVerified())
                {
                    var color = pointDatas[i].color;
                    helper.GenerateLineAndPoint(lpIns, color, hasAnimation, genParams);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("제작중 문제가 있어 취소");
#endif
                }
            }

            return instanceList;
        }

        [InvokeButton]
        void TestCreate(bool isLoop, bool hasAnim)
        {
            Create(testPointDatas.ToArray(), isLoop, hasAnim);
        }


        void GenerateLineAndPoint(LinePointIns linePointIns, Color color, bool hasAnimation, GenerateParams genParams)
        {
            var lineRenderer = linePointIns.lineRndr;
            var capRenderer = linePointIns.capRndr;
            bool single = linePointIns.isLineRndrSingle;
            Vector3 pointA = linePointIns.fromTrf.localPosition;
            Vector3 pointB = linePointIns.toTrf.localPosition;

            if(capRenderer!=null)
                capRenderer.material.color = color;
            lineRenderer.material.color = color;

            if (hasAnimation)
            {
#if UNITY_2023_1_OR_NEWER
                _= _GenerateLinePointWithAnimation(lineRenderer, capRenderer, single, pointA, pointB, color, genParams);
#else
                StartCoroutine(IE_GenerateLinePointWithAnimation(lineRenderer, capRenderer, single, pointA, pointB, color, genParams));
#endif
                return;
            } // or immediately



            var arrowLength = genParams.arrowLength;
            var arrowWidth = genParams.arrowWidth;

            var distance = Vector3.Distance(pointA, pointB);
            var minLen = Math.Sqrt(arrowLength * arrowLength + arrowWidth * arrowWidth);

            if (distance < minLen)
            {
                Debug.LogWarning("The line is not long enough for the arrow, adjusting the arrow length");
                arrowLength = distance;
            }

            var pointC = pointB + arrowLength * (pointA - pointB) / distance;

            var camPoint = genParams.cameraTrf.position;
            var s1 = pointA - camPoint;
            var s2 = pointB - camPoint;

            var normal = Vector3.Cross(s1, s2).normalized;

            var pointD = pointC + arrowWidth * normal;
            var pointE = pointC - arrowWidth * normal;

            lineRenderer.SetPosition(0, pointA);
            lineRenderer.SetPosition(1, pointC);

            if (single)
            {
                lineRenderer.SetPosition(2, pointD);
                lineRenderer.SetPosition(3, pointB);
                lineRenderer.SetPosition(4, pointE);
                lineRenderer.SetPosition(5, pointD);
            }
            else
            {
                capRenderer.SetPosition(0, pointD);
                capRenderer.SetPosition(1, pointB);
                capRenderer.SetPosition(2, pointE);
                //capRenderer.SetPosition(3, pointD);
            }
            return;
        }

        Coroutine CO_GenerateLinePointWithAnimation = null;

#if UNITY_2023_1_OR_NEWER
        async Awaitable _GenerateLinePointWithAnimation(LineRenderer lineRenderer, LineRenderer capRenderer, bool single, Vector3 pointA, Vector3 pointB, Color color, GenerateParams genParams)
#else
        IEnumerator IE_GenerateLinePointWithAnimation(LineRenderer lineRenderer, LineRenderer capRenderer, bool single, Vector3 pointA, Vector3 pointB, Color color, GenerateParams genParams)
#endif
        {
            yield return null;

            var maxWidth = 0f;
            foreach (var k in lineRenderer.widthCurve.keys)
            {
                if (k.value > maxWidth)
                {
                    maxWidth = k.value;
                }
            }
            maxWidth *= lineRenderer.widthMultiplier;

            var arrowLength = genParams.arrowLength;
            var arrowWidth = genParams.arrowWidth;
            var minLen = Math.Sqrt(arrowLength * arrowLength + arrowWidth * arrowWidth);
            var distance = Vector3.Distance(pointA, pointB);

            if (distance < arrowLength)
            {
                Debug.LogWarning("The line is not long enough for the arrow, adjusting the arrow length");
                arrowLength = distance;
            }

            var dir = pointB - pointA;
            var camPoint = genParams.cameraTrf.position;
            var s1 = pointA - camPoint;
            var s2 = pointB - camPoint;

            var normal = Vector3.Cross(s1, s2).normalized;

            var totalTime = 0f;
            while (totalTime < genParams.animationTime)
            {
                var timeRatio = totalTime / genParams.animationTime;

                var len = arrowLength * timeRatio;

                var pointB2 = pointA + timeRatio * (pointB - pointA);
                var pointC = pointB2 + len * (pointA - pointB) / distance;

                var pointD = pointC + arrowWidth * normal;
                var pointE = pointC - arrowWidth * normal;


                if (Vector3.Distance(pointA, pointC) >= maxWidth)
                {
                    lineRenderer.SetPosition(0, pointA);
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

            CO_GenerateLinePointWithAnimation = null;

        }
    }
}