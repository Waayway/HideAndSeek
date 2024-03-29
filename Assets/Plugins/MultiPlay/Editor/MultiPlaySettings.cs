using UnityEngine;
using UnityEngine.Serialization;

namespace MultiPlay
{
    //[CreateAssetMenu(menuName = "MultiPlay/Settings")] //Uncomment to create one object to control the global settings.
    internal class MultiPlaySettings : ScriptableObject
    {
        [FormerlySerializedAs("maxNumberOfClients")]
        [Range(1, 30)]
        [Tooltip("Maximum number of clients")]
        public int maxNumberOfClones;

        [Tooltip("Default Project Clones Path")]
        public string clonesPath;

        [Tooltip("Enabeling this will increase the project size but will transfer project data like startup scene")]
        public bool copyLibrary;

        private void OnEnable()
        {
            maxNumberOfClones = 3;
            copyLibrary = false;

            if (string.IsNullOrEmpty(clonesPath))
                clonesPath = Application.persistentDataPath;
        }
    }
}