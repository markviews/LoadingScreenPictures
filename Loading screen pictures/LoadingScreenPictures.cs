using Il2CppSystem;
using MelonLoader;
using System.IO;
using System.Security.Permissions;
using UnityEngine;

namespace Loading_screen_pictures {
    public class LoadingScreenPictures : MelonMod {

        GameObject screen;
        Texture lastTexture;
        Renderer screenRender;
        Renderer pic;
        int delay = 1;//delay for setup (set to 0 after setup)
        bool noPics = false;

        public override void OnUpdate() {

            if (delay != 0) {
                if (delay++ == 100) {
                    screen = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainScreen");
                    if (screen != null) {
                        setup();
                        delay = 0;
                    } else delay = 1;//not ready yet, start delay over
                }
                return;
            }

            if (noPics) return;

            if (lastTexture != screenRender.material.mainTexture) {
                lastTexture = screenRender.material.mainTexture;
                changePic();
            }
        }

        public override void OnLevelWasInitialized(int level) {
            if (noPics) setup();
        }

        public void changePic() {
            Texture2D texture = new Texture2D(2, 2);
            ImageConversion.LoadImage(texture, File.ReadAllBytes(randImage()));
            pic.material.mainTexture = texture;
        }

        public void setup() {

            //check if screenshots folder is empty
            String imageLink = randImage();
            if (imageLink == null) {
                noPics = true;
                MelonLogger.Log("No screenshots found in: " + Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\VRChat");
                return;
            }
            noPics = false;

            GameObject parentScreen = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN");
            screenRender = screen.GetComponent<Renderer>();
            lastTexture = screenRender.material.mainTexture;

            //create new image
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Plane);
            cube.transform.SetParent(parentScreen.transform);
            cube.transform.rotation = screen.transform.rotation;
            cube.transform.localPosition = new Vector3(0, 0, -0.19f);
            cube.transform.localScale = new Vector3(0.175f, 1, 0.099f);
            cube.GetComponent<Collider>().enabled = false;
            Texture2D texture = new Texture2D(2, 2);
            ImageConversion.LoadImage(texture, File.ReadAllBytes(imageLink));
            pic = cube.GetComponent<Renderer>();
            pic.material.mainTexture = texture;

            //disable origial picture
            screenRender.enabled = false;

            //resize frame
            GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").transform.localScale = new Vector3(19.20f, 10.80f, 1);

            //hide icon and title
            GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/ICON").active = false;
            GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/TITLE").active = false;

            MelonLogger.Log("Setup Game Objects.");
        }

        public static String randImage() {
            FileInfo[] Files = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\VRChat").GetFiles("*.png");
            if (Files.Length == 0) return null;
            int rand = new Il2CppSystem.Random().Next(0, Files.Length);
            return Files[rand].ToString();
        }


    }
}
