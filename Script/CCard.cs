using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CCard : MonoBehaviour
{
    public CardStruct cardclass;
    public SpriteRenderer sprite;
    public Sprite[] cardimg;
    private Vector3 startpos = new Vector3(0, 0, 0);
    public Vector3 targetpos = new Vector3(0, 0, 0);
    private Vector3 Rotate_z180 = new Vector3(0, 0, 180);
    private Vector3 Rotate0 = new Vector3(0, 0, 0);
    private Vector3 Rotate_y90 = new Vector3(0, 90, 0);
    private int imgnum = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetImgnum(int num)
    {
        imgnum = num;
    }
    public void SetCardImg(int num)
    {
        sprite.sprite = cardimg[num];
    }
    public void GotoDeck()
    {
        this.transform.DOMove(startpos, 0.1f);
        this.transform.DORotate(Rotate_z180, 0.1f);
        SetCardImg(20);

    }
    public void GotoHand()
    {
        Sequence s = DOTween.Sequence();
        s.Join(this.transform.DOMove(targetpos, 0.4f).SetEase(Ease.InBack));
        s.Join(this.transform.DORotate(Rotate0, 0.4f).SetEase(Ease.InCirc));
        s.Play();
    }
    public void FlipCard(int num)
    {
        
        this.transform.DORotate(Rotate_y90, 0.4f).OnComplete(() =>
        {
            this.SetCardImg(num);
            this.transform.DORotate(Rotate0, 0.4f);

        });
    }
}
