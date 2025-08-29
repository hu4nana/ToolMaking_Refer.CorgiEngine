using UnityEngine;


public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject Player;
    public GameObject Enemy;
    public GameObject _gameOver;

    // 캐릭터 참조
    public Character _playerChar { get; private set; }
    public Character _enemyChar { get; private set; }

    // 체력 관련
    public float _playerMaxHP { get; private set; }
    public float _playerCurHP { get; private set; }
    public float _enemyMaxHP { get; private set; }
    public float _enemyCurHP { get; private set; }

    // 상태
    private bool isOver = false;
    string _winner = null;

    // 내부 캐싱
    private Health _playerHealth;
    private Health _enemyHealth;
    private PlayerSkill1 _skill1;
    private PlayerSkill2 _skill2;
    private PlayerSkill3 _skill3;

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        _gameOver.SetActive(false);
        isOver = false;
        _winner = null;

        // 컴포넌트 캐싱
        _playerChar = Player.GetComponent<Character>();
        _enemyChar = Enemy.GetComponent<Character>();

        _playerHealth = _playerChar.GetComponent<Health>();
        _enemyHealth = _enemyChar.GetComponent<Health>();

        _skill1 = _playerChar.GetComponent<PlayerSkill1>();
        _skill2 = _playerChar.GetComponent<PlayerSkill2>();
        _skill3 = _playerChar.GetComponent<PlayerSkill3>();

        // 초기 체력값
        _playerMaxHP = _playerHealth.maxHP;
        _playerCurHP = _playerHealth.curHP;

        _enemyMaxHP = _enemyHealth.maxHP;
        _enemyCurHP = _enemyHealth.curHP;
    }

    private void Update()
    {
        GameOver();
    }
    public void GameOver()
    {
        if (_playerHealth.dead)
            _winner = _enemyChar.id;
        else if (_enemyHealth.dead)
            _winner = _playerChar.id;

        if(_winner != null)
        {
            isOver = true;
            _gameOver.SetActive(true);
        }
    }
}
