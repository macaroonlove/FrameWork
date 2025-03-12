using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FrameWork
{
    /// <summary>
    /// 게임 시작 시, PeristentSingleton을 상속받은 클래스를 로드하는 클래스
    /// </summary>
    public class PersistentLoad
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoLoadAll()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var singletonTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.BaseType != null && t.BaseType.IsGenericType)
                .Where(t => t.BaseType.GetGenericTypeDefinition() == typeof(PersistentSingleton<>))
                .ToList();

                foreach (var type in singletonTypes)
                {
                    PropertyInfo instanceProperty = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    instanceProperty?.GetValue(null);
                }
            }
        }
    }
}