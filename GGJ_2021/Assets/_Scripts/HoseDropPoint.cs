using UnityEngine;
using Obi;

public class HoseDropPoint : MonoBehaviour
{
    [SerializeField]
    private Interactor _playerInteractor;
    private GrabObject _interactObject;

    public GameObject goalPointMesh;
    public ObiParticleAttachment headPoint;

    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "Player") {
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
        headPoint.target = this.transform;
        goalPointMesh.SetActive(true);
        _playerInteractor.QuitInteraction();
        _interactObject.gameObject.SetActive(false);
    }
}
