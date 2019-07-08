namespace AMS_Helpers
{
	public static class Maf
	{

		/// <summary>
		/// clamps rotation to 0, 360f
		/// </summary>
		/// <param name="rotation"> rotation to clamp</param>
		/// <returns>clampled rotation</returns>
		public static float ClampRotation( float rotation )
		{

			if ( rotation > 0 && rotation <= 360f ) return rotation;    // is this pointless???

			while ( rotation > 360f )
				rotation -= 360f;

			while ( rotation < 0f )
				rotation += 360f;

			return rotation;

		}

	}
}
