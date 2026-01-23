using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class StageAnimatorManager : MonoBehaviour
{
    public const string STAGE_SUFFIX = "stage";  // РЕГИСТР ВАЖЕН: как в именах объектов!

    [Header("Конфигурация анимации")]
    [SerializeField] private string animationTrigger = "StageChange";  // Триггер в Animator

    // Имена: "1_Stage", "2_Stage" и т.д.
    private static readonly Regex stageRegex = new Regex(@"^\d+_" + STAGE_SUFFIX + "$");

    private readonly List<Animator> stageAnimators = new List<Animator>();

    [Header("Переменная-триггер")]
    [SerializeField] private int currentStageValue = 1;

    [Tooltip("Однократный флаг, чтобы дернуть триггер из инспектора")]
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
                Debug.Log($"Stage изменен на {currentStageValue} — анимация запущена");
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
            // ВАЖНО: меняем значение, а не присваиваем то же самое
            CurrentStageValue++;      // или CurrentStageValue = CurrentStageValue + 1;
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
                    Debug.Log($"Добавлен Animator у: {child.name}");
                }
                else
                {
                    Debug.LogWarning($"Animator отсутствует у stage-ребенка: {child.name}");
                }
            }
        }

        Debug.Log($"Собрано stage-Animator'ов: {stageAnimators.Count}");
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
