using Il2CppSystem;
using MelonLoader;
using System.IO;
using UnityEngine;
using Loading_screen_pictures;
using System.Linq;
using System.Collections.Generic;
using Console = System.Console;

[assembly: MelonInfo(typeof(LoadingScreenPictures), "Loading Screen Pictures", "1.2.4", "MarkViews", "https://github.com/markviews/LoadingScreenPictures")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace Loading_screen_pictures {
    internal class LoadingScreenPictures : MelonMod {

        private GameObject cube;
        private Texture lastTexture;
        private Renderer screenRender, pic;
        private string folder_dir;
        private readonly string default_dir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\VRChat";
        private bool initUI = false;

        public override void OnApplicationStart() {
            MelonPreferences.CreateCategory("LoadingScreenPictures", "Loading Screen Pictures");
            MelonPreferences.CreateEntry("LoadingScreenPictures", "directory", default_dir, "Folder to get pictures from");
            folder_dir = MelonPreferences.GetEntryValue<string>("LoadingScreenPictures", "directory");
        }

        public override void VRChat_OnUiManagerInit() {
            setup();
            initUI = true;
        }

        public override void OnUpdate()
        {
            if (lastTexture == null) return;
            if (lastTexture == screenRender.material.mainTexture) return;
            lastTexture = screenRender.material.mainTexture;
            changePic();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            switch (buildIndex)
            {
                case 1:
                case 2:
                    break;
                default: //Causes this to run only once instead of multiple times.
                    if (initUI && lastTexture == null) 
                        setup();
                    break;
            }
        }

        private void changePic() {
            Texture2D texture = new Texture2D(2, 2);
            ImageConversion.LoadImage(texture, File.ReadAllBytes(randImage()));
            pic.material.mainTexture = texture;
            if (pic.material.mainTexture.height > pic.material.mainTexture.width) {
                cube.transform.localScale = new Vector3(0.099f, 1, 0.175f);
                GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").transform.localScale = new Vector3(10.80f, 19.20f, 1);
            } else {
                cube.transform.localScale = new Vector3(0.175f, 1, 0.099f);
                GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").transform.localScale = new Vector3(19.20f, 10.80f, 1);
            }
        }

        private void setup() {
            GameObject screen = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainScreen");
            //check if screenshots folder is empty
            String imageLink = randImage();
            if (imageLink == null) {
                MelonLogger.Log("No screenshots found in: " + folder_dir);
                return;
            }

            GameObject parentScreen = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN");
            screenRender = screen.GetComponent<Renderer>();
            lastTexture = screenRender.material.mainTexture;

            //create new image
            cube = GameObject.CreatePrimitive(PrimitiveType.Plane);
            cube.transform.SetParent(parentScreen.transform);
            cube.transform.rotation = screen.transform.rotation;
            cube.transform.localPosition = new Vector3(0, 0, -0.19f);
            cube.GetComponent<Collider>().enabled = false;
            cube.layer = LayerMask.NameToLayer("UiMenu");
            Texture2D texture = new Texture2D(2, 2);
            ImageConversion.LoadImage(texture, File.ReadAllBytes(imageLink));
            pic = cube.GetComponent<Renderer>();
            pic.material.mainTexture = texture;

            //disable origial picture
            screenRender.enabled = false;

            //resize frame
            if (pic.material.mainTexture.height > pic.material.mainTexture.width) {
                cube.transform.localScale = new Vector3(0.099f, 1, 0.175f);
                GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").transform.localScale = new Vector3(10.80f, 19.20f, 1);
            } else {
                cube.transform.localScale = new Vector3(0.175f, 1, 0.099f);
                GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").transform.localScale = new Vector3(19.20f, 10.80f, 1);
            }

            //hide icon and title
            GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/ICON").active = false;
            GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/TITLE").active = false;

            MelonLogger.Msg("Setup Game Objects.");
        }

        private String randImage() {
            if (folder_dir == null) // One way to set up an image.
            {
                folder_dir = default_dir;
            }
            string[] pics = Directory.GetFiles(folder_dir, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpeg")).ToArray();
            if (pics.Length == 0) return null;
            int randPic = new Il2CppSystem.Random().Next(0, pics.Length);
            return pics[randPic].ToString();
            /* Option 2 To have it set up, basically if anything fails in the try, the catch defaults to the default_dir
             try
            {
                try
                {
                    string[] pics = Directory.GetFiles(folder_dir, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpeg")).ToArray();
                    if (pics.Length == 0) return null;
                    int randPic = new Il2CppSystem.Random().Next(0, pics.Length);
                    return pics[randPic].ToString();
                }
                catch (System.Exception)
                {
                    string[] pics = Directory.GetFiles(default_dir, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpeg")).ToArray();
                    if (pics.Length == 0) return null;
                    int randPic = new Il2CppSystem.Random().Next(0, pics.Length);
                    return pics[randPic].ToString();
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Error(e);
            }

            return "";*/
        }


    }
}