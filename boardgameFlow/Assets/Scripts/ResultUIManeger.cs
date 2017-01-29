using UnityEngine;
using System.Collections;
using Common;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResultUIManeger : MonoBehaviour {
	private const int _person_number = 2;		//プレイ人数
	private struct PLAYER_DATA {
		public int id;				//自分は何Ｐなのか？
		public List< CARD_DATA >  card_list;	//使ったカードのリスト
	}
	[SerializeField]
	//やりかた忘れたからてきとうに１P用と２Pようのカードデータの配置を用意
	private GameObject[] _card_object = new GameObject[6];
	[SerializeField]
	private GameObject[] _card_object2 = new GameObject[6];					//プレイヤー２
	private PLAYER_DATA[] _player_data = new PLAYER_DATA[_person_number];		/// リザルト用のプレイヤーデータ
	[SerializeField]
	private CardManager _card_manager;		//サーバーが取得したデータがとれればおそらく不要　デバック用
	// Use this for initialization
	void Start () {
		if (_card_manager == null){
			_card_manager = GameObject.Find("CardManager").GetComponent<CardManager>();
		}
		for (int i = 0; i < _player_data.Length; i++){
			_player_data [i].card_list = new List<CARD_DATA> ();
			_player_data [i].id = i;
			addCardData(i);
			setCardImage(i);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//なんかここがバグる
	private void addCardData(int id){
		CARD_DATA card;		
		//_player_data [id].card_list.Add();		//サーバーが持っている使用したカードを入れる
		//デバッグ用
		for (int i = 0; i < 5; i++){
			card = _card_manager.getCardData(i);
			Debug.Log (card);
			//_player_data[id].card_list.Add(card);
		}
	}
	private void setCardImage(int id){
		for (int i = 0; i < _player_data[id].card_list.Count; i++){
			if (id == 1) {
				// _player_data[id].card_listのカードデータをもとに　_card_pbjectのRawImageを変える
				//_card_object[i].texture = TexTure2Dリソースロード.(_player_data[id].card_list.name);
				RawImage image = _card_object[i].GetComponent<RawImage>();
				image.texture = Resources.Load("Graphics/Texture" + _player_data[id].card_list[i].name) as Texture2D;
			} else if (id == 2) {
				
			} else {
				return;
			}
		}
	}
}
