using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using UnityEngine.XR; 

namespace FurbackMenu
{
    [BepInPlugin("com.furback.gtag.menu", "Furback Free Menu", "1.0.2")]
    public class MainMenuPlugin : BaseUnityPlugin
    {
        // Menu Identity
        public static string MenuTitle = "Furback menu 1.0"; 
        
        // Menu Layout and Pagination States
        public static int pageNumber = 0;
        public static int maxPages = 4;
        private bool _menuInitialized = false;
        private bool _menuActive = false;
        private bool _isButtonButtonPressed = false; 

        // Theme and Styling Configuration
        private static readonly Color BackgroundColor = new Color(0.12f, 0.12f, 0.12f); // Dark Slate Base
        private static readonly Color TitleColor = Color.yellow;                         // Bright Yellow Title
        private static readonly Color ButtonInactiveColor = new Color(0.85f, 0.45f, 0f); // Solid Orange (Off)
        private static readonly Color ButtonActiveColor = new Color(1f, 0.85f, 0f);      // Electric Yellow (On)

        private GameObject menuPlate = null;
        private List<GameObject> menuButtons = new List<GameObject>();

        void Awake()
        {
            Logger.LogInfo("[FURBACK MENU] Menu Base Loaded Successfully!");
        }

        void Update()
        {
            // Listen for the Controller input (Y Button) to toggle the menu state
            HandleInputAndNavigation();

            // Refresh or destroy UI elements depending on toggle state
            if (_menuActive) 
            { 
                // Always recreate or update to capture page switches accurately
                CreateMenuUI(); 
            }
            else if (!_menuActive && _menuInitialized)
            {
                DestroyMenuUI();
            }
        }

        private void CreateMenuUI()
        {
            // Clear past frame buttons to prevent massive duplicates stacking up
            if (_menuInitialized) { DestroyMenuUI(); }

            // 1. Setup the main backplate
            menuPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            menuPlate.transform.localScale = new Vector3(0.25f, 0.35f, 0.02f);
            
            try
            {
                // Finds the left hand object natively via name if GorillaTagger instance isn't linked yet
                GameObject leftHand = GameObject.Find("LeftHandTriggerCollider");
                if (leftHand != null)
                {
                    menuPlate.transform.SetParent(leftHand.transform, false);
                    menuPlate.transform.localPosition = new Vector3(0f, 0.12f, 0f); 
                    menuPlate.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); 
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"[FURBACK MENU] Error attaching to VR hand: {ex.Message}");
            }

            Renderer plateRenderer = menuPlate.GetComponent<Renderer>();
            if (plateRenderer != null)
            {
                plateRenderer.material.shader = Shader.Find("GorillaTag/UberShader");
                plateRenderer.material.color = BackgroundColor;
            }

            // 2. Main Title Text
            GameObject titleObj = new GameObject("MenuTitle");
            titleObj.transform.SetParent(menuPlate.transform, false);
            titleObj.transform.localPosition = new Vector3(0f, 0.4f, -0.6f);
            titleObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            TextMesh titleText = titleObj.AddComponent<TextMesh>();
            titleText.text = $"{MenuTitle} [{pageNumber + 1}]";
            titleText.color = TitleColor;
            titleText.fontSize = 45;
            titleText.anchor = TextAnchor.MiddleCenter;
            titleText.alignment = TextAlignment.Center;

            // 3. Dynamic Page Layout Handler
            RenderActivePageButtons();

            _menuInitialized = true;
        }

        private void RenderActivePageButtons()
        {
            // Navigation Buttons (Constant across pages)
            CreateMenuButton("Previous Page", new Vector3(-0.3f, -0.4f, -0.55f));
            CreateMenuButton("Next Page", new Vector3(0.3f, -0.4f, -0.55f));

            // Dynamic Content Rendering according to current Page
            if (pageNumber == 0)
            {
                CreateMenuButton("Disconnect", new Vector3(0f, 0.25f, -0.55f));
                CreateMenuButton("Join Random", new Vector3(0f, 0.13f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, 0.01f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, -0.11f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, -0.23f, -0.55f));
            }
            else if (pageNumber == 1)
            {
                CreateMenuButton("Fly", new Vector3(0f, 0.25f, -0.55f));
                CreateMenuButton("Noclip", new Vector3(0f, 0.13f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, 0.01f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, -0.11f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, -0.23f, -0.55f));
            }
            else if (pageNumber == 2)
            {
                CreateMenuButton("Pee", new Vector3(0f, 0.25f, -0.55f));
                CreateMenuButton("Poop", new Vector3(0f, 0.13f, -0.55f));
                CreateMenuButton("Vomit", new Vector3(0f, 0.01f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, -0.11f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, -0.23f, -0.55f));
            }
            else if (pageNumber == 3)
            {
                CreateMenuButton("Admin Badge", new Vector3(0f, 0.25f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, 0.13f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, 0.01f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, -0.11f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, -0.23f, -0.55f));
            }
            else if (pageNumber == 4)
            {
                CreateMenuButton("Tracer", new Vector3(0f, 0.25f, -0.55f));
                CreateMenuButton("Bone ESP", new Vector3(0f, 0.13f, -0.55f));
                CreateMenuButton("Box ESP", new Vector3(0f, 0.01f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, -0.11f, -0.55f));
                CreateMenuButton("PLACEHOLDER", new Vector3(0f, -0.23f, -0.55f));
            }
        }

        private void CreateMenuButton(string buttonText, Vector3 localPosition)
        {
            GameObject buttonObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            buttonObj.transform.SetParent(menuPlate.transform, false);
            buttonObj.transform.localPosition = localPosition;
            buttonObj.transform.localScale = new Vector3(0.75f, 0.10f, 0.1f); 

            Renderer btnRenderer = buttonObj.GetComponent<Renderer>();
            if (btnRenderer != null)
            {
                btnRenderer.material.shader = Shader.Find("GorillaTag/UberShader");
                btnRenderer.material.color = ButtonInactiveColor; 
            }

            GameObject btnTextObj = new GameObject("ButtonText");
            btnTextObj.transform.SetParent(buttonObj.transform, false);
            btnTextObj.transform.localPosition = new Vector3(0f, 0f, -0.6f);
            btnTextObj.transform.localScale = new Vector3(0.12f, 0.85f, 1f); 

            TextMesh btnText = btnTextObj.AddComponent<TextMesh>();
            btnText.text = buttonText;
            btnText.color = Color.white; 
            btnText.fontSize = 35;
            btnText.anchor = TextAnchor.MiddleCenter;
            btnText.alignment = TextAlignment.Center;

            Destroy(buttonObj.GetComponent<BoxCollider>());
            menuButtons.Add(buttonObj);
        }

        private void DestroyMenuUI()
        {
            if (menuPlate != null)
            {
                Destroy(menuPlate);
                menuPlate = null;
            }
            menuButtons.Clear();
            _menuInitialized = false;
        }

        private void HandleInputAndNavigation()
        {
            var leftHandDevices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, leftHandDevices);

            if (leftHandDevices.Count > 0)
            {
                InputDevice device = leftHandDevices[0];
                
                if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValue) && secondaryValue)
                {
                    if (!_isButtonButtonPressed)
                    {
                        _menuActive = !_menuActive;
                        _isButtonButtonPressed = true; 
                    }
                }
                else
                {
                    _isButtonButtonPressed = false;
                }
            }
        }
    }
}