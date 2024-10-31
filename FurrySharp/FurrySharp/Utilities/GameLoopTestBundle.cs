using System;
using System.Collections;
using System.Collections.Generic;

namespace FurrySharp.Utilities;

public class GameLoopTestBundle
{
    /*
     * My thought was that multiple routines could be created and encapsulated in structs, to create routines that were
     * like, do this before the game loop does input, then do this, then do this other thing on post update, but I'm
     * not sure that will be necessary!
     */
    public IEnumerator TestRoutine;
    public bool Done;
    public bool Passed => Done && (TestRoutine.Current?.Equals(true) ?? false);

    private float currentWait;
    private Func<bool> currentPredicate;

    public void DoTest()
    {
        if (currentPredicate != null)
        {
            if (currentPredicate.Invoke())
            {
                currentPredicate = null;
            }
            else
            {
                return;
            }
        }
        else if (currentWait > 0f)
        {
            currentWait -= GameTimes.DeltaTime;
            if (currentWait <= 0f)
            {
                currentWait = 0f;
            }
            else
            {
                return;
            }
        }

        if (TestRoutine.MoveNext() == false)
        {
            Done = true;
        }
        else
        {
            var current = TestRoutine.Current;

            // Switch statement based on type of current
            switch (current)
            {
                case TimeSpan timeSpan:
                    // Wait for the specified time
                    currentWait = (float)timeSpan.TotalSeconds;
                    break;
                case Func<bool> predicate:
                    // Wait for the predicate to be true
                    currentPredicate = predicate;
                    break;
            }
        }
    }
}