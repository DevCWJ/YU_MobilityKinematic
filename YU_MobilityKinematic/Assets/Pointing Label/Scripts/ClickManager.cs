using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace poitinglabel
{
    // The position of the connection between the line and the panel
    public enum VERTICAL_LINE_POSITION_ON_PANEL
    {
        TOP,
        MIDDLE,
        BOTTOM,
    }

    /**
     * The click manager is used to manage the click on an object and display the text panel pointing on this object.
    */
    public class ClickManager : MonoBehaviour
    {

        // The DataBase that gives us the description of a clicked object
        public DataBase dataBase;

        // The Drawer of the line
        public LineDrawer theLineDrawer;

        // The Info Message Panel GameObject
        public GameObject panel;

        // The Title of the Info Message Panel
        public Text panelTitle;

        // The Description of the Info Message Panel
        public Text panelDescription;

        // The Scroll Bar of the Info Message Pane
        public Scrollbar scrollBar;

        // The GameObject that we put on the click point (very small sphere for exemple)
        public GameObject spherePointer;

        // The Canvas on where we draw line and panel (used to compute panel width)
        public Canvas canvas;

        //The position of the connection between the line and the panel
        public VERTICAL_LINE_POSITION_ON_PANEL verticalPositionOfLinePanelConnection;

        // True when we are dragging something with the mouse
        private bool dragging = false;

        // True when the first mouse button is down
        private bool mouseButtonDown = false;

        // Name of the last object clicked
        private string lastClickedPartname = "";

        // Called at initialization only
        void Start()
        {
            panel.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

            // If user click
            if (userClick())
            {
                // not on a UI element
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    RaycastHit hit;
                    Vector3 clickPosition = Input.mousePosition;
                    Ray ray = Camera.main.ScreenPointToRay(clickPosition);

                    // we intersect collider of game object
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        spherePointer.SetActive(false);

                        int screenWidth = Screen.width;
                        int screenHeight = Screen.height;

                        RectTransform panelToPosition = panel.GetComponent<RectTransform>();

                        spherePointer.transform.position = hit.point;
                        spherePointer.transform.SetParent(hit.transform);

                        panelToPosition.position = computePanelPosition(clickPosition, screenWidth, screenHeight);
                        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
                        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
                        float panelWidth = panelToPosition.rect.width * (Screen.width / canvasWidth);
                        float panelHeight = panelToPosition.rect.height * (Screen.height / canvasHeight);

                        Vector2 startpointPosition = computeLineStartPosition(clickPosition, panelToPosition.position, screenWidth, screenHeight, panelWidth, panelHeight);
                        theLineDrawer.setObjectToPointAt(spherePointer, startpointPosition);

                        panel.SetActive(true);
                        lastClickedPartname = hit.transform.name;
                        panelTitle.text = lastClickedPartname;
                        panelDescription.text = dataBase.getText(lastClickedPartname);
                        scrollBar.value = 1;
                    }
                    else // we click on nothing, we hide line and panel
                    {
                        theLineDrawer.setObjectToPointAt(null, panel.GetComponent<RectTransform>().position);
                        panel.SetActive(false);
                        spherePointer.SetActive(false);
                    }
                }
            }
        }

        /**
         * Compute the panel position.
         */
        Vector3 computePanelPosition(Vector3 clickPosition, int screenWidth, int screenHeight)
        {
            Vector2 sign = computeSign(clickPosition, screenWidth, screenHeight);

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

        /**
         * Compute the start of the line.
         */
        Vector2 computeLineStartPosition(Vector3 clickPosition, Vector3 panelPosition, int screenWidth, int screenHeight, float panelWidth, float panelHeight)
        {
            float yPos = 0.0f;
            Vector2 sign = computeSign(clickPosition, screenWidth, screenHeight);
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

        /**
         * compute for each coordinates (x,y) if click is on the right/top part of the screen or left/bottom part of the screen.
         * x = 1 , left part
         * x = -1 , right part
         * y = 1, top part
         * y = -1, bottom part
         */
        Vector2 computeSign(Vector3 clickPosition, int screenWidth, int screenHeight)
        {
            float signX = 1;
            float signY = 1;
            if (clickPosition.x > screenWidth / 2.0f) signX = -1;
            if (clickPosition.y > screenHeight / 2.0f) signY = -1;
            return new Vector2(signX, signY);
        }


        /** 
         * Test if user do a real click.
         */
        bool userClick()
        {
            bool accept = false;
            if (mouseButtonDown)
            {
                if (mouseIsMoving())
                {
                    dragging = true;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                mouseButtonDown = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!dragging)
                    accept = true;
                mouseButtonDown = false;
                dragging = false;
            }
            return accept;
        }

        /**
         * True if the mouse is moving in any direction.
         */
        bool mouseIsMoving()
        {
            float moveX = Input.GetAxis("Mouse X");
            float moveY = Input.GetAxis("Mouse Y");

            return moveX > 0 || moveX < 0 || moveY > 0 || moveY < 0;
        }
    }
}


