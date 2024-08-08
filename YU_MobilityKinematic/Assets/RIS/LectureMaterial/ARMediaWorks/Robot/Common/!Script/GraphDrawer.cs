using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CWJ.YU.Mobility
{
    public class GraphDrawer : MonoBehaviour
    {
        [System.Serializable]
        public struct PointContainer
        {
            public Transform[] pointTrfs;
            public Color color;
        }

        public PointContainer[] pointTrfs;

        [ColorUsage(true, true)]
        [SerializeField] Color[] colors;

        List<int> lineCahces = new List<int>();
        public void Draw(Transform[] pointTrfs, Color color, bool isLineLoop = false)
        {
            var pointDatas = pointTrfs.Select(t => new LinePointGenerator.PointData(t, color)).ToArray();

            int cacheID = LinePointGenerator.Create(pointDatas, isLineLoop, true);
            lineCahces.Add(cacheID);
        }

        private void OnDisable()
        {
            lineCahces.Do(id => { LinePointGenerator.Instance.DestroyLineObj(id); });
            lineCahces.Clear();
        }
    }
}
