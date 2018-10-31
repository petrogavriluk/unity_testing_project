using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public static class Controllers
    {
        public static ShapeController ShapeControllerInstance { get; } = UnityEngine.Object.FindObjectOfType<ShapeController>();

        public static AnimationController AnimationControllerInstance { get; } = UnityEngine.Object.FindObjectOfType<AnimationController>();

        public static CameraController CameraControllerInstance { get; } = UnityEngine.Object.FindObjectOfType<CameraController>();

        public static ContextMenuController ContextMenuControllerInstance { get; } = UnityEngine.Object.FindObjectOfType<ContextMenuController>();

        public static MoveHelper MoveHelperInstance { get; } = UnityEngine.Object.FindObjectOfType<MoveHelper>();

        public static Creator CreatorInstance { get; } = UnityEngine.Object.FindObjectOfType<Creator>();

    }
}
