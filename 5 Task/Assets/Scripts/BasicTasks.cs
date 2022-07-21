using System;
using GPP;

// Basic Task
/// This task will run the provided action endlessly
/// (or until it or the collection it's in is stopped)
public class Do : Task
{
    private readonly Action _work;

    public Do(Action work)
    {
        _work = work;
    }

    internal override void Update()
    {
        _work();
    }
}
