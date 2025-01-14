using System;
using System.Linq;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Animation = Spine.Animation;

public class DebugPanel : MonoBehaviour
{
    [Required] [SerializeField] private TMP_Text animationName;

    [Required] [SerializeField] private Button nextAnimation;

    [Required] [SerializeField] private Button nextStage;

    [Required] [SerializeField] private Button windows;

    [Required] [SerializeField] private SkeletonGraphic spine;

    [Required] [SerializeField] private TMP_Text resolution;

    [Required] [SerializeField] private TMP_Dropdown spineSkins;

    [Required] [SerializeField] private TMP_Dropdown gameStates;

    [Required] [SerializeField] private GameObject content;

    [Required] [SerializeField] private UIControl uiControl;

    private int _animationIndex;
    private Animation[] _animations;

    [Inject] private Process process;

    [Inject] private SpineAnimationControl spineAnimationControl;

    // Start is called before the first frame update
    private void Start()
    {
        content.SetActive(false);
        InitSpineSlineDropDown();
        InitGameStateDropDown();
        var skeletonDataAnimations = spine.AnimationState.Data.SkeletonData.Animations;
        _animations = new Animation[skeletonDataAnimations.Count];
        skeletonDataAnimations.CopyTo(_animations, 0);
        nextAnimation.onClick.AddListener(NextAnimation);
        windows.onClick.AddListener(WindowsControl);
        nextStage.onClick.AddListener(process.NextProcess);
        spineSkins.onValueChanged.AddListener(ChangeSkin);
        gameStates.onValueChanged.AddListener(GameStatesDropDownOnValueChange);

        spineAnimationControl.SubscribeOnSetAnimationEvent(OnChangeSpineAnimation).AddTo(this);
        process.SubscribeOnNextProcess(OnNextProcess).AddTo(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            content.SetActive(!content.activeInHierarchy);
        }

        if (content.activeInHierarchy)
        {
            resolution.text = $"{Screen.width} x {Screen.height}";
        }
    }

    private void OnNextProcess(string processType)
    {
        if (Enum.TryParse(processType, out ProcessType temp))
        {
            gameStates.SetValueWithoutNotify((int)temp);
        }
    }

    private void OnChangeSpineAnimation(TrackEntry trackentry)
    {
        animationName.text = trackentry.Animation.Name;
    }

    private void GameStatesDropDownOnValueChange(int index)
    {
        var gameStatesOption = gameStates.options[index];
        var text = gameStatesOption.text;
        var strings = Enum.GetNames(typeof(ProcessType));
        for (var i = 0; i < strings.Length; i++)
        {
            if (strings[i] == text)
            {
                uiControl.ChangeState((ProcessType)i, false);
            }
        }
    }

    private void WindowsControl()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

// Update is called once per frame
    private void NextAnimation()
    {
        var currentAnimation = _animations[_animationIndex++ % _animations.Length];
        spine.AnimationState.SetAnimation(0, currentAnimation, true);
    }

    private void InitSpineSlineDropDown()
    {
        var skinNames = spine.SkeletonData.Skins.Items.Select(x => x.Name);
        var optionDatas = skinNames.Select(x => new TMP_Dropdown.OptionData(x)).ToList();
        spineSkins.ClearOptions();
        spineSkins.AddOptions(optionDatas);
    }

    private void InitGameStateDropDown()
    {
        var list = Enum.GetNames(typeof(ProcessType)).ToList();
        gameStates.ClearOptions();
        gameStates.AddOptions(list);
    }

    private void ChangeSkin(int index)
    {
        var skinNames = spine.SkeletonData.Skins.Items.Select(x => x.Name).ToList();
        spine.Skeleton.SetSkin(skinNames[index]);
        spine.Skeleton.SetSlotsToSetupPose();
        spine.AnimationState.Apply(spine.Skeleton);
    }
}