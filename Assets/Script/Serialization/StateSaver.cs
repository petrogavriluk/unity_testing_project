using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Helpers;

namespace Serialization
{

    public class StateSaver : MonoBehaviour
    {
        const int savesNumber = 3;
        [Serializable]
        private class State
        {
            public List<CubeSaveData> cubesData = new List<CubeSaveData>();
            public List<ConnectorSaveData> pointsData = new List<ConnectorSaveData>();
            public CameraSaveData cameraData = new CameraSaveData();
        }

        private readonly int spriteHeight = 200;
        private readonly int spriteWidth = 300;

        [SerializeField]
        Button saveButton;
        [SerializeField]
        Button loadButton;
        [SerializeField]
        Dropdown saveDropdown;
        [SerializeField]
        Dropdown loadDropdown;
        [SerializeField]
        private GameObject cubeCollectionParent;
        [SerializeField]
        private Sprite emptyStateSprite;
        [SerializeField]
        private Camera renderingCamera;
        private readonly Sprite[] saveImages = new Sprite[savesNumber];
        private readonly bool[] hasSave = new bool[savesNumber];

        private string fileName => Application.persistentDataPath + "/SavedState{0}.json";
        private string imgName => Application.persistentDataPath + "/SavedState{0}.dat";


        private void Awake()
        {
            saveButton.onClick.AddListener(() => SaveState(saveDropdown.value));
            loadButton.onClick.AddListener(() => LoadState(loadDropdown.value));
            loadDropdown.onValueChanged.AddListener((v) => LoadDropdownChanged(v));
            renderingCamera.enabled = false;
            InitDropdowns();
        }

        private void InitDropdowns()
        {
            saveDropdown.ClearOptions();
            loadDropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            for (int i = 0; i < savesNumber; i++)
            {
                hasSave[i] = false;
                Sprite sprite = null;
                if (File.Exists(String.Format(imgName, i)) && File.Exists(String.Format(fileName, i)))
                {
                    sprite = LoadSprite(String.Format(imgName, i));
                    if (sprite != null)
                    {
                        hasSave[i] = true;
                    }
                }
                saveImages[i] = sprite ?? emptyStateSprite;
                options.Add(new Dropdown.OptionData((i + 1).ToString(), saveImages[i]));
            }
            saveDropdown.AddOptions(options);
            loadDropdown.AddOptions(options);
            saveDropdown.RefreshShownValue();
            loadDropdown.RefreshShownValue();
            loadButton.interactable = hasSave[0];
        }

        private void LoadDropdownChanged(int v)
        {
            loadButton.interactable = hasSave[v];
        }

        private Sprite LoadSprite(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(spriteWidth, spriteHeight);
            texture.LoadRawTextureData(data);
            texture.Apply();
            Rect rect = new Rect(0, 0, spriteWidth, spriteHeight);
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            return sprite;
        }

        public void SaveState(int saveNumber)
        {
            State state = new State();
            foreach (Transform child in cubeCollectionParent.transform)
            {
                var cube = child.gameObject.GetComponent<CubeControl>();
                if (!cube || !cube.gameObject.activeSelf)
                    continue;

                state.cubesData.Add(cube.SaveState());
                foreach (var connector in cube.Connectors.Values)
                {
                    var point = connector as ConnectionPointController;
                    if (point != null && point.ConnectedObject != null)
                        state.pointsData.Add(point.SaveState());
                }
            }
            state.cameraData.position = Camera.main.transform.position;
            state.cameraData.rotation = Camera.main.transform.rotation;

            string json = JsonUtility.ToJson(state);
            File.WriteAllText(String.Format(fileName, saveNumber), json);

            CreateScreenShot(saveNumber);
            saveDropdown.options[saveNumber].image = saveImages[saveNumber];
            loadDropdown.options[saveNumber].image = saveImages[saveNumber];
            saveDropdown.RefreshShownValue();
            loadDropdown.RefreshShownValue();
            hasSave[saveNumber] = true;
            loadButton.interactable = hasSave[loadDropdown.value];
        }

        public void LoadState(int saveNumber)
        {
            string json = File.ReadAllText(String.Format(fileName, saveNumber));
            State state = JsonUtility.FromJson<State>(json);

            foreach (Transform child in cubeCollectionParent.transform)
            {
                var cube = child.gameObject.GetComponent<CubeControl>();
                if (!cube)
                    continue;

                Controllers.CreatorInstance.DestroyCubeObject(cube);
            }

            Dictionary<string, List<ConnectorSaveData>> pointsByOwnerMap = state.pointsData.
                GroupBy(p => p.Owner).
                ToDictionary(g => g.Key, g => g.ToList());

            Dictionary<string, CubeControl> createdObjects = new Dictionary<string, CubeControl>();
            foreach (var cube in state.cubesData)
            {
                CubeControl cubeObject = Controllers.CreatorInstance.GetCubeObject((PrimitiveType)cube.primitiveType);
                cubeObject.RestoreFromState(cube);
                if (pointsByOwnerMap.ContainsKey(cube.ID))
                {
                    foreach (var connector in pointsByOwnerMap[cube.ID])
                    {
                        if(createdObjects.ContainsKey(connector.ConnectedID))
                        {
                            CubeControl connectedCube = createdObjects[connector.ConnectedID];
                            Side side = (Side)connector.side;
                            cubeObject.Connectors[side].Connect(connectedCube.Connectors[side.GetOppositeSide()]);
                        }
                    }
                }
                createdObjects[cube.ID] = cubeObject;
            }

            Camera.main.transform.position = state.cameraData.position;
            Camera.main.transform.rotation = state.cameraData.rotation;
        }

        private void CreateScreenShot(int saveNumber)
        {
            string path = String.Format(imgName, saveNumber);
            RenderTexture texture = new RenderTexture(spriteWidth, spriteHeight,24);
            renderingCamera.targetTexture = texture;
            renderingCamera.Render();

            var activeTexture = RenderTexture.active;
            RenderTexture.active = texture;

            Rect rect = new Rect(0, 0, spriteWidth, spriteHeight);
            Texture2D image = new Texture2D(spriteWidth, spriteHeight);
            image.ReadPixels(rect, 0, 0);
            image.Apply();
            Sprite sprite = Sprite.Create(image, rect, new Vector2(0.5f, 0.5f));
            saveImages[saveNumber] = sprite;
            File.WriteAllBytes(path, image.GetRawTextureData());

            RenderTexture.active = activeTexture;
        }
    }
}
