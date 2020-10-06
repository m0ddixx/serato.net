using System;
using System.Collections.Generic;
using Serato.Net.Structs;

namespace Serato.Net.Util
{
    public class FinishedParsingTracksEventArgs : EventArgs
    {
        public IEnumerable<TrackInfo> TrackInfos { get; private set;  }

        public FinishedParsingTracksEventArgs(IEnumerable<TrackInfo> trackInfos)
        {
            TrackInfos = trackInfos;
        }
    }
}