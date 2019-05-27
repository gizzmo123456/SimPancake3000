using UnityEngine;

namespace AMS_Helpers {

	[System.Serializable]
	public class MinMax {

		public float min = 0f;
		public float max = 0f;
		public float current = 0f;
		public float Clamped_current {
			get { return Mathf.Clamp(current, min, max); }
		}

		/// <summary>
		/// The diffents
		/// </summary>
		public float Range {

			get { return (max - min); }

		}

		public float Precent {
			get { return (current - min) / Range; }
		}

		public float PrecentValue {
			get { return Range * Precent; }
		}

		public float ClampedPrecent{
			get { return Range == 0 ? 0 : (Clamped_current-min) / Range; }
		}

		public bool InRange
		{
			get { return current > min && current < max; }
		}

		public MinMax(float mi, float ma)
		{

			min = mi;
			max = ma;

		}

		public MinMax(float mi, float ma, float c)
		{

			min = mi;
			max = ma;
			current = c;

		}

		public float GetValue(float precentage, bool clamped = true){

			return min + (Range * (clamped ? Mathf.Clamp01(precentage) : precentage));

		}

		public float GetRand() { return Random.Range(min, max); }
		public int GetRand_Int() { return Random.Range(Mathf.FloorToInt(min), Mathf.FloorToInt(max + 1)); }
		
		public static bool operator ==(MinMax lhs, MinMax rhs)
		{

			return lhs.min == rhs.min ? lhs.max == rhs.max ? true : false : false;

		}

		public static bool operator !=(MinMax lhs, MinMax rhs)
		{

			return lhs.min != rhs.min ? lhs.max != rhs.max ? true : false : false;

		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


	}

}