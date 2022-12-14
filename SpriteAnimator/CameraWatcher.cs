using UnityEngine;

namespace VoizehSpriteAnimator
{
    public class CameraWatcher
    {
        private readonly Camera mainCamera;

        private Vector2 minBounds;
        private Vector2 maxBounds;

        public CameraWatcher(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
        }

        public bool IsPointVisible(Vector2 position)
        {
            return position.x >= minBounds.x && position.x <= maxBounds.x
                && position.y >= minBounds.y && position.y <= maxBounds.y;
        }

        public bool IsRectVisible(Vector2 worldPosition, Rect rect)
        {
            Vector2 rectCenter = worldPosition + rect.position;
            Vector2 rectMinPosition = rectCenter;

            rectMinPosition.x -= rect.width * 0.5f;
            rectMinPosition.y -= rect.height * 0.5f;

            Vector2 rectMaxPosition = rectMinPosition;
            rectMaxPosition.x += rect.width;
            rectMaxPosition.y += rect.height;

            return minBounds.x <= rectMaxPosition.x && maxBounds.x >= rectMinPosition.x
                && maxBounds.y >= rectMinPosition.y && minBounds.y <= rectMaxPosition.y;
        }

        public void UpdateCameraInfo()
        {
            Vector2 position = mainCamera.transform.position;

            float verticalHalfSize = mainCamera.orthographicSize;
            float horizontalHalfSize = verticalHalfSize * mainCamera.aspect;

            minBounds = position;
            minBounds.x -= horizontalHalfSize;
            minBounds.y -= verticalHalfSize;

            maxBounds = position;
            maxBounds.x += horizontalHalfSize;
            maxBounds.y += verticalHalfSize;
        }
    }
}