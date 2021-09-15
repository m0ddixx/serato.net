using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serato.Net.Structs;

namespace Serato.Net.Api.Shared.Models
{
    public class TrackInfoEntity
    {
        public Guid Id { get; set; }
        public TrackInfo TrackInfo { get; set; }
    }
}
