﻿/////////////////////////////////////////////////////////////
/// Este script es para que si estas en la escena que querés
/// probar, detecte si pasaste primero por la carga.
/// pregunta si esta el Main, si no está... te lleva al Load.
/// Esto es basicamente para que no tengas que abrir el Load
/// cada vez que queres testear algo
/////////////////////////////////////////////////////////////
namespace Tools.Testing
{
    using UnityEngine;
    using System.Linq;
    public class DirectBegin_Scene : MonoBehaviour
    {
        public JumpData data;
        public bool LockMouse;
        [SerializeField] LocalPackageLoadComponent packageToLoad;
        public void Awake()
        {
            if (Main.instance == null) // si entra acá es porque nunca entro a la escena de carga
            {
                DontDestroyOnLoad(this.gameObject);
                data.SceneToJump = Scenes.GetActiveSceneName();
                Scenes.Load_Load();
            }
            else
            {
                //Debug.Log("entro mas veces aca");
                if (LockMouse)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                packageToLoad.LoadComponents();
                Destroy(this.gameObject);
            }

            
        }
    }

    [System.Serializable]
    public class JumpData
    {
        [SerializeField] string sceneToJump = "default";
        internal string SceneToJump { get => sceneToJump; set => sceneToJump = value; }
        
    }
}
