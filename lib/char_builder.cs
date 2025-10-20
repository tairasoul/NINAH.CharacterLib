using _Code.Characters;
using _Code.Infrastructure.Sound;
using _Code.Menues.HUD.Animations;
using _Code.Rooms;
using _Code.Utils.UI.ImageAnimating;
using DG.Tweening;
using UnityEngine;

namespace tairasoul.ninah.characterlib;

public class CustomCharacterBuilder
{
  private CustomCharacter result;
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

  public CustomCharacterBuilder SetNightRoomSound(AudioClip sound)
  {
    result._nightRoomSound = sound;
    return this;
  }

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

  public CustomCharacterBuilder SetHumanTeethSprite(Sprite sprite)
  {
    result._teethSpriteHuman = sprite;
    return this;
  }

  public CustomCharacterBuilder SetVisitorTeethSprite(Sprite sprite)
  {
    result._teethSpriteImposter = sprite;
    return this;
  }

  public CustomCharacterBuilder SetHumanHandSprite(Sprite sprite)
  {
    result._handsSpriteHuman = sprite;
    return this;
  }

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

  public CustomCharacterBuilder SetHumanEarAnimation(EAnimationCyclingType cyclingType, IEnumerable<Sprite> frames, int framesPerSecond = 8)
  {
    AnimationData data = new(cyclingType, (Sprite[])[.. frames], framesPerSecond);
    result._earHuman = data;
    return this;
  }

  public CustomCharacterBuilder SetVisitorEarAnimation(EAnimationCyclingType cyclingType, IEnumerable<Sprite> frames, int framesPerSecond = 8)
  {
    AnimationData data = new(cyclingType, (Sprite[])[.. frames], framesPerSecond);
    result._earImposter = data;
    return this;
  }

  public CustomCharacterBuilder SetHumanArmpitAnimation(EAnimationCyclingType cyclingType, IEnumerable<Sprite> frames, int framesPerSecond = 8)
  {
    AnimationData data = new(cyclingType, (Sprite[])[.. frames], framesPerSecond);
    result._armpitHuman = data;
    return this;
  }

  public CustomCharacterBuilder SetVisitorArmpitAnimation(EAnimationCyclingType cyclingType, IEnumerable<Sprite> frames, int framesPerSecond = 8)
  {
    AnimationData data = new(cyclingType, (Sprite[])[.. frames], framesPerSecond);
    result._armpitImposter = data;
    return this;
  }

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

  public CustomCharacterBuilder SetKnockingSound(ESound sound)
  {
    result._knockSound = sound;
    return this;
  }

  public CustomCharacterBuilder SetEntranceTheme(ESound theme)
  {
    result._entranceTheme = theme;
    return this;
  }

  public CustomCharacterBuilder SetEntranceRedirect(string entranceName)
  {
    SetNodeRedirect("entrance_{CID}", entranceName);
    return this;
  }

  public CustomCharacterBuilder SetTalkRedirect(int talkCount, string node) {
    SetNodeRedirect($"talk_{{CID}}_{talkCount}", node);
    return this;
  }

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

  public CustomCharacterBuilder SetDialogueCount(int dialogueCount)
  {
    result.maxTalks = dialogueCount;
    return this;
  }

  public CustomCharacterBuilder SetDaysCanAppear(params int[] allowedDays)
  {
    result._allowedDays = allowedDays;
    return this;
  }

  public CustomCharacterBuilder SetRoom(ERoom room)
  {
    result._room = room;
    return this;
  }

  public CustomCharacterBuilder SetRoomPosition(ECharacterPlace place)
  {
    result._place = place;
    return this;
  }

  public CustomCharacterBuilder SetRandomlyGenerated(bool random)
  {
    result._isStatusRandomlyGenerated = random;
    return this;
  }

  public CustomCharacterBuilder SetIsVisitor(bool visitor)
  {
    result._isImposter = visitor;
    return this;
  }

  public CustomCharacterBuilder SetIsPreset(bool preset)
  {
    result._isPreset = preset;
    return this;
  }

  public CustomCharacterBuilder SetEmotions(IEnumerable<ACharacterSpriteByEmotion> emotions)
  {
    result._emotions = (ACharacterSpriteByEmotion[])[.. emotions];
    return this;
  }

  public CustomCharacterBuilder SetEntranceScale(float scale)
  {
    result._entranceScale = scale;
    return this;
  }

  public CustomCharacter Build()
  {
    return result;
  }
}