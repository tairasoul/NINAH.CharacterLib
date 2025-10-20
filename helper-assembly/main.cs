using Yarn.Compiler;

namespace tairasoul.ninah.characterlib.helperassembly;

class HelperAssembly
{
  public static (Yarn.Program, IDictionary<string, StringInfo>) CompileFile(string filename, string content, Action<string> logError, string assemblyName) {
    CompilationJob job = CompilationJob.CreateFromString(filename, content, null);
    CompilationResult result = Compiler.Compile(job);
    if (result.Program == null) {
      logError($"Encountered an error while trying to compile dialogue for {assemblyName}.{filename}");
      foreach (Diagnostic diagnostic in result.Diagnostics) {
        logError(diagnostic.ToString());
      }
      return (null, null);
    }
    return (result.Program, result.StringTable);
  }

  public static IDictionary<string, StringInfo> MergeDictionaries(IDictionary<string, StringInfo> dict1, IDictionary<string, StringInfo> dict2) {
    return dict1.Concat(dict2).ToDictionary(keySelector: v => v.Key, elementSelector: v => v.Value);
  }
}