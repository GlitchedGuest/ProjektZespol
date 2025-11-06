using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class SkillCheckScript : MonoBehaviour
{
    //WITAM W SPAGHETTI BOLONEZE KOD ROKU 1993

    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private SpriteRenderer flag;
    [SerializeField] private RaptorCore raptorCore;
    private Queue<GameObject> bricks = new Queue<GameObject>();


    void Update()
    {
        if (this.gameObject.activeSelf && !flag.enabled)
        {
            CheckMouse0();
            CheckMouse1();
            if (bricks.Count != 0 && bricks.Peek().GetComponent<BrickMovementScript>().overlap == 3)
                ResetSkillCheck(-5);
        }
    }

    void CheckMouse0()
    {
        if (Input.GetMouseButtonDown(0))
            if (bricks.Peek().GetComponent<BrickMovementScript>().overlap == 1)
            {
                Destroy(bricks.Dequeue());
                if (bricks.Count == 0)
                    ResetSkillCheck(5);
            }
            else
                ResetSkillCheck(-5);       

    }

    void CheckMouse1()
    {
        if (Input.GetMouseButtonDown(1))
            if (bricks.Peek().GetComponent<BrickMovementScript>().overlap == 2)
            {
                Destroy(bricks.Dequeue());
                if (bricks.Count == 0)
                    ResetSkillCheck(5);
            }
            else
                ResetSkillCheck(-5);

    }

    public void StartSkillCheck()
    {
        StartCoroutine(SpawnBricks(10, 0.9f));
    }

    private void ResetSkillCheck(int value)
    {
        flag.enabled = true;
        if(value > 0)
            raptorCore.AddCurrency(value);
        else
            raptorCore.SubCurrency(-value);
        ClearBricks();
        this.gameObject.SetActive(false);
    }

    void ClearBricks()
    {
        bricks.Clear();
        foreach (var brick in this.gameObject.GetComponentsInChildren<BrickMovementScript>()) //broski nie ma czasu na ³adny kod trzeba wypuszczaæ szybko pronto
        {
            Destroy(brick.gameObject);
        }
    }

    IEnumerator SpawnBricks(int count, float waitTime)
    {
        for(int i = 0; i<3; i++)
        {
            yield return new WaitForSeconds(1f);
            switch (i)//temp rozwi¹zanie na danie czasu graczowi na ograniêcie siê œwiat³a ostrzegawcze moment
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
