using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public class Controllers
    {
        private static readonly ShapeController shapeControllerField = UnityEngine.Object.FindObjectOfType<ShapeController>();
        private static readonly MoveHelper moveHelper = UnityEngine.Object.FindObjectOfType<MoveHelper>();
        private static readonly AnimationController animationControllerField = UnityEngine.Object.FindObjectOfType<AnimationController>();
        private static readonly CameraController cameraControllerField = UnityEngine.Object.FindObjectOfType<CameraController>();
        private static readonly ContextMenuController contextMenuController = UnityEngine.Object.FindObjectOfType<ContextMenuController>();
        private static readonly Creator creator = UnityEngine.Object.FindObjectOfType<Creator>();

        public static ShapeController ShapeControllerInstance
        {
            get
            {
                return shapeControllerField;
            }
        }

        public static AnimationController AnimationControllerInstance
        {
            get { return animationControllerField; }
        }

        public static CameraController CameraControllerInstance
        {
            get { return cameraControllerField; }
        }

        public static ContextMenuController ContextMenuControllerInstance
        {
            get { return contextMenuController; }
        }

        public static MoveHelper MoveHelperInstance
        {
            get { return moveHelper; }
        }

        public static Creator CreatorInstance
        {
            get { return creator; }
        }

    }
}
