using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MichaelWolfGames;
using RootMotion.FinalIK;

public class LookAtCrow : MonoBehaviour
{
    [SerializeField] private float lookRange = 0f;
    [SerializeField] private float speed = 0f;
    [SerializeField] private float maxAngle = 0f;
    private LookAtIK grannyIK;
    
    // Start is called before the first frame update
    void Start()
    {
        grannyIK = this.gameObject.GetComponent<LookAtIK>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(this.gameObject.transform.position, PlayerInstance.Instance.gameObject.transform.position);
        if(distance <= lookRange)
        {
            Vector3 grannyToCrow = this.gameObject.transform.position - PlayerInstance.Instance.gameObject.transform.position;          
            float angle = Vector3.Angle(grannyToCrow, -1f * this.gameObject.transform.forward);
            if(angle > maxAngle)
            {
                grannyIK.solver.SetLookAtWeight(Mathf.Clamp(grannyIK.solver.IKPositionWeight - (speed * Time.deltaTime), 0f, 1f));
                return;
            }
            grannyIK.solver.SetLookAtWeight(Mathf.Clamp(grannyIK.solver.IKPositionWeight + (speed * Time.deltaTime) , 0f, 1f));
        }
        else
        {
            grannyIK.solver.SetLookAtWeight(Mathf.Clamp(grannyIK.solver.IKPositionWeight - (speed * Time.deltaTime) , 0f, 1f));
        }
    }
}
