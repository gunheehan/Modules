using System;

[Serializable]
public class StabilityResponse
{
    public StabilityImageResponse[] artifacts;
}

[Serializable]
public class StabilityImageResponse
{
    public string base64;
}

[Serializable]
public class StabilityRequestInfo
{
    public int step = 40;
    public int width = 512;
    public int height = 512;
    public int seed = 0;
    public int cfg_scale = 5;
    public int samples = 1;
    public StabilityTextPrompt[] text_prompts;
}

[Serializable]
public class StabilityTextPrompt
{
    public string text;
    public int weight = 1;
}