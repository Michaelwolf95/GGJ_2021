using UnityEngine;
using Obi;

public class HoseDropPoint : MonoBehaviour
{
    [SerializeField]
    private Interactor _playerInteractor;
    private GrabObject _interactObject;

    public AK.Wwise.Event HoseStart;

    public GameObject goalPointMesh;
    public ObiParticleAttachment headPoint;
    public Animation coinAnimation;

    public InteractableReticle reticle;

    private bool used = false;
    
    public void OnTriggerEnter(Collider other) {
        if(used) return;
        if(other.gameObject.name == "Player" && _playerInteractor.heldObject != null) {
            _interactObject = _playerInteractor.heldObject;
            _playerInteractor.onFinishInteractionEvent += DropHose;
            
            reticle.ToggleInteractReticle(true);
        }
    }

    public void OnTriggerExit(Collider other) {
        if(used) return;
        if (other.gameObject.name == "Player") {
            _interactObject = null;
            _playerInteractor.onFinishInteractionEvent -= DropHose;
            
            reticle.ToggleInteractReticle(false);
        }
    }

    private void DropHose() {
        if(_interactObject == null || headPoint == null) { return; }

        goalPointMesh.SetActive(true);
        _interactObject.transform.position = goalPointMesh.transform.position;
        _interactObject.GetComponent<Rigidbody>().isKinematic = true;
        _interactObject.gameObject.SetActive(false);

        coinAnimation.Play("coinAnimation");
       
        
        reticle.ToggleInteractReticle(false);
        
        _playerInteractor.onFinishInteractionEvent -= DropHose;
        HoseStart.Post(gameObject);
        used = true;
    }
}
