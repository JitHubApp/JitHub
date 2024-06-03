using JitHub.Services.Interfaces;
using Microsoft.ML.OnnxRuntimeGenAI;

namespace JitHub.Services.AI;

public class LocalSLMService
{
    private Dictionary<SLMName, SLM> _slms;
    private ISettingsService _settingsService;
    public LocalSLMService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _slms = new Dictionary<SLMName, SLM>
        {
            {
                SLMName.PHI3_MINI_4K,
                new()
                {
                    Name = SLMName.PHI3_MINI_4K,
                    DownloadUrl = "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/tree/main",
                    Runtime = SLMRuntime.DIREECTML,
                }
            },
            {
                SLMName.PHI3_MINI_128K,
                new()
                {
                    Name = SLMName.PHI3_MINI_128K,
                    DownloadUrl = "https://huggingface.co/microsoft/Phi-3-mini-128k-instruct-onnx/tree/main",
                    Runtime = SLMRuntime.DIREECTML,
                }
            }
        };
    }
    public bool IsAvailable(SLMName sml)
    {
        try
        {
            //we try to load it. It will throw if it's not there.
            using Model _model = new Model(GetModelPath(sml));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public SLM GetModel(SLMName slm)
    {
        if (_slms.ContainsKey(slm))
        {
            return _slms[slm];
        }
        else
        {
            return null;
        }
    }

    public SLMSession LoadModel(SLMName slm)
    {
        var path = GetModelPath(slm);
        return new SLMSession(new Model(path));
    }

    public void SetModelPath(SLMName slm, string path)
    {
        var key = $"SLM-Path-{GetSLMName(slm)}";
        _settingsService.Save(key, path);
    }

    public string GetSLMName(SLMName slm)
    {
        return slm switch
        {
            SLMName.PHI3_MINI_4K => "PHI3_MINI_4K",
            SLMName.PHI3_MINI_128K => "PHI3_MINI_128K",
            _ => ""
        };
    }

    public string GetModelPath(SLMName slm)
    {
        var key = $"SLM-Path-{GetSLMName(slm)}";
        return _settingsService.Get<string>(key);
    }
}
