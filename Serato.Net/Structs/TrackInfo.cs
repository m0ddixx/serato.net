using System;
using Serato.Net.Util;

namespace Serato.Net.Structs
{
    public struct TrackInfo
    {
        [FieldProperties(1)]
        public int Row;
        
        [FieldProperties(2)]
        public string FullPath;

        [FieldProperties(3)]
        public string Location;
        
        [FieldProperties(4)]
        public string Filename;
        
        [FieldProperties(6)]
        public string Title;
        
        [FieldProperties(7)]
        public string Artist;
        
        [FieldProperties(8)]
        public string Album;
        
        [FieldProperties(9)]
        public string Genre;
        
        [FieldProperties(10)]
        public string Length;
        
        [FieldProperties(11)]
        public string FileSize;
        
        [FieldProperties(13)]
        public string Bitrate;
        
        [FieldProperties(14)]
        public string Frequency;
        
        [FieldProperties(15)]
        public int BeatsPerMinute;
        
        [FieldProperties(16)]
        public string Unknown16;
        
        [FieldProperties(17)]
        public string Comments;
        
        [FieldProperties(18)]
        public string Lang;
        
        [FieldProperties(19)]
        public string Grouping;
        
        [FieldProperties(20)]
        public string Remixer;
        
        [FieldProperties(21)]
        public string Label;
        
        [FieldProperties(22)]
        public string Composer;
        
        [FieldProperties(23)]
        public string Year;
        
        [FieldProperties(28)]
        public DateTime StartTime;
        
        [FieldProperties(29)]
        public DateTime EndTime;
        
        [FieldProperties(31)]
        public int Deck;
        
        [FieldProperties(33)]
        public string Unknown33;
        
        [FieldProperties(39)]
        public string Unknown39;
        
        [FieldProperties(45)]
        public int Playtime;
        
        [FieldProperties(48)]
        public int SessionId;
        
        [FieldProperties(50)]
        public int Played;
        
        [FieldProperties(51)]
        public string Key;
        
        [FieldProperties(52)]
        public int Added;
        
        [FieldProperties(53)]
        public DateTime UpdatedAt;

        public override string ToString()
        {
            return $"{Artist} - {Title}";
        }
    }
}