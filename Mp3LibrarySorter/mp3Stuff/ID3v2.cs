using System;
using System.Collections;
using System.Text;
using System.IO;
using mp3info;

namespace Mp3LibrarySorter.mp3Stuff
{
	/// <summary>
	/// Summary description for ID3v2.
	/// </summary>
	public class ID3V2
	{
		public string Filename;

		// id3v2 header
		public int MajorVersion;
		public int MinorVersion;
		
		public bool FA_Unsynchronisation;
		public bool FB_ExtendedHeader;
		public bool FC_ExperimentalIndicator;
		public bool FD_Footer;

		// ext header 
		public ulong ExtHeaderSize;
		public int ExtNumFlagBytes;

		public bool EB_Update;
		public bool EC_CRC;
		public bool ED_Restrictions;

		public ulong extC_CRC;
		public byte extD_Restrictions;

		private BinaryReader _br;
	    public bool HasTag;

		public ulong HeaderSize;
		
		public Hashtable FramesHash;
		public ArrayList Frames;

		// song info
		public string Title;
		public string Artist;
		public string Album;
		public string Year;
		public string Comment;
		public string Genre;
		public string Track;
		public string TotalTracks;
		
		private void Initialize_Components()
		{
			Filename = "";
			MajorVersion = 0;
			MinorVersion = 0;

			FA_Unsynchronisation = false;
			FB_ExtendedHeader = false;
			FC_ExperimentalIndicator = false;

			Frames = new ArrayList();
			FramesHash = new Hashtable();

			Album = "";
			Artist = "";
			Comment = "";
			extC_CRC = 0;
			EB_Update = false;
			EC_CRC = false;
			ED_Restrictions = false;
			extD_Restrictions = 0;
			ExtHeaderSize = 0;
			ExtNumFlagBytes = 0;
			HasTag = false;
			HeaderSize = 0;
			Title = "";
			TotalTracks = "";
			Track = "";
			Year = "";
		}
		 
		public ID3V2( string fileName)
		{
			Initialize_Components();
			Filename = fileName;
		}
	
		private void ReadHeader ( )
		{
			// bring in the first three bytes.  it must be ID3 or we have no tag
			// TODO add logic to check the end of the file for "3D1" and other
			// possible starting spots
			var id3start = new string (_br.ReadChars(3));

			// check for a tag
			if  (!id3start.Equals("ID3"))
			{
				// TODO we are fucked.
				//throw id3v2ReaderException;
				HasTag = false;
				return;
			}
		    HasTag = true;

		    // read id3 version.  2 bytes:
		    // The first byte of ID3v2 version is it's major version,
		    // while the second byte is its revision number
		    MajorVersion = Convert.ToInt32(_br.ReadByte());
		    MinorVersion = Convert.ToInt32(_br.ReadByte());
		
		    //read next byte for flags
		    bool [] boolar = BitReader.ToBitBool(_br.ReadByte());
		    // set the flags
		    FA_Unsynchronisation= boolar[0];
		    FB_ExtendedHeader = boolar[1];
		    FC_ExperimentalIndicator = boolar[2];

		    // read teh size
		    // this code is courtesy of Daniel E. White w/ minor modifications by me  Thanx Dan
		    //Dan Code 
		    char[] tagSize = _br.ReadChars(4);    // I use this to read the bytes in from the file
		    int[] bytes = new int[4];      // for bit shifting
		    ulong newSize = 0;    // for the final number
		    // The ID3v2 tag size is encoded with four bytes
		    // where the most significant bit (bit 7)
		    // is set to zero in every byte,
		    // making a total of 28 bits.
		    // The zeroed bits are ignored
		    //
		    // Some bit grinding is necessary.  Hang on.
			

		    bytes[3] =  tagSize[3]             | ((tagSize[2] & 1) << 7) ;
		    bytes[2] = ((tagSize[2] >> 1) & 63) | ((tagSize[1] & 3) << 6) ;
		    bytes[1] = ((tagSize[1] >> 2) & 31) | ((tagSize[0] & 7) << 5) ;
		    bytes[0] = ((tagSize[0] >> 3) & 15) ;

		    newSize  = ((UInt64)10 +	(UInt64)bytes[3] |
		                ((UInt64)bytes[2] << 8)  |
		                ((UInt64)bytes[1] << 16) |
		                ((UInt64)bytes[0] << 24)) ;
		    //End Dan Code
		
		    HeaderSize = newSize;
		}

		private void ReadFrames ()
		{
			var f = new id3v2Frame();
			do 
			{
				f = f.ReadFrame(_br, MajorVersion);
				
				// check if we have hit the padding.
				if (f.padding)
				{
					//we hit padding.  lets advance to end and stop reading.
					_br.BaseStream.Position = Convert.ToInt64(HeaderSize);
					break;
				}
				Frames.Add(f);
				FramesHash.Add(f.frameName, f);
				#region frameprocessing

				/*
				else 
				{
					// figure out which type it is
					if (f.frameName.StartsWith("T"))
					{
						if (f.frameName.Equals("TXXX"))
						{
							ProcessTXXX(f);
						}
						else 
						{
							ProcessTTYPE(f);
						}
					}
					else
					{
						if (f.frameName.StartsWith("W"))
						{
							if (f.frameName.Equals("WXXX"))
							{
								ProcessWXXX(f);
							}
							else 
							{
								ProcessWTYPE(f);
							}
						}
						else
						{
							// if it isn't  a muliple reader case (above) then throw it into the switch to process
							switch (f.frameName)
							{
							
								case "IPLS":
									ProcessIPLS(f);
									break;
								case "MCDI":
									ProcessMCDI(f);
									break;
								case "UFID":
									ProcessUFID(f);
									break;
								case "COMM":
									ProcessCOMM(f);
									break;
									
								default:
									frames.Add(f.frameName, f.frameContents);
									AddItemToList(f.frameName, "non text");
									break;
							}
				}
			
		}


			}*/
				#endregion
			} while (_br.BaseStream.Position  <= Convert.ToInt64(HeaderSize));
		}		
		
		public void Read()
		{
		    using (var fs = new FileStream(Filename, FileMode.Open, FileAccess.Read))
		    {
		        _br = new BinaryReader (fs);
		        ReadHeader();
		        if (HasTag)
		        {
		            if (FB_ExtendedHeader)
		            {
		                ReadExtendedHeader();
		            }
		            ReadFrames();
		            if (FD_Footer)
		            {
		                ReadFooter();
		            }
		            ParseCommonHeaders();
		        }
		    }
		    _br.Close();
			#region tag reader

			/*if (!fileOpen)
					{
						if (!this.filename == "")
						{
							OpenFile();
						}
						else
						{
							// we are fucked.  throw an exception or something
						}
					}
					// bring in the first three bytes.  it must be ID3 or we have no tag
					// TODO add logic to check the end of the file for "3D1" and other
					// possible starting spots
					string id3start = new string (br.ReadChars(3));

					// check for a tag
					if  (!id3start.Equals("ID3"))
					{
						// TODO we are fucked.  not really we just don't ahve a tag
						// and we need to bail out gracefully.
						throw id3v23ReaderException;
					}
					else 
					{
						// we have a tag
						this.header = true;
					}

					// read id3 version.  2 bytes:
					// The first byte of ID3v2 version is it's major version,
					// while the second byte is its revision number
					this.MajorVersion = System.Convert.ToInt32(br.ReadByte());
					this.MinorVersion = System.Convert.ToInt32(br.ReadByte());

					// here is where we get fancy.  I am useing silisoft's php code as 
					// a reference here.  we are going to try and parse for 2.2, 2.3 and 2.4
					// in one pass.  hold on!!

					if ((this.header) && (this.MajorVersion <= 4)) // probably won't work on higher versions
					{
						// (%ab000000 in v2.2, %abc00000 in v2.3, %abcd0000 in v2.4.x)
						//read next byte for flags
						bool [] boolar = BitReader.ToBitBool(br.ReadByte());
						// set the flags
						if (this.MajorVersion == 2)
						{
							this.FA_Unsyncronisation = boolar[0];
							this.FB_ExtendedHeader = boolar[1];
						}
						else if ( this.MajorVersion == 3 )
						{
							// set the flags
							this.FA_Unsyncronisation = boolar[0];
							this.FB_ExtendedHeader = boolar[1];
							this.FC_ExperimentalIndicator = boolar[2];
						}
						else if (this.MajorVersion == 4)
						{
							// set the flags
							this.FA_Unsyncronisation = boolar[0];
							this.FB_ExtendedHeader = boolar[1];
							this.FC_ExperimentalIndicator = boolar[2];
							this.FD_Footer = boolar[3];
						}

						// read teh size
						// this code is courtesy of Daniel E. White w/ minor modifications by me  Thanx Dan
						//Dan Code 
						char[] tagSize = br.ReadChars(4);    // I use this to read the bytes in from the file
						int[] bytes = new int[4];      // for bit shifting
						ulong newSize = 0;    // for the final number
						// The ID3v2 tag size is encoded with four bytes
						// where the most significant bit (bit 7)
						// is set to zero in every byte,
						// making a total of 28 bits.
						// The zeroed bits are ignored
						//
						// Some bit grinding is necessary.  Hang on.
				
			

						bytes[3] =  tagSize[3]             | ((tagSize[2] & 1) << 7) ;
						bytes[2] = ((tagSize[2] >> 1) & 63) | ((tagSize[1] & 3) << 6) ;
						bytes[1] = ((tagSize[1] >> 2) & 31) | ((tagSize[0] & 7) << 5) ;
						bytes[0] = ((tagSize[0] >> 3) & 15) ;

						newSize  = ((UInt64)10 +	(UInt64)bytes[3] |
							((UInt64)bytes[2] << 8)  |
							((UInt64)bytes[1] << 16) |
							((UInt64)bytes[0] << 24)) ;
						//End Dan Code
		
						this.id3v2HeaderSize = newSize;
			

					}
					*/
				#endregion
		}

		private void ParseCommonHeaders()
		{
			if (MajorVersion == 2)
			{
				if(FramesHash.Contains("TT2"))
				{
					byte [] bytes = ((id3v2Frame)FramesHash["TT2"]).frameContents;
					var sb = new StringBuilder();

				    for (var i = 1; i < bytes.Length; i++)
					{
    					sb.Append(Convert.ToChar(bytes[i]));
					}
                    Title = sb.ToString();
                }
			
				if(FramesHash.Contains("TP1"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["TP1"]).frameContents;

				    for (int i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Artist = sb.ToString();
					}
				}
				if(FramesHash.Contains("TAL"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["TAL"]).frameContents;

				    for (int i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Album = sb.ToString();
					}
				}
				if(FramesHash.Contains("TYE"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["TYE"]).frameContents;

				    for (var i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Year = sb.ToString();
					}
				}
				if(FramesHash.Contains("TRK"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["TRK"]).frameContents;

				    for (var i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Track = sb.ToString();
					}
				}
				if(FramesHash.Contains("TCO"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["TCO"]).frameContents;

				    for (var i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Genre = sb.ToString();
					}
				}
				if(FramesHash.Contains("COM"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["COM"]).frameContents;

				    for (var i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Comment = sb.ToString();
					}
				}
			}
			else 
			{ // $id3info["majorversion"] > 2
				if(FramesHash.Contains("TIT2"))
				{
					byte [] bytes = ((id3v2Frame)FramesHash["TIT2"]).frameContents;
					var sb = new StringBuilder();

				    for (var i = 1; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						
						//this.Title = myString.Substring(1);
						Title = sb.ToString();
					}
				}

				if(FramesHash.Contains("TPE1"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["TPE1"]).frameContents;

				    for (int i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Artist = sb.ToString();
					}
				}
				if(FramesHash.Contains("TALB"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["TALB"]).frameContents;

				    for (var i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Album = sb.ToString();
					}
				}
				if(FramesHash.Contains("TYER"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["TYER"]).frameContents;

				    for (var i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Year = sb.ToString();
					}
				}
				if(FramesHash.Contains("TRCK"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["TRCK"]).frameContents;

				    for (var i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Track = sb.ToString();
					}
				}
				if(FramesHash.Contains("TCON"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["TCON"]).frameContents;

				    for (var i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Genre = sb.ToString();
					}
				}
				if(FramesHash.Contains("COMM"))
				{
					var sb = new StringBuilder();
					byte [] bytes = ((id3v2Frame)FramesHash["COMM"]).frameContents;

				    for (var i = 0; i < bytes.Length; i++)
					{
						if (i == 0)
						{
							//read the text encoding.
						}
						else
						{
							sb.Append(Convert.ToChar(bytes[i]));
						}
						Comment = sb.ToString();
					}
				}
			}
	
			string [] trackHolder = Track.Split('/');
			Track = trackHolder[0];
			if (trackHolder.Length > 1)
				TotalTracks = trackHolder[1];
				
		}

		private void ReadFooter()
		{
			
			// bring in the first three bytes.  it must be ID3 or we have no tag
			// TODO add logic to check the end of the file for "3D1" and other
			// possible starting spots
			var id3Start = new string (_br.ReadChars(3));

			// check for a tag
			if  (!id3Start.Equals("3DI"))
			{
				// TODO we are fucked.  not really we just don't ahve a tag
				// and we need to bail out gracefully.
				//throw id3v23ReaderException;
			}
			else 
			{
				// we have a tag
				HasTag = true;
			}

			// read id3 version.  2 bytes:
			// The first byte of ID3v2 version is it's major version,
			// while the second byte is its revision number
			MajorVersion = Convert.ToInt32(_br.ReadByte());
			MinorVersion = Convert.ToInt32(_br.ReadByte());

			// here is where we get fancy.  I am useing silisoft's php code as 
			// a reference here.  we are going to try and parse for 2.2, 2.3 and 2.4
			// in one pass.  hold on!!

			if ((HasTag) && (MajorVersion <= 4)) // probably won't work on higher versions
			{
				// (%ab000000 in v2.2, %abc00000 in v2.3, %abcd0000 in v2.4.x)
				//read next byte for flags
				bool [] boolar = BitReader.ToBitBool(_br.ReadByte());
				// set the flags
				if (MajorVersion == 2)
				{
					FA_Unsynchronisation = boolar[0];
					FB_ExtendedHeader = boolar[1];
				}
				else if ( MajorVersion == 3 )
				{
					// set the flags
					FA_Unsynchronisation = boolar[0];
					FB_ExtendedHeader = boolar[1];
					FC_ExperimentalIndicator = boolar[2];
				}
				else if (MajorVersion == 4)
				{
					// set the flags
					FA_Unsynchronisation = boolar[0];
					FB_ExtendedHeader = boolar[1];
					FC_ExperimentalIndicator = boolar[2];
					FD_Footer = boolar[3];
				}

				// read teh size
				// this code is courtesy of Daniel E. White w/ minor modifications by me  Thanx Dan
				//Dan Code 
				char[] tagSize = _br.ReadChars(4);    // I use this to read the bytes in from the file
				var bytes = new int[4];      // for bit shifting
			    // The ID3v2 tag size is encoded with four bytes
				// where the most significant bit (bit 7)
				// is set to zero in every byte,
				// making a total of 28 bits.
				// The zeroed bits are ignored
				//
				// Some bit grinding is necessary.  Hang on.
				bytes[3] =  tagSize[3]             | ((tagSize[2] & 1) << 7) ;
				bytes[2] = ((tagSize[2] >> 1) & 63) | ((tagSize[1] & 3) << 6) ;
				bytes[1] = ((tagSize[1] >> 2) & 31) | ((tagSize[0] & 7) << 5) ;
				bytes[0] = ((tagSize[0] >> 3) & 15) ;

				ulong newSize = (10 +	(UInt64)bytes[3] |
				                 ((UInt64)bytes[2] << 8)  |
				                 ((UInt64)bytes[1] << 16) |
				                 ((UInt64)bytes[0] << 24));
				//End Dan Code
		
				HeaderSize = newSize;
			}
		}

		private void ReadExtendedHeader()
		{
			//			Extended header size   4 * %0xxxxxxx
			//			Number of flag bytes       $01
			//			Extended Flags             $xx
			//			Where the 'Extended header size' is the size of the whole extended header, stored as a 32 bit synchsafe integer.
			// read teh size
			// this code is courtesy of Daniel E. White w/ minor modifications by me  Thanx Dan
			//Dan Code 
			char[] extHeaderSize = _br.ReadChars(4);    // I use this to read the bytes in from the file
			var bytes = new int[4];      // for bit shifting
		    // The ID3v2 tag size is encoded with four bytes
			// where the most significant bit (bit 7)
			// is set to zero in every byte,
			// making a total of 28 bits.
			// The zeroed bits are ignored
			//
			// Some bit grinding is necessary.  Hang on.
			bytes[3] =  extHeaderSize[3]              | ((extHeaderSize[2] & 1) << 7) ;
			bytes[2] = ((extHeaderSize[2] >> 1) & 63) | ((extHeaderSize[1] & 3) << 6) ;
			bytes[1] = ((extHeaderSize[1] >> 2) & 31) | ((extHeaderSize[0] & 7) << 5) ;
			bytes[0] = ((extHeaderSize[0] >> 3) & 15) ;

			ulong newSize = (10 +	(UInt64)bytes[3] |
			                 ((UInt64)bytes[2] << 8)  |
			                 ((UInt64)bytes[1] << 16) |
			                 ((UInt64)bytes[0] << 24));
			//End Dan Code

			ExtHeaderSize = newSize;
			// next we read the number of flag bytes

			ExtNumFlagBytes = Convert.ToInt32(_br.ReadByte());

			// read the flag byte(s) and set the flags
			bool[] extFlags = BitReader.ToBitBools(_br.ReadBytes(ExtNumFlagBytes));

			EB_Update = extFlags[1];
			EC_CRC = extFlags[2];
			ED_Restrictions = extFlags[3];

			// check for flags
			if (EB_Update)
			{
				// this tag has no data but will have a null byte so we need to read it in
				//Flag data length      $00
				_br.ReadByte();
			}

			if (EC_CRC)
			{
				//        Flag data length       $05
				//       Total frame CRC    5 * %0xxxxxxx
				// read the first byte and check to make sure it is 5.  if not the header is corrupt
				// we will still try to process but we may be funked.

				int extC_FlagDataLength = Convert.ToInt32(_br.ReadByte());
				if (extC_FlagDataLength == 5)
				{
					extHeaderSize = _br.ReadChars(5);    // I use this to read the bytes in from the file
					bytes = new int[4];      // for bit shifting
				    // The ID3v2 tag size is encoded with four bytes
					// where the most significant bit (bit 7)
					// is set to zero in every byte,
					// making a total of 28 bits.
					// The zeroed bits are ignored
					//
					// Some bit grinding is necessary.  Hang on.
					bytes[4] = extHeaderSize[4]		|  ((extHeaderSize[3] & 1) << 7 ) ;
					bytes[3] = ((extHeaderSize[3] >> 1) & 63) | ((extHeaderSize[2] & 3) << 6) ;
					bytes[2] = ((extHeaderSize[2] >> 2) & 31) | ((extHeaderSize[1] & 7) << 5) ;
					bytes[1] = ((extHeaderSize[1] >> 3) & 15) | ((extHeaderSize[0] & 15) << 4) ;
					bytes[0] = ((extHeaderSize[0] >> 4) & 7);

					newSize  = (10 +	(UInt64)bytes[4] |
						((UInt64)bytes[3] << 8)  |
						((UInt64)bytes[2] << 16) |
						((UInt64)bytes[1] << 24) |
						((UInt64)bytes[0] << 32)) ;
				
					ExtHeaderSize = newSize;
				}
			}

			if (ED_Restrictions)
			{
				// Flag data length       $01
				//Restrictions           %ppqrrstt
				// advance past flag data lenght byte
				_br.ReadByte();
				extD_Restrictions = _br.ReadByte();
			}
		}
	}
}
