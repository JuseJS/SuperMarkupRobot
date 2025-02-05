using UnityEngine;
using System;

public static class EventSystem
{
   public static event Action<TagObject> OnTagPickedUp;
   public static event Action<TagObject> OnTagPlaced;
   public static event Action OnLevelCompleted;
   public static event Action<int> OnLevelStarted;

   public static void RaiseTagPickedUp(TagObject tag)
   {
       OnTagPickedUp?.Invoke(tag);
   }

   public static void RaiseTagPlaced(TagObject tag)
   {
       OnTagPlaced?.Invoke(tag);
   }

   public static void RaiseLevelCompleted()
   {
       OnLevelCompleted?.Invoke();
   }

   public static void RaiseLevelStarted(int level)
   {
       OnLevelStarted?.Invoke(level);
   }
}