using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace poitinglabel
{
    public class LineDrawer : MaskableGraphic
    {
        // Line Thickness
        public float lineThickness = 2;

        public Vector2 referenceResolution = new Vector2(800, 600);

        private Vector2 a;
        private Vector2 b;

        private static Vector2[] uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };

        private Vector2 startPoint;

        private GameObject objectToPointAt;


        // Use this for initialization
        protected override void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (objectToPointAt != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(objectToPointAt.transform.position);
                float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
                float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

                Vector2 start = new Vector2(
                    (startPoint.x / Screen.width) * canvasWidth,
                    (startPoint.y / Screen.height) * canvasHeight);
                Vector2 end = new Vector2(
                    (screenPosition.x / Screen.width) * canvasWidth,
                    (screenPosition.y / Screen.height) * canvasHeight);

                drawLine(start, end);
            }
        }

        public void setObjectToPointAt(GameObject o, Vector2 startPoint)
        {
            this.startPoint = startPoint;
            this.objectToPointAt = o;
            if (o == null)
            {
                SetAllDirty();
            }
        }

        private void drawLine(float x1, float y1, float x2, float y2)
        {
            a = new Vector2(x1, y1);
            b = new Vector2(x2, y2);
            SetAllDirty();
        }

        private void drawLine(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
            SetAllDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (objectToPointAt == null)
            {
                return;
            }
            var offset_x = -rectTransform.pivot.x;
            var offset_y = -rectTransform.pivot.y;

            var p1 = new Vector2(a.x + offset_x, a.y + offset_y);
            var p2 = new Vector2(b.x + offset_x, b.y + offset_y);

            vh.AddUIVertexQuad(CreateLineVertices(p1, p2));
        }

        private UIVertex[] CreateLineVertices(Vector2 p1, Vector2 p2)
        {
            Vector2 offset = new Vector2((p1.y - p2.y), p2.x - p1.x).normalized * lineThickness / 2;

            var v1 = p1 - offset;
            var v2 = p1 + offset;
            var v3 = p2 + offset;
            var v4 = p2 - offset;
            return CreateVbo(new[] { v1, v2, v3, v4 }, uv);
        }

        private UIVertex[] CreateVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }
    }
}