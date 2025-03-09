using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(TableManager))]
public class BetAreaEditor : Editor
{
    private List<GameObject> betObjects = new List<GameObject>();
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TableManager targetScript = (TableManager)target;
        if (GUILayout.Button("Fill Bet Areas"))
        {
            for (int i = 0; i < targetScript.InsideBetAreas.Length; i++)
            {
                targetScript.InsideBetAreas[i].betValue = i;
            }
        }
        // var newBetObjects = new List<BetObject>();
        // TableManager targetScript = (TableManager)target;
        // if (GUILayout.Button("Fill Bet Areas"))
        // {
        //     return;
        //     // targetScript.OutsideBetAreas = new OutsideBetArea[targetScript.outsideBets.Length];
        //     // targetScript.InsideBetAreas = new InsideBetArea[targetScript.insideBets.Length];
        //     // for (var i = 0; i < targetScript.outsideBets.Length; i++)
        //     // {
        //     //     targetScript.OutsideBetAreas[i] = new OutsideBetArea {
        //     //         minValue = targetScript.outsideBets[i].minValue,
        //     //         maxValue = targetScript.outsideBets[i].maxValue,
        //     //         BetCenter = targetScript.outsideBets[i].betCenter,
        //     //         BetMultiplier = GetBetMultiplier(targetScript.outsideBets[i].betCenter),
        //     //         betType = GetBetType(targetScript.outsideBets[i].betCenter, targetScript.outsideBets[i].minValue)
        //     //     };
        //     // }
        //     // for (var i = 0; i < targetScript.insideBets.Length; i++)
        //     // {
        //     //     targetScript.InsideBetAreas[i] = new InsideBetArea() {
        //     //         BetCenter = targetScript.insideBets[i].betCenter,
        //     //         betCorners = targetScript.insideBets[i].betCorners,
        //     //         betSplits = targetScript.insideBets[i].betSplits,
        //     //         BetMultiplier = GetBetMultiplier(targetScript.insideBets[i].betCenter)
        //     //     };
        //     // }
        //     //
        //     // return;
        //     // betObjects = GameObject.FindGameObjectsWithTag("StraightUp").ToList();
        //     // betObjects = betObjects.OrderBy(obj => GetHierarchyDepth(obj.transform))
        //     //     .ThenBy(obj => obj.transform.GetSiblingIndex())
        //     //     .ToList();
        //     foreach (var betObject in betObjects)
        //     {
        //         var newBetObject = new BetObject();
        //         newBetObject.betCenter = betObject.transform;
        //         var betCorners = new List<Transform>();
        //         var betSplits = new List<Transform>();
        //         var borderObjects = GetStraightUpBorders(betObject.transform);
        //         foreach (var borderObject in borderObjects)
        //         {
        //             if(borderObject.transform.CompareTag("Corner"))
        //                 betCorners.Add(borderObject.transform);
        //             if(borderObject.transform.CompareTag("Split"))
        //                 betSplits.Add(borderObject.transform);
        //         }
        //         newBetObject.betCorners = betCorners.ToArray();
        //         newBetObject.betSplits = betSplits.ToArray();
        //         newBetObjects.Add(newBetObject);
        //     }
        //     // targetScript.insideBets = newBetObjects.ToArray();
        // }
    }
    private int GetBetMultiplier(Transform betObject)
    {
        switch (betObject.transform.tag)
        {
            case "StraightUp":
                return 36;
            case "Corner":
                return 9;
            case "Split":
                return 18;
            case "Street":
                return 12;
            case "FirstFour":
                return 9;
            case "SixLine":
                return 6;
            case "Dozen":
            case "Column":
                return 3;
            case "OddOrEven":
            case "RedOrBlack":
            case "LowOrHigh":
                return 2;
            default:
                
                return 1;
        }
    }
    private BetType GetBetType(Transform betObject,int value)
    {
        switch (betObject.transform.tag)
        {
            case "StraightUp":
            case "Corner":
            case "Split":
            case "Street":
            case "FirstFour":
            case "SixLine":
            case "Dozen":
                return BetType.Normal;
            case "Column":
                return BetType.LowToHigh;
            case "OddOrEven":
                return value % 2 == 0 ? BetType.Even : BetType.Odd;
            case "RedOrBlack":
                return value == 2 ? BetType.Red : BetType.Black;
            case "LowOrHigh":
                return BetType.LowToHigh;
            default:
                return BetType.Normal;
        }
    }
    private GameObject[] GetStraightUpBorders(Transform centerObject)
    {
        Vector3 expandedSize = centerObject.localScale * 1.25f;
        Vector3 boxPosition = centerObject.position;
        Collider[] colliders = Physics.OverlapBox(boxPosition, expandedSize * 0.5f, centerObject.rotation);
        List<GameObject> touchingObjects = new List<GameObject>();
        foreach (Collider col in colliders)
        {
            if (col.gameObject != centerObject.gameObject)
            {
                touchingObjects.Add(col.gameObject);
            }
        }

        return touchingObjects.ToArray();
    }
    private int GetHierarchyDepth(Transform obj)
    {
        int depth = 0;
        while (obj.parent != null)
        {
            depth++;
            obj = obj.parent;
        }
        return depth;
    }
}
