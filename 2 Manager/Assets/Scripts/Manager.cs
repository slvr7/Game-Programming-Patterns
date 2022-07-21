using System;
using System.Collections.Generic;

// Here is simple Manager base class
// It's probably not a good fit for 
// production, but it's useful for
// creating quick, simple managers
// that support the basics of 
// managing lifecycles and queries.

// We're using a generic abstract class here...
// Why and what does that mean?

// Generic means that the Manager type can be "parameterized"
// with different types. You've probably seen generics
// before when using `List` or `Dictionary`. When you 
// create a `List` you need to tell the compiler what
// it's a list **of** (e.g. List<float> is a List of floats).
// Likewise we need to tell the Manager what kind of objects
// it's responsible for managing. Generics are too big a topic
// for a comment, so ask me in class or at office hours if
// you're not familiar with them. Here's a quick tutorial
// to help with that: https://bit.ly/2AozQQt

// `abstract` means that you can't create an instance
// of this class directly. You need to create 
// concrete (i.e. regular) sub-classes of the class
// if you want instances of these running around the world.
// Typically you use `abstract` classes when you don't have
// enough information about how something is going to be
// used.

public abstract class Manager<T>
{
    // A helper property to let us know how many objects
    // are being managed
    public int Population => ManagedObjects.Count;

    protected readonly List<T> ManagedObjects = new List<T>();

    // We can use a manager to manage just about anything
    // so we can't know how create objects at this point - subclasses
    // who know what type they are managing will need to do that.
    // We mark this method as abstract so sub-classes will have to
    // implement the specifics of creation.
    protected abstract T CreateObject();
    
    // override this if you need to do anything when you destroy
    // a component besides removing it from the manager's list
    protected virtual void DestroyObject(T obj) {}

    // This is the public `interface` for creating/destroying
    // managed objects. We use it to call our internal methods
    // and to make sure sub-classes don't have to keep reimplementing
    // the code for adding and removing elements from the list

    // *** IMPORTANT ***: When you're managing the lifecycle of objects
    // in a manager you ***MUST NOT*** create or destroy objects  
    // except through the Create and Destroy methods of the manager. 
    public T Create()
    {
        var obj = CreateObject();
        ManagedObjects.Add(obj);
        return obj;
    }

    public void Destroy(T obj)
    {
        DestroyObject(obj);
        ManagedObjects.Remove(obj);
    }
    
    // Predicates are a fancy term for a function that takes an object 
    // and returns true or false if that object meets some condition.
    // For example you might have a predicate named IsOdd that takes
    // an integer and returns true or false if the integer is odd.
    // Here we use them as a general way to make queries about the
    // objects being managed.
    public T Find(Predicate<T> predicate)
    {
        return ManagedObjects.Find(predicate);
    }

    public List<T> FindAll(Predicate<T> predicate)
    {
        return ManagedObjects.FindAll(predicate);
    }

}