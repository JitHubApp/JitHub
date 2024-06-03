using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitHub.Services.AI;

public record SLM
{
    public SLMName Name { get; set; }
    public SLMRuntime Runtime { get; set; }
    public string DownloadUrl { get; set; }
}
