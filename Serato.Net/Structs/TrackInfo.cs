using System;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serato.Net.Enum;
using Serato.Net.Util;

namespace Serato.Net.Structs
{
    public class TrackInfo
    {
        [FieldProperties(1)] public int Row { get; private set; }

        [FieldProperties(2)] public string? FullPath { get; private set; }

        [FieldProperties(3)] public string? Location { get; private set; }

        [FieldProperties(4)] public string? Filename { get; private set; }

        [FieldProperties(6)] public string? Title { get; private set; }

        [FieldProperties(7)] public string? Artist { get; private set; }

        [FieldProperties(8)] public string? Album { get; private set; }

        [FieldProperties(9)] public string? Genre { get; private set; }

        [FieldProperties(10)] public TimeSpan Length { get; private set; }

        [FieldProperties(11)] public string? FileSize { get; private set; }

        [FieldProperties(13)] public string? Bitrate { get; private set; }

        [FieldProperties(14)] public string? Frequency { get; private set; }

        [FieldProperties(15)] public int BeatsPerMinute { get; private set; }

        [FieldProperties(16)] public string? Unknown16 { get; private set; }

        [FieldProperties(17)] public string? Comments { get; private set; }

        [FieldProperties(18)] public string? Lange { get; private set; }

        [FieldProperties(19)] public string? Grouping { get; private set; }

        [FieldProperties(20)] public string? Remixer { get; private set; }

        [FieldProperties(21)] public string? Label { get; private set; }

        [FieldProperties(22)] public string? Composer { get; private set; }

        [FieldProperties(23)] public string? Year { get; private set; }

        [FieldProperties(28)] public DateTimeOffset StartTime { get; private set; }

        [FieldProperties(29)] public DateTimeOffset EndTime { get; private set; }

        [FieldProperties(31)] public int Deck { get; private set; }

        [FieldProperties(33)] public string? Unknown33 { get; private set; }

        [FieldProperties(39)] public string? Unknown39 { get; private set; }

        [FieldProperties(45)] public TimeSpan Playtime { get; private set; }

        [FieldProperties(48)] public int SessionId { get; private set; }

        [FieldProperties(50)] public int Played { get; private set; }

        [FieldProperties(51)] public string? Key { get; private set; }

        [FieldProperties(52)] public int Added { get; private set; }

        [FieldProperties(53)] public DateTimeOffset UpdatedAt { get; private set; }

        public TrackStatus TrackStatus => GetStatus();

        public override string ToString()
        {
            return $"{Artist} - {Title}";
        }

        private TrackStatus GetStatus()
        {
            if (Played != default)
            {
                if (Playtime != default)
                {
                    return TrackStatus.Played;
                }
                else
                {
                    return TrackStatus.Playing;
                }
            }
            else
            {
                if (Playtime != default)
                {
                    return TrackStatus.Skipped;
                }
                else
                {
                    return TrackStatus.New;
                }
            }
        }
    }
}