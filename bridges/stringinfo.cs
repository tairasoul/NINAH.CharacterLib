using tairasoul.ninah.characterlib.helpers;

namespace tairasoul.ninah.characterlib.bridges;

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

