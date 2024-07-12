/*
Generico locator di instances da usare per oggetti come scene, lobby etc...
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Locator
{
    private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void RegisterService<T>(T service)
    {
        Type serviceType = typeof(T);

        if (!services.ContainsKey(serviceType))
        {
            services.Add(serviceType, service);
        }
        else
        {
            throw new InvalidOperationException($"Service of type {serviceType} is already registered");
        }
    }

    public static T GetService<T>()
    {
        Type serviceType = typeof(T);

        if (services.TryGetValue(serviceType, out var service))
        {
            return (T)service;
        }
        else
        {
            throw new KeyNotFoundException($"Service of type {serviceType} not registered");
        }
    }
    
}
