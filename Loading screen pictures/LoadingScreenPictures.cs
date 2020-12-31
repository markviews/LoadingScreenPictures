﻿using Il2CppSystem;
using MelonLoader;
using System.IO;
using UnityEngine;

namespace Loading_screen_pictures {
    public class LoadingScreenPictures : MelonMod {

        GameObject screen, cube;
        Texture lastTexture;
        Renderer screenRender, pic;
        static String folder_dir;
        bool initUI = false;

        public override void OnApplicationStart() {
            MelonPrefs.RegisterCategory("LoadingScreenPictures", "Loading Screen Pictures");
            String default_dir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\VRChat";
            MelonPrefs.RegisterString("LoadingScreenPictures", "directory", default_dir, "Folder to get pictures from");
            folder_dir = MelonPrefs.GetString("LoadingScreenPictures", "directory");
        }

        public override void VRChat_OnUiManagerInit() {
            screen = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainScreen");
            setup();
            initUI = true;
        }

        public override void OnUpdate() {
            if (lastTexture != null)
                if (lastTexture != screenRender.material.mainTexture) {
                   lastTexture = screenRender.material.mainTexture;
                  changePic();
                }
        }

        public override void OnLevelWasInitialized(int level) {
            if (initUI && screen == null) 
                setup();
        }

        public void changePic() {
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

        public void setup() {
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

            MelonLogger.Log("Setup Game Objects.");
        }

        public static String randImage() {
            FileInfo[] Files = new DirectoryInfo(folder_dir).GetFiles("*.png");
            if (Files.Length == 0) {
                string[] dirs = Directory.GetDirectories(folder_dir, "*", SearchOption.TopDirectoryOnly);
                if (dirs.Length == 0) return null;
                int randDir = new Il2CppSystem.Random().Next(0, dirs.Length);
                FileInfo[] dirFiles = new DirectoryInfo(dirs[randDir]).GetFiles("*.png");
                int randPic2 = new Il2CppSystem.Random().Next(0, Files.Length);
                return dirFiles[randPic2].ToString();
            }
            int randPic = new Il2CppSystem.Random().Next(0, Files.Length);
            return Files[randPic].ToString();
        }


    }
}