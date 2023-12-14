using System;
using System.Collections.Generic;
using UnityEngine;

namespace MercivKit
{
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator _instance;

        private Dictionary<Type, ServiceBase> _services = new Dictionary<Type, ServiceBase>();

        private void Awake()
        {
            _instance = this;
            var services = GetComponentInChildren<ServiceBase>();
            foreach (var service in services.GetComponentsInChildren<ServiceBase>())
            {
                _services.Add(service.GetType(), service);
            }
        }

        public static T GetService<T>() where T : ServiceBase
        {
            return (T)_instance._services[typeof(T)];
        }
    }
}