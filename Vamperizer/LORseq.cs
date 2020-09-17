using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;
using LORUtils;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Vamperizer
{
	public partial class frmTune : Form
	{
		public Sequence4 seq;
		private bool doGroups = true;
		private bool useRampsPoly = false;
		private bool useRampsBeats = false;
		private Track vampTrack = null;
		private Channel[] noteChannels = null;
		private ChannelGroup octaveGroups = null;
		private int firstCobjIdx = utils.UNDEFINED;
		private int firstCsavedIndex = utils.UNDEFINED;
		private int centiseconds = 0;
		private string fileSeqName = "";
		private MRU mruSequences = new MRU("sequence", 9);

		private void SaveAsNewSequence()
		{
			string filter = "Musical Sequence *.lms|*.lms";
			string idr = utils.DefaultSequencesPath;

			string ifile = Path.GetFileNameWithoutExtension(fileCurrent);
			if (ifile.Length < 2)
			{
				//ifile = seq.info.music.Title + " by " + seq.info.music.Artist;
				ifile = audioData.Title + " by " + audioData.Artist;
			}
			ifile += ".lms";

			dlgSaveFile.Filter = filter;
			dlgSaveFile.InitialDirectory = idr;
			dlgSaveFile.FileName = ifile;
			dlgSaveFile.FilterIndex = 1;
			dlgSaveFile.OverwritePrompt = true;
			dlgSaveFile.Title = "Save Sequence As...";
			dlgSaveFile.ValidateNames = true;
			DialogResult result = dlgSaveFile.ShowDialog(this);
			if (result == DialogResult.OK)
			{
				fileSeqName = dlgSaveFile.FileName;
				txtSaveName.Text = Path.GetFileNameWithoutExtension(fileSeqName);
				//CreateNewSequence(fileSeqName);
				seq = new Sequence4();
				seq.filename = fileSeqName;
				ImportVampsToSequence();
				SaveSequence(fileSeqName);
				if (chkAutolaunch.Checked)
				{
					System.Diagnostics.Process.Start(fileSeqName);
				}


			}
			//btnBrowseSequence.Focus();


		}

		private void SaveInExistingSequence()
		{
			string filt = "Musical Sequences *.lms|*lms";
			string idir = utils.DefaultSequencesPath;
			string ifl = txtSeqName.Text.Trim();
			string theFile = "";
			if (utils.ValidFilename(ifl, true, false))
			{
				idir = Path.GetDirectoryName(ifl);
				if (utils.ValidFilename(ifl, true, true))
				{
					if (Path.GetExtension(ifl.ToLower()) == "lms")
					{
						theFile = Path.GetFileName(ifl);
					}
				}
			}
			else
			{
				// Keep default sequence path
			}


			dlgOpenFile.Filter = filt;
			dlgOpenFile.FilterIndex = 2;    //! Temporary?  Set back to 1 and/or change filter string?
			dlgOpenFile.InitialDirectory = idir;
			//dlgOpenFile.FileName = Properties.Settings.Default.fileSeqLast;

			DialogResult dr = dlgOpenFile.ShowDialog(this);
			if (dr == DialogResult.OK)
			{
				fileCurrent = dlgOpenFile.FileName;
				txtSeqName.Text = Path.GetFileName(fileCurrent);
				string ex = Path.GetExtension(fileCurrent).ToLower();
				// If they picked an existing musical sequence
				if (ex == ".lms")
				{
					seq.ReadSequenceFile(fileCurrent);
					//fileAudioOriginal = seq.info.music.File;
					//txtFileAudio.Text = Path.GetFileNameWithoutExtension(fileAudioOriginal);
					grpAudio.Text = " Original Audio File ";
					btnBrowseAudio.Text = "Analyze";
					//fileSeqCur = fileCurrent;
					//fileChanCfg = "";
					// Add to Sequences MRU

					centiseconds = ParseCentiseconds(audioData.Duration);

					ImportVampsToSequence();
					SaveSequence(fileCurrent);


				}
				//grpGrids.Enabled = true;
				//grpTracks.Enabled = true;
				//grpAudio.Enabled = true;
				//btnBrowseAudio.Focus();

			}
		}

		private void SaveSequence(string newFilename)
		{
			ImBusy(true);
			// normal default when not testing
			seq.WriteSequenceFile_DisplayOrder(newFilename, false, false);
			System.Media.SystemSounds.Beep.Play();
			//dirtySeq = false;
			//fileSeqSave = newFilename;
			//Add to MRU
			ImBusy(false);

		}

		private Sequence4 CreateNewSequence(string theFilename)
		{
			seq = new Sequence4();
			//seq.info.author = utils.DefaultAuthor;
			// Save what we have so far...
			//seq.WriteSequenceFile_DisplayOrder(theFilename);

			return seq;
		}

		private int ParseCentiseconds(TimeSpan duration)
		{
			double rcs = Math.Round(duration.TotalMilliseconds);
			rcs = Math.Round(rcs / 10);
			int ics = (int)rcs;
			return ics;
		}

		private void SaveSongInfo()
		{
			centiseconds = ParseCentiseconds(audioData.Duration);
			seq.Centiseconds = centiseconds;
			seq.SequenceType = SequenceType.Musical;
			//seq.Tracks[0].Centiseconds = 0;
			seq.info.music.Album = audioData.Album;
			string artst = audioData.Artist;
			if (artst.Length < 1)
			{
				artst = audioData.AlbumArtist;
			}
			seq.info.music.Artist = artst;
			seq.info.music.File = audioData.Filename;
			seq.info.music.Title = audioData.Title;
			theSong = audioData.Title;
			songTitle = audioData.Title;
			if (audioData.Artist.Length > 1)
			{
				theSong += BY + audioData.Artist;
				songArtist = audioData.Artist;
			}
			else
			{
				if (audioData.AlbumArtist.Length > 1)
				{
					theSong += BY + audioData.AlbumArtist;
					songArtist = audioData.AlbumArtist;
				}
			}

		}




		private int ImportVampsToSequence()
		{
			int errs = 0;

			SaveSongInfo();
			TimingGrid ftg = GetGrid("20 FPS", true);
			ftg.TimingGridType = TimingGridType.FixedGrid;
			ftg.Centiseconds = centiseconds;
			ftg.spacing = 5;
			vampTrack = GetTrack("Tune-O-Rama", true);
			vampTrack.Centiseconds = centiseconds;
			vampTrack.timingGrid = ftg;

			string lorAuth = utils.DefaultAuthor;
			seq.info.modifiedBy = lorAuth + " / Vamperizer";

			if (doBarsBeats && (errLevel == 0))
			{
				//errLevel = ImportBarBeats();
			}
			if (doNoteOnsets && (errLevel == 0))
			{
				//errLevel = ImportNoteOnsets();
			}
			if (doTranscribe && (errLevel == 0))
			{
				//errLevel = Importranscribe();
			}
			if (doPitchKey && (errLevel == 0))
			{
				//errLevel = ImportPitchKey();
			}
			if (doSegments && (errLevel == 0))
			{
				//errLevel = ImportSegments();
			}



			if (seq.Channels.Count < 1)
			{
				Channel ch = seq.CreateChannel("null");
				seq.Tracks[0].Members.Add(ch);
			}



			return errs;
		}

		private int ImportBarsBeats()
		{
			int errs = 0;
			ChannelGroup beatGroup = GetGroup("Bars & Beats", vampTrack);
			if (xBars != null)
			{
				if (xBars.effects.Count > 0)
				{
					TimingGrid barGrid = GetGrid("Bars");
					Channel barCh = GetChannel("Bars", beatGroup.Members);
					ImportTimingGrid(barGrid, xBars);
					ImportBeatChannel(barCh, xBars);
				}
			}
			if (chkBeatsFull.Checked)
			{
				if (xBeatsFull != null)
				{
					if (xBeatsFull.effects.Count > 0)
					{
						TimingGrid barGrid = GetGrid("Beats-Full");
						Channel beatCh = GetChannel("Beats-Full", beatGroup.Members);
						ImportTimingGrid(barGrid, xBeatsFull);
						ImportBeatChannel(beatCh, xBeatsFull);
					}
				}
			}
			if (chkBeatsHalf.Checked)
			{
				if (xBeatsHalf != null)
				{
					if (xBeatsHalf.effects.Count > 0)
					{
						TimingGrid barGrid = GetGrid("Beats-Half");
						Channel beatCh = GetChannel("Beats-Half", beatGroup.Members);
						ImportTimingGrid(barGrid, xBeatsHalf);
						ImportBeatChannel(beatCh, xBeatsHalf);
					}
				}
			}
			if (chkBeatsThird.Checked)
			{
				if (xBeatsThird != null)
				{
					if (xBeatsThird.effects.Count > 0)
					{
						TimingGrid barGrid = GetGrid("Beats-Third");
						Channel beatCh = GetChannel("Beats-Third", beatGroup.Members);
						ImportTimingGrid(barGrid, xBeatsThird);
						ImportBeatChannel(beatCh, xBeatsThird);
					}
				}
			}
			if (chkBeatsQuarter.Checked)
			{
				if (xBeatsQuarter != null)
				{
					if (xBeatsQuarter.effects.Count > 0)
					{
						TimingGrid barGrid = GetGrid("Beats-Quarter");
						Channel beatCh = GetChannel("Beats-Quarter", beatGroup.Members);
						ImportTimingGrid(barGrid, xBeatsQuarter);
						ImportBeatChannel(beatCh, xBeatsQuarter);
					}
				}
			}
			if (chkNoteOnsets.Checked)
			{
				if (xOnsets != null)
				{
					if (xOnsets.effects.Count > 0)
					{
						TimingGrid noteGrid = GetGrid("Note Onsets");
						//Channel noteCh = GetChannel("Note Onsets", beatGroup.Members);
						ImportTimingGrid(noteGrid, xBeatsQuarter);
						//ImportBeatChannel(noteCh, xBeatsQuarter);
					}
				}
			}
			if (chkTranscribe.Checked)
			{
				if (xTranscription != null)
				{
					if (xTranscription.effects.Count > 0)
					{
						//ChannelGroup transGroup = GetGroup("Polyphonic Transcription");
						//ImportTranscription(transGroup);
					}
				}
			}
			if (chkPitchKey.Checked)
			{
				if (xKey != null)
				{
					if (xKey.effects.Count > 0)
					{
						//ChannelGroup transGroup = GetGroup("Polyphonic Transcription");
						//ImportTranscription(transGroup);
					}
				}
			}
			if (chkSegments.Checked)
			{
				if (xSegments != null)
				{
					if (xSegments.effects.Count > 0)
					{
						//ChannelGroup transGroup = GetGroup("Polyphonic Transcription");
						//ImportTranscription(transGroup);
					}
				}
			}







			return errs;
		}

		private int ImportTimingGrid(TimingGrid beatGrid, xTimings xEffects)
		{
			int errs = 0;

			// If grid already has timings (from a previous run) clear them, start over fresh
			if (beatGrid.timings.Count > 0)
			{
				beatGrid.timings.Clear();
			}
			for (int q = 0; q < xEffects.effects.Count; q++)
			{
				xEffect xef = xEffects.effects[q];
				double xt = xef.starttime / 10;
				int t = (int)Math.Round(xt);
				beatGrid.AddTiming(t);
			}
			return errs;
		}

		private int ImportBeatChannel(Channel beatCh, xTimings xEffects)
		{
			int errs = 0;

			// If channel already has effects (from a previous run) clear them, start over fresh
			if (beatCh.effects.Count > 0)
			{
				beatCh.effects.Clear();
			}
			for (int q = 0; q < xEffects.effects.Count; q++)
			{
				xEffect xef = xBars.effects[q];
				Effect lef = new Effect();
				lef.EffectType = EffectType.Intensity;
				lef.startIntensity = 100;
				lef.endIntensity = 100;
				double xt = xef.starttime / 10;
				int t = (int)Math.Round(xt);
				lef.startCentisecond = t;
				xt = xef.starttime + ((xef.endtime - xef.starttime)) / 2;
				xt /= 10;
				t = (int)Math.Round(xt);
				lef.endCentisecond = t;
				beatCh.AddEffect(lef);
			}


			return errs;
		}

		private int ImportPolyChannels(ChannelGroup polyGroup, xTimings xEffects)
		{
			int errs = 0;



			return errs;
		}

		private int ImportPoly(string polyFile)
		{
			string PolyFile;
			int pcount = 0;

			string lineIn = "";
			int ppos = 0;
			int centisecs = 0;
			string[] parts;
			int ontime = 0;
			int note = 0;
			Channel ch;
			Effect ef;

			//Track trk = new Track("Polyphonic Transcription");
			Track trk = GetTrack(MASTERTRACK);
			//trk.Centiseconds = seq.Centiseconds;
			TimingGrid tg = seq.FindTimingGrid(GRIDONSETS);
			trk.timingGrid = tg;
			//trk.timingGridObjIndex = tg.identity.myIndex;
			ChannelGroup grp = GetGroup(GROUPPOLY, trk);
			CreatePolyChannels(grp, "Poly ", doGroups);
			if (tg == null)
			{
				trk.timingGrid = seq.TimingGrids[0];
				//trk.timingGridSaveID = 0;
			}
			else
			{
				trk.timingGrid = tg;
				for (int tgs = 0; tgs < seq.TimingGrids.Count; tgs++)
				{
					if (seq.TimingGrids[tgs].SaveID == tg.SaveID)
					{
						trk.timingGrid = seq.TimingGrids[tgs];
						tgs = seq.TimingGrids.Count; // break loop
					}
				}
			}

			StreamReader reader = new StreamReader(polyFile);

			while ((lineIn = reader.ReadLine()) != null)
			{
				parts = lineIn.Split(',');
				if (parts.Length == 3)
				{
					pcount++;
					centisecs = ParseCentiseconds(parts[0]);
					ontime = ParseCentiseconds(parts[1]);
					note = Int16.Parse(parts[2]);
					//ch = seq.Channels[firstCobjIdx + note];
					//ch = GetChannel("theName");
					ch = noteChannels[note];
					ef = new Effect();
					ef.EffectType = EffectType.Intensity;
					ef.startCentisecond = centisecs;
					ef.endCentisecond = centisecs + ontime;
					if (useRampsPoly)
					{
						ef.startIntensity = 100;
						ef.endIntensity = 0;
					}
					else
					{
						ef.Intensity = 100;
					}
					//ch.effects.Add(ef);
					ch.AddEffect(ef);
				}

			} // end while loop more lines remaining

			reader.Close();

			//seq.AddTrack(trk);



			return pcount;
		}

		//private int SpectrogramToChannels(string spectroFile)
		private int ImportSpectro(string spectroFile)
		{
			string SpectroFile;
			int pcount = 0;

			string lineIn = "";
			int ppos = 0;
			int centisecs = 0;
			string[] parts;
			int ontime = 0;
			int note = 0;
			Channel ch;
			Effect ef;

			//Track trk = new Track("Spectrogram");
			Track trk = GetTrack(MASTERTRACK);
			//trk.identity.Centiseconds = seq.totalCentiseconds;
			TimingGrid tg = seq.FindTimingGrid(GRIDONSETS);
			trk.timingGrid = tg;
			//trk.timingGridObjIndex = tg.identity.myIndex;
			ChannelGroup grp = GetGroup(GROUPSPECTRO, trk);
			CreatePolyChannels(grp, "Spectro ", doGroups);
			if (tg == null)
			{
				trk.timingGrid = seq.TimingGrids[0];
				//trk.timingGridSaveID = 0;
			}
			else
			{
				trk.timingGrid = tg;
				for (int tgs = 0; tgs < seq.TimingGrids.Count; tgs++)
				{
					if (seq.TimingGrids[tgs].SaveID == tg.SaveID)
					{
						trk.timingGrid = seq.TimingGrids[tgs];
						tgs = seq.TimingGrids.Count; // break loop
					}
				}
			}

			// Pass 1, Get max values
			double[] dVals = new double[1024];
			StreamReader reader = new StreamReader(spectroFile);

			while ((lineIn = reader.ReadLine()) != null)
			{
				parts = lineIn.Split(',');
				if (parts.Length == 1025)
				{
					pcount++;
					//centisecs = ParseCentiseconds(parts[0]);
					//Debug.Write(centisecs);
					//Debug.Write(":");
					for (int n = 0; n < 1024; n++)
					{
						double d = Double.Parse(parts[n]);
						if (d > dVals[n]) dVals[n] = d;
					}
				}
			} // end while loop more lines remaining
			reader.Close();

			// Pass 2, Convert those maxvals to a scale factor
			for (int n = 0; n < 1024; n++)
			{
				dVals[n] = 140 / dVals[n];
			}

			// Pass 3, convert to percents
			int lastcs = utils.UNDEFINED;
			double lastdt = 0;
			int lastix = 0;

			reader = new StreamReader(spectroFile);

			while ((lineIn = reader.ReadLine()) != null)
			{
				parts = lineIn.Split(',');
				if (parts.Length == 1025)
				{
					pcount++;
					centisecs = ParseCentiseconds(parts[0]);
					//Debug.Write(centisecs);
					//Debug.Write(":");
					for (int n = 0; n < 128; n++)
					{
						double dt = 0;
						for (int m = 0; m < 8; m++)
						{
							int i = n * 8 + m + 1;
							double d = Double.Parse(parts[i]);
							d *= dVals[i];
							dt += d;
						}
						dt /= 8;
						int ix = (int)dt;
						if (ix < 20)
						{
							ix = 0;
						}
						else
						{
							if (ix > 120)
							{
								ix = 100;
							}
							else
							{
								ix -= 20;
							}
						}
						if (centisecs == lastcs)
						{
							ix += lastix;
							ix /= 2;
						}



						lastix = ix;
						lastdt = dt;
						lastcs = centisecs;
					}
				}
			} // end while loop more lines remaining
			reader.Close();









			//seq.AddTrack(trk);



			return pcount;
		}

		private int ConstQToChannels(string constQFile)
		{
			int pcount = 0;

			string lineIn = "";
			int ppos = 0;
			int centisecs = 0;
			string[] parts;
			int ontime = 0;
			//int note = 0;
			Channel ch;
			Effect ef;

			//Track trk = new Track("Constant Q Spectrogram");
			Track trk = GetTrack(MASTERTRACK);
			//trk.identity.Centiseconds = seq.totalCentiseconds;
			TimingGrid tg = seq.FindTimingGrid(GRIDONSETS);
			trk.timingGrid = tg;
			//trk.timingGridObjIndex = tg.identity.myIndex;
			ChannelGroup grp = GetGroup(GROUPCONSTQ, trk);
			CreatePolyChannels(grp, "ConstQ ", doGroups);
			if (tg == null)
			{
				trk.timingGrid = seq.TimingGrids[0];
				//trk.timingGridSaveID = 0;
			}
			else
			{
				trk.timingGrid = tg;
				for (int tgs = 0; tgs < seq.TimingGrids.Count; tgs++)
				{
					if (seq.TimingGrids[tgs].SaveID == tg.SaveID)
					{
						trk.timingGrid = seq.TimingGrids[tgs];
						tgs = seq.TimingGrids.Count; // break loop
					}
				}
			}

			// Pass 1, Get max values
			double[] dVals = new double[128];
			StreamReader reader = new StreamReader(constQFile);

			while ((lineIn = reader.ReadLine()) != null)
			{
				parts = lineIn.Split(',');
				if (parts.Length == 129)
				{
					pcount++;
					//centisecs = ParseCentiseconds(parts[0]);
					//Debug.Write(centisecs);
					//Debug.Write(":");
					for (int note = 0; note < 128; note++)
					{
						double d = Double.Parse(parts[note + 1]);
						if (d > dVals[note]) dVals[note] = d;
					}
				}
			} // end while loop more lines remaining
			reader.Close();

			// Pass 2, Convert those maxvals to a scale factor
			for (int n = 0; n < 128; n++)
			{
				dVals[n] = 140 / dVals[n];
			}

			// Pass 3, convert to percents
			int[] lastcs = new int[128];
			double lastdVal = 0;
			int[] lastiVal = new int[128];

			reader = new StreamReader(constQFile);

			while ((lineIn = reader.ReadLine()) != null)
			{
				parts = lineIn.Split(',');
				if (parts.Length == 129)
				{
					pcount++;
					centisecs = ParseCentiseconds(parts[0]);
					//Debug.Write(centisecs);
					//Debug.Write(":");
					for (int note = 0; note < 128; note++)
					{
						double dt = 0;
						double d = Double.Parse(parts[note + 1]);
						d *= dVals[note];
						dt += d;
						int iVal = (int)dt;
						if (iVal < 21)
						{
							iVal = 0;
						}
						else
						{
							if (iVal > 120)
							{
								iVal = 100;
							}
							else
							{
								iVal -= 20;
							}
						}

						if (iVal != lastiVal[note])
						{
							//ch = seq.Channels[firstCobjIdx + note];
							ch = noteChannels[note];
							//Identity id = seq.Members.bySavedIndex[noteChannels[note]];
							//if (id.PartType == MemberType.Channel)
							//{
							//ch = (Channel)id.owner;
							ef = new Effect();
							ef.EffectType = EffectType.Intensity;
							ef.startCentisecond = lastcs[note];
							ef.endCentisecond = centisecs;
							ef.startIntensity = lastiVal[note];
							ef.endIntensity = iVal;
							ch.effects.Add(ef);
							lastcs[note] = centisecs;
							lastiVal[note] = iVal;
							//}
							//else
							//{
							//	string emsg = "Crash! Burn! Explode!";
							//}
						}


					}
				}
			} // end while loop more lines remaining
			reader.Close();









			//seq.AddTrack(trk);



			return pcount;
		}

		private int ParseCentiseconds(string secondsValue)
		{
			int ppos = secondsValue.IndexOf('.');
			// Get number of seconds before the period
			int sec = Int16.Parse(secondsValue.Substring(0, ppos));
			// Get the fraction of a second after the period, only keep most significant 4 digits
			int dotsec = Int16.Parse(secondsValue.Substring(ppos + 1, 4));
			// turn it from an int into an actual fraction
			decimal ds = (dotsec / 100);
			// Round up or down from 4 digits to 2
			dotsec = (int)Math.Round(ds);  // man is this stupid call picky as hell about syntax
																		 // Combine seconds and fraction of a second into Centiseconds
			int centisecs = sec * 100 + dotsec;

			return centisecs;

		}


		private int ImportNoteOnsets(string noteOnsetFile)
		{
			//string noteOnsetFile;
			int onsetCount = 0;
			string lineIn = "";
			int ppos = 0;
			int centisecs = 0;

			//TimingGrid grid = new TimingGrid("Note Onsets");
			TimingGrid grid = GetGrid(GRIDONSETS);
			grid.TimingGridType = TimingGridType.Freeform;
			//grid.type = timingGridType.freeform;
			grid.AddTiming(0); // Needs a timing of zero at the beginning

			StreamReader reader = new StreamReader(noteOnsetFile);

			while ((lineIn = reader.ReadLine()) != null)
			{
				ppos = lineIn.IndexOf('.');
				if (ppos > utils.UNDEFINED)
				{
					centisecs = ParseCentiseconds(lineIn);
					// Add centisecond value to the timing grid
					grid.AddTiming(centisecs);
					onsetCount++;
				} // end line contains a period
			} // end while loop more lines remaining

			reader.Close();

			//seq.TimingGrids.Add(grid);
			//seq.AddTimingGrid(grid);
			//Track trk = seq.FindTrack(applicationName);
			//trk.timingGridObjIndex = seq.TimingGrids.Count - 1;
			//trk.timingGridObjIndex = grid.identity.SavedIndex;
			//trk.totalCentiseconds = seq.totalCentiseconds;

			return onsetCount;
		} // end Note Onsets to Timing Grid

		private Track GetTrack(string trackName, bool createIfNotFound = false)
		{
			// Gets existing track specified by Name if it already exists
			// Creates it if it does not
			Track ret = seq.FindTrack(trackName, createIfNotFound);
			if (ret == null)
			{
				if (createIfNotFound)
				{
					ret = seq.CreateTrack(trackName);
					ret.Centiseconds = centiseconds;
				}
				//seq.AddTrack(ret);
			}
			return ret;
		}

		private TimingGrid GetGrid(string gridName, bool createIfNotFound = true)
		{
			// Gets existing track specified by Name if it already exists
			// Creates it if it does not
			TimingGrid ret = seq.FindTimingGrid(gridName, createIfNotFound);
			//TimingGrid ret = seq.TimingGrids.Find(gridName, MemberType.TimingGrid, true);
			if (ret == null)
			{
				if (createIfNotFound)
				{
					// ERROR! Should not ever get here
					System.Diagnostics.Debugger.Break();
					//ret = seq.CreateTimingGrid(gridName);
					//ret.Centiseconds = centiseconds;
					//seq.AddTimingGrid(ret);
				}
			}
			else
			{
				// Clear any existing timings from a previous run
				if (ret.timings.Count > 0)
				{
					ret.timings = new List<int>();
				}
			}
			return ret;
		}


		private ChannelGroup GetGroup(string groupName, IMember parent)
		{
			// Gets existing group specified by Name if it already exists in the track or group
			// Creates it if it does not
			// Can't use 'Find' functions because we only want to look in this one particular track or group

			// Make dummy item list
			Membership Children = new Membership(seq);
			// Get the parent
			MemberType parentType = parent.MemberType;
			// if parent is a group
			if (parent.MemberType == MemberType.ChannelGroup)
			{
				// Get it's items saved index list
				Children = ((ChannelGroup)parent).Members;
			}
			else // not a group
			{
				// if parent is a track
				if (parent.MemberType == MemberType.Track)
				{
					Children = ((Track)parent).Members;
				}
				else // not a track either
				{
					string emsg = "WTF? Parent is not group or track, but should be!";
				} // end if track, or not
			} // end if group, or not

			// Create blank/null return object
			ChannelGroup ret = null;
			int gidx = 0; // loop counter
										// loop while we still have no group, and we haven't reached to end of the list
			while ((ret == null) && (gidx < Children.Count))
			{
				// Get each item's ID
				//int SI = Children.Items[gidx].SavedIndex;
				IMember part = Children.Items[gidx];
				if (part.MemberType == MemberType.ChannelGroup)
				{
					ChannelGroup group = (ChannelGroup)part;
					if (part.Name == groupName)
					{
						ret = group;
						gidx = Children.Count;
					}
				}
				gidx++;
			}

			if (ret == null)
			{
				//int si = seq.Members.HighestSavedIndex + 1;
				ret = seq.CreateChannelGroup(groupName);
				ret.Centiseconds = centiseconds;
				//seq.AddChannelGroup(ret);
				//ID = seq.Members.bySavedIndex[parentSI];
				if (parent.MemberType == MemberType.Track)
				{
					((Track)parent).Members.Add(ret);
				}
				if (parent.MemberType == MemberType.ChannelGroup)
				{
					((ChannelGroup)parent).Members.Add(ret);
				}
			}

			return ret;
		}

		private Channel GetChannel(string channelName, Membership parentSubItems)
		{
			// Gets existing channel specified by Name if it already exists in the group
			// Creates it if it does not
			Channel ret = null;
			IMember part = null;
			int gidx = 0;
			while ((ret == null) && (gidx < parentSubItems.Count))
			{
				part = parentSubItems.Items[gidx];
				if (part.MemberType == MemberType.Channel)
				{
					if (part.Name == channelName)
					{
						ret = (Channel)part;
						// Clear any existing effects from a previous run
						if (ret.effects.Count > 0)
						{
							ret.effects = new List<Effect>();
						}
					}
				}
				gidx++;
			}

			if (ret == null)
			{
				//int si = seq.Members.HighestSavedIndex + 1;
				ret = seq.CreateChannel(channelName);
				ret.Centiseconds = centiseconds;
				parentSubItems.Add(ret);
			}

			return ret;
		}

		private void CreatePolyChannels(IMember parent, string prefix, bool useGroups)
		{
			string dmsg = "";
			//Channel chan;
			int octave = 0;
			int lastOctave = 0;
			Membership parentSubs = new Membership(seq);
			ChannelGroup grp = new ChannelGroup("null");
			if (useGroups)
			{
				grp = GetGroup(prefix + octaveNamesA[octave], parent);
				parentSubs = grp.Members;
				//grp.identity.Centiseconds = seq.totalCentiseconds;
			}
			else
			{
				if (parent.MemberType == MemberType.Track)
				{
					parentSubs = ((Track)parent).Members;
				}
				else
				{
					// useGroups is false, so the parent should be a track, but it's not!
					System.Diagnostics.Debugger.Break();
				}
			}
			Array.Resize(ref noteChannels, noteNames.Length);
			for (int n = 0; n < noteNames.Length; n++)
			{
				if (useGroups)
				{
					octave = n / 12;
					if (octave != lastOctave)
					{
						// add group from last octave
						//AddChildToParent(grp, parent);
						// then create new octave group
						grp = GetGroup(prefix + octaveNamesA[octave], parent);
						//grp.identity.Centiseconds = seq.totalCentiseconds;
						lastOctave = octave;
						parentSubs = grp.Members;
						dmsg = "Adding Group '" + grp.Name + "' SI:" + grp.SavedIndex;
						dmsg += " Octave #" + octave.ToString();
						dmsg += " to Parent '" + parent.Name + "' SI:" + parent.SavedIndex;
						Debug.WriteLine(dmsg);
					}
				}
				Channel chan = GetChannel(prefix + noteNames[n], parentSubs);
				chan.color = NoteColor(n);
				//chan.identity.Centiseconds = seq.totalCentiseconds;
				noteChannels[n] = chan;
				//grp.Add(chan);
				dmsg = "Adding Channel '" + chan.Name + "' SI:" + chan.SavedIndex;
				dmsg += " Note #" + n.ToString();
				dmsg += " to Parent '" + parentSubs.owner.Name + "' SI:" + parentSubs.owner.SavedIndex;
				//Debug.WriteLine(dmsg);
				Debug.WriteLine(dmsg);


				if (n == 0)
				{
					firstCobjIdx = seq.Channels.Count - 1;
					firstCsavedIndex = chan.SavedIndex;
				}
			}
			if (useGroups)
			{
				//AddChildToParent(grp, parent);
			}
			seq.Members.ReIndex();



		}

		private void AddChildToParent(IMember child, IMember parent)
		{
			// Tests for, and works with either a track or a channel group as the parent
			if (parent.MemberType == MemberType.Track)
			{
				Track trk = (Track)parent;
				trk.Members.Add(child);
			}
			if (parent.MemberType == MemberType.ChannelGroup)
			{
				ChannelGroup grp = (ChannelGroup)parent;
				grp.Members.Add(child);
			}


		}



		private int RGBtoLOR(int RGBclr)
		{
			int b = RGBclr & 0xFF;
			int g = RGBclr & 0xFF00;
			g /= 0x100;
			int r = RGBclr & 0xFF0000;
			r /= 0x10000;

			int n = b * 0x10000;
			n += g * 0x100;
			n += r;

			return n;
		}























		public int SaveTimings(string timingsName)
		{
			int ret = 0;


			return ret;
		}

		public int SaveTranscriptionChannels()
		{
			int ret = 0;


			return ret;

		}

		public int SaveSpectrogramChannels()
		{
			int ret = 0;


			return ret;

		}

		public int SaveChromagramChannels()
		{
			int ret = 0;


			return ret;

		}

		public int SaveBeatChannels()
		{
			int ret = 0;


			return ret;

		}

		public int SaveKeyChannels()
		{
			int ret = 0;


			return ret;

		}

		public int SaveTempoChannels()
		{
			int ret = 0;


			return ret;

		}

		public int SaveSongPartsChannels()
		{
			int ret = 0;


			return ret;

		}


	}


}
