﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vamperizer
{
	class xEffect
	{
		public string xlabel = "";
		public object Tag = null;
		public int Midi = -1;
		private int _starttime = 0;
		private int _endtime = 999999999;
		private static int lastEnd = 0;

		private static string TABLE_Effect = "Effect";
		private static string FIELD_label = "label";
		private static string FIELD_start = "starttime";
		private static string FIELD_end = "endtime";
		private static string TABLE_timing = "timing";
		private static string TABLE_LORtime5 = "<LORTiming version=\"1\">";
		private static string TABLE_Grids = "<TimingGrids>";
		private static string TABLE_FreeGrid = "";
		private static string FIELD_centisecond = "centisecond";
		private static string LEVEL2 = "    ";
		private static string RECORD_start = "<";
		private static string RECORD_end = "/>";
		private static string RECORD_final = ">";
		private static string SPC = " ";
		private static string CRLF = "\r\n";
		private static string VALUE_start = "=\"";
		private static string VALUE_end = "\"";

		public xEffect(string theLabel, int startTime, int endTime)
		{
			if (startTime >= endTime)
			{
				// Raise Exception
				System.Diagnostics.Debugger.Break();
			}
			else
			{
				xlabel = theLabel;
				_starttime = startTime;
				_endtime = endTime;
				lastEnd = endTime;
			}
		}

		public xEffect(int startTime, int endTime)
		{
			if (startTime >= endTime)
			{
				// Raise Exception
				System.Diagnostics.Debugger.Break();
			}
			else
			{
				_starttime = startTime;
				_endtime = endTime;
				lastEnd = endTime;
			}
		}

		public xEffect(int endTime)
		{
			if (lastEnd >= endTime)
			{
				// Raise Exception
				System.Diagnostics.Debugger.Break();
			}
			else
			{
				_starttime = lastEnd;
				_endtime = endTime;
				lastEnd = endTime;
			}
		}

		public xEffect(string theLabel, int endTime)
		{
			if (lastEnd >= endTime)
			{
				// Raise Exception
				System.Diagnostics.Debugger.Break();
			}
			else
			{
				xlabel = theLabel;
				_starttime = lastEnd;
				_endtime = endTime;
				lastEnd = endTime;
			}
		}

		public int starttime
		{
			get
			{
				return _starttime;
			}
			set
			{
				if (value >= _endtime)
				{
					System.Diagnostics.Debugger.Break();
					// Raise Exception
				}
				else
				{
					_starttime = value;
				}
			}
		}

		public int endtime
		{
			get
			{
				return _endtime;
			}
			set
			{
				if (_starttime >= value)
				{
					System.Diagnostics.Debugger.Break();
					// Raise Exception
				}
				else
				{
					_endtime = value;
				}
			}
		}

		public string LineOutX()
		{
			StringBuilder ret = new StringBuilder();
			//    <Effect 
			ret.Append(LEVEL2);
			ret.Append(RECORD_start);
			ret.Append(TABLE_Effect);
			ret.Append(SPC);
			//  label="foo" 
			ret.Append(FIELD_label);
			ret.Append(VALUE_start);
			ret.Append(xlabel);
			ret.Append(VALUE_end);
			ret.Append(SPC);
			//  starttime="50" 
			ret.Append(FIELD_start);
			ret.Append(VALUE_start);
			ret.Append(_starttime.ToString());
			ret.Append(VALUE_end);
			ret.Append(SPC);
			//  endtime="350" />
			ret.Append(FIELD_end);
			ret.Append(VALUE_start);
			ret.Append(_endtime.ToString());
			ret.Append(VALUE_end);
			ret.Append(SPC);

			ret.Append(RECORD_end);
			ret.Append(CRLF);

			return ret.ToString();
		}

		public string LineOut4()
		{
			StringBuilder ret = new StringBuilder();
			int cs = _starttime / 10;
			//    <timing 
			ret.Append(LEVEL2);
			ret.Append(RECORD_start);
			ret.Append(TABLE_timing);
			ret.Append(SPC);
			//  label="foo" 
			ret.Append(FIELD_centisecond);
			ret.Append(VALUE_start);
			ret.Append(cs.ToString());
			ret.Append(VALUE_end);
			ret.Append(SPC);

			ret.Append(RECORD_end);
			ret.Append(CRLF);

			return ret.ToString();
		}

		public string LineOut5()
		{
			StringBuilder ret = new StringBuilder();
			int cs = _starttime / 10;
			//    <timing 
			ret.Append(LEVEL2);
			ret.Append(RECORD_start);
			ret.Append(TABLE_timing);
			ret.Append(SPC);
			//  label="foo" 
			ret.Append(FIELD_centisecond);
			ret.Append(VALUE_start);
			ret.Append(cs.ToString());
			ret.Append(VALUE_end);
			ret.Append(SPC);

			ret.Append(RECORD_end);
			ret.Append(CRLF);

			return ret.ToString();
		}

	}

	class xTimings
	{
		public string timingName = "";
		public string sourceVersion = "2019.32";
		public List<xEffect> effects = new List<xEffect>();
		//public int effectCount = 0;
		public readonly static string SourceVersion = "2019.32";
		public readonly static string XMLinfo = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
		private int maxMillis = 0;

		private static string TABLE_timing = "timing";
		private static string FIELD_name = "name";
		private static string FIELD_source = "SourceVersion";
		private static string TABLE_layers = "EffectLayer";
		private static string TABLE_LORtime5 = "LORTiming";
		private static string FIELD_version = "version";
		private static string TABLE_Grids = "TimingGrids";
		private static string TABLE_grid = "timingGrid";
		private static string TABLE_FreeGrid = "TimingGridFree";
		private static string TABLE_BeatChans = "BeatChannels";
		private static string FIELD_centisecond = "centisecond";
		private static string FIELD_saveID = "saveID";
		private static string FIELD_type = "type";
		private static string TYPE_freeform = "freeform";
		private static string PLURAL = "s";
		private static string SAVEID_X = "X";
		private static string LEVEL0 = "";
		private static string LEVEL1 = "  ";
		private static string LEVEL2 = "    ";
		private static string RECORD_start = "<";
		private static string RECORD_end = ">";
		private static string RECORD_done = "/>";
		private static string SPC = " ";
		private static string CRLF = "\r\n";
		private static string VALUE_start = "=\"";
		private static string VALUE_end = "\"";
		private static string RECORDS_done = "</";

		public xTimings(string theName)
		{
			timingName = theName;
		}

		public void Add(xEffect newEffect)
		{
			if (effects.Count > 0)
			{
				if (newEffect.starttime < effects[effects.Count - 1].endtime)
				{
					// Is this truly an error?  How will xLights respond?
					//System.Diagnostics.Debugger.Break();
					// Raise Exception
				}
				else
				{
					effects.Add(newEffect);
					maxMillis = newEffect.endtime;
					//					effectCount++;
					//Array.Resize(ref effects, effectCount);
					//effects[effectCount - 1] = newEffect;
				}
			}
			else
			{
				effects.Add(newEffect);
				maxMillis = newEffect.endtime;
				//effectCount = 1;
				//Array.Resize(ref effects, 1);
				//effects[0] = newEffect;
			}
		}

		public xEffect Add(string theLabel, int startTime, int endTime)
		{
			xEffect newEff = new xEffect(theLabel, startTime, endTime);
			Add(newEff);
			return newEff;
		}

		public xEffect Add(int startTime, int endTime)
		{
			xEffect newEff = new xEffect(startTime, endTime);
			Add(newEff);
			return newEff;
		}

		public xEffect Add(int endTime)
		{
			xEffect newEff = new xEffect(endTime);
			Add(newEff);
			return newEff;
		}

		public xEffect Add(string theLabel, int endTime)
		{
			xEffect newEff = new xEffect(theLabel, endTime);
			Add(newEff);
			return newEff;
		}

		public void Clear()
		{
			timingName = "";
			sourceVersion = "2019.32";
			//effects = null;
			effects = new List<xEffect>();
			//effectCount = 0;
		}

		public string LineOutX()
		{
			StringBuilder ret = new StringBuilder();
			//  <timing
			ret.Append(LEVEL0);
			ret.Append(RECORD_start);
			ret.Append(TABLE_timing);
			ret.Append(SPC);
			//  name="the Name"
			ret.Append(FIELD_name);
			ret.Append(VALUE_start);
			ret.Append(timingName);
			ret.Append(VALUE_end);
			ret.Append(SPC);
			//  SourceVersion="2019.21">
			ret.Append(FIELD_source);
			ret.Append(VALUE_start);
			ret.Append(sourceVersion);
			ret.Append(VALUE_end);
			ret.Append(RECORD_end);
			ret.Append(CRLF);
			//    <EffectLayer>
			ret.Append(LEVEL1);
			ret.Append(RECORD_start);
			ret.Append(TABLE_layers);
			ret.Append(RECORD_end);
			ret.Append(CRLF);

			for (int i = 0; i < effects.Count; i++)
			{
				ret.Append(effects[i].LineOutX());
			}

			//     </EffectLayer>
			ret.Append(LEVEL1);
			ret.Append(RECORDS_done);
			ret.Append(TABLE_layers);
			ret.Append(RECORD_end);
			ret.Append(CRLF);
			//  </timing>
			ret.Append(LEVEL0);
			ret.Append(RECORDS_done);
			ret.Append(TABLE_timing);
			ret.Append(RECORD_end);

			return ret.ToString();
		}

		public string LineOut4()
		{
			const int ver = 1;
			StringBuilder ret = new StringBuilder();
			//  	<timingGrids>
			//ret.Append(LEVEL1);
			//ret.Append(RECORD_start);
			//ret.Append(TABLE_grid);
			//ret.Append(PLURAL);
			//ret.Append(RECORD_end);
			//ret.Append(CRLF);
			//
			ret.Append(LEVEL2);
			ret.Append(RECORD_start);
			ret.Append(TABLE_grid);
			ret.Append(SPC);

			ret.Append(FIELD_saveID);
			ret.Append(VALUE_start);
			ret.Append(SAVEID_X);
			ret.Append(VALUE_end);
			ret.Append(SPC);

			ret.Append(FIELD_name);
			ret.Append(VALUE_start);
			ret.Append(timingName);
			ret.Append(VALUE_end);
			ret.Append(SPC);

			ret.Append(FIELD_type);
			ret.Append(VALUE_start);
			ret.Append(TYPE_freeform);
			ret.Append(VALUE_end);
			ret.Append(RECORD_end);
			ret.Append(CRLF);

			for (int i = 0; i < effects.Count; i++)
			{
				ret.Append(effects[i].LineOut4());
			}

			//      </TimingGridFree>
			ret.Append(LEVEL2);
			ret.Append(RECORDS_done);
			ret.Append(TABLE_grid);
			ret.Append(RECORD_end);

			return ret.ToString();
		}

		public string LineOut5()
		{
			const int ver = 1;
			StringBuilder ret = new StringBuilder();
			//  <?xml version="1.0" encoding="utf-8"?>
			ret.Append(LEVEL0);
			ret.Append(XMLinfo);
			ret.Append(CRLF);
			//  <LORTiming version="1">
			ret.Append(LEVEL0);
			ret.Append(RECORD_start);
			ret.Append(TABLE_LORtime5);
			ret.Append(SPC);
			ret.Append(FIELD_version);
			ret.Append(VALUE_start);
			ret.Append(ver.ToString());
			ret.Append(VALUE_end);
			ret.Append(RECORD_end);
			ret.Append(CRLF);
			//    <TimingGrids>
			ret.Append(LEVEL1);
			ret.Append(RECORD_start);
			ret.Append(TABLE_Grids);
			ret.Append(RECORD_end);
			ret.Append(CRLF);
			//    <TimingGridFree name="Full Beats - Whole Notes">
			ret.Append(LEVEL2);
			ret.Append(RECORD_start);
			ret.Append(TABLE_FreeGrid);
			ret.Append(SPC);
			ret.Append(FIELD_name);
			ret.Append(VALUE_start);
			ret.Append(timingName);
			ret.Append(VALUE_end);
			ret.Append(RECORD_end);
			ret.Append(CRLF);

			for (int i = 0; i < effects.Count; i++)
			{
				ret.Append(effects[i].LineOut5());
			}

			//      </TimingGridFree>
			ret.Append(LEVEL2);
			ret.Append(RECORDS_done);
			ret.Append(TABLE_FreeGrid);
			ret.Append(RECORD_end);
			ret.Append(CRLF);
			//    </TimingGrids>
			ret.Append(LEVEL1);
			ret.Append(RECORDS_done);
			ret.Append(TABLE_Grids);
			ret.Append(RECORD_end);
			ret.Append(CRLF);
			//    <BeatChannels />
			ret.Append(LEVEL1);
			ret.Append(RECORD_start);
			ret.Append(TABLE_BeatChans);
			ret.Append(SPC);
			ret.Append(RECORD_done);
			ret.Append(CRLF);
			//  </LORTiming>
			ret.Append(LEVEL0);
			ret.Append(RECORDS_done);
			ret.Append(TABLE_LORtime5);
			ret.Append(RECORD_end);

			return ret.ToString();
		}


		public int Milliseconds
	{
		get
			{
			return maxMillis;
		}
	}
}


}
