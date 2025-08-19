using UnityEngine;
using DG.Tweening;

public class CCoin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var temp = new Vector3(-0.15f, 2f, 0f);
        var ran = Random.Range(-0.2f, 0.2f);
        temp.x += ran;
        temp.y += ran;
        this.transform.DOMove(temp, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
