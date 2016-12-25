using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common;

public class PhaseManager : MonoBehaviour {

	[SerializeField]
	private PHASE _phase;

	// Use this for initialization
	void Start () {
	
	}

	public PHASE getPhase(){
		return _phase;
	}

	public void setPhase( PHASE phase ){
		_phase = phase;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
