using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liste : MonoBehaviour {
    public int val;
    public Liste next, prev;
    public Liste first, last;
	public int getSecondBiggest()
    {
        Liste cur = this.first;     //Incase not first
        Liste n1 = cur, n2 = cur.next;
        if(cur.val < cur.next.val)
        {
            n1 = cur.next;
            n2 = cur;
        }
        cur = cur.next;
        while(cur.next != null)
        {
            cur = cur.next;
            if(cur.val >= n1.val)
            {
                n1 = cur;
                n2 = n1;
                continue;
            }
            if(cur.val >= n2.val)
            {
                n2 = cur;
                continue;
            }
        }
        return n2.val;
    }

    public int getSecontBiggestRec(Liste cur, Liste n1, Liste n2)
    {
        if(cur == null)
        {
            return this.first.getSecontBiggestRec(this.first, null, null);
        }
        if(n1 == null || n2 == null)
        {
            n1 = cur;
            n2 = cur.next;
            if (cur.val < cur.next.val)
            {
                n1 = cur.next;
                n2 = cur;
            }
            return cur.next.getSecontBiggestRec(cur.next, n1, n2);
        }
        if(cur.next == null)
        {
            return n2.val;
        }
        cur = cur.next;
        if (cur.val >= n1.val)
        {
            n1 = cur;
            n2 = n1;
            return cur.getSecontBiggestRec(cur, n1, n2);
        }
        if (cur.val >= n2.val)
        {
            n2 = cur;
            return cur.getSecontBiggestRec(cur, n1, n2);
        }
        return this.val;
    }

}
