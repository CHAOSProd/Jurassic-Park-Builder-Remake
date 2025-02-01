using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : StateMachineBehaviour
{
    public string _animationToListen;
    public string _animationToListen2;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        if(stateInfo.IsName(_animationToListen))
        {
            Debug.Log(animator.gameObject.name);
            animator.GetComponent<AnimationEventsListener>().OnAnimationEnded();
        }

        if(stateInfo.IsName(_animationToListen2))
        {
            Debug.Log(animator.gameObject.name);
            animator.GetComponent<AnimationEventsListener>().OnEatAnimationEnded();
        }
    }
}