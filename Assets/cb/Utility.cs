//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using Object = UnityEngine.Object;

//static class Utility
//{
//    [DebuggerStepThrough]
//    public static T GetComponentAnywhere<T>(this GameObject gameObject)
//        where T : Component
//    {
//        var c = gameObject.GetComponent<T>();
//        if (c == null)
//            c = gameObject.GetComponentInParent<T>();
//        if (c == null)
//            c = gameObject.GetComponentInChildren<T>();
//        return c;
//    }

//    [DebuggerStepThrough]
//    public static IEnumerable<T> GetComponentsAnywhere<T>(this GameObject gameObject)
//        where T : Component
//    {
//        var components = new List<T>();
//        foreach (var c in gameObject.GetComponents<T>())
//            components.Add(c);
//        foreach (var c in gameObject.GetComponentsInChildren<T>())
//            components.Add(c);
//        foreach (var c in gameObject.GetComponentsInParent<T>())
//            components.Add(c);
//        return components;
//    }

//    [DebuggerStepThrough]
//    public static T GetComponentAnywhere<T>(this Component component)
//        where T : Component
//    {
//        var c = component.GetComponent<T>();
//        if (c == null)
//            c = component.GetComponentInParent<T>();
//        if (c == null)
//            c = component.GetComponentInChildren<T>();
//        return c;
//    }


//    [DebuggerStepThrough]
//    public static IEnumerable<T> GetComponentsAnywhere<T>(this Component component)
//        where T : Component
//    {
//        var components = new List<T>();
//        foreach (var c in component.GetComponents<T>())
//            components.Add(c);
//        foreach (var c in component.GetComponentsInChildren<T>())
//            if (!components.Contains(c))
//                components.Add(c);
//        foreach (var c in component.GetComponentsInParent<T>())
//            if (!components.Contains(c))
//                components.Add(c);
//        return components;
//    }
//}