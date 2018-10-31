using System;

namespace Serialization
{
    [Serializable]
    public class ConnectorSaveData
    {
        public string Owner = Guid.Empty.ToString();
        public string ConnectedID = Guid.Empty.ToString();
        public int side = 0;
    }
}
