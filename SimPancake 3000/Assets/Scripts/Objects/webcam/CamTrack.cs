using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTrack : MonoBehaviour
{
	public WebCamTexture wct;
	public Renderer renderer;
	public Material liveMaterial;

	public Color32 searchColor;

	[Range(0.01f, 0.75f)]
	public float thresshold = 0.1f;
	public int testRange = 3; //amount of px to test that are not in the thresshold
	Color32[] image;

	private bool canUpdate = false;

	private List<TrackingData> trackingData;

	public int size = 4;

	public Color debugColor = Color.red;
	public bool debugPause = false;
	public bool debugSkipSizeCheck = false;
	public bool debugDraw = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {

		if ( WebCamTexture.devices.Length > 0 )
		{
			string webcamName = WebCamTexture.devices[ 0 ].name;
			wct = new WebCamTexture( webcamName );
			print("Connected to webcame: "+webcamName);
		}

		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

		if ( Application.HasUserAuthorization( UserAuthorization.WebCam ) )
		{
			Debug.Log( "webcam found" );
		}
		else
		{
			Debug.Log( "webcam not found" );
		}
		wct.requestedFPS = 30;
		wct.Play();

		print( "Webcame w/h: " + wct.width + "/" + wct.height +" # FPS: "+wct.requestedFPS );

		image = new Color32[wct.width * wct.height];

		renderer = GetComponent<Renderer>();
		liveMaterial = renderer.material;

		if ( liveMaterial != null )
			liveMaterial.mainTexture = wct;
		else
			Debug.LogError("No Material to applie texture to");

		trackingData = new List<TrackingData>();

		canUpdate = true;
	}

	private void Update() //not working :|
	{

		if ( !canUpdate ) return;

		trackingData.Clear();

		int foundX_ID = 0;

		image = wct.GetPixels32( image );

		for (int y = 0; y < wct.height; y++ )
		{
			foundX_ID = 0;

				foundX_ID = TraceRow_getFirst( foundX_ID, y );

				if ( foundX_ID >= 0 && !IsTrackingPosition( foundX_ID, y ) )
				{

					Vector2Int[] trackPosition = TraceRow_getMinMax( foundX_ID, y, true );
					TrackingData td = new TrackingData( trackPosition[ 0 ], trackPosition[ 1 ] );

					if ( debugSkipSizeCheck || td.IsMinSize( size, size ) )
					{
						trackingData.Add( td );
						DrawDebugBox( trackPosition[ 0 ], trackPosition[ 1 ] );
					}

					foundX_ID = td.bottomRight_position.x;

				}


		}

	}



	private void Update__()//_old_1()
	{

		if ( !wct.didUpdateThisFrame ) return;

		image = wct.GetPixels32( image );

		int x_minPx = 0, x_maxPx = 0;
		int y_minPx = 0, y_maxPx = 0;
		bool inToll = false;

		bool found = false;

		print( "Searching..." );

		for ( int x = 0; x < wct.width; x++ )
		{
			for ( int y = 0; y < wct.height; y++ )
			{

				Color32 pxCol = GetPixleAt(x, y);

				if(!inToll && InThresshold(pxCol))
				{
					x_minPx = x;
					y_minPx = y;
					x_maxPx = TraceRow_max( x, y );
					//y++;

					inToll = true;

					

				}
				else if( inToll && InThresshold( pxCol ) )
				{

					int tempX_min = TraceRow_min( x, y );
					int tempX_max = TraceRow_max( x, y );

					if ( tempX_min < x_minPx )
						x_minPx = tempX_min;

					if ( tempX_max > x_maxPx )
						x_maxPx = tempX_max;

					y_maxPx = y;

					

				}
				else if( !InThresshold(pxCol) )
				{
					inToll = false;

					if(x_maxPx - x_minPx > 8 && y_maxPx - y_minPx > 8)
					{
						print( "Iv Found somethink thats at least 8x8 (" + ( x_maxPx - x_minPx ) + "x" + ( y_maxPx - y_minPx ) + "), exiting search..." );
						found = true;
						break;
					}

				}


			}
		}

		if ( !found ) return;

		float y_mi = ( x_minPx / (float)wct.width ) * 100f;
		float y_ma = ( x_maxPx / (float)wct.width ) * 100f;
		float x_mi = ( y_minPx / (float)wct.height ) * 100f;
		float x_ma = ( y_maxPx / (float)wct.height ) * 100f;

		//Hoz lines
		Debug.DrawLine( new Vector3( x_mi, y_mi, 0 ), new Vector3( x_ma, y_mi, 0 ), debugColor );
		Debug.DrawLine( new Vector3( x_mi, y_ma, 0 ), new Vector3( x_ma, y_ma, 0 ), debugColor );

		//vert Lines
		Debug.DrawLine( new Vector3( x_mi, y_mi, 0 ), new Vector3( x_mi, y_ma, 0 ), debugColor );
		Debug.DrawLine( new Vector3( x_ma, y_mi, 0 ), new Vector3( x_ma, y_ma, 0 ), debugColor );

		print( "min x/y: " + x_mi + "/" + y_mi + " # max x/y: " + x_ma + "/" + y_ma +" ## ("+x_minPx+"|"+y_minPx+"/"+x_maxPx+"|" + y_maxPx +")");

	}

	/// <summary>
	/// First first pixel in thresshold
	/// </summary>
	/// <param name="y">row id to trace</param>
	/// <returns>first collum (X axis) id of pixel in thresshold, -1 if not found </returns>
	int TraceRow_getFirst( int startX, int y )
	{
				

		for ( int x = startX; x < wct.width; x++ )
		{
			Color32 pxCol = GetPixleAt( x, y );

			if ( InThresshold( pxCol ) )
			{
			//	print( "hello #" + x + "#" + y + " # Col: " + pxCol );

				return x;
			}
		}

		return -1;

	}

	Vector2Int[] TraceRow_getMinMax(int x, int y, bool first, int testingNext = -1)
	{

		Vector2Int[] position = new Vector2Int[ 2 ];	//0 = topLeft, 1 = bottomRight
		position[ 0 ] = Vector2Int.zero;
		position[ 1 ] = Vector2Int.zero;

		if ( first )
			position[ 0 ] = new Vector2Int( x, y );
		else
			position[ 0 ] = new Vector2Int( TraceRow_min( x, y ), y );

		position[ 1 ] = new Vector2Int(TraceRow_max( x, y ), y);

	//	bool testNext = !testingNext && position[ 0 ].x != -1 && position[ 1 ].x != -1;

		if ( ( position[ 0 ].x != -1 || position[ 1 ].x != -1 ) )
			testingNext = -1;
		else
			testingNext++;

		//if ( testNext )
		//	doFuckAll();
			

			if ( ( position[ 0 ].x != -1 || position[ 1 ].x != -1 ) && y + 1 < wct.height || ( testingNext < testRange && y + 1 < wct.height ) )
			{
		
				// Get next row.
				Vector2Int[] nextRowPosition = TraceRow_getMinMax( position[ 0 ].x, y + 1, false, testingNext );

				if ( nextRowPosition[ 0 ].x != -1 && nextRowPosition[ 0 ].x < position[ 0 ].x )
					position[ 0 ].x = nextRowPosition[ 0 ].x;

				if ( nextRowPosition[ 1 ].x > position[ 1 ].x )
					position[ 1 ].x = nextRowPosition[ 1 ].x;

				if ( nextRowPosition[ 0 ].x != -1 || nextRowPosition[ 1 ].x != -1)
					position[ 0 ].y = nextRowPosition[ 0 ].y;

			}
					

		return position;

	}
	void doFuckAll() { }
	int TraceRow_max(int x, int y)
	{

		int foundCount = 0;
		int outOfRangeCount = 0;
		bool inThress = false;

		for( ; x < wct.width; x++ )
		{

			Color32 pxCol = GetPixleAt( x, y );
			inThress = InThresshold( pxCol );

			if ( inThress )
			{
				foundCount++;
			}

			if ( !inThress || x == wct.width - 1 )
			{
				// return the previous id if we have found at least 1 px in the thresshold
				// else break out of the loop to return none found (-1)
				if(outOfRangeCount < testRange && x != wct.width - 1 )
					outOfRangeCount++;
				else if ( foundCount > 0 )
					return x - 1;
				else
					break;
			}
			
		}

		return -1;
	}

	int TraceRow_min( int x, int y )
	{

		int foundCount = 0;
		int outOfRangeCount = 0;
		bool inThress = false;

		for ( ; x >= 0; x-- )
		{
			Color32 pxCol = GetPixleAt( x, y );
			inThress = InThresshold( pxCol );

			if(inThress)
			{
				foundCount++;
			}

			if ( !inThress || x == 0 )
			{
				// return the previous id if we have found at least 1 px in the thresshold
				// else break out of the loop to return none found (-1)
				if ( outOfRangeCount < testRange && x != wct.width - 1 )
					outOfRangeCount++;
				else if ( foundCount > 0 )
					return x + 1;
				else
					break;
			}
		}

		return -1;
	}


	void Update_old()
    {
		if ( !wct.didUpdateThisFrame ) return;

		image = wct.GetPixels32(image);
		bool found = false;

		int lastRow = -1;
		int lastCol = -1;
		int firstColInRow = -1;
		int mc = 0;

		for(int i = 0; i < image.Length; i++ )
		{
			
			int row = (int)Mathf.Floor( (float)i / (float)wct.width );
			int col = i - ( row * wct.width );

			if (InThresshold(image[i])) //searchColor == image[i])
			{
				
			//	print( "Found @ " + i + "  # row:" + row + " # col: " + col );
				found = true;

				if ( row != lastRow )
				{
					firstColInRow = col;
					Debug.DrawLine( new Vector3( ( lastRow / (float)wct.height ) * 100f, ( firstColInRow / (float)wct.width ) * 100f, 0 ), new Vector3( ( lastRow / (float)wct.height ) * 100f, ( lastCol / (float)wct.width ) * 100f, 0 ), debugColor);
			//		print( "DRAW: " + new Vector3( ( lastRow / (float)wct.height)*100f, (firstColInRow/ (float)wct.width)*100f, 0 ) + " TO: " + new Vector3( ( lastRow / (float)wct.height ) * 100f, ( lastCol / (float)wct.width ) * 100f, 0 ) );
				}
				lastRow = row;
				lastCol = col;
				mc++;
			}

			mc++;

		}

		Debug.DrawLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 100, 0 ), Color.blue );
		Debug.DrawLine( new Vector3( 0, 0, 0 ), new Vector3( 100, 0, 0 ), Color.blue );

		if ( found && debugPause)
			UnityEditor.EditorApplication.isPaused = true;

    }

	Color32 GetPixleAt(int x, int y)
	{

		int index = x + ( y * wct.width );

		if ( index >= image.Length || index < 0 )
			print( "Error" );

		return image[ index ];

	}

	//Here we should really get its distance.
	bool InThresshold_(Color32 color)
	{

		if ( color.r < searchColor.r - ( 255f * thresshold ) || color.r > searchColor.r + ( 255f * thresshold ) )
			return false;
		else if ( color.g < searchColor.g - ( 255f * thresshold ) || color.g > searchColor.g + ( 255f * thresshold ) )
			return false;
		else if ( color.b < searchColor.b - ( 255f * thresshold ) || color.b > searchColor.b + ( 255f * thresshold ) )
			return false;

		return true;
	}

	bool InThresshold(Color32 color)
	{
		return ColorDistance( color ) <= thresshold;
	}

	float ColorDistance(Color32 color)
	{

		float r = ( searchColor.r - color.r ) ^ 2;
		float g = ( searchColor.g - color.g ) ^ 2;
		float b = ( searchColor.b - color.b ) ^ 2;

		return Mathf.Sqrt(r + g + b);

	}

	bool IsTrackingPosition(int x, int y)
	{

		for(int i = 0; i < trackingData.Count; i++ )
		{

			if ( trackingData[i].Contains(x, y) ) return true;

		}

		return false;

	}

	void DrawDebugBox(Vector2 topLeft, Vector2 bottomRight)
	{

		if ( !debugDraw ) return;

		Vector2 min = topLeft;
		Vector2 max = bottomRight;

		topLeft.y = ( min.x / (float)wct.width ) * 100f;
		bottomRight.y = ( max.x / (float)wct.width ) * 100f;

		topLeft.x = ( min.y / (float)wct.height ) * 100f;
		bottomRight.x = ( max.y / (float)wct.height ) * 100f;

		//Hoz lines
		Debug.DrawLine( new Vector3( topLeft.x, topLeft.y, 0 ), new Vector3( bottomRight.x, topLeft.y, 0 ), debugColor );			//top
		Debug.DrawLine( new Vector3( topLeft.x, bottomRight.y, 0 ), new Vector3( bottomRight.x, bottomRight.y, 0 ), debugColor );	//bottom

		//vert Lines
		Debug.DrawLine( new Vector3( topLeft.x, topLeft.y, 0 ), new Vector3( topLeft.x, bottomRight.y, 0 ), debugColor );			//left
		Debug.DrawLine( new Vector3( bottomRight.x, topLeft.y, 0 ), new Vector3( bottomRight.x, bottomRight.y, 0 ), debugColor );   //right

		print( " Found tl: " + topLeft + " # br: " + bottomRight + " ## min: " + min + " # max: " + max);

	}

	[System.Serializable]
	public struct TrackingData
	{

		public Vector2Int topLeft_position, bottomRight_position;
		
		public TrackingData(Vector2Int tlPos, Vector2Int br_pos)
		{
			topLeft_position = tlPos;
			bottomRight_position = br_pos;
		}

		/// <summary>
		/// returns true if x and y are within the tracking position
		/// </summary>
		public bool Contains(int x, int y)
		{

			return x >= topLeft_position.x	    &&  y <= topLeft_position.y		&&
				   x <= bottomRight_position.x  &&  y >= bottomRight_position.y;

		}

		/// <summary>
		/// returns true if width, height >= x, y
		/// </summary>
		public bool IsMinSize(int x, int y)
		{

			return bottomRight_position.x - topLeft_position.x >= x && topLeft_position.y - bottomRight_position.y >= y;

		}

	}

}
