using CharacterLib.Helpers;

namespace CharacterLib.Bridges;

class StringInfo(object mono) {
  private object mono = mono;
  public string text {
    get {
      return mono.GetField<string>("text");
    }
  }
  public string[] metadata {
    get {
      return mono.GetField<string[]>("metadata");
    }
  }

  public string nodeName {
    get {
      return mono.GetField<string>("nodeName");
    }
  }
}

