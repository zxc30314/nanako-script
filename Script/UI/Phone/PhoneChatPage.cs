using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;
using Zenject;
using Button = pruss.Tool.UI.Button;

public class PhoneChatPage : PageComponent, IPointerClickHandler
{
    private const string ManPrefix = "M";
    private const string NanakoPrefix = "N";
    private const string TableName = "PhoneChat";

    [Required] [SerializeField] private TextAsset text;
    [Required] [SerializeField] private ChatBox myChatBox;
    [Required] [SerializeField] private ChatBox otherChatBox;
    [Required] [SerializeField] private RectTransform chatList;
    [Required] [SerializeField] private ScrollRect scrollRect;
    [Required] [SerializeField] private Button nextButton;
    [Required] [SerializeField] private Image arrow;
    [Inject] private AudioManager _audioManager;
    private bool _canClick = true;
    [Inject] private MaskManager _maskManager;
    private bool _onEnd;
    [Inject] private Process _process;
    private Queue<KeyValuePair<long, StringTableEntry>> _stringTable = new();
    [Inject] private UIComponents _uiComponents;

    private void OnDisable()
    {
        foreach (Transform child in chatList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_canClick)
        {
            return;
        }

        OnNext().Forget();
    }

    private void OnEnd()
    {
        _process.NextProcess();
    }

    private void ShoChatBox(string suffix, string table, string key)
    {
        if (suffix == ManPrefix)
        {
            ShowManChatBox(table, key);
        }
        else if (suffix == NanakoPrefix)
        {
            ShowNanakoChatBox(table, key);
        }
    }

    private void ShowNanakoChatBox(string table, string key)
    {
        _audioManager.PlayShortSE("message_nanako_se");
        InstantiateChatBox(otherChatBox, table, key);
    }

    private void ShowManChatBox(string table, string key)
    {
        _audioManager.PlayShortSE("message_player_se");
        InstantiateChatBox(myChatBox, table, key);
    }

    private void InstantiateChatBox(ChatBox chatBoxPrefab, string table, string key)
    {
        var chatBox = Instantiate(chatBoxPrefab, chatList);
        chatBox.SetChatKey(table, key);
    }

    private void ScrollToBottom()
    {
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    public override void StartProcess()
    {
        OnNext().Forget();
    }

    public override void Show()
    {
        RectTransform.gameObject.SetActive(true);
        _uiComponents.Show(ProcessType.PhoneChat);
        arrow.gameObject.SetActive(false);
        _stringTable = new Queue<KeyValuePair<long, StringTableEntry>>(LocalizationSettings.StringDatabase.GetTable(TableName)
            .Where(x => x.Value.Key.Split('_').FirstOrDefault() == "message")
            .OrderBy(x => int.Parse(x.Value.Key.Split('_')[1])));
    }

    private async UniTaskVoid OnNext()
    {
        if (_stringTable.Count == 0 && !_onEnd)
        {
            _onEnd = true;
            arrow.gameObject.SetActive(true);
            await DOTween.Sequence().Append(arrow.DOFade(0, 0.5f)).Append(arrow.DOFade(1, 0.5f)).SetLoops(-1);
            return;
        }

        if (_onEnd)
        {
            _canClick = false;
            OnEnd();
            return;
        }

        var s = _stringTable.Dequeue();
        var key = s.Value.Key;
        var suffix = key.Split('_').Last();
        ShoChatBox(suffix, TableName, key);
        await UniTask.Delay(TimeSpan.FromTicks(1));
        ScrollToBottom();
    }

    public override void Hide()
    {
        RectTransform.gameObject.SetActive(false);
        _onEnd = false;
        _canClick = true;
    }
}