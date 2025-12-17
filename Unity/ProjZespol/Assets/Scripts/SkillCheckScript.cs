using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport.PostProcessing;

public class SkillCheckScript : MonoBehaviour
{
    //WITAM W SPAGHETTI BOLONEZE KOD ROKU 1993

    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private SpriteRenderer flag;
    [SerializeField] private RaptorCore raptorCore;
    [SerializeField] private CharacterClass characterClass;
    private Queue<GameObject> bricks = new Queue<GameObject>();

    private int kombo = 0;
    private int Bonus = 5;
    private float waitTime = 0.9f;
    private int brickCount = 10;

    [SerializeField] private GameObject Layout;
    private ComboVisual combovisual;
    private void Start()
    {
        combovisual = Layout.GetComponent<ComboVisual>();
    }
    void Update()
    {
        if (this.gameObject.activeSelf && !flag.enabled)
        {
            CheckMouse0();
            CheckMouse1();
            if (bricks.Count != 0 && bricks.Peek().GetComponent<BrickMovementScript>().overlap == 3)
                ResetSkillCheck(-Bonus);
        }
    }

    void CheckMouse0()
    {
        if (Input.GetMouseButtonDown(0))
            if (bricks.Peek().GetComponent<BrickMovementScript>().overlap == 1)
            {
                Destroy(bricks.Dequeue());
                if (bricks.Count == 0)
                    ResetSkillCheck(Bonus);
            }
            else
                ResetSkillCheck(-Bonus);       

    }

    void CheckMouse1()
    {
        if (Input.GetMouseButtonDown(1))
            if (bricks.Peek().GetComponent<BrickMovementScript>().overlap == 2)
            {
                Destroy(bricks.Dequeue());
                if (bricks.Count == 0)
                    ResetSkillCheck(Bonus);
            }
            else
                ResetSkillCheck(-Bonus);

    }
    
    public void StartSkillCheck()
    {
        if(characterClass.mortalClicker)
            StartCoroutine(SpawnBricks(brickCount + kombo, waitTime - (0.05f * kombo)));
        else
            StartCoroutine(SpawnBricks(brickCount, waitTime));
    }

    private void ResetSkillCheck(int value)
    {
        flag.enabled = true;
        if (value > 0)
        {
            if (characterClass.mortalClicker || characterClass.championOfClicks)
            {
                kombo++;
                
                if (characterClass.mortalClicker)
                {
                    raptorCore.SkillMultiplier -= 10 * (kombo - 1);
                    raptorCore.SkillMultiplier += 10 * kombo;
                }
                    
                if(characterClass.championOfClicks && kombo == 3)
                {
                    kombo = 0;
                    characterClass.EnableAlanWake();
                }
                combovisual.UpdateCombo(kombo);
            }      
            raptorCore.AddCurrency(value);
            raptorCore.SkillManager.EnableChickenDinner(true);
            characterClass.EnableGenius(false);
            characterClass.Pedator(true);
        }
        else
        {          
            if (characterClass.mortalClicker)
            {
                raptorCore.SkillMultiplier -= 10 * kombo;
            }
            kombo = 0;
            combovisual.UpdateCombo(kombo);
            raptorCore.SubCurrency(-value);
            raptorCore.SkillManager.EnableChickenDinner(false);
            characterClass.Pedator(false); 
            characterClass.EnableGenius(true);
            raptorCore.SkillManager.RandomPotionEffect();
            characterClass.EnableExpBoost();
        }
        ClearBricks();
        this.gameObject.SetActive(false);
    }

    void ClearBricks()
    {
        bricks.Clear();
        foreach (var brick in this.gameObject.GetComponentsInChildren<BrickMovementScript>()) //broski nie ma czasu na �adny kod trzeba wypuszcza� szybko pronto
        {
            Destroy(brick.gameObject);
        }
    }

    IEnumerator SpawnBricks(int count, float waitTime)
    {
        for(int i = 0; i<3; i++)
        {
            yield return new WaitForSeconds(1f);
            switch (i)//temp rozwi�zanie na danie czasu graczowi na ograni�cie si� �wiat�a ostrzegawcze moment
            {
                case 0: flag.color = Color.yellow; break;
                case 1: flag.color = Color.green; break;
                case 2: { flag.enabled = false; flag.color = Color.red; break; }
            }                
        }
        for (int i = 0; i < count; i++)
        {
            bricks.Enqueue(Instantiate(brickPrefab, this.gameObject.transform.localPosition, Quaternion.identity, this.gameObject.transform));
            yield return new WaitForSeconds(waitTime);
        }       
    }

}
