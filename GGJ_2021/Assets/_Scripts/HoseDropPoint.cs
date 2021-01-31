using UnityEngine;
using Obi;

public class HoseDropPoint : MonoBehaviour
{
    [SerializeField]
    private Interactor _playerInteractor;
    private GrabObject _interactObject;

    public GameObject goalPointMesh;
    public ObiParticleAttachment headPoint;
    public Animation coinAnimation;

    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "Player" && _playerInteractor.heldObject != null) {
            _interactObject = _playerInteractor.heldObject;
            _playerInteractor.onFinishInteractionEvent += DropHose;
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.gameObject.name == "Player") {
            _interactObject = null;
            _playerInteractor.onFinishInteractionEvent -= DropHose;
        }
    }

    private void DropHose() {
        if(_interactObject == null || headPoint == null) { return; }
        goalPointMesh.SetActive(true);
        headPoint.target = goalPointMesh.transform;
        coinAnimation.Play("coinAnimation");
        _interactObject.transform.position = Vector3.zero; // banish this
        _interactObject.gameObject.SetActive(false);
    }
}
