using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using _Code.Characters;
using _Code.DialogSystem.Commands;
using _Code.Infrastructure;
using BepInEx.Logging;
using tairasoul.ninah.characterlib.helpers;
using Il2CppSystem.Dynamic.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using _Code.DialogSystem;
using tairasoul.ninah.characterlib.bridges;
using Yarn.Unity.UnityLocalization;
using System.Collections;
using _Code.Infrastructure.Randomization;
using _Code.Infrastructure.GameEvents;
using _Code.Utils.CustomYarnReading;
using _Code.Infrastructure.Rooms;
using _Code.Rooms;
using System.Diagnostics.CodeAnalysis;
using tairasoul.ninah.characterlib.patches;
using _Code.Infrastructure._NINAH__MainMenu.Gacha;
using _Code.Infrastructure.ActionableObjects;
using _Code.Infrastructure.Locations;
using _Code.Events;
using Yarn;
using UnityEngine.UI;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Metadata;
using tairasoul.ninah.characterlib.utils;
using _Code.Infrastructure.DataModel;
using _Code.Infrastructure._NINAH__Endings.View;
using _Code.Infrastructure.DataModel.Models.GameSave;

namespace tairasoul.ninah.characterlib;

class DialogueRegistrar {
  public Assembly assembly = Assembly.GetCallingAssembly();
  public string filename;
  public string locale;
}

class DialogueRegistryProgram {
  public Program program;
  public List<object> mergedWith = [];
  public bool merged = false;
}

class DialogueRegistryTable {
  public object table;
  public bool merged = false;
}

class DialogueRegistry {
  public DialogueRegistryProgram? program;
  public DialogueRegistryTable? table;
  [MemberNotNullWhen(true, nameof(table))]
  public bool HasTable {
    get;
    set;
  }
  [MemberNotNullWhen(true, nameof(program))]
  public bool HasProgram {
    get;
    set;
  }
  public bool needsRegistry = true;

  public DialogueRegistry() {
    HasTable = false;
    HasProgram = false;
  }
}

public class CreationException(string name) : Exception {

  public override string ToString()
  {
    return $"CharacterLib.CreationException: Tried to register character under taken name {name}";
  }
}

public class CustomCharacter : CharacterSOData {
  internal int maxTalks;
  internal ERoomPeopleState startState;
  internal bool HasObjectView = false;
  internal Dictionary<string, string> NodeReplacements = [];
  internal Dictionary<string, string> EmotionRedirects = [];

  internal CustomCharacter() {
  }
}

class CharacterLibBehaviour : MonoBehaviour {
  readonly CharacterLib lib = new();

  public void Start() {
    lib.Start();
  }

  public void Update() {
    lib.Update();
  }
}

class YarnContext : AssemblyLoadContext {
}

public class CharacterLib {
  internal static ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("CharacterLib/Registry");
  internal static ManualLogSource DialogueLog = BepInEx.Logging.Logger.CreateLogSource("CharacterLib/DialogueCompiler");
  internal static List<CustomCharacter> characters = [];
  internal static List<ECharacterType> characterTypes = [];
  internal static Dictionary<ECharacterType, CustomCharacter> references = [];
  private static readonly Dictionary<DialogueRegistrar, DialogueRegistry> DialogPrograms = [];
  private static readonly YarnContext context = new();
  private static readonly Dictionary<string, Type> YarnTypes = [];
  private static readonly Dictionary<string, MethodInfo> YarnMethods = [];
  internal static Il2CppReferenceArray<CharacterSOData> soDataList = null;
  internal List<CustomCharacter> registered = [];
  internal static Dictionary<string, int> alreadyRegistered;
  //private IGameSaveDataHandler gameSave = null;
  //internal static ModdedCharSaveData charSaveData;
  //internal static ASavableClass<ModdedCharSaveData> savable;

  internal static void CreateRegisteredList() {
    EnumPatches.Bypass = true;
    alreadyRegistered = Enum.GetValues<ECharacterType>().ToDictionary(keySelector: v => v.ToString(), elementSelector: v => (int)v);
    EnumPatches.Bypass = false;
  }

  internal static int GetNextEnumInt(string name) {
    if (alreadyRegistered.ContainsKey(name)) {
      return -1;
    }
    int[] registered = [.. alreadyRegistered.Values];
    int highestInt = registered.Max();
    Log.LogInfo($"Registering {name} as {highestInt + 1}");
    alreadyRegistered[name] = highestInt + 1;
    return highestInt + 1;
  }

  internal static bool shouldRegister = false;

  internal static void RegisterYarnAssemblies() {
    using MemoryStream ha = new(AssemblyUtils.GetResourceBytes(Assembly.GetExecutingAssembly(), "HelperAssembly"));
    using MemoryStream yc = new(AssemblyUtils.GetResourceBytes(Assembly.GetExecutingAssembly(), "YarnCompiler"));
    using MemoryStream ya = new(AssemblyUtils.GetResourceBytes(Assembly.GetExecutingAssembly(), "Yarn"));
    using MemoryStream at = new(AssemblyUtils.GetResourceBytes(Assembly.GetExecutingAssembly(), "Antlr4"));
    using MemoryStream pb = new(AssemblyUtils.GetResourceBytes(Assembly.GetExecutingAssembly(), "Protobuf"));
    context.LoadFromStream(at);
    Assembly protobuf = context.LoadFromStream(pb);
    Assembly yarn = context.LoadFromStream(ya);
    context.LoadFromStream(yc);
    Assembly helperAssembly = context.LoadFromStream(ha);
    SetupYarnTypes(yarn, helperAssembly, protobuf);
  }

  private static void SetupYarnTypes(Assembly yarn, Assembly helperAssembly, Assembly protobuf) {
    Type codedOutput = protobuf.GetType("Google.Protobuf.CodedOutputStream", true);
    YarnTypes["CodedOutputStream"] = codedOutput;
    //YarnTypes["CodedInputStream"] = protobuf.GetType("Google.Protobuf.CodedInputStream", true);
    Type program = yarn.GetType("Yarn.Program", true);
    //YarnTypes["Library"] = yarn.GetType("Yarn.Library", true);
    Type helper = helperAssembly.GetType("tairasoul.ninah.characterlib.helperassembly.HelperAssembly", true);
    //YarnTypes["Node"] = yarn.GetType("Yarn.Node", true);
    //YarnTypes["Instruction"] = yarn.GetType("Yarn.Instruction", true);
    SetupYarnMethods(program, helper, codedOutput);

  }

  private static void SetupYarnMethods(Type program, Type helper, Type codedOutput) {
    YarnMethods["WriteTo"] = program.GetMethod("WriteTo");
    YarnMethods["Flush"] = codedOutput.GetMethod("Flush");
    YarnMethods["CompileFile"] = helper.GetMethod("CompileFile");
    YarnMethods["MergeDictionaries"] = helper.GetMethod("MergeDictionaries");
  }
  
  /// <summary>
  /// Add a character to be registered.
  /// </summary>
  /// <param name="character">The CustomCharacter instance to register.</param>
  public static void AddCharacter(CustomCharacter character) {
    if (!characters.Any((v) => v.CharacterType == character.CharacterType)) {
      characters.Add(character);
      characterTypes.Add(character.CharacterType);
      references[character.CharacterType] = character;
    }
  }

  /// <summary>
  /// Add a dialogue from your assembly with a specific filename and content.
  /// </summary>
  /// <param name="locale">Locale this dialogue is for. Can be <c>en</c>, <c>ru</c>, <c>zh</c>, <c>zh-Hant</c>, <c>fr</c>, <c>de</c>, <c>ja</c>, <c>ko</c>, <c>pt-BR</c>, <c>es-AR</c> or <c>es</c></param>
  /// <param name="fileName">The file name this dialogue is registered under.</param>
  /// <param name="content">The content of the dialogue.</param>
  public static void AddDialogue(string locale, string fileName, string content, Assembly? callingAssembly = null)
  {
    callingAssembly ??= Assembly.GetCallingAssembly();
    var result = YarnMethods["CompileFile"].Invoke(null, [fileName, content, (string str) => DialogueLog.LogError(str), callingAssembly.GetName().Name]);
    var program = result.GetField<object>("Item1");
    var dictionary = result.GetField<object>("Item2");
    DialogueRegistrar registrar = new()
    {
      assembly = callingAssembly,
      filename = fileName,
      locale = locale
    };
    if (DialogPrograms.TryGetValue(registrar, out var existing))
    {
      if (program != null)
      {
        if (existing.HasProgram)
        {
          try
          {
            existing.program.program.MergeFrom(GetTranslatedProgram(program));
          }
          catch (ArgumentException exc) {
            DialogueLog.LogError($"Encountered error while merging {locale} {fileName}: {exc.ToString()}");
          }
        }
        else
        {
          existing.HasProgram = true;
          existing.program = new()
          {
            program = GetTranslatedProgram(program),
            mergedWith = [],
            merged = false
          };
        }
      }
      if (dictionary != null) {
        if (existing.HasTable) {
          object newTable = YarnMethods["MergeDictionaries"].Invoke(null, [existing.table, dictionary]);
          existing.table.table = newTable;
        }
        else {
          existing.HasTable = true;
          existing.table = new()
          {
            table = dictionary,
            merged = false
          };
        }
      }
    }
    else
    {
      DialogueRegistry registry = new();
      if (program != null) {
        registry.HasProgram = true;
        registry.program = new()
        {
          program = GetTranslatedProgram(program),
          mergedWith = [],
          merged = false
        };
      }
      if (dictionary != null) {
        registry.HasTable = true;
        registry.table = new()
        {
          table = dictionary,
          merged = false
        };
      }
      DialogPrograms[registrar] = registry;
    }
  }
  // possible locales: en, ru, zh, zh-Hant, fr, de, ja, ko, pt-BR, es-AR, es
  /// <summary>
  /// Add a dialogue from your assembly's resources.
  /// </summary>
  /// <param name="locale">Locale this dialogue is for. Can be <c>en</c>, <c>ru</c>, <c>zh</c>, <c>zh-Hant</c>, <c>fr</c>, <c>de</c>, <c>ja</c>, <c>ko</c>, <c>pt-BR</c>, <c>es-AR</c> or <c>es</c></param>
  /// <param name="resource">Name of the resource in your assembly.</param>
  public static void AddDialogue(string locale, string resource) {
    Assembly assembly = Assembly.GetCallingAssembly();
    byte[] resourceBytes = AssemblyUtils.GetResourceBytes(assembly, resource);
    if (resourceBytes.Length == 0) {
      DialogueLog.LogError($"Got no data for resource {resource} (locale {locale}) in assembly {assembly.GetName().Name}. Is the path correct?");
      return;
    }
    string utf8 = Encoding.UTF8.GetString(resourceBytes);
    AddDialogue(locale, resource, utf8, assembly);
  }

  internal static Program GetTranslatedProgram(object program) {
    MemoryStream outputS = new();
    var outputStream = Activator.CreateInstance(YarnTypes["CodedOutputStream"], [outputS]);
    YarnMethods["WriteTo"].Invoke(program, [outputStream]);
    YarnMethods["Flush"].Invoke(outputStream, null);
    byte[] byteArr = outputS.ToArray();
    Program prog = Program.Parser.ParseFrom(byteArr);
    return prog;
  }

  public void Start() {
    Action<Scene, LoadSceneMode> action = (scene, _) =>
    {
      if (scene.buildIndex != 1)
      {
        registered.Clear();
        shouldRegister = false;
      }
      else
      {
        shouldRegister = true;
      }
    };
    SceneManager.add_sceneLoaded(action);
    soDataList = new([.. Resources.FindObjectsOfTypeAll<CharacterSOData>().Where((v) => !characterTypes.Contains(v.CharacterType))]);
  }

  public void Update() {
    if (!shouldRegister)
    {
      if (SceneManager.GetActiveScene().buildIndex == 0)
      {
        /*if (gameSave == null) {
          EndingView view = Resources.FindObjectsOfTypeAll<EndingView>().First();
          if (view != null)
          {
            gameSave = view._dataModelService.Cast<DataModelService>().GameSaveDataHandler;
            charSaveData = gameSave.LoadKey(savable);
            gameSave.ConnectKey(savable, charSaveData);
          }
        }*/
        GachaWindow? window = Resources.FindObjectsOfTypeAll<GachaWindow>().FirstOrDefault();
        if (window != null)
        {
          Il2CppArrayBase<GachaCharacterView> views = window._gachaCharacterViews;
          foreach (CustomCharacter character in characters)
          {
            if (!views.Any(v => v._characterData?._characterType == character._characterType))
            {
              AddGachaCharacter(window, character);
            }
          }
        }
        return;
      }
    }
    AttemptRegistry();
  }

  void AddGachaCharacter(GachaWindow window, CustomCharacter character) {
    GameObject ReferenceObject = window.gameObject.Find("BG").Find("Characters").Find("Scroll View").Find("Viewport").Find("Content").Find("GachaCharacterView");
    GameObject clone = ReferenceObject.Instantiate();
    GachaCharacterView cv = clone.GetComponent<GachaCharacterView>();
    GachaCharacterView refe = ReferenceObject.GetComponent<GachaCharacterView>();
    clone.transform.SetParent(ReferenceObject.transform.parent, false);
    bool isUnlocked = false;
    if (window._dataModelService.MetaPrefsDataHandler.GetUnlockedCharacters().Contains(character.CharacterType))
      isUnlocked = true;
    cv.Init(character, isUnlocked, refe._inputHandling);
    cv.add_CharacterSelected(refe.CharacterSelected);
    window._charactersTotalCount++;
    if (isUnlocked)
      window._charactersCurrentCount++;
    window._gachaCharacterViews = (Il2CppReferenceArray<GachaCharacterView>)window._gachaCharacterViews.AddLast(cv);
  }

  void AttemptRegistry() {
    foreach (CustomCharacter character in characters) {
      if (!registered.Contains(character)) {
        RegisterCharacter(character);
      }
    }
    foreach (KeyValuePair<DialogueRegistrar, DialogueRegistry> kvp in DialogPrograms) {
      RegisterDialogue(kvp.Key, kvp.Value);
    }
  }

  void RegisterDialogue(DialogueRegistrar origin, DialogueRegistry dialogue) {
    if (!shouldRegister) return;
    UnityLocalisedLineProvider lineProvider = GameObject.FindFirstObjectByType<UnityLocalisedLineProvider>();
    CustomYarnReader yarnReader = GameObject.FindFirstObjectByType<CustomYarnReader>();
    if (yarnReader == null) return;
    if (lineProvider.currentStringsTable == null) return;
    string localeCode = lineProvider.LocaleCode;
    if (origin.locale == localeCode) {
      if (dialogue.HasProgram && !dialogue.program.merged) {
        if (!dialogue.program.mergedWith.Contains(yarnReader._project.cachedProgram))
        {
          foreach (var kvp in dialogue.program.program.nodes_.list) {
            if (!yarnReader._project.cachedProgram.nodes_.ContainsKey(kvp.Key))
              yarnReader._project.cachedProgram.nodes_.Add(kvp.Key, kvp.Value);
          }
          if (dialogue.program.program.initialValues_ != null)
            if (dialogue.program.program.initialValues_.list != null) {
              if (yarnReader._project.cachedProgram.initialValues_ == null) {
                yarnReader._project.cachedProgram.initialValues_ = dialogue.program.program.initialValues_;
              }
              else {
                foreach (var kvp in dialogue.program.program.initialValues_.list) {
                  if (!yarnReader._project.cachedProgram.initialValues_.ContainsKey(kvp.Key))
                    yarnReader._project.cachedProgram.initialValues_.Add(kvp.Key, kvp.Value);
                }
              }
            }
          dialogue.program.mergedWith.Add(yarnReader._project.cachedProgram);
        }
        object[] objectsNeeded = [yarnReader];
        if (dialogue.program.mergedWith.All((v) => objectsNeeded.Contains(v)))
        {
          Log.LogInfo($"Registered Yarn instructions for {origin.assembly.GetName().Name}.{origin.filename}");
          dialogue.program.merged = true;
          if (dialogue.HasTable && dialogue.table.merged)
            dialogue.needsRegistry = false;
        }
      }
      if (dialogue.HasTable && !dialogue.table.merged) {
        foreach (DictionaryEntry kvp in (IDictionary)dialogue.table.table)
        {
          StringInfo info = new(kvp.Value!);
          StringTableEntry entry = lineProvider.currentStringsTable.AddEntry((string)kvp.Key, info.text);
          LineMetadata md = new()
          {
            tags = info.metadata,
            nodeName = info.nodeName
          };
          entry.SharedEntry.Metadata.AddMetadata(md.Cast<IMetadata>());
        }
        Log.LogInfo($"Registered {localeCode} localization table data for {origin.assembly.GetName().Name}.{origin.filename}");
        dialogue.table.merged = true;
        if (dialogue.HasProgram && dialogue.program.merged)
          dialogue.needsRegistry = false;
      }
    }
    else {
      if (dialogue.HasProgram && dialogue.program.merged) {
        foreach (var kvp in dialogue.program.program.nodes_.list) {
          if (yarnReader._project.cachedProgram.nodes_.ContainsKey(kvp.Key))
            yarnReader._project.cachedProgram.nodes_.Remove(kvp.Key);
        }
        if (dialogue.program.program.initialValues_ != null)
          if (dialogue.program.program.initialValues_.list != null) {
            if (yarnReader._project.cachedProgram.initialValues_ != null) {
              foreach (var kvp in dialogue.program.program.initialValues_.list) {
                if (yarnReader._project.cachedProgram.initialValues_.ContainsKey(kvp.Key))
                  yarnReader._project.cachedProgram.initialValues_.Remove(kvp.Key);
              }
            }
          }
        dialogue.program.merged = false;
        dialogue.program.mergedWith = [];
        if (dialogue.HasTable && !dialogue.table.merged)
          dialogue.needsRegistry = true;
      }
    }
  }

  void RegisterCharacter(CustomCharacter character) {
    if (!soDataList.Any((v) => v.CharacterType == character.CharacterType)) {
      soDataList = (Il2CppReferenceArray<CharacterSOData>)soDataList.AddLast(character);
    }
    DialogCommandsInstance instance = GameObject.FindFirstObjectByType<DialogCommandsInstance>();
    if (instance == null) return;
    ResourceMother? resourceMother = instance._charactersSODataProvider?.Cast<ResourceMother>();
    if (resourceMother == null) return;
    CharactersManager? manager = instance._charactersManager?.Cast<CharactersManager>();
    if (manager == null) return;
    GameplayRandomizer? randomizer = instance._gameEventsManager.Cast<GameEventsManager>()?._gameplayRandomizer?.Cast<GameplayRandomizer>();
    if (randomizer == null) return;
    DialogManager? dm = GameObject.FindFirstObjectByType<RoomsViewProvider>()?._entrance.DialogManager?.Cast<DialogManager>();
    if (dm == null) return;
    DialogSaveData dsd = dm._saveData;
    RoomsManager? cat = GameObject.FindFirstObjectByType<InteractablesViewProvider>()?.Cat?._actionableObjectsManager?.Cast<ActionableObjectsManager>()?._roomsManager?.Cast<RoomsManager>();
    if (cat == null) return;
    LocationsManager? locations = GameObject.FindFirstObjectByType<InteractablesViewProvider>()?.HatchBasement?._locationsManager.Cast<LocationsManager>();
    if (locations == null) return;
    locations._charactersSOData = soDataList;
    cat._charactersList = soDataList;
    resourceMother._charactersData = soDataList;
    randomizer._charactersList = soDataList;
    manager._characterData = soDataList;
    dm._characters = soDataList;
    if (!dsd.CharactersTalksCount._charactersMaxTalksCount.ContainsKey(character._characterType))
      dsd.CharactersTalksCount._charactersMaxTalksCount.Add(character._characterType, character.maxTalks);
    if (character.HasObjectView) {
      bool foundRoom = false;
      DoorTrigger[] triggers = Resources.FindObjectsOfTypeAll<DoorTrigger>();
      foreach (DoorTrigger trigger in triggers) {
        if (trigger._linkedRoom == character._room) {
          if (trigger.Room != null && trigger.Room.Cast<ARoom>() != null && trigger.Room.Cast<ARoom>().View != null)
          {
            ARoomView roomView = trigger._room.Cast<ARoom>().View.Cast<ARoomView>();
            if (!roomView.CharacterViews.Any((v) => v.Data._characterType == character._characterType))
              AddObjectView(character, roomView);
            //roomView.CharacterViews = (Il2CppReferenceArray<CharacterRoomObjectView>)roomView.CharacterViews.AddLast(character.objectView);
            foundRoom = true;
          }
        }
      }
      if (!foundRoom) return;
    }
    registered.Add(character);
    /*RandomGenerationSettingsSOData random = GameObject.FindFirstObjectByType<DialogCommandsInstance>()._charactersSODataProvider.Cast<ResourceMother>()._randomGenerationSettingsData;
    random._randomGuestsNumberByNights[1] = 0;
    GameEventsManager mang = GameObject.FindFirstObjectByType<DialogCommandsInstance>()._gameEventsManager.Cast<GameEventsManager>();
    CharacterDingDongEvent ev = new((ECharacterType)63, new GameEventConditions("entrance.Neighbour.0", (Il2CppStructArray<int>)(int[])[0], ETimeOfDay.Night, 1f, (Il2CppStringArray)(string[])[], false));
    Il2CppSystem.ValueTuple<CharacterDingDongEvent, int> tuple = new(ev, 3);
    mang._preDingDongEvents[1]._items[1].Item1.Character = (ECharacterType)63;*/
  }

  internal static void AddObjectView(CustomCharacter character, ARoomView roomView)
  {
    GameObject reference = roomView.transform.parent.gameObject.Find("Kitchen").Find("BG").Find("Blind");
    GameObject cloned = reference.Instantiate();
    cloned.transform.SetParent(roomView.gameObject.GetChildren().First().transform, false);
    cloned.name = character.name;
    CharacterRoomObjectView view = cloned.GetComponent<CharacterRoomObjectView>();
    view._startState = character.startState;
    view.Data = character;
    view.Init(roomView.DayNightController, roomView.DialogManager, roomView.CloseUpsController);
    UIButton button = cloned.GetComponent<UIButton>();
    roomView._CharacterViews_k__BackingField = (Il2CppReferenceArray<CharacterRoomObjectView>)roomView._CharacterViews_k__BackingField.AddLast(view);
    roomView._uiButtons = (Il2CppReferenceArray<UIButton>)roomView._uiButtons.AddLast(button);
  }
}