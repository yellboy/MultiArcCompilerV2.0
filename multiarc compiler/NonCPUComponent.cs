using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CSharp;

namespace MultiArc_Compiler
{
    public abstract class NonCPUComponent : SystemComponent
    {
        protected CompilerResults results;

        protected Thread thread;

        protected volatile bool running;

        public NonCPUComponent(string projectPath) : base(projectPath) { }

        public void StartWorking()
        {
            thread = new Thread(Run);
            thread.Start();
            running = true;
        }

        public void StopWorking()
        {
            running = false;
            thread.Abort();
        }

        public void Run()
        {
            while (true)
            {
                if (running)
                {
                    lock (Form1.LockObject)
                    {
                        var t = results.CompiledAssembly.GetType("DynamicClass" + name);
                        t.GetMethod("Cycle").Invoke(null, new object[] { this });
                        Thread.EndCriticalRegion();
                    }
                }
            }
        }

        public override int CompileCode(string dataFolder)
        {
            string methodBody = File.ReadAllText(dataFolder + ArcDirectoryName + FileName);
            string executableCode =
@"
using System;
using System.IO;
using MultiArc_Compiler;

public class DynamicClass" + name + @"
{
" + methodBody + @"
}";
            var provider = CSharpCodeProvider.CreateProvider("c#");
            var options = new CompilerParameters();
            var assemblyContainingNotDynamicClass = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            options.ReferencedAssemblies.Add(assemblyContainingNotDynamicClass);
            var assemblyContaningForms = Assembly.GetAssembly(typeof(System.Windows.Forms.Control)).Location;
            options.ReferencedAssemblies.Add(assemblyContaningForms);
            var assemblyContainingComponent = Assembly.GetAssembly(typeof(System.ComponentModel.Component)).Location;
            options.ReferencedAssemblies.Add(assemblyContainingComponent);
            results = provider.CompileAssemblyFromSource(options, new[] { executableCode });
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    Form1.Instance.AddToOutput(DateTime.Now.ToString() + "Error in " + FileName + ": " + error.ErrorText + " in line " + (error.Line - 8) + ".\n");
                }
            }
            return results.Errors.Count;
        }
    }
}
