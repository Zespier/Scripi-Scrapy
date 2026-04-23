using System;
using UnityEngine;

public class Menu : MonoBehaviour
{
    // me podrían quitar el carne de programador por esto
    protected bool _isInTutorial;
    [SerializeField] private CanvasGroup _canvasGroup;

    public CanvasGroup canvasGroup => _canvasGroup;

    public bool IsInTutorial => _isInTutorial;
    public bool isOpen => _canvasGroup.alpha >= 0.99f;
    public Action<bool> OnMenuDisplayed;

    public virtual void UseItem()
    {
    }

    public virtual void SetTutorialState(bool state)
    {
        _isInTutorial = state;
    }

    public virtual void ActiveCanvasGroup(bool active)
    {
        _canvasGroup.alpha = active ? 1 : 0;
        _canvasGroup.interactable = active;
        _canvasGroup.blocksRaycasts = active;

        OnMenuDisplayed?.Invoke(active);
    }

    public void FadeCanvasGroup(bool active, float time, bool timeScaled = false, Action onComplete = null)
    {
        //UIAnimator.Fade(canvasGroup, active, time, timeScaled, onComplete);
    }
}
//TODO: Task list shows the number of this line, I'm interested in seeing the total amount of lines my game has, so I will put this in the last line of every script I find.
