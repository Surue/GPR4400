using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AABB {
    public Vector2 bottomLeft;
    public Vector2 topRight;

    public bool Overlap(AABB other) {

        float minX = bottomLeft.x;
        float minY = bottomLeft.y;
        float maxX = topRight.x;
        float maxY = topRight.y;

        float otherMinX = other.bottomLeft.x;
        float otherMinY = other.bottomLeft.y;
        float otherMaxX = other.topRight.x;
        float otherMaxY = other.topRight.y;

        //Other inside me
        if (otherMinX >= minX && otherMinX <= maxX) {
            if (otherMinY >= minY && otherMinY <= maxY) {
                return true;
            }

            if (otherMaxY >= minY && otherMaxY <= maxY) {
                return true;
            }
        }
        
        if (otherMaxX >= minX && otherMaxX <= maxX) {
            if (otherMinY >= minY && otherMinY <= maxY) {
                return true;
            }

            if (otherMaxY >= minY && otherMaxY <= maxY) {
                return true;
            }
        }
        
        //Me inside other
        if (minX >= otherMinX && minX <= otherMaxX) {
            if (minY >= otherMinY && minY <= otherMaxY) {
                return true;
            }

            if (maxY >= otherMinY && maxY <= otherMaxY) {
                return true;
            }
        }
        
        if (maxX >= otherMinX && maxX <= otherMaxX) {
            if (minY >= otherMinY && minY <= otherMaxY) {
                return true;
            }

            if (maxY >= otherMinY && maxY <= otherMaxY) {
                return true;
            }
        }
        
        return false;
    }
}
