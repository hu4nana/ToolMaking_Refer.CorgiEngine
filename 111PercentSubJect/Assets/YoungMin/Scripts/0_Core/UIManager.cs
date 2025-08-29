using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public GameManager _gameManager;

    public GameObject PlayerInfoUI;
    public GameObject EnemyInfoUI;

    public GameObject[] PlayerSkillUI;

    Image p_Info_HealthBar;
    Text p_Info_curHpText;
    Text p_Info_IdText;

    Image e_Info_HealthBar;
    Text e_Info_curHpText;
    Text e_Info_IdText;

    float p_maxHp;
    float p_curHp;

    float e_maxHp;
    float e_curHp;

    void Start()
    {
        Initialization();
    }

    public virtual void Initialization()
    {
        Image[] p_images = PlayerInfoUI.GetComponentsInChildren<Image>();
        foreach (Image img in p_images)
        {
            if (img.name == "Healthbar")   // Hierarchy 안의 오브젝트 이름
                p_Info_HealthBar = img;
        }

        Text[] p_texts = PlayerInfoUI.GetComponentsInChildren<Text>();
        foreach (Text text in p_texts)
        {
            if (text.name == "Health_Text")
                p_Info_curHpText = text;
            else if (text.name == "Id")
                p_Info_IdText = text;
        }

        // Enemy UI
        Image[] e_images = EnemyInfoUI.GetComponentsInChildren<Image>();
        foreach (Image img in e_images)
        {
            if (img.name == "Healthbar")
                e_Info_HealthBar = img;
        }

        Text[] e_texts = EnemyInfoUI.GetComponentsInChildren<Text>();
        foreach (Text text in e_texts)
        {
            if (text.name == "Health_Text")
                e_Info_curHpText = text;
            else if (text.name == "Id")
                e_Info_IdText = text;
        }

        // 캐릭터 HP 세팅
        p_maxHp = _gameManager.Player.GetComponent<Health>().maxHP;
        p_curHp = _gameManager.Player.GetComponent<Health>().curHP;

        e_maxHp = _gameManager.Enemy.GetComponent<Health>().maxHP;
        e_curHp = _gameManager.Enemy.GetComponent<Health>().curHP;

        // 초기 UI 표시
        p_Info_HealthBar.fillAmount = p_curHp / p_maxHp;
        p_Info_curHpText.text = p_curHp.ToString();
        p_Info_IdText.text = _gameManager.Player.GetComponent<Character>().id;

        e_Info_HealthBar.fillAmount = e_curHp / e_maxHp;
        e_Info_curHpText.text = e_curHp.ToString();
        e_Info_IdText.text = _gameManager.Enemy.GetComponent<Character>().id;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateInfoUI();
        UpdatePlayerSkillUI();
    }

    public void UpdateInfoUI()
    {
        if (p_curHp != _gameManager.Player.GetComponent<Health>().curHP)
        {
            p_curHp = _gameManager.Player.GetComponent<Health>().curHP;
            p_Info_curHpText.text = p_curHp.ToString();
            p_Info_HealthBar.fillAmount=p_curHp / p_maxHp;
        }

        if (e_curHp != _gameManager.Enemy.GetComponent<Health>().curHP)
        {
            e_curHp = _gameManager.Enemy.GetComponent<Health>().curHP;
            e_Info_curHpText.text=e_curHp.ToString();
            e_Info_HealthBar.fillAmount = e_curHp / e_maxHp;
        }
        
    }

    
    public void UpdatePlayerSkillUI()
    {
        if (_gameManager.Player.GetComponent<PlayerSkill1>().GetCoolDownTimer() != 0)
        {
            PlayerSkillUI[0].GetComponentInChildren<Text>().text = _gameManager.Player.GetComponent<PlayerSkill1>().GetCoolDownTimer().ToString();
            PlayerSkillUI[0].GetComponent<Image>().fillAmount = _gameManager.Player.GetComponent<PlayerSkill1>().GetCoolDownTimer() / _gameManager.Player.GetComponent<PlayerSkill1>().CooldownTime;
        }
        else if(PlayerSkillUI[0].GetComponentInChildren<Text>().text != null)
        {
            PlayerSkillUI[0].GetComponentInChildren<Text>().text = null;
        }
        if (_gameManager.Player.GetComponent<PlayerSkill2>().GetCoolDownTimer() != 0)
        {
            PlayerSkillUI[1].GetComponentInChildren<Text>().text = _gameManager.Player.GetComponent<PlayerSkill2>().GetCoolDownTimer().ToString();
            PlayerSkillUI[1].GetComponent<Image>().fillAmount = _gameManager.Player.GetComponent<PlayerSkill2>().GetCoolDownTimer() / _gameManager.Player.GetComponent<PlayerSkill1>().CooldownTime;
        }
        else if (PlayerSkillUI[1].GetComponentInChildren<Text>().text != null)
        {
            PlayerSkillUI[1].GetComponentInChildren<Text>().text = null;
        }
        if (_gameManager.Player.GetComponent<PlayerSkill3>().GetCoolDownTimer() != 0)
        {
            PlayerSkillUI[2].GetComponentInChildren<Text>().text = _gameManager.Player.GetComponent<PlayerSkill3>().GetCoolDownTimer().ToString();
            PlayerSkillUI[2].GetComponent<Image>().fillAmount = _gameManager.Player.GetComponent<PlayerSkill3>().GetCoolDownTimer() / _gameManager.Player.GetComponent<PlayerSkill1>().CooldownTime;
        }
        else if (PlayerSkillUI[2].GetComponentInChildren<Text>().text != null)
        {
            PlayerSkillUI[2].GetComponentInChildren<Text>().text = null;
        }
    }
}
