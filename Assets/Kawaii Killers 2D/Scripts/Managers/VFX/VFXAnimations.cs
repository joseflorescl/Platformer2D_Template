using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VFXAnimations : VFXPlayable
{
    [SerializeField] private GameObject[] animationPrefabArray;
    Animator[] animatorInstanceArray;

    int currentAnimatorIdx;
    Animator currentAnimation;   

    protected override void Init()
    {

        animatorInstanceArray = new Animator[animationPrefabArray.Length];
        for (int i = 0; i < animationPrefabArray.Length; i++)
        {
            var obj = Instantiate(animationPrefabArray[i], transform);
            animatorInstanceArray[i] = obj.GetComponentInChildren<Animator>();
        }
        currentAnimatorIdx = 0;
    }

    public override void PlayOneShot(Vector3 position)
    {
        currentAnimation = animatorInstanceArray[currentAnimatorIdx];
        currentAnimation.transform.position = position;
        currentAnimation.SetTrigger("PlayOneShot");
        currentAnimatorIdx = (currentAnimatorIdx + 1) % animatorInstanceArray.Length;
    }

    public override void PlayOneShot()
    {
        currentAnimation = animatorInstanceArray[currentAnimatorIdx];        
        currentAnimation.SetTrigger("PlayOneShot");
        currentAnimatorIdx = (currentAnimatorIdx + 1) % animatorInstanceArray.Length;
    }

    public override void Play()
    {
        currentAnimation = animatorInstanceArray[currentAnimatorIdx];
        currentAnimation.SetTrigger("Play");
        currentAnimatorIdx = (currentAnimatorIdx + 1) % animatorInstanceArray.Length;
    }

    public override void Stop()
    {
        currentAnimation.SetTrigger("Stop");
    }

    public void PlayOneShotInReverse()
    {
        currentAnimation = animatorInstanceArray[currentAnimatorIdx];
        currentAnimation.SetTrigger("PlayOneShotInReverse");
        currentAnimatorIdx = (currentAnimatorIdx + 1) % animatorInstanceArray.Length;       
    }

    public void PlayInReverse()
    {
        currentAnimation = animatorInstanceArray[currentAnimatorIdx];
        currentAnimation.SetTrigger("PlayInReverse");
        currentAnimatorIdx = (currentAnimatorIdx + 1) % animatorInstanceArray.Length;       
    }

    
}
