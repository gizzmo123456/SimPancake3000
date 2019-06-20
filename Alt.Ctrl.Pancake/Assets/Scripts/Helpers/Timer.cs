using UnityEngine;

namespace AMS_Helpers
{
	[System.Serializable]
	public class Timer {

		public float TimerLength {
			get;
			private set;
		} = 0;

		public float CurrentTime {
			get;
			private set;
		} = -1;

		public bool IsCompleat {
			get { return CurrentTime >= TimerLength; }
		}

		/// <summary>
		/// Sets the timer start time.
		/// </summary>
		/// <param name="len">How long until the timer expires</param>
		/// <param name="reset">Can the timer be reset? if false, if ther has already been set it will not get set agen</param>
		public void SetTimer(float len, bool reset) {

			if (CurrentTime <= 0 || CurrentTime > 0 && reset) {
				CurrentTime = 0;
				TimerLength = len;
			}
		}

		/// <summary>
		/// Get the precentage of the timer.
		/// </summary>
		/// <param name="clamp01">Should the precentage be clamped 01</param>
		/// <returns></returns>
		public float TimerPrecentage(bool clamp01 = true) {

			return !clamp01 ? CurrentTime / TimerLength : Mathf.Clamp01( CurrentTime / TimerLength );

		}

		public void Update(float deltaTime)
		{
			CurrentTime += deltaTime;
		}

	}

}