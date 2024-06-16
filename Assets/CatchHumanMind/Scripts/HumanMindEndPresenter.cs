using UnityEngine;

public class HumanMindEndPresenter : MonoBehaviour
{
    [SerializeField] private HumanMindManager manager;
    [SerializeField] private HumanMindEndView view;
    [SerializeField] private HumanMindEndModel model;

    private void Start()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        if (manager != null)
        {
            manager.OnEndProcess += model.UpdateEndState;
        }
        if (view != null)
        {
            model.UpdateTimer += view.UpdateTimer;
            model.OnUpdateAnswer += view.UpdateAnswerInfo;
            model.OnStageEffect += view.OnPlayStageEffect;
            model.OnUpdatEexplan += view.SetDataResponse;
            model.OnEndRound += view.OnEndRound;
            model.OnEndGame += view.OnEndGame;

            view.OnClickLeave += model.LeaveGame;
        }

        if (manager != null && view != null)
        {
            manager.OnRecivePromptResponse += view.SetDataResponse;
        }

        model.OnEndViewActive += OnEndViewActive;
    }
    
    private void OnEndViewActive(bool isState)
    {
        view.gameObject.SetActive(isState);
    }
}