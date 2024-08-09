using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CWJ.YU.Mobility
{
    public class DescriptonHelper : CWJ.Singleton.SingletonBehaviour<DescriptonHelper> , INeedSceneObj
    {
        [VisualizeProperty] public Camera playerCamera { get; set; }
        [VisualizeProperty] public Transform playerCamTrf { get; set; }
        [VisualizeProperty] public Canvas worldCanvas { get; set; }
        [VisualizeProperty] public RectTransform worldCanvasRectTrf { get; set; }


        public Description_LineDrawer theLineDrawer;

        public RectTransform panelRectTrf;

        public TextMeshProUGUI panelTitle;

        public TextMeshProUGUI panelDescription;

        public Scrollbar scrollBar;

        public Transform spherePointer;


        public VERTICAL_LINE_POSITION_ON_PANEL verticalPositionOfLinePanelConnection;
        public enum VERTICAL_LINE_POSITION_ON_PANEL
        {
            TOP,
            MIDDLE,
            BOTTOM,
        }

        private bool isDragging = false;

        private bool isMousePress = false;

        private string lastClickedPartname = "";

        public CWJ.Serializable.DictionaryVisualized<string, string> dataBase = new CWJ.Serializable.DictionaryVisualized<string, string>();

        protected override void _Awake()
        {
            if (worldCanvasRectTrf == null) worldCanvasRectTrf = GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
            if (playerCamera == null) playerCamera = Camera.main;
        }

        protected override void _Start()
        {
            panelRectTrf.gameObject.SetActive(false);
            var leftClickCallback = KeyEventManager_PC.GetKeyListener(KeyCode.Mouse0);
            leftClickCallback.onTouchBegan.AddListener(OnLeftMouseDown);
            leftClickCallback.onTouchEnded.AddListener(OnLeftMouseUp);
            leftClickCallback.onTouchCanceled.AddListener(OnLeftMouseUp);
            leftClickCallback.onUpdateEnded.AddListener(OnLoopChecker);
        }

        [InvokeButton]
        public void OpenDescription(Transform targetTrf, string title, string desc)
        {
            Vector3 targetPos = targetTrf.position;
            _OpenDescription(targetTrf, targetPos, Input.mousePosition, title, desc);
        }

        [InvokeButton]
        public void CloseDescription()
        {
            theLineDrawer.SetObjectToPointAt(null, panelRectTrf.position);
            panelRectTrf.gameObject.SetActive(false);
            spherePointer.gameObject.SetActive(false);
        }

        Vector3 GetPanelPosition(Vector3 clickPosition, int screenWidth, int screenHeight)
        {
            Vector2 sign = GetClickToSign(clickPosition, screenWidth, screenHeight);

            Vector3 p = new Vector3(clickPosition.x + sign.x * (screenWidth * 35.0f / 100.0f), clickPosition.y + sign.y * (screenHeight * 15 / 100.0f), 0);
            float limitPercent = 30.0f;

            float lowPxLimit = (limitPercent / 100.0f) * screenWidth;
            float highPxLimit = ((100.0f - limitPercent) / 100.0f) * screenWidth;

            if (p.x > highPxLimit)
            {
                p = new Vector3(highPxLimit, p.y, p.z);
            }
            else if (p.x < lowPxLimit)
            {
                p = new Vector3(lowPxLimit, p.y, p.z);
            }

            return p;
        }

        Vector2 GetLineStartPosition(Vector3 clickPosition, Vector3 panelPosition, int screenWidth, int screenHeight, float panelWidth, float panelHeight)
        {
            float yPos = 0.0f;
            Vector2 sign = GetClickToSign(clickPosition, screenWidth, screenHeight);
            if (verticalPositionOfLinePanelConnection == VERTICAL_LINE_POSITION_ON_PANEL.BOTTOM)
            {
                yPos = panelPosition.y - panelHeight / 2.0f + 1;
            }
            else if (verticalPositionOfLinePanelConnection == VERTICAL_LINE_POSITION_ON_PANEL.MIDDLE)
            {
                yPos = panelPosition.y;
            }
            else if (verticalPositionOfLinePanelConnection == VERTICAL_LINE_POSITION_ON_PANEL.TOP)
            {
                yPos = panelPosition.y + panelHeight / 2.0f - 1;
            }

            return new Vector2(panelPosition.x - sign.x * panelWidth / 2.0f, yPos);
        }

        Vector2 GetClickToSign(Vector3 clickPosition, int screenWidth, int screenHeight)
        {
            float signX = 1;
            float signY = 1;
            if (clickPosition.x > screenWidth / 2.0f) signX = -1;
            if (clickPosition.y > screenHeight / 2.0f) signY = -1;
            return new Vector2(signX, signY);
        }



        void _OpenDescription(Transform targetTrf, Vector3 pointerPos, Vector3 mousePos, string title, string desc)
        {
            spherePointer.gameObject.SetActive(true);

            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            spherePointer.SetParent(targetTrf);
            spherePointer.position = pointerPos;
            panelRectTrf.position = GetPanelPosition(mousePos, screenWidth, screenHeight);
            float canvasWidth = worldCanvasRectTrf.rect.width;
            float canvasHeight = worldCanvasRectTrf.rect.height;
            float panelWidth = panelRectTrf.rect.width * (screenWidth / canvasWidth);
            float panelHeight = panelRectTrf.rect.height * (screenHeight / canvasHeight);

            Vector2 startpointPosition = GetLineStartPosition(mousePos, panelRectTrf.position, screenWidth, screenHeight, panelWidth, panelHeight);
            theLineDrawer.SetObjectToPointAt(spherePointer, startpointPosition);

            panelRectTrf.gameObject.SetActive(true);
            panelTitle.SetText(title);
            panelDescription.SetText(desc);
            StartCoroutine(ScrollBarDelay());
        }

        IEnumerator ScrollBarDelay()
        {
            yield return null;
            scrollBar.value = 1;
            yield return new WaitForEndOfFrame();
            scrollBar.value = 1;
        }



        void OnLeftMouseDown()
        {
            isMousePress = true;
        }

        bool isUserClicked;
        void OnLeftMouseUp()
        {
            isUserClicked = false;
            if (!isDragging)
                isUserClicked = true;
            isMousePress = false;
            isDragging = false;
        }

        void OnLoopChecker()
        {
            if (isMousePress)
            {
                if (IsMouseMoving())
                    isDragging = true;
            }

            if (!isUserClicked)
            {
                return;
            }
            isUserClicked = false;

            if (playerCamera == null)
            {
                return;
            }

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 clickPosition = Input.mousePosition;
                bool isHit = Physics.Raycast(playerCamera.ScreenPointToRay(clickPosition), out RaycastHit hit, 100.0f);

                string hitName = null;
                string description = null;
                if (isHit && hit.transform != null)
                {
                    hitName = hit.transform.name;
                    if (!dataBase.TryGetValue(hitName, out description))
                    {
                        hitName = null; description = null;
                    }
                }

                if (description != null)
                {
                    _OpenDescription(hit.transform, hit.point, clickPosition, hitName, description);
                }
                else
                {
                    CloseDescription();
                }
            }
        }

        bool IsMouseMoving()
        {
            float moveX = Input.GetAxis("Mouse X");
            if (moveX < 0)
                moveX *= -1;
            if (moveX > 0.05f)
                return true;

            float moveY = Input.GetAxis("Mouse Y");
            if (moveY < 0)
                moveY *= -1;
            if (moveY > 0.05f)
                return true;

            return false;
        }


    }
}


