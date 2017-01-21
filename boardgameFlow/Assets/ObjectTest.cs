using UnityEngine;
using System.Collections;
using Common;

public class ObjectTest : MonoBehaviour {


	[SerializeField]
	private Material _default_material;		//元のテクスチャ
	[SerializeField]
	private Material _target_material;		//変更するテクスチャ

	[SerializeField]
	private float _target_distanse = 3.0f;			//プレイヤーとの距離設定

	private PlayerManager _player_manager;
	private PLAYER_DATA _player1;
	private PLAYER_DATA _player2;


	// Use this for initialization
	void Awake () {
		//テクスチャの設定
		_default_material = GetComponent< Renderer >( ).material;
		_target_material = (Material)Resources.Load( "Materials/TestMaterial" );

		//プレイヤーの取得
		_player_manager = GameObject.Find( "PlayerManager" ).GetComponent< PlayerManager >( );
	}
	
	// Update is called once per frame
	void Update () {
		//プレイヤーを取得
		_player1 = _player_manager.GetComponent< PlayerManager >( ).getTopPlayer( PLAYER_RANK.RANK_FIRST );
		_player2 = _player_manager.GetComponent< PlayerManager >( ).getTopPlayer( PLAYER_RANK.RANK_SECOND );

		//プレイヤーが近くにいるならばマテリアルを変える
		if ( _player1.obj != null || _player2.obj != null ) {
			Vector3 mypos = gameObject.transform.position;
			float player1_distance = Vector3.Distance (mypos, _player1.obj.transform.position);
			float player2_distance = Vector3.Distance (mypos, _player2.obj.transform.position);
			if (player1_distance <= _target_distanse || player2_distance <= _target_distanse) {
				gameObject.GetComponent< Renderer > ().material = _target_material;
			} else {					 		    
				gameObject.GetComponent< Renderer > ().material = _default_material;
			}
		}
	}
}
