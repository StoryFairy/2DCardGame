using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarController : MonoBehaviour
{
    private CharacterBase currentCharacter;
    private Enemy enemy;
    
    [Header("Elements")] 
    public Transform healthBarTransform;
    private UIDocument healthBarDocument;
    private ProgressBar healthBar;
    
    private VisualElement defenseElement;
    private Label defenseAmountLabel;

    private VisualElement buffElement;
    private Label buffRound;

    private VisualElement intentSprite;
    private Label intentAmount;

    [Header("Buff图标")] 
    public Sprite buffSprite;
    public Sprite debuffSprite;

    private void Awake()
    {
        currentCharacter = GetComponent<CharacterBase>();
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        InitHealthBar();
    }

    private void MoveToWorldPosition(VisualElement element, Vector3 worldPosition, Vector2 size)
    {
        Rect rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(element.panel, worldPosition, size, Camera.main);
        element.transform.position = rect.position;
    }
    
    [ContextMenu("GetUIPosition")]
    public void InitHealthBar()
    {
        healthBarDocument = GetComponent<UIDocument>();
        healthBar = healthBarDocument.rootVisualElement.Q<ProgressBar>("HealthBar");

        healthBar.highValue = currentCharacter.MaxHp;
        MoveToWorldPosition(healthBar, healthBarTransform.position, Vector2.zero);
        
        defenseElement=healthBar.Q<VisualElement>("Defense");
        defenseAmountLabel=defenseElement.Q<Label>("DefenseAmount");

        defenseElement.style.display = DisplayStyle.None;
        
        buffElement=healthBar.Q<VisualElement>("Buff");
        buffRound=buffElement.Q<Label>("BuffRound");
        
        buffElement.style.display = DisplayStyle.None;
        
        intentSprite=healthBar.Q<VisualElement>("Intent");
        intentAmount=healthBar.Q<Label>("IntentAmount");
        intentSprite.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (currentCharacter.isDead)
        {
            healthBar.style.display = DisplayStyle.None;
            return;
        }

        if (healthBar != null)
        {
            healthBar.title = $"{currentCharacter.CurrentHp}/{currentCharacter.MaxHp}";
            healthBar.value = currentCharacter.CurrentHp;

            healthBar.RemoveFromClassList("highHealth");
            healthBar.RemoveFromClassList("mediumHealth");
            healthBar.RemoveFromClassList("lowHealth");

            var percentage = (float)currentCharacter.CurrentHp / (float)currentCharacter.MaxHp;

            if (percentage < 0.3f)
            {
                healthBar.AddToClassList("lowHealth");
            }
            else if (percentage < 0.6f)
            {
                healthBar.AddToClassList("mediumHealth");
            }
            else
            {
                healthBar.AddToClassList("highHealth");
            }

            //防御属性更新
            defenseElement.style.display =
                currentCharacter.defense.currentValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            defenseAmountLabel.text = currentCharacter.defense.currentValue.ToString();
            
            //buff回合更新
            buffElement.style.display =
                currentCharacter.buffRound.currentValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            buffRound.text=currentCharacter.buffRound.currentValue.ToString();
            buffElement.style.backgroundImage = currentCharacter.baseStrength > 1
                ? new StyleBackground(buffSprite)
                : new StyleBackground(debuffSprite);
        }
    }

    /// <summary>
    /// 玩家回合开始时调用
    /// </summary>
    public void SetIntentElement()
    {
        intentSprite.style.display = DisplayStyle.Flex;
        intentSprite.style.backgroundImage = new StyleBackground(enemy.currentAction.intentSprite);

        //判断是否为攻击
        var value = enemy.currentAction.effect.value;
        if (enemy.currentAction.effect.GetType() == typeof(DamageEffect))
        {
            value = (int)math.round(enemy.currentAction.effect.value * enemy.baseStrength);
        }

        intentAmount.text = value.ToString();
    }

    /// <summary>
    /// 敌人回合结束调用
    /// </summary>
    public void HideIntentElement()
    {
        intentSprite.style.display = DisplayStyle.None;
    }
}
