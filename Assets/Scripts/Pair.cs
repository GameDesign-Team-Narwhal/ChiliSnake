using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//sigh
//the Tuple class is not in Unity, so I have to use this as a substitute
public class Pair<Left, Right>
{
    public Left left;

    public Right right;

    public Pair(Left left, Right right)
    {
        this.left = left;
        this.right = right;
    }

    public override string ToString()
    {
        StringBuilder resultBuilder = new StringBuilder();
        resultBuilder.Append("Pair: {");
        resultBuilder.Append(left.ToString());
        resultBuilder.Append(", ");
        resultBuilder.Append(right.ToString());
        resultBuilder.Append('}');

        return resultBuilder.ToString();
    }
}

