using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace Helpers
{
    public enum Side
    {
        YMin,
        Bottom = YMin,
        YMax,
        Top = YMax,
        XMin,
        Left = XMin,
        XMax,
        Right = XMax,
        ZMin,
        Front = ZMin,
        ZMax,
        Back = ZMax,
    }

    public static class ConnectionHelper
    {
        public static IEnumerable<IAttachable> GetAllConnected(IAttachable start)
        {
            HashSet<IAttachable> result = new HashSet<IAttachable>();
            Queue<IAttachable> unvisited = new Queue<IAttachable>();
            unvisited.Enqueue(start);
            while(unvisited.Count>0)
            {
                IAttachable attachable = unvisited.Dequeue();
                result.Add(attachable);
                foreach (var connector in attachable.Connectors.Values)
                {
                    if(connector.ConnectedObject !=null && !result.Contains(connector.ConnectedObject.Owner))
                    {
                        unvisited.Enqueue(connector.ConnectedObject.Owner);
                    }
                }
            }
            return result;
        }
    }
}
