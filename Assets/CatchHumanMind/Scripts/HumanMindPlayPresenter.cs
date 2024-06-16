using UnityEngine;

public class HumanMindPlayPresenter : MonoBehaviour
{
    [SerializeField] private HumanMindManager manager;
    [SerializeField] private HumanMindPlayView view;
    [SerializeField] private StabilityAIModel stabilityAIModel;
    [SerializeField] private HumanMindPlayModel model;
    [SerializeField] private InputFieldScaler inputFieldScaler;
    private OpenAIImageModel openAIImageModel;
    
    private void Start()
    {
        openAIImageModel = gameObject.AddComponent<OpenAIImageModel>();
        Subscribe();
        view.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (manager != null)
        {
            manager.OnPlayingProcess += model.UpdatePlayState;
        }

        if (view != null)
        {
            model.OnUpdateGameInfo += view.SetPlayInfo;
            model.OnUpdateTimer += view.UpdateTimer;
            model.OnMyTrunBeging += view.SetOwnerTurnView;
            model.OnUpdateImage += view.SetImage;
            model.OnImageCreateError += view.SetErrorText;

            view.UpdatePlayState += model.RequestTurnChange;
        }

        if (view != null && manager != null)
        {
            manager.OnRecivePromptResponse += view.SetDataResponse;
        }
        
        if (inputFieldScaler != null)
        {
            model.SetInputFieldOpen += inputFieldScaler.SetInputFieldOpen;
        }

        if (view != null && stabilityAIModel != null)
        {
            view.OnClickImageRequest += stabilityAIModel.RequestImageMake;
            view.OnClickImageRequest += model.RequestImageCreate;
            view.OnClickSubmit += model.CheckImageAnswer;
            stabilityAIModel.OnReciveBase64Incoding += model.UpdateImageProperty;
        }

        model.OnSendImageCheck += CheckImageWordByOpenAI;
        model.OnPlayViewActive += OnPlayViewActive;
    }

    private void Unsubscribe()
    {
        if (manager != null)
        {
            manager.OnPlayingProcess -= model.UpdatePlayState;
        }

        if (view != null)
        {
            model.OnUpdateGameInfo -= view.SetPlayInfo;
            model.OnUpdateTimer -= view.UpdateTimer;
            model.OnMyTrunBeging -= view.SetOwnerTurnView;
            model.OnUpdateImage -= view.SetImage;
            model.OnImageCreateError -= view.SetErrorText;

            view.UpdatePlayState -= model.RequestTurnChange;
        }

        if (view != null && manager != null)
        {
            manager.OnRecivePromptResponse -= view.SetDataResponse;
        }
        
        if (inputFieldScaler != null)
        {
            model.SetInputFieldOpen -= inputFieldScaler.SetInputFieldOpen;
        }

        if (view != null && stabilityAIModel != null)
        {
            view.OnClickImageRequest -= stabilityAIModel.RequestImageMake;
            view.OnClickImageRequest -= model.RequestImageCreate;
            view.OnClickSubmit -= model.CheckImageAnswer;
            stabilityAIModel.OnReciveBase64Incoding -= model.UpdateImageProperty;
        }

        model.OnSendImageCheck -= CheckImageWordByOpenAI;
        model.OnPlayViewActive -= OnPlayViewActive;
    }

    private void CheckImageWordByOpenAI(string image64)
    {
        openAIImageModel.RequestImageToOpenAI(HumanMindText.OPENAIPROMPT, image64, model.CheckAnswer);
    }

    private void OnPlayViewActive(bool isState)
    {
        view.gameObject.SetActive(isState);
    }
}
