using UnityEngine;
using Helpers;

namespace Interfaces
{
    public interface IConnector : IObjectWithRenderer
    {
        IConnector ConnectedObject { get; set; }
        IAttachable Owner { get; set; }

        bool Hovered { get; set; }

        bool Visible { get; set; }

        Side GetSide();

        GameObject Body { get; }

        void Connect(IConnector connector);
        void Disconnect();
    }
}
