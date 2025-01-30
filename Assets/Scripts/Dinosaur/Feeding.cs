using System.Collections;
using UnityEngine;

public class Feeding : MonoBehaviour
{
    [SerializeField] private Animator _dinosaurAnimator;

    private FeedButton _feedButton;
    public static bool isEating = false;

    private void Awake()
    {
        StartCoroutine(FindFeedButtonWhenActive());
    }

    private IEnumerator FindFeedButtonWhenActive()
    {
        while (_feedButton == null)
        {
            _feedButton = FindObjectOfType<FeedButton>();
            yield return null;
        }

        if (_feedButton == null)
        {
            Debug.LogError("FeedButton not found even after waiting.");
        }
        else
        {
            Debug.Log("FeedButton found!");
            _feedButton.OnClickEvent.AddListener(OnFeedButtonClick);
        }
    }

    private void OnFeedButtonClick()
    {
        if (_dinosaurAnimator != null)
        {
            _dinosaurAnimator.SetTrigger("Eat");
        }
    }
}