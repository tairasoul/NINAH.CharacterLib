using _Code.Characters;
using _Code.Infrastructure.Sound;
using _Code.Menues.HUD.Animations;
using _Code.Rooms;
using _Code.Utils.UI.ImageAnimating;
using DG.Tweening;
using UnityEngine;

namespace tairasoul.ninah.characterlib;

/// <summary>
/// Helper class for building a custom character.
/// </summary>
public class CustomCharacterBuilder
{
  private CustomCharacter result;
  /// <summary>
  /// Create a custom character builder.
  /// </summary>
  /// <param name="name">The character name.</param>
  /// <exception cref="CreationException"></exception>
  public CustomCharacterBuilder(string name)
  {
    result = new()
    {
      name = name,
      DialogPosition = new(-298.5f, -119.5f),
      DialogScale = 1f,
      maxTalks = -1,
      _entranceTheme = ESound.None
    };
    int enumInt = CharacterLib.GetNextEnumInt(name);
    if (enumInt == -1)
    {
      throw new CreationException($"Attempted to create character {name} who already exists!");
    }
    result._characterType = (ECharacterType)enumInt;
  }

  public CustomCharacterBuilder SetFEMARatingBonus(float bonus)
  {
    result._FEMARatingBonus = bonus;
    return this;
  }

  /// <summary>
  /// Set the sound played at this character's room during nighttime.
  /// </summary>
  /// <param name="sound">The audio clip used.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetNightRoomSound(AudioClip sound)
  {
    result._nightRoomSound = sound;
    return this;
  }

  /// <summary>
  /// Set how many times the player has to complete the game before this character appears.
  /// </summary>
  /// <param name="completions">How many times the player has to complete the game.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetMinimumCompletionsToAppear(int completions)
  {
    result._minimumGamesCompletedToAppear = completions;
    return this;
  }

  public CustomCharacterBuilder SetCorpseMask(Texture2D mask)
  {
    result._corpseMask = mask;
    return this;
  }

  /// <summary>
  /// Set the sprite used for the Teeth sign when this character is a human.
  /// </summary>
  /// <param name="sprite">The sprite instance.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetHumanTeethSprite(Sprite sprite)
  {
    result._teethSpriteHuman = sprite;
    return this;
  }

  /// <summary>
  /// Set the sprite used for the Teeth sign when this character is a visitor.
  /// </summary>
  /// <param name="sprite">The sprite instance.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetVisitorTeethSprite(Sprite sprite)
  {
    result._teethSpriteImposter = sprite;
    return this;
  }

  /// <summary>
  /// Set the sprite used for the Hands sign when this character is a human.
  /// </summary>
  /// <param name="sprite">The sprite instance.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetHumanHandSprite(Sprite sprite)
  {
    result._handsSpriteHuman = sprite;
    return this;
  }

  /// <summary>
  /// Set the sprite used for the Hands sign when this character is a visitor.
  /// </summary>
  /// <param name="sprite">The sprite instance.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetVisitorHandSprite(Sprite sprite)
  {
    result._handsSpriteImposter = sprite;
    return this;
  }

  public CustomCharacterBuilder SetGachaScaleBias(Vector2 vec)
  {
    result._gachaScaleBias = vec;
    return this;
  }
  public CustomCharacterBuilder SetGachaScaleBias(float x, float y)
  {
    result._gachaScaleBias = new(x, y);
    return this;
  }

  public CustomCharacterBuilder SetGachaPosBias(float x, float y)
  {
    result._gachaPosBias = new(x, y);
    return this;
  }

  public CustomCharacterBuilder SetGachaPosBias(Vector2 vec)
  {
    result._gachaPosBias = vec;
    return this;
  }

  /// <summary>
  ///  Set the animation used for the Eyes sign when this character is a visitor.
  /// </summary>
  /// <param name="iris">The sprite for the iris.</param>
  /// <param name="white">The sprite used for the white of the eye.</param>
  /// <param name="ease">The easing used when moving the iris.</param>
  /// <param name="distanceMultiplier">How much to multiply the distance when moving. Not recommended to put above 2.</param>
  /// <param name="minMoveDuration">Minimum time (in seconds) the iris should move for.</param>
  /// <param name="maxMoveDuration">Maximum time (in seconds) the iris should move for.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetVisitorEyeAnimation(Sprite iris, Sprite white, Ease ease, float distanceMultiplier, float minMoveDuration, float maxMoveDuration)
  {
    CharacterEyeData eye = new()
    {
      _iris = iris,
      _white = white,
      _easing = ease,
      _distanceMultiplier = distanceMultiplier,
      _minMoveDuration = minMoveDuration,
      _maxMoveDuration = maxMoveDuration
    };
    result._eyeSpriteImposter = eye;
    return this;
  }

  /// <summary>
  ///  Set the animation used for the Eyes sign when this character is a human.
  /// </summary>
  /// <param name="iris">The sprite for the iris.</param>
  /// <param name="white">The sprite used for the white of the eye.</param>
  /// <param name="ease">The easing used when moving the iris.</param>
  /// <param name="distanceMultiplier">How much to multiply the distance when moving. Not recommended to put above 2.</param>
  /// <param name="minMoveDuration">Minimum time (in seconds) the iris should move for.</param>
  /// <param name="maxMoveDuration">Maximum time (in seconds) the iris should move for.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetHumanEyeAnimation(Sprite iris, Sprite white, Ease ease, float distanceMultiplier, float minMoveDuration, float maxMoveDuration)
  {
    CharacterEyeData eye = new()
    {
      _iris = iris,
      _white = white,
      _easing = ease,
      _distanceMultiplier = distanceMultiplier,
      _minMoveDuration = minMoveDuration,
      _maxMoveDuration = maxMoveDuration
    };
    result._eyeSpriteHuman = eye;
    return this;
  }

  /// <summary>
  /// Set the animation used for the Ears sign when this character is a human.
  /// </summary>
  /// <param name="cyclingType">The cycling type for this animation.</param>
  /// <param name="frames">Each frame of the animation.</param>
  /// <param name="framesPerSecond">How many frames to play per second.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetHumanEarAnimation(EAnimationCyclingType cyclingType, IEnumerable<Sprite> frames, int framesPerSecond = 8)
  {
    AnimationData data = new(cyclingType, (Sprite[])[.. frames], framesPerSecond);
    result._earHuman = data;
    return this;
  }

  /// <summary>
  /// Set the animation used for the Ears sign when this character is a visitor.
  /// </summary>
  /// <param name="cyclingType">The cycling type for this animation.</param>
  /// <param name="frames">Each frame of the animation.</param>
  /// <param name="framesPerSecond">How many frames to play per second.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetVisitorEarAnimation(EAnimationCyclingType cyclingType, IEnumerable<Sprite> frames, int framesPerSecond = 8)
  {
    AnimationData data = new(cyclingType, (Sprite[])[.. frames], framesPerSecond);
    result._earImposter = data;
    return this;
  }

  /// <summary>
  /// Set the animation used for the Armpits sign when this character is a human.
  /// </summary>
  /// <param name="cyclingType">The cycling type for this animation.</param>
  /// <param name="frames">Each frame of the animation.</param>
  /// <param name="framesPerSecond">How many frames to play per second.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>

  public CustomCharacterBuilder SetHumanArmpitAnimation(EAnimationCyclingType cyclingType, IEnumerable<Sprite> frames, int framesPerSecond = 8)
  {
    AnimationData data = new(cyclingType, (Sprite[])[.. frames], framesPerSecond);
    result._armpitHuman = data;
    return this;
  }

  /// <summary>
  /// Set the animation used for the Armpits sign when this character is a visitor.
  /// </summary>
  /// <param name="cyclingType">The cycling type for this animation.</param>
  /// <param name="frames">Each frame of the animation.</param>
  /// <param name="framesPerSecond">How many frames to play per second.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>

  public CustomCharacterBuilder SetVisitorArmpitAnimation(EAnimationCyclingType cyclingType, IEnumerable<Sprite> frames, int framesPerSecond = 8)
  {
    AnimationData data = new(cyclingType, (Sprite[])[.. frames], framesPerSecond);
    result._armpitImposter = data;
    return this;
  }

  /// <summary>
  /// Set the poses this character can have in their room.
  /// </summary>
  /// <param name="objectStates">A list of poses.</param>
  /// <param name="startState">The pose the character starts in.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetPoses(IEnumerable<RoomObjectState<ERoomPeopleState>> objectStates, ERoomPeopleState startState)
  {
    result._poses = (RoomObjectState<ERoomPeopleState>[])[.. objectStates];
    result.HasObjectView = true;
    result.startState = startState;
    return this;
  }

  public CustomCharacterBuilder SetDialogPosition(float x, float y)
  {
    result.DialogPosition = new(x, y);
    return this;
  }

  public CustomCharacterBuilder SetDialogPosition(Vector2 position)
  {
    result.DialogPosition = position;
    return this;
  }

  public CustomCharacterBuilder SetDialogScale(float scale)
  {
    result.DialogScale = scale;
    return this;
  }

  /// <summary>
  /// Set the sound used when this character knocks at the door.
  /// </summary>
  /// <param name="sound">The knocking sound.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetKnockingSound(ESound sound)
  {
    result._knockSound = sound;
    return this;
  }

  /// <summary>
  /// Set the background sound played when viewing this character through the peephole.
  /// </summary>
  /// <param name="theme">The sound played.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetEntranceTheme(ESound theme)
  {
    result._entranceTheme = theme;
    return this;
  }

  /// <summary>
  /// Set the entrance node redirect.
  /// </summary>
  /// <param name="entranceName">The node to redirect to.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetEntranceRedirect(string entranceName)
  {
    SetNodeRedirect("entrance_{CID}", entranceName);
    return this;
  }

  /// <summary>
  /// Set the redirect for talking to this character on a specific day.
  /// </summary>
  /// <param name="talkCount">The day to redirect for. Starts at 2 and increments by one per day.</param>
  /// <param name="node">The node to redirect to.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetTalkRedirect(int talkCount, string node) {
    SetNodeRedirect($"talk_{{CID}}_{talkCount}", node);
    return this;
  }

  /// <summary>
  /// Redirects an emotion for your character to another emotion.
  /// </summary>
  /// <param name="from">The emotion to redirect.</param>
  /// <param name="to">The emotion to redirect to.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetEmotionRedirect(string from, string to)
  {
    result.EmotionRedirects[from] = to;
    return this;
  }

  public CustomCharacterBuilder SetEmotionRedirects(Dictionary<string, string> redirects)
  {
    result.EmotionRedirects = redirects;
    return this;
  }

  public CustomCharacterBuilder AddEmotionRedirects(Dictionary<string, string> redirects)
  {
    result.EmotionRedirects = result.EmotionRedirects.Concat(redirects).ToDictionary(keySelector: v => v.Key, elementSelector: v => v.Value);
    return this;
  }

  /// <summary>
  /// Redirect a node for this character to a specified node.
  /// </summary>
  /// <param name="from">The node to redirect.</param>
  /// <param name="to">The node to redirect to.</param>
  /// <returns></returns>
  public CustomCharacterBuilder SetNodeRedirect(string from, string to)
  {
    if (from.Contains("{CID}"))
    {
      from = from.Replace("{CID}", ((int)result._characterType).ToString());
    }
    if (to.Contains("{CID}"))
    {
      to = to.Replace("{CID}", ((int)result._characterType).ToString());
    }
    result.NodeReplacements[from] = to;
    return this;
  }

  public CustomCharacterBuilder SetNodeRedirects(Dictionary<string, string> redirects)
  {
    result.NodeReplacements = redirects;
    return this;
  }

  public CustomCharacterBuilder AddNodeRedirects(Dictionary<string, string> redirects)
  {
    result.NodeReplacements = result.NodeReplacements.Concat(redirects).ToDictionary(keySelector: v => v.Key, elementSelector: v => v.Value);
    return this;
  }

  /// <summary>
  /// Set how many times you can talk to this character (max once per day).
  /// </summary>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetDialogueCount(int dialogueCount)
  {
    result.maxTalks = dialogueCount;
    return this;
  }

  /// <summary>
  /// Set the days on which this character can appear.
  /// </summary>
  /// <param name="allowedDays">Each day which this character can appear on.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetDaysCanAppear(params int[] allowedDays)
  {
    result._allowedDays = allowedDays;
    return this;
  }

  /// <summary>
  /// Set the room this character is in.
  /// </summary>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetRoom(ERoom room)
  {
    result._room = room;
    return this;
  }

  /// <summary>
  /// Set where in the room this character is.
  /// </summary>
  /// <param name="place">Which position in the room this character lives at. Technically not limited to positions in the room, but it's recommended to stick to them.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetRoomPosition(ECharacterPlace place)
  {
    result._place = place;
    return this;
  }

  /// <summary>
  /// Set whether or not this character should randomly be either a Visitor or a Human.
  /// </summary>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetRandomlyGenerated(bool random)
  {
    result._isStatusRandomlyGenerated = random;
    return this;
  }

  /// <summary>
  /// Set whether or not this character is a Visitor by defualt.
  /// </summary>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetIsVisitor(bool visitor)
  {
    result._isImposter = visitor;
    return this;
  }

  /// <summary>
  /// Set whether or not this character should stay as is.
  /// Unsure if this has any other effects besides nullifying visitor state changes.
  /// </summary>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetIsPreset(bool preset)
  {
    result._isPreset = preset;
    return this;
  }

  /// <summary>
  /// Set the emotions this character can use during dialogue.
  /// </summary>
  /// <param name="emotions">A list of emotions.</param>
  /// <returns>CustomCharacterBuilder instance.</returns>
  public CustomCharacterBuilder SetEmotions(IEnumerable<ACharacterSpriteByEmotion> emotions)
  {
    result._emotions = (ACharacterSpriteByEmotion[])[.. emotions];
    return this;
  }

  // cannot determine what this does from code, need to test.
  public CustomCharacterBuilder SetEntranceScale(float scale)
  {
    result._entranceScale = scale;
    return this;
  }

  /// <summary>
  /// Get the underlying character from this builder.
  /// </summary>
  /// <returns>CustomCharacter instance.</returns>
  public CustomCharacter Build()
  {
    return result;
  }
}