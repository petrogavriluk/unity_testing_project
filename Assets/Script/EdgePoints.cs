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

        public static Vector3 GetEdgePoint(this Renderer renderer, Side side, float shiftOut = 0f)
        {
            switch (side)
            {
                case Side.YMin:
                    return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Min, EdgePosition.Mid, shiftOut);
                case Side.YMax:
                    return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Max, EdgePosition.Mid, shiftOut);
                case Side.XMin:
                    return renderer.GetEdgePoint(EdgePosition.Min, EdgePosition.Mid, EdgePosition.Mid, shiftOut);
                case Side.XMax:
                    return renderer.GetEdgePoint(EdgePosition.Max, EdgePosition.Mid, EdgePosition.Mid, shiftOut);
                case Side.ZMin:
                    return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Mid, EdgePosition.Min, shiftOut);
                case Side.ZMax:
                    return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Mid, EdgePosition.Max, shiftOut);
            }
            return renderer.GetEdgePoint(EdgePosition.Mid, EdgePosition.Mid, EdgePosition.Mid);
        }

        public static Vector3 GetEdgePoint(this Renderer renderer, EdgePosition xPos, EdgePosition yPos, EdgePosition zPos, float shiftOut = 0f)
        {
            if (!renderer)
                return new Vector3();

            return renderer.bounds.GetEdgePoint(xPos, yPos, zPos, shiftOut);
        }
        public static Vector3 GetEdgePoint(this Bounds bounds, EdgePosition xPos, EdgePosition yPos, EdgePosition zPos, float shiftOut = 0.0f)
        {
            Vector3 res = new Vector3();

            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            Vector3 mid = bounds.center;

            switch (xPos)
            {
                case EdgePosition.Min:
                    res.x = min.x - shiftOut;
                    break;
                case EdgePosition.Mid:
                    res.x = mid.x;
                    break;
                case EdgePosition.Max:
                    res.x = max.x + shiftOut;
                    break;
            }
            switch (yPos)
            {
                case EdgePosition.Min:
                    res.y = min.y - shiftOut;
                    break;
                case EdgePosition.Mid:
                    res.y = mid.y;
                    break;
                case EdgePosition.Max:
                    res.y = max.y + shiftOut;
                    break;
            }
            switch (zPos)
            {
                case EdgePosition.Min:
                    res.z = min.z - shiftOut;
                    break;
                case EdgePosition.Mid:
                    res.z = mid.z;
                    break;
                case EdgePosition.Max:
                    res.z = max.z + shiftOut;
                    break;
            }
            return res;
        }

        public static Vector3 _GetFaceMidPoint(this Renderer renderer, Axis axis, EdgePosition position)
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

        public static Vector3[] GetBoxFrameLines(this Bounds bounds, float shiftOut = 0f)
        {
            Vector3[] points = new Vector3[]
            {
                bounds.GetEdgePoint(EdgePosition.Min, EdgePosition.Min, EdgePosition.Min, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Min, EdgePosition.Max, EdgePosition.Min, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Min, EdgePosition.Max, EdgePosition.Max, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Min, EdgePosition.Min, EdgePosition.Max, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Min, EdgePosition.Min, EdgePosition.Min, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Max, EdgePosition.Min, EdgePosition.Min, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Max, EdgePosition.Max, EdgePosition.Min, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Min, EdgePosition.Max, EdgePosition.Min, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Max, EdgePosition.Max, EdgePosition.Min, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Max, EdgePosition.Max, EdgePosition.Max, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Min, EdgePosition.Max, EdgePosition.Max, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Max, EdgePosition.Max, EdgePosition.Max, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Max, EdgePosition.Min, EdgePosition.Max, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Min, EdgePosition.Min, EdgePosition.Max, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Max, EdgePosition.Min, EdgePosition.Max, shiftOut),
                bounds.GetEdgePoint(EdgePosition.Max, EdgePosition.Min, EdgePosition.Min, shiftOut),
            };
            return points;
        }
    }
}
