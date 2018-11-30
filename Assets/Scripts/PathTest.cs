﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[ExecuteInEditMode]
public class PathTest : MonoBehaviour
{
    //public Transform ob1;
    public PathTest ob2;
    public LineRenderer lineRenderer;
    public Material lineMaterial;
    public Color lineColor = new Color(0.40396f, 0.8078f, 0.9803f); 
    public float lineWidth = 0.5f;
    public float lineAlpha = 1f;
  
    // Use this for initialization
    void Start ()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateLine();
      
    }
   

    private void OnValidate()
    {
        updateLine();
    }
    public void setupLine()
    {
        print("test1");

        if ( ob2 != null && lineRenderer == null)
        {
            lineRenderer =  gameObject.AddComponent<LineRenderer>();
            updateLine();
        }
       
    }
    [ContextMenu("UpdateLine")]
    public void updateLine()
    {
        if ( ob2 != null && lineRenderer!= null)
        {
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, ob2.transform.position);
            lineRenderer.numCornerVertices = 12;
            lineRenderer.numCapVertices = 12;
        }
    }
    public void createNextPoint()
    {
        
        PathTest pt = Instantiate(this.transform).GetComponent<PathTest>();
      
        pt.ob2 = this ;
        pt.transform.position = transform.position;//new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        pt.gameObject.name = "pt";
        pt.transform.SetParent(transform.parent, true);
        pt.setupLine();
        Selection.activeGameObject = pt.gameObject;
        
      
    }
}
[CustomEditor(typeof(PathTest))]
public class ObjectBuilderEditor : Editor
{
    bool JustPressed = false;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathTest myScript = (PathTest)target;
       
        if(myScript.lineRenderer != null)
        {
            if (GUILayout.Button("update line"))
            {
                myScript.updateLine();
            }
        }
        else
        {
            if (GUILayout.Button("Build line"))
            {
                myScript.setupLine();
            }
        }
        
    }
    public void OnSceneGUI()
    {
        PathTest myScript = (PathTest)target;
        Event e = Event.current;
        switch (e.type)
        {
            case EventType.KeyDown:
                if (e.keyCode == KeyCode.W && JustPressed == false)
                {
                    myScript.createNextPoint();
                    JustPressed = true;
                }
                break;
            case EventType.KeyUp:
                if (e.keyCode == KeyCode.W && JustPressed == true)
                {
                    JustPressed = false;
                    
                }
                break;

        }


    }
}
