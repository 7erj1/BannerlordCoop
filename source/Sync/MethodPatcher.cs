﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace Sync
{
    public class MethodPatcher
    {
        private readonly List<MethodAccess> m_Access = new List<MethodAccess>();
        private readonly Type m_Declaring;

        public MethodPatcher([NotNull] Type declaringClass)
        {
            m_Declaring = declaringClass;
        }

        public IEnumerable<MethodAccess> Methods => m_Access;

        ~MethodPatcher()
        {
            MethodInfo factoryMethod =
                typeof(MethodPatchFactory).GetMethod(nameof(MethodPatchFactory.GetPatch));
            foreach (MethodAccess syncMethod in m_Access)
            {
                MethodPatchFactory.Unpatch(syncMethod.MemberInfo);
            }
        }

        public MethodPatcher Patch(MethodInfo method)
        {
            PatchMethod(method);
            return this;
        }

        public MethodPatcher Patch(string sMethodName)
        {
            PatchMethod(AccessTools.Method(m_Declaring, sMethodName));
            return this;
        }

        public bool TryGetMethod(string sMethodName, out MethodAccess methodAccess)
        {
            return TryGetMethod(AccessTools.Method(m_Declaring, sMethodName), out methodAccess);
        }

        public bool TryGetMethod(MethodInfo methodInfo, out MethodAccess methodAccess)
        {
            methodAccess = m_Access.FirstOrDefault(m => m.MemberInfo.Equals(methodInfo));
            return methodAccess != null;
        }

        private void PatchMethod(MethodInfo original)
        {
            MethodInfo dispatcher = AccessTools.Method(
                typeof(MethodPatcher),
                nameof(DispatchCallRequest));
            m_Access.Add(MethodPatchFactory.Patch(original, dispatcher));
        }

        public static bool DispatchCallRequest(
            MethodAccess methodAccess,
            object instance,
            params object[] args)
        {
            return methodAccess.RequestCall(instance, args);
        }
    }
}