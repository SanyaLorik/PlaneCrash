using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class StageAnimatorManager : MonoBehaviour
{
    public const string STAGE_SUFFIX = "stage";  // ������� �����: ��� � ������ ��������!

    [Header("������������ ��������")]
    [SerializeField] private string animationTrigger = "StageChange";  // ������� � Animator

    // �����: "1_Stage", "2_Stage" � �.�.
    private static readonly Regex stageRegex = new Regex(@"^\d+_" + STAGE_SUFFIX + "$");

    private readonly List<Animator> stageAnimators = new List<Animator>();

    [Header("����������-�������")]
    [SerializeField] private int currentStageValue = 1;

    [Tooltip("����������� ����, ����� ������� ������� �� ����������")]
    public bool ChangeValue;

    [field: SerializeField]
    public int CurrentStageValue
    {
        get => currentStageValue;
        set
        {
            if (currentStageValue != value)
            {
                currentStageValue = value;
                TriggerStageAnimation();
                Debug.Log($"Stage ������� �� {currentStageValue} � �������� ��������");
            }
        }
    }

    private void Start()
    {
        CollectStageAnimators();
        currentStageValue = 1;
    }

    private void Update()
    {
        if (ChangeValue)
        {
            // �����: ������ ��������, � �� ����������� �� �� �����
            CurrentStageValue++;      // ��� CurrentStageValue = CurrentStageValue + 1;
            ChangeValue = false;
        }
    }

    private void CollectStageAnimators()
    {
        stageAnimators.Clear();

        foreach (Transform child in transform)
        {
            if (stageRegex.IsMatch(child.name))
            {
                Animator anim = child.GetComponent<Animator>();
                if (anim != null)
                {
                    stageAnimators.Add(anim);
                    // Debug.Log($"�������� Animator �: {child.name}");
                }
                else
                {
                    // Debug.LogWarning($"Animator ����������� � stage-�������: {child.name}");
                }
            }
        }

        // Debug.Log($"������� stage-Animator'��: {stageAnimators.Count}");
    }

    private void TriggerStageAnimation()
    {
        foreach (Animator animator in stageAnimators)
        {
            if (animator == null) continue;

            animator.ResetTrigger(animationTrigger);
            animator.SetTrigger(animationTrigger);
        }
    }
}
