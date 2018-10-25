using UnityEngine;
using Helpers;

namespace Interfaces
{
    public interface IConnector
    {
        IConnector ConnectedObject { get; set; }
        IAttachable Owner { get; set; }

        bool Hovered { get; set; }

        Side GetSide();

        GameObject Body { get; }

        void Connect(IConnector connector);
        void Disconnect();
    }
}
