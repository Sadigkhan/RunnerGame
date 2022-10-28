using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;
using Cinemachine;

public class PlayerController : MonoBehaviour
{

    float Horizontal;
    SplineFollower splineFollower;
    Animator anim;



    [Header("Detect & Collect")]
    public Stack<Collider> MoneyStack;
    Collider[] colliders;
    [SerializeField] Transform detectTransform;
    [SerializeField] float DetectionRange = 0.05f;
    [SerializeField] LayerMask layer;
    [SerializeField] Transform holdTransform;
    [SerializeField] int itemCount = 0;
    [SerializeField] float ItemDistanceBetween = 0f;
    Rigidbody rb;
    float NextDropTime;
    float zDropDistance = 0;
    [SerializeField] float DropRate = 1;
    [SerializeField] float DropSecond = 1;
    [SerializeField] Transform DropArea;
    int dropCount = 0;
    [SerializeField] float DropDistanceBetween = 1f;
    GameObject prevObj;
    CinemachineVirtualCamera cnm;
    CinemachineTransposer Ctr;
    float speed=1f;
    void Start()
    {
        splineFollower = GetComponentInParent<SplineFollower>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        MoneyStack = new Stack<Collider>();
        cnm=GetComponentInChildren<CinemachineVirtualCamera>();
        Ctr = cnm.GetCinemachineComponent<CinemachineTransposer>();

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(detectTransform.position, DetectionRange);
    }
    void Update()
    {
        float movement = (speed * Input.GetAxis("Horizontal")) * Time.deltaTime;
        transform.Translate(1 * movement, 0, 0);
        transform.localPosition = new Vector3((Mathf.Clamp(transform.localPosition.x, -0.1f, 0.1f)), transform.localPosition.y, transform.localPosition.z);
        colliders = Physics.OverlapSphere(detectTransform.position, DetectionRange, layer);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * 2.5f, ForceMode.Impulse);
            anim.SetTrigger("Jump");
        }
        foreach (var hit in colliders)
        {
            //prevObj = hit.transform.parent.gameObject;
            if (hit.CompareTag("Collectable"))
            {
                //Destroy(prevObj);
                hit.tag = "Collected";
                hit.transform.parent = holdTransform;
                MoneyStack.Push(hit);
                var seq = DOTween.Sequence();
                seq.Append(hit.transform.DOLocalJump(new Vector3(0, itemCount * ItemDistanceBetween), 2, 1, 0.3f))
                   .Join(hit.transform.DOScale(1.25f, 0.1f))
                   .Insert(0.1f, hit.transform.DOScale(0.3f, 0.2f));
                seq.AppendCallback(() =>
                {
                    hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
                });
                itemCount++;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EndPoint"))
        {
            transform.DOLocalRotate(new Vector3(0, 180, 0),1f);
            if (Time.time >= NextDropTime)
            {


                //if (MoneyStack.Count <= 0) return;
                //GameObject go = MoneyStack.Pop().gameObject;
                //go.transform.parent = null;
                //var Seq = DOTween.Sequence();
                //Seq.Append(go.transform.DOJump(DropArea.position + new Vector3(0, (dropCount * DropDistanceBetween), 0), 2, 1, 0.3f))
                //        .Join(go.transform.DOScale(1.5f, 0.1f))
                //        .Insert(0.1f, go.transform.DOScale(1, 0.2f))
                //        .AppendCallback(() => { go.transform.rotation = Quaternion.Euler(0, 0, 0); });
                //other.GetComponent<DropArea>().StackedDropItems.Push(go);
                ////other.GetComponent<DropArea>().StackedDropItems.Enqueue(go);
                //dropCount++;
                //itemCount--;
                //NextDropTime = Time.time + DropSecond / DropRate;

                if (MoneyStack.Count <= 0) return;
                //GameObject go = MoneyStack.Pop().gameObject;
                //go.transform.parent = DropArea;
               if(dropCount <2)
                {
                    GameObject go = MoneyStack.Pop().gameObject;
                    go.transform.parent = DropArea;
                    var Seq = DOTween.Sequence();
                    Seq.Append(go.transform.DOJump(DropArea.position + new Vector3(0, (dropCount * DropDistanceBetween), zDropDistance), 2, 1, 0.3f))
                            .Join(go.transform.DOScale(1.5f, 0.1f))
                            .Insert(0.1f, go.transform.DOScale(1, 0.2f))
                            .AppendCallback(() => { go.transform.rotation = Quaternion.Euler(-90, 0, 0); });
                    //other.GetComponent<DropArea>().StackedDropItems.Push(go);
                    //other.GetComponent<DropArea>().StackedDropItems.Enqueue(go);
                    dropCount++;
                    itemCount--;
                    NextDropTime = Time.time + DropSecond / DropRate;
                }
                else
                {
                    dropCount = 0;
                    zDropDistance -= 0.07f;
                }
            }
        }
       
    }
    private void OnCollisionEnter(Collision other)
    {

        if (other.transform.CompareTag("EndPoint"))
        {
            splineFollower.follow = false;
            splineFollower.followSpeed = 0;
            anim.SetTrigger("Dance");
            //cnm.FollowTargetAsVcam = null;
            Ctr.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
            cnm.transform.localRotation = Quaternion.Euler(0, -180, 0);
            Ctr.m_FollowOffset = new Vector3(0.1f, 0.3f, 0.5f);

        }
        if (other.transform.CompareTag("Barrier"))
        {
            splineFollower.follow = false;
            splineFollower.followSpeed = 0;
            anim.SetTrigger("FallDown");
            speed = 0;
        }
    }

    
}
