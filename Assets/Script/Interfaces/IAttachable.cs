using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;

namespace Interfaces
{
    public interface IAttachable
    {
        bool ShowConnectors { get; set; }

        IDictionary<Side,IConnector> Connectors {get;}

        void OnConnectorHoverStart(IConnector onConnector);
        void OnConnectorHoverEnd(IConnector onConnector);

        void DetachAll();
    }
}
