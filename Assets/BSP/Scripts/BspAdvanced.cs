﻿using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BspAdvanced : MonoBehaviour
{
    //Option 
    [Range(0, 50)] [SerializeField] float sizeX;
    [Range(0, 50)] [SerializeField] float sizeY;

    [Range(0, 50)] [SerializeField] float minSizeX;
    [Range(0, 50)] [SerializeField] float minSizeY;

    [Range(0, 50)] [SerializeField] float maxSizeX;
    [Range(0, 50)] [SerializeField] float maxSizeY;

    [Range(0, 1)] [SerializeField] float probabilityToCut;
    [Range(0, 1)] [SerializeField] float probabilityToByXOrByY;

    //Struct
    struct Room
    {
        public Vector2 center;
        public Vector2 extends;

        public List<Room> children;
    }

    Room _RootRoom;


    public void Generate()
    {
        _RootRoom.extends = new Vector2(sizeX * 2, sizeY * 2);
        _RootRoom.center = Vector2.zero;
        _RootRoom.children = new List<Room>();

        _RootRoom.children.AddRange(CheckDivision(_RootRoom));
    }

    public void Clear() {
        _RootRoom = new Room();
    }

    List<Room> CheckDivision(Room room)
    {
        List<Room> childrenList = new List<Room>();

        //Divide by max size X
        if (room.extends.x > maxSizeX || room.extends.y > maxSizeY) {
            if (room.extends.x > maxSizeX && room.extends.y > maxSizeY) {
                childrenList.AddRange(DivideByProbability(room));
            }else if (room.extends.x > maxSizeX) {
                childrenList.AddRange(DivideByX(room));
            } else {
                childrenList.AddRange(DivideByY(room));
            }
        }
        else if (room.extends.x > minSizeX * 2 || room.extends.y > minSizeY * 2) { //Divide by probability
            if(room.extends.x > minSizeX * 2 && room.extends.y > minSizeY * 2) {
                float probability = Random.Range(0f, 1f);

                if (probability > probabilityToCut) {
                    childrenList.AddRange(DivideByProbability(room));
                }
            } else if(room.extends.x > minSizeX * 2) {
                float probability = Random.Range(0f, 1f);

                if (probability > probabilityToCut) {
                    childrenList.AddRange(DivideByX(room));
                }
            } else {
                float probability = Random.Range(0f, 1f);

                if(probability > probabilityToCut) {
                    childrenList.AddRange(DivideByY(room));
                }
            }
        }

        return childrenList;
    }

    List<Room> DivideByProbability(Room room)
    {
        float probability = Random.Range(0f, 1f);

        return probability > probabilityToByXOrByY ? DivideByX(room) : DivideByY(room);
    }

    List<Room> DivideByX(Room room)
    {
        List<Room> rooms = new List<Room>();

        Room roomLeft;
        Room roomRight;

        //Value for cut
        float posX = Random.Range(0 + minSizeX, room.extends.x - minSizeX * 2);

        //Extends
        roomRight.extends = new Vector2(posX, room.extends.y);
        roomLeft.extends = new Vector2(room.extends.x - posX, room.extends.y);

        //Center
        roomRight.center = new Vector2(room.center.x + room.extends.x * 0.5f - roomRight.extends.x * 0.5f, room.center.y);
        roomLeft.center = new Vector2(room.center.x - room.extends.x * 0.5f + roomLeft.extends.x * 0.5f, room.center.y);

        //Children
        roomRight.children = new List<Room>();
        roomLeft.children = new List<Room>();

        //Add children
        room.children.Add(roomRight);
        room.children.Add(roomLeft);

        roomRight.children.AddRange(CheckDivision(roomRight));
        roomLeft.children.AddRange(CheckDivision(roomLeft));

        return rooms;
    }

    List<Room> DivideByY(Room room)
    {
        List<Room> rooms = new List<Room>();

        Room roomUp;
        Room roomDown;

        //Value for cut
        float posY = Random.Range(0 + minSizeY, room.extends.y - (minSizeY * 2));

        //Extends
        roomDown.extends = new Vector2(room.extends.x, posY);
        roomUp.extends = new Vector2(room.extends.x, room.extends.y - posY);

        //Center
        roomDown.center = new Vector2(room.center.x, room.center.y - room.extends.y * 0.5f + roomDown.extends.y * 0.5f);
        roomUp.center = new Vector2(room.center.x, room.center.y + room.extends.y * 0.5f - roomUp.extends.y * 0.5f);

        //Children
        roomDown.children = new List<Room>();
        roomUp.children = new List<Room>();

        //Add children
        room.children.Add(roomUp);
        room.children.Add(roomDown);

        roomDown.children.AddRange(CheckDivision(roomDown));
        roomUp.children.AddRange(CheckDivision(roomUp));

        return rooms;
    }

    void OnDrawGizmos()
    {
        DrawRoom(_RootRoom);
    }

    static void DrawRoom(Room room)
    {
        Gizmos.DrawWireCube(room.center, room.extends);

        if (room.children == null) return;
        foreach (Room roomChild in room.children) {
            Gizmos.color = Color.cyan;
            DrawRoom(roomChild);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BspAdvanced))]
public class BspAdvancedEditor : Editor
{
    public override void OnInspectorGUI() {
        BspAdvanced myTarget = (BspAdvanced)target;

        if (GUILayout.Button("Generate")) {
            myTarget.Generate();
        }

        if(GUILayout.Button("Clear")) {
            myTarget.Clear();
        }

        DrawDefaultInspector();
    }
}
#endif
