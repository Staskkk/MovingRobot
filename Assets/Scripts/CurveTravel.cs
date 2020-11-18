using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CurveTravel : ITravel
{
    private Type funcType;
    private object funcInstance;
    private float startX;
    private float endX;
    private float step;

    public CurveTravel(string funcCode, float startX, float endX, float step)
    {
        CompileFunc(funcCode);
        this.startX = startX;
        this.endX = endX;
        this.step = step;
    }

    public IEnumerator<Vector2> GetEnumerator()
    {
        for (float xx = startX; xx <= endX; xx += step)
        {
            yield return new Vector2(xx, GetFuncValue(xx));
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void CompileFunc(string funcCode)
    {
        var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
        var parameters = new CompilerParameters() // new[] { "mscorlib.dll", "System.Core.dll" }
        {
            GenerateInMemory = true,
            TreatWarningsAsErrors = false,
            GenerateExecutable = false,
            IncludeDebugInformation = false,
            ReferencedAssemblies = { "System.dll" }
        };
        CompilerResults results = csc.CompileAssemblyFromSource(parameters,
        @"using System;

            namespace FunctionHolder
            {
                public class FuncManager
                {
                    public double Function(double x)
                    {
                        " + funcCode + @"
                    }
                }
            }");
        funcType = results.CompiledAssembly.GetModules()[0].GetType("FunctionHolder.FuncManager");
        funcInstance = Activator.CreateInstance(funcType);
    }

    private float GetFuncValue(float x)
    {
        return (float) (double) funcType.InvokeMember(
            "Function",
            BindingFlags.Default | BindingFlags.InvokeMethod,
            null,
            funcInstance,
            new object[] { (double) x });
    }
}
