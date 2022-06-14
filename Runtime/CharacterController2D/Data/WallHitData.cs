using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieGabo.CharacterController2D.Data
{
    public class WallHitData
    {
        public bool hittingAnyWall = false;

        public bool leftHitting = false;
        public bool rightHitting = false;

        public bool upperRight = false;
        public float upperRightHitAngle;

        public bool lowerRight = false;
        public float lowerRightHitAngle;

        public bool centerRight = false;
        public float centerRightHitAngle;

        public bool upperLeft = false;
        public float upperLeftHitAngle;

        public bool lowerLeft = false;
        public float lowerLeftHitAngle;

        public bool centerLeft = false;
        public float centerLeftHitAngle;
    }
}
