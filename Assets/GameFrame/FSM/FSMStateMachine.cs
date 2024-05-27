using System;
using System.Collections.Generic;

public class FSMStateMachine
{
    public IFSMState CurState;

    private Dictionary<IFSMState, List<TransitionModel>> allStateTransitionDict = new ();

    private List<TransitionModel> singleStateTransitionList = new ();

    private List<TransitionModel> tempList;

    public void ChangeStage(IFSMState iFsmState)
    {
        if(iFsmState == null || CurState == iFsmState) return;
        CurState?.Exit();
        CurState = iFsmState;
        if (allStateTransitionDict.TryGetValue(iFsmState, out var value))
        {
            tempList = value;
        }
        CurState.Enter();
    }

    public void Update()
    {
        CurState?.Update();

        foreach (var iTransitionModel in singleStateTransitionList)
        {
            if (iTransitionModel.Transition.Invoke())
            {
                ChangeStage(iTransitionModel.To);
                return;
            }
        }

        if (tempList != null)
        {
            foreach (var iTransitionModel in tempList)
            {
                if (iTransitionModel.Transition.Invoke())
                {
                    ChangeStage(iTransitionModel.To);
                    return;   
                }
            }
        }
    }

    public void AddStateTransition(IFSMState from, IFSMState to, Func<bool> transition)
    {
        if (allStateTransitionDict.ContainsKey(from))
        {
            allStateTransitionDict[from].Add(new TransitionModel(to, transition));
        }
        else
        {
            allStateTransitionDict.Add(from, new List<TransitionModel>
            {
                new (to, transition)
            });
        }
    }

    public void AddSingleTransition(IFSMState to, Func<bool> transition)
    {
        singleStateTransitionList.Add(new TransitionModel(to, transition));
    }
    
    public class TransitionModel
    {
        public IFSMState To { get; private set; }

        public Func<bool> Transition;

        public TransitionModel(IFSMState to, Func<bool> transition)
        {
            To = to;
            Transition = transition;
        }
    }
}
