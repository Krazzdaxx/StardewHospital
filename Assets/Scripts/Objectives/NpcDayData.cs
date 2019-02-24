﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// holds npc data for a specific day 
/// </summary>
[CreateAssetMenu]
public class NpcDayData : ScriptableObject
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Sprite sprite;
    // Use this for initialization
    public string talkToNode = "";
    public TextAsset scriptToLoad;
  
    public string GameObjectName;
    [Header("waypoints stuff")]
    public pointsVector3[] waypoints;
}
[System.Serializable]
public struct pointsVector3
{
    public string Name;

    public List<Vector3> location;

    public pointsVector3(string name, Vector3 location)
    {
        Name = name;
        this.location = new List<Vector3>();
        this.location.Add( location);
    }
}