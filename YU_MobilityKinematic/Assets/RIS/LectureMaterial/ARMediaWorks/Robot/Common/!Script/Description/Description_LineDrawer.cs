using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CWJ.YU.Mobility
{
    public class Description_LineDrawer : MaskableGraphic, INeedSceneObj
    {
        [VisualizeProperty] public Camera playerCamera { get; set; }
        [VisualizeProperty] public Transform playerCamTrf { get; set; }
        [VisualizeProperty] public Canvas worldCanvas { get; set; }
        [VisualizeProperty] public RectTransform worldCanvasRectTrf { get; set; }

        public float lineThickness = 2;

        private Vector2 a;
        private Vector2 b;

        private static Vector2[] uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };

        private Vector2 startPoint;

        [SerializeField] Transform objectToPointAt;

        private const float TimeBetweenScreenChangeCalculations = 0.5f;
        private float _lastScreenChangeCalculationTime = 0;
        int screenWidth, screenHeight;

        protected override void Awake()
        {
            base.Awake();

            if (playerCamera == null)
                playerCamera = Camera.main;
            if(worldCanvasRectTrf ==null)
                worldCanvasRectTrf = GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
            UpdateScreenSize();
        }

        void UpdateScreenSize()
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            _lastScreenChangeCalculationTime = Time.time;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            if (!Application.isPlaying) return;
            if (Time.time - _lastScreenChangeCalculationTime < TimeBetweenScreenChangeCalculations)
                return;

            UpdateScreenSize();
        }


        // Update is called once per frame
        void Update()
        {
            if (playerCamera == null) return;
            if (objectToPointAt == null) return;

            Vector3 screenPosition = playerCamera.WorldToScreenPoint(objectToPointAt.position);
            float canvasWidth = worldCanvasRectTrf.rect.width;
            float canvasHeight = worldCanvasRectTrf.rect.height;

            Vector2 start = new Vector2(
                (startPoint.x / screenWidth) * canvasWidth,
                (startPoint.y / screenHeight) * canvasHeight);
            Vector2 end = new Vector2(
                (screenPosition.x / screenWidth) * canvasWidth,
                (screenPosition.y / screenHeight) * canvasHeight);

            DrawLine(start, end);
        }

        public void SetObjectToPointAt(Transform trf, Vector2 startPoint)
        {
            this.startPoint = startPoint;
            this.objectToPointAt = trf;
            if (trf == null)
            {
                SetAllDirty();
            }
        }

        private void DrawLine(float x1, float y1, float x2, float y2)
        {
            a = new Vector2(x1, y1);
            b = new Vector2(x2, y2);
            SetAllDirty();
        }

        private void DrawLine(Vector2 a, Vector2 b)
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