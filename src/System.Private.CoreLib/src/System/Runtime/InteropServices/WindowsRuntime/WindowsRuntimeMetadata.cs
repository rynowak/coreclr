// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.InteropServices.WindowsRuntime
{
    internal static class WindowsRuntimeMetadata
    {
        private static EventHandler<DesignerNamespaceResolveEventArgs> DesignerNamespaceResolve;

        internal static string[] OnDesignerNamespaceResolveEvent(AppDomain appDomain, string namespaceName)
        {
            EventHandler<DesignerNamespaceResolveEventArgs> eventHandler = DesignerNamespaceResolve;
            if (eventHandler != null)
            {
                foreach (EventHandler<DesignerNamespaceResolveEventArgs> handler in eventHandler.GetInvocationList())
                {
                    DesignerNamespaceResolveEventArgs eventArgs = new DesignerNamespaceResolveEventArgs(namespaceName);

                    handler(appDomain, eventArgs);

                    Collection<string> assemblyFilesCollection = eventArgs.ResolvedAssemblyFiles;
                    if (assemblyFilesCollection.Count > 0)
                    {
                        string[] retAssemblyFiles = new string[assemblyFilesCollection.Count];
                        int retIndex = 0;
                        foreach (string assemblyFile in assemblyFilesCollection)
                        {
                            if (string.IsNullOrEmpty(assemblyFile))
                            {   // DesignerNamespaceResolve event returned null or empty file name - that is not allowed
                                throw new ArgumentException(SR.Arg_EmptyOrNullString, "DesignerNamespaceResolveEventArgs.ResolvedAssemblyFiles");
                            }
                            retAssemblyFiles[retIndex] = assemblyFile;
                            retIndex++;
                        }

                        return retAssemblyFiles;
                    }
                }
            }

            return null;
        }
    }


    internal class DesignerNamespaceResolveEventArgs : EventArgs
    {
        private string _NamespaceName;
        private Collection<string> _ResolvedAssemblyFiles;

        public Collection<string> ResolvedAssemblyFiles
        {
            get
            {
                return _ResolvedAssemblyFiles;
            }
        }

        public DesignerNamespaceResolveEventArgs(string namespaceName)
        {
            _NamespaceName = namespaceName;
            _ResolvedAssemblyFiles = new Collection<string>();
        }
    }
}
