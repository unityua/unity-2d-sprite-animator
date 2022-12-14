using UnityEngine;
using System;

namespace VoizehSpriteAnimator
{
    [Serializable]
    public class SpriteAnimation
    {
        public Sprite this[int index] => frames[index];
        public int FramesCount => frames.Length;
        public Sprite FirstFrame => frames[0];
        public Sprite LastFrame => frames[frames.Length - 1];

        public string Name => name;
        public int HashName => nameHash;
        public bool Loop => loop;


        [SerializeField] private string name = string.Empty;
        [SerializeField] private int nameHash;
        [Space]
        [SerializeField] private Sprite[] frames;
        [Space]
        [SerializeField] private float frameLenght = 0.1f;
        [SerializeField] private FrameRateType frameLenghtType = FrameRateType.Constant;
        [SerializeField] private AnimationCurve frameLenghtCurve = AnimationCurve.Constant(0f, 1f, 1f);
        [Space]
        [SerializeField] private bool loop = true;

        public enum FrameRateType { Constant, Curved }

        public SpriteAnimation() { }

        public SpriteAnimation(string name, Sprite[] frames, FrameRateType frameLenghtType, float frameLenght,
            AnimationCurve frameLenghtCurve, bool loop)
        {
            this.name = name;
            nameHash = Animator.StringToHash(name);

            this.frames = frames;
            this.frameLenghtType = frameLenghtType;
            this.frameLenght = frameLenght;
            this.frameLenghtCurve = frameLenghtCurve;

            this.loop = loop;
        }

        public float GetFrameLenght(int currentFrame)
        {
            if (frameLenghtType == FrameRateType.Constant)
                return frameLenght;

            return frameLenght * frameLenghtCurve.Evaluate((float)currentFrame / frames.Length);
        }

#if UNITY_EDITOR
        public SpriteAnimation UpdateValues_Editor()
        {
            return new SpriteAnimation(name, frames, frameLenghtType, frameLenght, frameLenghtCurve, loop);
        }
#endif
    }
}