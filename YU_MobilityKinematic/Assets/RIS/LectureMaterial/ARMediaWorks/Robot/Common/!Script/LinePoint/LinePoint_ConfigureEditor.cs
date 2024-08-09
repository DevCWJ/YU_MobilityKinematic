using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using static CWJ.LinePoint_Generator;

namespace CWJ.YU.Mobility
{
    /// <summary>
    /// CWJ - 24.08.08
    /// <para/> LinePoint_Generator 이용해 좌표랑 어울리는 형태로 빠르게 구성해주게끔 녀석
    /// <para/> 감지되고싶으면 활성화 상태여야함
    /// </summary>
    public class LinePoint_ConfigureEditor : MonoBehaviour
#if UNITY_EDITOR
        , CWJ.AccessibleEditor.InspectorHandler.ISelectHandler
#endif
    {
#if UNITY_EDITOR
        [DrawHeaderAndLine("Editor 영역")]
        [SerializeField] bool defaultIsAngleLine = true;
        [SerializeField] bool defaultIsLineLoop = false;
        [ColorUsage(true, true)]
        [SerializeField] Color defaultColor = new Color(1, 1, 1, 1);

        [DrawHeaderAndLine(nameof(autoSyncRoot) + " 자식들로 자동삽입/제거됨.")]
        [SerializeField] Transform autoSyncRoot;
        [SerializeField] PointDataContainer[] autoSyncPdcs;

        [DrawHeaderAndLine(nameof(userCustomAddParent) + " 에 넣으면 추가됨.")]
        [SerializeField] Transform userCustomAddParent = null;
        [SerializeField] PointDataContainer[] userInputPdcs;



        private void OnValidate()
        {
            if (UserCustomConfigure())
            {
                AutoSyncConfigure();
                AutoListing();
            }
        }

        public void CWJEditor_OnSelect(MonoBehaviour target)
        {
            if (AutoSyncConfigure())
            {
                AutoListing();
            }
        }

        PointDataContainer ConvertToPdc(Transform[] trfs)
        {
            bool isAngleLine = defaultIsAngleLine;
            bool isLineLoop = defaultIsLineLoop;
            if (trfs.Length < 3)
            {
                isAngleLine = false;
                isLineLoop = false;
            }
            if (trfs.Length == 1)
            {
                trfs = new Transform[2] { trfs[0], trfs[0] };
            }

            return new PointDataContainer(trfs, isAngleLine, defaultColor, isLineLoop);
        }

        bool AutoSyncConfigure()
        {
            if (autoSyncRoot == null || autoSyncRoot.childCount == 0)
            {
                return false;
            }

            List<PointDataContainer> list = new List<PointDataContainer>();

            foreach (Transform pdcCoreTrf in autoSyncRoot)
            {
                list.Add(ConvertToPdc(pdcCoreTrf.GetComponentsInChildren_New<Transform>(false, true)));
            }

            autoSyncPdcs = list.ToArray();

            return true;
        }

        bool UserCustomConfigure()
        {
            if (userCustomAddParent == null)
            {
                return false;
            }

            var pdc = ConvertToPdc(userCustomAddParent.GetComponentsInChildren_New<Transform>(false, false));

            userCustomAddParent = null;

            CWJ.AccessibleEditor.AccessibleEditorUtil.PingObj(pdc.pointTrfs[0].gameObject, false);

            ArrayUtil.Add(ref userInputPdcs, pdc);

            return true;
        }

        void AutoListing()
        {
            pointDataContainers = new PointDataContainer[0];
            List<PointDataContainer> list = new List<PointDataContainer>(capacity: autoSyncPdcs.LengthSafe() + userInputPdcs.LengthSafe());
            list.AddRange(autoSyncPdcs);
            list.AddRange(userInputPdcs);
            pointDataContainers = list.ToArray();
        }
#endif

        [DrawHeaderAndLine("Setting 영역")]
        public PointDataContainer[] pointDataContainers;

        public float arrowHeight = 0.025f;
        public float arrowWidth = 0.025f;
        public float animTime = 1;


        [Serializable]
        public struct PointDataContainer
        {
            [SerializeField]
            string editorDevComment;
            public Transform[] pointTrfs;
            public bool isAngleLine;
            [ColorUsage(true, true)]
            public Color color;
            public bool isLineLoop;


            public PointDataContainer(Transform[] pointTrfs, bool isAngleLine, Color color, bool isLineLoop)
            {
                editorDevComment = $"\"{pointTrfs[0].name}\" ~ \"{pointTrfs[pointTrfs.Length - 1].name}\"";
                this.pointTrfs = pointTrfs;
                this.isAngleLine = isAngleLine;
                this.color = color;
                this.isLineLoop = isLineLoop;
            }
        }



        public void ChangePos2D(Transform target, Vector2 pos, bool isDoDraw)
        {
            ChangePos3D(target, new Vector3(pos.x, pos.y, 0), isDoDraw);
        }

        [InvokeButton]
        public void ChangePointWithAngleLine(Transform relatedTrf, Vector3 pos, bool withName = true)
        {
            var pdc = pointDataContainers.FirstOrDefault(p => p.isAngleLine && p.pointTrfs.IsExists(relatedTrf));
            if (!default(PointDataContainer).Equals(pdc))
            {
                ChangePos3D(pdc.pointTrfs[0], pos, true);
            }
            else
            {
                Debug.LogError("ChangePointWithAngleLine null !?");
            }
        }

        [InvokeButton]
        public void ChangePos3D(Transform target, Vector3 pos, bool isDoDraw)
        {
            if (lineCahces.Count == 0)
                Draw();

            int scaleOffset = 10;

            if (!pointDataContainers.IsExists(p => p.pointTrfs.IsExists(target)))
            {
                Debug.LogError("바꿀 object가 없다!");
                return;
            }
            var localPos = pos * scaleOffset;
            target.localPosition = localPos;

            if (isDoDraw)
                Draw();
        }

        List<int> lineCahces = new List<int>();

        [InvokeButton]
        public void Draw()
        {
            if (pointDataContainers.LengthSafe()==0)
            {
                return;
            }

            pointDataContainers.ForEach(p =>
            {
                if (p.isAngleLine)
                {
                    var parentPoint = new PointData(p.pointTrfs[0], p.color);
                    for (int i = 1; i < p.pointTrfs.Length; i++)
                    {
                        var pointDatas = new PointData[2] { parentPoint, new PointData(p.pointTrfs[i], p.color) };
                        lineCahces.Add(Generate(pointDatas, p.isLineLoop, animTime, arrowWidth, arrowHeight));
                    }
                }
                else
                {
                    var pointDatas = p.pointTrfs.Select(t => new PointData(t, p.color)).ToArray();
                    lineCahces.Add(Generate(pointDatas, p.isLineLoop, animTime, arrowWidth, arrowHeight));
                }
            });
        }

        public void DisableDraw()
        {
            if (lineCahces.Count > 0)
                lineCahces.ForEach(c => LinePoint_Generator.Instance.DestroyLineObj(c));
        }


    }
}
