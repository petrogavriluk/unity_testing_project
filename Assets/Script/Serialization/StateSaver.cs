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
        [Serializable]
        private class State
        {
            public List<CubeSaveData> cubesData = new List<CubeSaveData>();
            public List<ConnectorSaveData> pointsData = new List<ConnectorSaveData>();
        }

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

        private readonly string fileName = "/SavedState{0}.json";

        private void Awake()
        {
            saveButton.onClick.AddListener(() => SaveState(saveDropdown.value + 1));
            loadButton.onClick.AddListener(() => LoadState(loadDropdown.value + 1));
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

            string json = JsonUtility.ToJson(state);
            File.WriteAllText(Application.persistentDataPath + String.Format(fileName, saveNumber), json);
        }

        public void LoadState(int saveNumber)
        {
            string json = File.ReadAllText(Application.persistentDataPath + String.Format(fileName, saveNumber));
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
        }
    }
}
