using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YagizEraslan.EclipsedEcho
{
    public class MovingBackground : MonoBehaviour
    {
        private RawImage backgroundImage;
        private float x = 0.02f, y = 0.02f;

        private float referenceWidth = 1368f;
        private float referenceHeight = 2960f;

        void Start()
        {
            backgroundImage = GetComponent<RawImage>();
            AdjustUVRectForScreenSize();
        }

        void Update()
        {
            // Adjust UV offset for scrolling
            backgroundImage.uvRect = new Rect(backgroundImage.uvRect.position + new Vector2(x, y) * Time.deltaTime, backgroundImage.uvRect.size);
        }

        private void AdjustUVRectForScreenSize()
        {
            float currentAspectRatio = (float)Screen.width / Screen.height;

            float referenceAspectRatio = referenceWidth / referenceHeight;

            float scaleWidth = Screen.width / referenceWidth;
            float scaleHeight = Screen.height / referenceHeight;

            // Apply the scaling to the UV Rect's width and height
            // Adjust the UV Rect height to maintain aspect ratio across different resolutions
            if (currentAspectRatio > referenceAspectRatio)
            {
                // Wider screens, adjust height
                float newHeight = backgroundImage.uvRect.height * (referenceAspectRatio / currentAspectRatio);
                backgroundImage.uvRect = new Rect(backgroundImage.uvRect.x, backgroundImage.uvRect.y, backgroundImage.uvRect.width, newHeight);
            }
            else
            {
                // Taller screens or square, adjust width
                float newWidth = backgroundImage.uvRect.width * (currentAspectRatio / referenceAspectRatio);
                backgroundImage.uvRect = new Rect(backgroundImage.uvRect.x, backgroundImage.uvRect.y, newWidth, backgroundImage.uvRect.height);
            }
        }
    }
}