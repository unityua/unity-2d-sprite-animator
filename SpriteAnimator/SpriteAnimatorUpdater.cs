using System.Collections.Generic;
using UnityEngine;

namespace VoizehSpriteAnimator
{
    public class SpriteAnimatorUpdater : MonoBehaviour
    {
        public static SpriteAnimatorUpdater Instance => instance;
        public static bool HasInstance => hasInstance;


        private static SpriteAnimatorUpdater instance;
        private static bool hasInstance;


        private List<SpriteAnimator> activeAnimators = new List<SpriteAnimator>(2048);
        private List<SpriteAnimator> animatorsToRemove = new List<SpriteAnimator>(128);

        private CameraWatcher cameraWatcher;

        private System.Predicate<SpriteAnimator> removeAnimatorsPredicate;

        private void Awake()
        {
            instance = this;
            hasInstance = true;

            cameraWatcher = new CameraWatcher(Camera.main);

            removeAnimatorsPredicate = IsAnimatorUnused;
        }

        public static void CreateInstance()
        {
            if (hasInstance == false)
            {
                new GameObject("Sprite Animator Updater", typeof(SpriteAnimatorUpdater));
            }
        }

        public void RegisterAnimator(SpriteAnimator animator)
        {
            activeAnimators.Add(animator);
        }

        public void AnimatorDestroyed(SpriteAnimator animator)
        {
            activeAnimators.Remove(animator);
        }

        public void AnimatorDeactivated(SpriteAnimator animator)
        {
            animatorsToRemove.Add(animator);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            cameraWatcher.UpdateCameraInfo();

            if (animatorsToRemove.Count > 0)
                RemoveDisabledAnimators();

            int animatorCount = activeAnimators.Count;

            for (int i = 0; i < animatorCount; i++)
            {
                SpriteAnimator animator = activeAnimators[i];

                animator.UpdateAnimation(deltaTime);

                if (animator.CullMode == SpriteAnimator.CullingMode.CullAutomatic)
                {
                    animator.Culled = cameraWatcher.IsRectVisible(animator.Position, animator.Bounds) == false;
                }
            }
        }

        private void RemoveDisabledAnimators()
        {
            activeAnimators.RemoveAll(removeAnimatorsPredicate);

            animatorsToRemove.Clear();
        }

        private bool IsAnimatorUnused(SpriteAnimator possibleAnimator)
        {
            return animatorsToRemove.Remove(possibleAnimator);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                hasInstance = false;
            }
        }
    }
}