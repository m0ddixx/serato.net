using System;
using System.Collections.Generic;
using Serato.Net.Structs;

namespace Serato.Net.Util
{
    public class FinishedParsingTracksEventArgs : EventArgs
    {
        public IAsyncEnumerable<TrackInfo> TrackInfos { get; private set;  }
        public string Name { get; private set; }
        public FinishedParsingTracksEventArgs(IAsyncEnumerable<TrackInfo> trackInfos, string name)
        {
            TrackInfos = trackInfos;
            Name = name;
        }
    }
}