using UnityEngine;
using VoizehSpriteAnimator;


[CreateAssetMenu(fileName = "New Sprite Animator Controller", menuName = "SpriteAnimatorController")]
public class SpriteAnimatorController : ScriptableObject
{
    [SerializeField] private SpriteAnimation[] animations = new SpriteAnimation[0];
    
    public int AnimationsCount => animations.Length;

    public SpriteAnimation GetAnimationByIndex(int index)
    {
        return animations[index];
    }

    public bool HasAnimation(string name)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].Name == name)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasAnimation(int hash)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].HashName == hash)
            {
                return true;
            }
        }

        return false;
    }

    public SpriteAnimation GetAnimation(string name)
    {
        for (int i = 0; i < animations.Length; ++i)
        {
            if (animations[i].Name == name)
            {
                return animations[i];
            }
        }

        throw new System.Exception("No Such Animation With Name: " + name);
    }

    public SpriteAnimation GetAnimation(int hash)
    {
        for (int i = 0; i < animations.Length; ++i)
        {
            if (animations[i].HashName == hash)
            {
                return animations[i];
            }
        }

        throw new System.Exception("No Such Animation With Hash: " + hash);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < animations.Length; i++)
        {
            animations[i] = animations[i].UpdateValues_Editor();
        }
    }
#endif
}