using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : IResettable
{
    public List<T> members = new List<T>();
    public HashSet<T> unavailable = new HashSet<T>();
    IFactory<T> factory;

    public Pool(IFactory<T> factory, int size)
    {
        this.factory = factory;
        for (int i = 0; i < size; i++)
        {
            Create();
        }
    }

    T Create()
    {
        T member = factory.Create();
        members.Add(member);
        return member;
    }


    public T Allocate()
    {
        for (int i = 0; i < members.Count; i++)
        {
            if (!unavailable.Contains(members[i]))
            {
                unavailable.Add(members[i]);
                return members[i];
            }
        }
        T newMembers = Create();
        unavailable.Add(newMembers);
        return newMembers;
    }

    public void Release(T member)
    {
        member.Reset();
        unavailable.Remove(member);
    }
}
