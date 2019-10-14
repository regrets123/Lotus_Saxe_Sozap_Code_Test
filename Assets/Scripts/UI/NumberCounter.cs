using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NumberCounter : StateMachineBehaviour
{   //Countdown Text animation behaviour for start of game.
    private float _counter = 3;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RoundsManager manager = RoundsManager.Instance;
        if (_counter == 1)
        {
            manager.textAnimator.GetComponent<Text>().text = "Start";
            manager.textAnimator.GetComponent<Animator>().SetTrigger("LastState");
            _counter = 3;
        }
        else
        {           
            manager.textAnimator.GetComponent<Text>().text = Convert.ToString(_counter);
            manager.textAnimator.GetComponent<Animator>().SetTrigger("NextNumber");
            _counter -= 1;
        }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RoundsManager.Instance.textAnimator.GetComponent<Text>().text = Convert.ToString(_counter);
    }
}
