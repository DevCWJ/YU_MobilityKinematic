using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace CWJ.YU.Mobility
{
    public class LineConfigure : MonoBehaviour
    {
        public Transform[] pointTrfs;
        public Color color;
        public bool isLineLoop;
        public float arrowHeight = 0.025f;
        public float arrowWidth = 0.025f;
        public float animTime = 1;

        [SerializeField] int lineCahces = -1;

        [InvokeButton]
        public void Draw()
        {
            if (pointTrfs.LengthSafe()==0)
            {
                return;
            }
            var pointDatas = pointTrfs.Select(t => new LinePointGenerator.PointData(t, color)).ToArray();

            lineCahces = LinePointGenerator.Generate(pointDatas, isLineLoop, animTime, arrowWidth, arrowHeight);
        }

        public void DisableDraw()
        {
            if (lineCahces != -1)
                LinePointGenerator.Instance.DestroyLineObj(lineCahces);
        }
    }
}
