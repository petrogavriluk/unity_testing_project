using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Interfaces;

namespace Helpers
{
    public enum EdgePosition
    {
        Min,
        Mid,
        Max
    };

    public enum Axis
    {
        X,
        Y,
        Z
    }
    public static class EdgePoints
    {
        public static Side GetOppositeSide(this Side side)
        {
            switch (side)
            {
                case Side.YMin:
                    return Side.YMax;
                case Side.YMax:
                    return Side.YMin;
                case Side.XMin:
                    return Side.XMax;
                case Side.XMax:
                    return Side.XMin;
                case Side.ZMin:
                    return Side.ZMax;
                case Side.ZMax:
                    return Side.ZMin;
            }
            return 0;
        }

        public static Vector3 GetEdgePoint(this Renderer renderer, Side side)
        {
            switch (side)
            {
                case Side.YMin:
                    return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Min, EdgePosition.Mid);
                case Side.YMax:
                    return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Max, EdgePosition.Mid);
                case Side.XMin:
                    return renderer.GetEdgePoint(EdgePosition.Min, EdgePosition.Mid, EdgePosition.Mid);
                case Side.XMax:
                    return renderer.GetEdgePoint(EdgePosition.Max, EdgePosition.Mid, EdgePosition.Mid);
                case Side.ZMin:
                    return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Mid, EdgePosition.Min);
                case Side.ZMax:
                    return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Mid, EdgePosition.Max);
            }
            return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Mid, EdgePosition.Mid);
        }

        public static Vector3 GetEdgePoint(this Renderer renderer, EdgePosition xPos, EdgePosition yPos, EdgePosition zPos)
        {
            Vector3 res = new Vector3();
            if (!renderer)
                return res;

            Vector3 min = renderer.bounds.min;
            Vector3 max = renderer.bounds.max;
            Vector3 mid = renderer.bounds.center;

            switch (xPos)
            {
                case EdgePosition.Min:
                    res.x = min.x;
                    break;
                case EdgePosition.Mid:
                    res.x = mid.x;
                    break;
                case EdgePosition.Max:
                    res.x = max.x;
                    break;
            }
            switch (yPos)
            {
                case EdgePosition.Min:
                    res.y = min.y;
                    break;
                case EdgePosition.Mid:
                    res.y = mid.y;
                    break;
                case EdgePosition.Max:
                    res.y = max.y;
                    break;
            }
            switch (zPos)
            {
                case EdgePosition.Min:
                    res.z = min.z;
                    break;
                case EdgePosition.Mid:
                    res.z = mid.z;
                    break;
                case EdgePosition.Max:
                    res.z = max.z;
                    break;
            }
            return res;
        }

        public static Vector3 _GetFaceMidPoint(this Renderer renderer, Axis axis,EdgePosition position)
        {
            switch (axis)
            {
                case Axis.X:
                    return renderer.GetEdgePoint(position, EdgePosition.Mid, EdgePosition.Mid);
                case Axis.Y:
                    return renderer.GetEdgePoint( EdgePosition.Mid,position, EdgePosition.Mid);
                default:
                    return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Mid, position);
            }
        }



        public static Bounds GetBounds(this MonoBehaviour obj)
        {
            return obj is IObjectWithRenderer ? ((IObjectWithRenderer)obj).ObjectRenderer.bounds : obj.gameObject.GetBounds();
        }
        public static Bounds GetBounds(this GameObject gameObject)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            return renderer ? renderer.bounds : new Bounds();
        }
    }
}
