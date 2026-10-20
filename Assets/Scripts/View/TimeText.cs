using UnityEngine;

namespace LostControl.View
{
    public static class TimeText
    {
        /// <summary>Seconds as m:ss.</summary>
        public static string Format(float seconds)
        {
            int total = Mathf.FloorToInt(seconds);
            return $"{total / 60}:{total % 60:00}";
        }
    }
}
