using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public static class InputFabric
{
    private static IInput input;

    public static IInput GetOrCreateInpit(bool dontDestroy = false)
    {
        //if (input != null)
        //    return input;

        GameObject goInput = new GameObject("IInput");
#if UNITY_EDITOR
        if (IsSimulatorView())
            input = goInput.AddComponent<TouchInput>();
        else
            input = goInput.AddComponent<TouchInput>();

#elif UNITY_ANDROID || UNITY_IOS
        input = goInput.AddComponent<TouchInput>();
#else
        input = goInput.AddComponent<MouseInput>();
#endif

        if(dontDestroy)
            GameObject.DontDestroyOnLoad(goInput);

        return input;
    }

#if UNITY_EDITOR
    private static bool IsSimulatorView()
    {
        var deviceSimulatorType = Type.GetType("UnityEditor.DeviceSimulation.DeviceSimulator, UnityEditor.DeviceSimulatorModule");
        if (deviceSimulatorType != null)
        {
            var isSimulatingProperty = deviceSimulatorType.GetProperty("IsSimulating", BindingFlags.Public | BindingFlags.Static);
            if (isSimulatingProperty != null)
            {
                return (bool)isSimulatingProperty.GetValue(null);
            }
        }

        return false;
    }
#endif
}
