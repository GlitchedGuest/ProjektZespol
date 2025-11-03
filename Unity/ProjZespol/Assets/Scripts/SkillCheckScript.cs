using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class SkillCheckScript : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;
    private Queue<GameObject> bricks = new Queue<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //this.gameObject.SetActive(false); //testowe na ten moment
        StartCoroutine(SpawnBricks(10,0.9f));
    }

    // Update is called once per frame
    void Update()
    {
        if (bricks.Count != 0)
        {
            if (Input.GetMouseButtonDown(0) && bricks.Peek().GetComponent<BrickMovementScript>().overlap == 1)
                Destroy(bricks.Dequeue());
            if (Input.GetMouseButtonDown(1) && bricks.Peek().GetComponent<BrickMovementScript>().overlap == 2)
                Destroy(bricks.Dequeue());
        }
    }

    IEnumerator SpawnBricks(int count, float waitTime)
    {
        for (int i = 0; i < count; i++)
        {
            bricks.Enqueue(Instantiate(brickPrefab, this.gameObject.transform.localPosition, Quaternion.identity, this.gameObject.transform));
            yield return new WaitForSeconds(waitTime);
        }
    }

}
