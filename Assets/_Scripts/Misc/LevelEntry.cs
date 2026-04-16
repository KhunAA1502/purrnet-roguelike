using PurrNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelEntry : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private AttackData attack;
    private GameState_LevelUp levelUp;

    public void Init(AttackData attack, GameState_LevelUp levelUp)
    {
        if (!InstanceHandler.TryGetInstance(out AttackHandler attackHandler))
        {
            return;
        }
        this.attack = attack;
        this.levelUp = levelUp;
        icon.sprite = attack.icon;
        nameText.text = attack.attackName;
        descriptionText.text = attack.GetLevelDescription(attackHandler.GetLevel(attack.attackId) + 1);
    }

    public void PickUpgrade()
    {
        if (!InstanceHandler.TryGetInstance(out AttackHandler attackHandler))
        {
            return;
        }
        attackHandler.AddAttack(attack);
        levelUp.SetReady();
    }
}
