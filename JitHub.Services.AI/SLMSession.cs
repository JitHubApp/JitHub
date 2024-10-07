using Microsoft.ML.OnnxRuntimeGenAI;

namespace JitHub.Services.AI;

public delegate void WriteNextToken(string token);

public class SLMSession : IDisposable
{
    private const string DEFAULT_SYSTEM_PROMPT = "You are a helpful AI assistant. The user will be asking you questions or finish a task. You will try to help the user by answering those questions or completing the tasks like writing or generating text. If the answer is not of high confident, say you don't know.";
    private Model _model;
    private Tokenizer _tokenizer;

    public WriteNextToken WriteNextToken { get; set; } = Console.Write;
    public string SystemPrompt { get; set; } = DEFAULT_SYSTEM_PROMPT;

    public SLMSession(Model model)
    {
        _model = model;
        _tokenizer = new Tokenizer(model);
    }

    public void Complete(string prompt)
    {
        var templatedPrompt = $"<|system|>{SystemPrompt}<|end|><|user|>{prompt}<|end|><|assistant|>";
        var sequences = _tokenizer.Encode(templatedPrompt);
        using GeneratorParams generatorParams = new GeneratorParams(_model);
        generatorParams.SetSearchOption("max_length", 800);
        generatorParams.SetInputSequences(sequences);
        generatorParams.TryGraphCaptureWithMaxBatchSize(1);
        using var tokenizerStream = _tokenizer.CreateStream();
        using var generator = new Generator(_model, generatorParams);
        while (!generator.IsDone())
        {
            generator.ComputeLogits();
            generator.GenerateNextToken();
            WriteNextToken(tokenizerStream.Decode(generator.GetSequence(0)[^1]));
        }
    }

    public void Dispose()
    {
        _tokenizer.Dispose();
        _model.Dispose();
    }
}
