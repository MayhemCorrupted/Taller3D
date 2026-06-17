using UnityEngine;

public interface ISwitchVariantLogic
{
    void ProcessSwitch(int indexClicked, bool[] currentStates);
}

public struct PuzzleVariant
{
    public int variantID;
    public ISwitchVariantLogic logicInstance;

    public PuzzleVariant(int id, ISwitchVariantLogic logic)
    {
        variantID = id;
        logicInstance = logic;
    }
}

public abstract class BaseSwitchLogic : ISwitchVariantLogic
{
    public abstract void ProcessSwitch(int indexClicked, bool[] currentStates);

    protected void Toggle(bool[] states, params int[] indices)
    {
        foreach (int i in indices)
        {
            if (i >= 0 && i < states.Length)
            {
                states[i] = !states[i];
            }
        }
    }
}

public class Variant1Logic : BaseSwitchLogic
{
    public override void ProcessSwitch(int indexClicked, bool[] states)
    {
        bool[] previousStates = (bool[])states.Clone();

        switch (indexClicked)
        {
            case 0: Toggle(states, 0, 4); break;
            case 1: Toggle(states, 1); break;
            case 2:
                if (states[2]) return; 
                Toggle(states, 2, 0, 3, 5);
                break;
            case 3: Toggle(states, 3, 1); break;
            case 4: Toggle(states, 4, 0); break;
            case 5: Toggle(states, 5, 3); break;
        }

    
        bool f5Changed = previousStates[4] != states[4];
        bool f2WentDown = previousStates[1] && !states[1];

        if (states[2] && (f5Changed || f2WentDown))
        {
            states[2] = false; 
        }
    }
}
public class Variant2Logic : BaseSwitchLogic
{
    private int moveCounter = 0;
    private bool isTrackingMoves = false;

    public override void ProcessSwitch(int indexClicked, bool[] states)
    {
        bool[] previousStates = (bool[])states.Clone();

        switch (indexClicked)
        {
            case 0: Toggle(states, 0, 5); break;
            case 1: Toggle(states, 1); break;
            case 2:
                if (!states[1]) return; 
                Toggle(states, 2, 3, 0);
                break;
            case 3:
                if (!states[4]) return; 
                Toggle(states, 3, 2, 5);
                break;
            case 4: Toggle(states, 4); break;
            case 5: Toggle(states, 5, 0); break;
        }

        bool f2WentDown = previousStates[1] && !states[1];
        bool f5WentDown = previousStates[4] && !states[4];

        if (states[2] && f2WentDown) states[2] = false;
        if (states[3] && f5WentDown) states[3] = false;

        bool isF3OrF4Active = states[2] || states[3];

        if (isF3OrF4Active)
        {
            if (!isTrackingMoves)
            {
                isTrackingMoves = true;
                moveCounter = 2; 
            }
            else
            {
                moveCounter--;

                if (moveCounter <= 0)
                {
                    if (!(states[2] && states[3])) 
                    {
                        states[0] = states[2];
                        states[5] = states[3];
                        states[1] = false;
                        states[2] = false;
                        states[3] = false;
                        states[4] = false;
                    }
                    isTrackingMoves = false;
                }
            }
        }
        else
        {
            isTrackingMoves = false;
        }
    }
}