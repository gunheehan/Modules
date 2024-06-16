using System;
using System.Collections.Generic;

public class OpenAIInfo
{
    public const string OpenAiAuthKey = "";
    public const string OpenAiOrganization = "org-0zSkIm4YwTCOypLxWYrUqofs";

    public const string OpenAIRootUrl = "https://api.openai.com/v1";
    public const string OpenAICompletions = "/chat/completions";

    public const string USER_ROLE = "user";

    public const string PreviewModel = "gpt-4-0125-preview";
    public const string VisionModel = "gpt-4o";
}

[Serializable]
public struct OpenAIMessage
{
    public string role;
    public string content;
}

[Serializable]
public struct OpenAIResponse
{
    public string id;
    public string created;
    public string model;
    public OpenAIResult[] choices;
}

[Serializable]
public struct OpenAIResult
{
    public int index;
    public OpenAIMessage message;
}

[Serializable]
public class OpenAIVisionResponse
{
    public string id;
    public long created;
    public string model;
    public List<OpenAIResult> choices;
}