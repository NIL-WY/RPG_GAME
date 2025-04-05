using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;
using UnityEngine.Serialization;

/// <summary>
/// 战斗UI管理器 - 控制战斗场景中所有用户界面元素
/// [组件需求] 需要UISoundHandler组件处理界面音效
/// </summary>
[RequireComponent(typeof(UISoundHandler))]
public class BattleUIHandler : MonoBehaviour
{
    // 单例实例
    public static BattleUIHandler Instance { get; private set; }

    // 选择器类型枚举
    private enum SelectorType { Attack, Ability }
    private SelectorType _selectorType;

    [Header("主操作菜单")]
    [Tooltip("普通攻击按钮")]
    [SerializeField] Button _attackButton;
    [Tooltip("防御按钮")]
    [SerializeField] Button _defendButton;
    [Tooltip("技能菜单按钮")]
    [SerializeField] Button _abilitiesButton;
    [Tooltip("道具菜单按钮")]
    [SerializeField] Button _itemsButton;
    [Tooltip("主操作菜单面板")]
    [SerializeField] GameObject _actionMenu;

    [Header("子菜单系统")]
    [Tooltip("技能/道具子菜单面板")]
    [SerializeField] GameObject _subMenu;
    [Tooltip("子菜单按钮预制体")]
    [SerializeField] GameObject _subMenuButtonPrefab;
    [Tooltip("初始按钮池大小")]
    [SerializeField] private int _startingButtonPoolSize;

    [Header("英雄状态UI")]
    [Tooltip("英雄状态控制器列表")]
    [SerializeField] List<HeroUIController> _heroInfoControllers;

    [Header("敌人选择器")]
    [Tooltip("敌人选择器指示器")]
    [SerializeField] GameObject _selector;
    [Tooltip("选择器X轴偏移量")]
    [SerializeField] float _selectorOffsetX = 2f;
    [Tooltip("选择器Z轴偏移量")]
    [SerializeField] float _selectorOffsetZ = -1f;
    [Tooltip("操作菜单相对于英雄的偏移量")]
    [SerializeField] private Vector2 _actionMenuOffset;
    [Tooltip("胜利")]
    [SerializeField] GameObject victoryPanel;
    [Tooltip("失败")]
    [SerializeField] GameObject defeatPanel;

    // 内部状态
    private Camera _camera;
    private bool _isSelectingEnemy = false;
    private bool _isInSubMenu = false;
    private int _selectorIndex;
    private Ability _selectedAbility;
    private UISoundHandler _soundHandler;
    private readonly List<Button> _subMenuButtonPool = new List<Button>();

    // 事件系统
    public delegate void AttackSelectEnemyEventHandler(Enemy enemy);
    public static event AttackSelectEnemyEventHandler OnSelectEnemyAttack;

    public delegate void AbilitySelectEnemyEventHandler(Enemy enemy, Ability ability);
    public static event AbilitySelectEnemyEventHandler OnSelectEnemyAbility;

    /// <summary>
    /// 单例初始化
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _soundHandler = GetComponent<UISoundHandler>();
        _camera = Camera.main;
    }

    /// <summary>
    /// 初始化UI系统
    /// </summary>
    private void Start()
    {
        if (BattleManager.Instance != null)
        {
            // 绑定按钮事件
            _attackButton.onClick.AddListener(delegate { StartSelectEnemy(SelectorType.Attack); });
            _defendButton.onClick.AddListener(BattleManager.Instance.ChoseDefend);
            _abilitiesButton.onClick.AddListener(DisplayAbilitiesMenu);
            _itemsButton.onClick.AddListener(DisplayItemsMenu);

            InitializeHeroUI();
            InitializeButtonPool(_startingButtonPoolSize);
        }
        else
        {
            Debug.LogError("Battle Manager Instance was not found!");
        }
    }

    /// <summary>
    /// 初始化英雄状态UI
    /// </summary>
    /// <remarks>
    /// 根据当前参战英雄数量激活对应的UI控制器
    /// </remarks>
    private void InitializeHeroUI()
    {
        for (int i = 0; i < _heroInfoControllers.Count; i++)
        {
            if (i >= BattleManager.Instance.heroes.Count)
            {
                _heroInfoControllers[i].gameObject.SetActive(false);
                continue;
            }
            Hero hero = BattleManager.Instance.heroes[i];
            _heroInfoControllers[i].Initialize(hero);
        }
    }

    /// <summary>
    /// 初始化子菜单按钮池
    /// </summary>
    /// <param name="startingSize">初始池大小</param>
    private void InitializeButtonPool(int startingSize)
    {
        for (int i = 0; i < startingSize; i++)
        {
            CreateSubMenuButton();
        }
    }

    /// <summary>
    /// 创建新的子菜单按钮实例
    /// </summary>
    private void CreateSubMenuButton()
    {
        var button = Instantiate(_subMenuButtonPrefab, _subMenu.transform, false);
        _subMenuButtonPool.Add(button.GetComponent<Button>());
    }

    /// <summary>
    /// 每帧更新选择器位置
    /// </summary>
    private void Update()
    {
        MoveEnemySelector();

        // ESC键返回逻辑
        if (_isInSubMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            _subMenu.SetActive(false);
            _actionMenu.SetActive(true);
            _isInSubMenu = false;
        }
    }

    /// <summary>
    /// 显示道具子菜单
    /// </summary>
    /// <remarks>
    /// 动态生成道具按钮并绑定使用逻辑
    /// </remarks>
    private void DisplayItemsMenu()
    {
        _isInSubMenu = true;
        _subMenu.gameObject.SetActive(true);
        Hero currentHero = BattleManager.Instance.GetCurrentHero();

        ResetSubButtons();

        // 动态扩展按钮池
        if (PartyManager.Instance.Inventory.Items.Count > _subMenuButtonPool.Count)
        {
            int difference = PartyManager.Instance.Inventory.Items.Count - _subMenuButtonPool.Count;
            for (int i = 0; i < difference; i++)
            {
                CreateSubMenuButton();
            }
        }

        // 配置道具按钮
        for (int i = 0; i < _subMenuButtonPool.Count; i++)
        {
            if (i >= PartyManager.Instance.Inventory.Items.Count)
            {
                _subMenuButtonPool[i].gameObject.SetActive(false);
                continue;
            }

            Item item = PartyManager.Instance.Inventory.Items[i];
            ConfigureItemButton(_subMenuButtonPool[i], item, currentHero);
        }
    }

    /// <summary>
    /// 配置单个道具按钮
    /// </summary>
    private void ConfigureItemButton(Button button, Item item, Hero hero)
    {
        var texts = button.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
        {
            if (text.gameObject.CompareTag("UI_Name"))
            {
                text.SetText(item.Name);
            }
            else if (text.gameObject.CompareTag("UI_Quantity"))
            {
                text.SetText("x" + item.Quantity);
            }
        }
        button.onClick.AddListener(() => {
            BattleManager.Instance.ChoseUseItem(item, hero);
            _subMenu.gameObject.SetActive(false);
            _actionMenu.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// 重置子菜单按钮状态
    /// </summary>
    private void ResetSubButtons()
    {
        foreach (Button button in _subMenuButtonPool)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(true);
            button.interactable = true;
        }
    }

    /// <summary>
    /// 显示技能子菜单
    /// </summary>
    /// <remarks>
    /// 根据当前英雄技能动态生成技能按钮
    /// </remarks>
    private void DisplayAbilitiesMenu()
    {
        _isInSubMenu = true;
        _subMenu.gameObject.SetActive(true);
        Hero currentHero = BattleManager.Instance.GetCurrentHero();

        ResetSubButtons();

        // 动态扩展按钮池（注意：当前检查条件疑似错误）
        if (currentHero.Abilities.Count > _subMenuButtonPool.Count)
        {
            int difference = currentHero.Abilities.Count - _subMenuButtonPool.Count;
            for (int i = 0; i < difference; i++)
            {
                CreateSubMenuButton();
            }
        }

        // 配置技能按钮
        for (int i = 0; i < _subMenuButtonPool.Count; i++)
        {
            if (i >= currentHero.Abilities.Count)
            {
                _subMenuButtonPool[i].gameObject.SetActive(false);
                continue;
            }

            Ability ability = currentHero.Abilities[i];
            ConfigureAbilityButton(_subMenuButtonPool[i], ability, currentHero);
        }
    }

    /// <summary>
    /// 配置单个技能按钮
    /// </summary>
    private void ConfigureAbilityButton(Button button, Ability ability, Hero hero)
    {
        var texts = button.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
        {
            if (text.gameObject.CompareTag("UI_Name"))
            {
                text.SetText(ability.Name);
            }
            else if (text.gameObject.CompareTag("UI_Quantity"))
            {
                text.SetText(ability.ManaCost + " MP");
            }
        }

        button.interactable = ability.ManaCost <= hero.CurrentMana;

        // 根据技能类型绑定不同逻辑
        if (ability is AttackAbility)
        {
            button.onClick.AddListener(() => StartSelectEnemy(SelectorType.Ability, ability));
        }
        else if (ability is SupportAbility)
        {
            button.onClick.AddListener(() => {
                BattleManager.Instance.ChoseAbility(ability);
                _subMenu.SetActive(false);
                _actionMenu.SetActive(false);
            });
        }
        else
        {
            Debug.LogError("未知技能类型: " + ability.GetType());
        }
    }

    /// <summary>
    /// 切换主操作菜单可见状态
    /// </summary>
    /// <param name="value">是否显示菜单</param>
    public void ToggleActionMenu(bool value)
    {
        var currentHero = BattleManager.Instance.GetCurrentHero();
        Vector3 menuPosition = currentHero.transform.position
            + new Vector3(-currentHero.MoveDistance, 0, 0)
            + (Vector3)_actionMenuOffset;

        _actionMenu.transform.position = _camera.WorldToScreenPoint(menuPosition);
        _actionMenu.gameObject.SetActive(value);
    }

    /// <summary>
    /// 开始选择敌人（普通攻击）
    /// </summary>
    private void StartSelectEnemy(SelectorType type)
    {
        _selectorType = type;
        _actionMenu.SetActive(false);
        _selector.gameObject.SetActive(true);
        _isSelectingEnemy = true;
        ResetSelectorPosition();
    }

    /// <summary>
    /// 开始选择敌人（技能攻击）
    /// </summary>
    private void StartSelectEnemy(SelectorType type, Ability ability)
    {
        _subMenu.gameObject.SetActive(false);
        _selectedAbility = ability;
        StartSelectEnemy(type);
    }

    /// <summary>
    /// 重置选择器初始位置
    /// </summary>
    private void ResetSelectorPosition()
    {
        _selectorIndex = 0;
        _selector.transform.position = BattleManager.Instance.enemies[0].transform.position
            + new Vector3(_selectorOffsetX, 0, _selectorOffsetZ);
    }

    /// <summary>
    /// 处理选择器移动逻辑
    /// </summary>
    private void MoveEnemySelector()
    {
        if (!_isSelectingEnemy) return;

        List<Enemy> enemies = BattleManager.Instance.enemies;

        // 方向键控制
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _selectorIndex = (_selectorIndex + 1) % enemies.Count;
            _soundHandler.PlayHighBeep();
            UpdateSelectorPosition(enemies);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _selectorIndex = (_selectorIndex - 1 + enemies.Count) % enemies.Count;
            _soundHandler.PlayLowBeep();
            UpdateSelectorPosition(enemies);
        }

        // 确认选择
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmSelection(enemies);
        }
        // 取消选择
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelSelection();
        }
    }

    /// <summary>
    /// 更新选择器位置
    /// </summary>
    private void UpdateSelectorPosition(List<Enemy> enemies)
    {
        _selector.transform.position = enemies[_selectorIndex].transform.position
            + new Vector3(_selectorOffsetX, 0, _selectorOffsetZ);
    }

    /// <summary>
    /// 确认当前选择
    /// </summary>
    private void ConfirmSelection(List<Enemy> enemies)
    {
        _selector.SetActive(false);
        _isSelectingEnemy = false;

        switch (_selectorType)
        {
            case SelectorType.Attack:
                OnSelectEnemyAttack?.Invoke(enemies[_selectorIndex]);
                break;
            case SelectorType.Ability:
                OnSelectEnemyAbility?.Invoke(enemies[_selectorIndex], _selectedAbility);
                break;
            default:
                Debug.LogError("未知选择器类型: " + _selectorType);
                break;
        }
    }

    /// <summary>
    /// 取消选择返回菜单
    /// </summary>
    private void CancelSelection()
    {
        _actionMenu.SetActive(true);
        _selector.SetActive(false);
        _isSelectingEnemy = false;
    }
    public void ShowBattleResult(bool isVictory)
    {
        victoryPanel.SetActive(isVictory);
        defeatPanel.SetActive(!isVictory);
        ToggleActionMenu(false);
    }
}

