using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Degage.Reflection.Dynamic
{

    /// <summary>
    /// 为动态加载程序集提供一系列方法
    /// </summary>
    public class AssemblyLoader
    {
        /// <summary>
        /// 关联的应用程序域
        /// </summary>
        public AppDomain AppDomain { get; private set; }

        /// <summary>
        /// 当前应用程序域未能加载程序集时，将尝试从此目录中加载
        /// </summary>
        public String AssemblyResolveDirectory { get; private set; }
        /// <summary>
        /// 表示此加载器是否已注册至相应程序域中
        /// </summary>
        public Boolean Registered { get; private set; }

        /// <summary>
        /// 存储已动态加载的程序集的表
        /// </summary>
        public Dictionary<String, Assembly> AssemblyTable { get; private set; } = new Dictionary<String, Assembly>();
        private readonly Object _syncObj = new Object();

        /// <summary>
        ///  为指定应用程序域注册缺失程序集解析的方法，此方法使得当前应用程序域在未能定位到程序集时，统一再从指定的目录中加载相应的程序集，此方法通过一次性将程序集加载到内存中避免当前进程占用相应程序集文件。
        /// </summary>
        /// <param name="domain">被注册的程序域对象</param>
        /// <param name="resolveDirectory">加载缺失程序集的目录</param>
        public void RegisterAssemblyResolve(AppDomain domain, String resolveDirectory)
        {
            if (this.Registered)
            {
                throw new Exception(DynamicSR.E_RepeatRegisterAssemblyResolve);
            }

            if (!Directory.Exists(resolveDirectory))
            {
                throw new DirectoryNotFoundException(resolveDirectory);
            }
            lock (_syncObj)
            {
                if (this.Registered) return;
                this.Registered = true;
                this.AppDomain = domain;
                this.AssemblyResolveDirectory = resolveDirectory;
                domain.AssemblyResolve += Domain_AssemblyResolve;
            }

        }

        private Assembly Domain_AssemblyResolve(Object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            var dllName = assemblyName.Name + ".dll";
            if (this.AssemblyTable.ContainsKey(dllName))
            {
                return this.AssemblyTable[dllName];
            }

            if (assemblyName.Name.EndsWith(".resources"))
            {
                return null;
            }
            lock (_syncObj)
            {
                if (this.AssemblyTable.ContainsKey(dllName))
                {
                    return this.AssemblyTable[dllName];
                }

                Assembly assembly = null;

                var path = Path.Combine(this.AssemblyResolveDirectory, dllName);
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    using (memoryStream)
                    {
                        stream.CopyTo(memoryStream);
                        var assemblyRaw = memoryStream.ToArray();
                        assembly = Assembly.Load(assemblyRaw);
                        this.AssemblyTable.Add(dllName, assembly);
                    }
                }
                return assembly;
            }
        }
    }
}
