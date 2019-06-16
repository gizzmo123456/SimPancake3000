using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO:
 * Add chack dir
 * Add Write to file
 * Add Create file
 * 
 */
/// <summary>
/// Helper class for handlering files.
/// </summary>
[System.Serializable]
public class FileHelper
{

	[SerializeField]private string fullFilePath; // path and file name :)

	public bool debug = false;

	public FileHelper(string fullPath)
	{
		fullFilePath = fullPath;
	}

	public bool FileExist()
	{
		if ( !System.IO.File.Exists( fullFilePath ) )
		{

			if ( debug )
				Debug.Log( "File Does not exists ("+fullFilePath+")" );

			return false;
		}

		return true;
	}

	public void DeleteFile()
	{

		if ( FileExist() )
			System.IO.File.Delete(fullFilePath);

	}

	public string[] ReadAllLines()
	{
		if ( FileExist() )
			return System.IO.File.ReadAllLines( fullFilePath );
		else
			return new string[ 0 ];
	}

	public string ReadLine(int lineId)
	{
		string[] lines = ReadAllLines();

		// check the line id exist.
		if ( lineId < 0 || lineId > lines.Length ) return string.Empty;	

		return lines[ lineId ];

	}

	public void WriteToFile()
	{

	}


}
