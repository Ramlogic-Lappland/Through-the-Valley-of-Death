using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UISound : MonoBehaviour , IPointerEnterHandler
{
    [SerializeField] private AK.Wwise.Event hoverEvent;
    [SerializeField] private AK.Wwise.Event selectEvent;
    private Button  _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlaySelectedSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayOnSound();
    }

    private void PlaySelectedSound()
    {
        AkUnitySoundEngine.PostEvent(selectEvent.Id, gameObject);
    }

    private void PlayOnSound()
    {
        AkUnitySoundEngine.PostEvent(hoverEvent.Id, gameObject);
    }
}
