using UnityEngine;

public class HumanMindReadyPresenter : MonoBehaviour
{
    [SerializeField] private HumanMindManager manager;
    [SerializeField] private HumanMindReadyView view;
    [SerializeField] private HumanMindReadyModel model;
    [SerializeField] private InputFieldScaler inputFieldScaler;

    private void Start()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (manager != null)
        {
            manager.OnReadyProcess += model.UpdateReadyState;
            manager.OnClickGuideSkip += () => OnEndReadyFlow(true);
        }

        if (view != null)
        {
            model.OnUpdateReadyInfo += view.UpdateStateInfo;
            model.OnUpdateTimer += view.UpdateTimer;
            model.SetReadyView += view.SetStartButton;
            model.SetCategoryList += view.SetCategoryList;
            model.SetRandomKeyword += view.SetRandomKeyword;

            view.SendPlayerInput += model.SendPlayerText;
            view.OnGameStart += model.OnClickStartGame;
        }

        if (inputFieldScaler != null)
        {
            model.SetInputFieldOpen += inputFieldScaler.SetInputFieldOpen;
        }

        model.OnViewStartFlow += OnEndReadyFlow;
    }

    private void Unsubscribe()
    {
        if (manager != null)
        {
            manager.OnReadyProcess -= model.UpdateReadyState;
            manager.OnClickGuideSkip -= () => OnEndReadyFlow(true);
        }
        
        if (view != null)
        {
            model.OnUpdateReadyInfo -= view.UpdateStateInfo;
            model.OnUpdateTimer -= view.UpdateTimer;
            model.SetReadyView -= view.SetStartButton;
            model.SetCategoryList -= view.SetCategoryList;
            model.SetRandomKeyword -= view.SetRandomKeyword;

            view.SendPlayerInput -= model.SendPlayerText;
            view.OnGameStart -= model.OnClickStartGame;
        }
        
        if (inputFieldScaler != null)
        {
            model.SetInputFieldOpen -= inputFieldScaler.SetInputFieldOpen;
        }

        model.OnViewStartFlow -= OnEndReadyFlow;
    }

    private void OnEndReadyFlow(bool isState)
    {
        view.gameObject.SetActive(isState);
    }
}
