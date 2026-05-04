using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> connections;
    public float gScore;
    public float hScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float FScore()
    {
        
            return gScore + hScore;
        
    }
}
