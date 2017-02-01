using UnityEngine;
using System.Collections;
using Common;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResultUIManeger : MonoBehaviour {
	private const int _person_number = 2;		//プレイ人数
	private struct PLAYER_DATA {
		public int player_id;				//自分は何Ｐなのか？
        public int use_card_id;
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
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// ResultUIの初期設定を行う
    /// </summary>
    /// <param name="player_id"></param>
    /// <param name="use_card_id"></param>
    public void Init(List<int> use_card_id, int player_id){
        //CardManagerが存在していなかったら設定する
        if (_card_manager == null){
			_card_manager = GameObject.Find("CardManager").GetComponent<CardManager>();
		}
		_player_data[player_id].card_list = new List<CARD_DATA> ();
        _player_data[player_id].player_id = player_id;
        /*for (var i = 0; i < use_card_id.Count; i++){
            Debug.Log(use_card_id[i] + "  "+ (player_id + 1) + "PのしようしたカードID");
        }*/
        //受け取ったカードIDに対応するカードを表示
        for( var i = 0; i < use_card_id.Count; i++){
            //1P
            if(player_id == (int)PLAYER_ORDER.PLAYER_ONE){
                _card_object[i].SetActive(true);
            }
            //2P
            else if(player_id == (int)PLAYER_ORDER.PLAYER_TWO){
                _card_object2[i].SetActive(true);
            }
        }
        addCardData(use_card_id, player_id);
        setCardImage(player_id);
    }
	private void addCardData(List<int> use_card_id, int player_id){
		CARD_DATA card;
        for (int i = 0; i < use_card_id.Count; i++){
            card = _card_manager.getCardData(use_card_id[i]);
            _player_data[player_id].card_list.Add(card);
        }
	}
	private void setCardImage(int player_id){
		for (int i = 0; i < _player_data[player_id].card_list.Count; i++) {
			if (player_id == (int)PLAYER_ORDER.PLAYER_ONE) {
				// _player_data[id].card_listのカードデータをもとに　_card_pbjectのRawImageを変える
				//_card_object[i].texture = TexTure2Dリソースロード.(_player_data[id].card_list.name);
                Debug.Log(_player_data[player_id].card_list[i].name);
				Material material = Resources.Load<Material>( "Materials/Cards/" +  _player_data[player_id].card_list[i].name );
                _card_object[i].GetComponent<Image>().material = material;
            } 
            else if (player_id == (int)PLAYER_ORDER.PLAYER_TWO) {
                Material material = Resources.Load<Material>( "Materials/Cards/" +  _player_data[player_id].card_list[i].name );
                _card_object2[i].GetComponent<Image>().material = material;
			}
		}
	}
}