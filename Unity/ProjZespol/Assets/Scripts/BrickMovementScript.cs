using UnityEngine;

public class BrickMovementScript : MonoBehaviour
{
    private float speed = 0f;
    public int overlap = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float random = UnityEngine.Random.Range(0.00f, 100.00f);
        if (random > 50.00f)
            speed = 2f;
        else
            speed = -2f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * Time.deltaTime * speed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LeftZone") 
            overlap = 1;
        else if( collision.gameObject.name == "RightZone")
            overlap = 2;
    }
}
