using UnityEngine;
using UnityEngine.UI;

public enum EnemyType { Melee, Range, Boss, Object, Etc }

public class Enemy : MonoBehaviour
{
    [Header("�� ����")]
    public EnemyType eType;
    [Range(1, 3)] public int enemyGrade = 1;
    Vector3 originalPos;

    public int haveExp;

    // �⺻ ����
    public int hpMax;
    public int attackPower;
    public float moveSpeed;
    public float attackDelay;
    public float attackRange;
    public float sightRange;

    // ���� ����
    public float hp;
    public float currentDelay;
    public bool isDie = false;

    // UI �� Ÿ��
    public Image hpSlider;
    public Text enemyName;
    public PlayerStats target;

    [Header("�߻���ų ������")]
    public GameObject[] dropItems;

    void Start()
    {
        hp = hpMax;
        originalPos = transform.position;
        currentDelay = attackDelay;
        UpdateHP();

        #region ���� �̸� ����
        switch (eType)
        {
            case EnemyType.Melee:
                enemyName.text = "�ٰŸ� " + enemyGrade + " �ܰ�";
                break;
            case EnemyType.Range:
                enemyName.text = "���Ÿ� " + enemyGrade + " �ܰ�";
                break;
            case EnemyType.Boss:
                enemyName.text = "���� " + enemyGrade + " �ܰ�";
                break;
            case EnemyType.Object:
                enemyName.text = "�̼� ������Ʈ";
                break;
        }
        #endregion

        InvokeRepeating("SearchTarget", 1, 1);
    }

    void Update()
    {
        if (target == null || isDie) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > sightRange) return;

        if (distance <= attackRange)
        {
            if (currentDelay >= attackDelay)
            {
                target.TakeDamage(attackPower);
                currentDelay = 0;
            }
        }
        else
        {
            transform.LookAt(target.transform);
            transform.Translate(0, 0, moveSpeed * Time.deltaTime);
        }

        currentDelay += Time.deltaTime;
    }

    void SearchTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats.isDie) continue;

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                target = playerStats;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            isDie = true;

            if (eType != EnemyType.Boss && eType != EnemyType.Object && eType != EnemyType.Etc)
                transform.parent.GetComponent<EnemyGroup>().groupCount--;

            GameManager.Instance.AddScore(10 * enemyGrade);

            #region óġ�� ����ġ ȹ��
            foreach (var player in PlayerManager.instance.players)
            {
                int value = (int)(haveExp * 0.1f);
                player.GetComponent<PlayerStats>().GetExp(value);
            }
            UIManager.Instance.ShowMsg("�÷��̾� ��� 10% ����ġ ȹ��");
            #endregion

            #region �� ������ �̺�Ʈ ó��
            switch (eType)
            {
                case EnemyType.Object:
                    GameManager.Instance.mission[1].goalObjectCount--;
                    break;
                case EnemyType.Etc:
                    DropItem();
                    break;
                case EnemyType.Boss:
                    switch (enemyGrade)
                    {
                        case 1:
                            GameManager.Instance.NextStage();
                            break;
                        case 2:
                            //GameManager.Instance.CallFinalBoss();
                            break;
                        case 3:
                            //GameManager.Instance.ShowFinalUI();
                            break;
                    }
                    break;
                default:
                    DropItem();
                    GameManager.Instance.DestroyEnemy(enemyGrade);
                    break;
            }
            #endregion

            gameObject.SetActive(false);
        }

        UpdateHP();
    }

    void UpdateHP()
    {
        hpSlider.fillAmount = hp / hpMax;
    }

    void DropItem()
    {
        /*int randomResult = Random.Range(0, dropItems.Length);
        Destroy(Instantiate(dropItems[randomResult], transform.position, Quaternion.identity), 5f);*/
    }

    public void Reset()
    {
        hp = hpMax;
        isDie = false;
        UpdateHP();
        gameObject.SetActive(true);
        transform.position = originalPos;
    }
}