//-----------------------------------------------------------------------
// <copyright file="MeshTerrainMolder.cs" company="Let it roll AB">
// Copyright (c) Let it roll AB. All rights reserved.
// <author>Marcus Forsmoo</author>
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Text;

namespace Lirp
{
	public class NameUtility
	{
		public static string GetNumberedNameBad(string nameprefix,int number,int numberOfZeros) {
			StringBuilder sb = new StringBuilder(nameprefix);
			
			int currentI = numberOfZeros;
			int threshold = 0;
			while( currentI > 0 )
			{
				threshold = 1;
				for( int i=0;i<currentI;i++ )
					threshold *= 10;
				if( number < threshold )
				{
					sb.Append("0");
				}
				currentI--;
			}
			return sb.Append(number).ToString();
		}
	
	    public static string GetNumberedName(string nameprefix, int number, int numberOfDigits)
	    {
	        StringBuilder sb = new StringBuilder(nameprefix);
	        string postFix = "";
	        if (numberOfDigits <= 1)
	            postFix = number.ToString();
	        else if ( numberOfDigits == 2  )
	        {
	            if (number < 10)
	                postFix = "0" + number;
	            else
	                postFix = number.ToString();
	        }
	        else if (numberOfDigits == 3)
	        {
	            if (number < 10)
	                postFix = "00" + number;
	            else if( number < 100 )
	                postFix = "0" + number;
	            else
	                postFix = number.ToString();
	        }
	        else if (numberOfDigits == 4)
	        {
	            if (number < 10)
	                postFix = "000" + number;
	            else if (number < 100)
	                postFix = "00" + number;
	            else if (number < 1000)
	                postFix = "0" + number;
	            else
	                postFix = number.ToString();
	        }
	
	        return nameprefix + postFix;
	    }
	
	    public static string GetNumberedName(string nameprefix,int number) {
			return GetNumberedName(nameprefix,number,2);
		}
	
	    public static string ReadPrefixName(string name, int numberOfDigits,out int numDigits,out int currentIndex)
	    {
	        numDigits = 0;
	        string prefix = name;
	        bool isDigit = false;
	        currentIndex = 0;
	
	        for ( int i=1;i<= numberOfDigits; i++)
	        {
	            string testName = name.Substring(name.Length - i, i);
	            int result;
	            isDigit = int.TryParse(testName, out result);
	               
	            if( !isDigit )
	            {
	                return prefix;
	            }
	            else
	            {
	                currentIndex = result;
	                numDigits = i;
	                prefix = name.Substring(0,name.Length-i);
	            }
	        }
	        return prefix;
	    }
	
	    public static int ReadNumberedName(string name,int numberOfZeroes)
	    {
	        if (name.Length < numberOfZeroes)
	            return 0;
	        string strNum = name.Substring(name.Length - numberOfZeroes-1, numberOfZeroes);
	        int result;
	        if (int.TryParse(strNum, out result))
	            return result;
	        
	        return -1;
	    }
	}
}
