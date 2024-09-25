using System;
using System.Collections;
using System.Collections.Generic;

public class MyCollection : IEnumerable<string>
{
    private List<string> items = new List<string>();

    public void Add(string item)
    {
        items.Add(item);
    }

    public IEnumerator<string> GetEnumerator()
    {
        foreach (string item in items)
        {
            yield return item;
        }
    }





    IEnumerator IEnumerable.GetEnumerator()
    {
        Console.WriteLine(22222222);
        return this.GetEnumerator();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine(1.1 * 100);
    }
}
