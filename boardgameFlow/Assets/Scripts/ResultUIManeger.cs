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
        public int attak_point;
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
    private bool _current_battle = false;
    [SerializeField]
    private GameObject _player_one;
    [SerializeField]
    private GameObject _player_two;
    private float _current_time = 0f;
    private float _end_time = 4f;
    private bool _card_rend = false;
	// Use this for initialization
	void Start () {
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
         _player_data[player_id].attak_point = 0;
        /*for (var i = 0; i < use_card_id.Count; i++){
            Debug.Log(use_card_id[i] + "  "+ (player_id + 1) + "PのしようしたカードID");
        }*/
        //受け取ったカードIDに対応するカードを表示
        for( var i = 0; i < use_card_id.Count; i++){
            //1P
            if(player_id == (int)PLAYER_ORDER.PLAYER_ONE){
                _card_object[i].GetComponent<Image>().enabled = true;
            }
            //2P
            else if(player_id == (int)PLAYER_ORDER.PLAYER_TWO){
                _card_object2[i].GetComponent<Image>().enabled = true;
            }
        }
        addCardData(use_card_id, player_id);
        setCardImage(player_id);

    }
    /// <summary>
    /// カードデータを追加する
    /// </summary>
    /// <param name="use_card_id"></param>
    /// <param name="player_id"></param>
	private void addCardData(List<int> use_card_id, int player_id){
		CARD_DATA card;
        for (int i = 0; i < use_card_id.Count; i++){
            card = _card_manager.getCardData(use_card_id[i]);
            _player_data[player_id].card_list.Add(card);
        }
	}
    /// <summary>
    /// カードのテクスチャをかえる
    /// </summary>
    /// <param name="player_id"></param>
	private void setCardImage(int player_id){
        setCurrentBattle(true);
        _card_rend = true;
		for (int i = 0; i < _player_data[player_id].card_list.Count; i++) {
			if (player_id == (int)PLAYER_ORDER.PLAYER_ONE) {
				Material material = Resources.Load<Material>( "Materials/Cards/" +  _player_data[player_id].card_list[i].name );
                _card_object[i].GetComponent<Image>().material = material;
            }
            else if (player_id == (int)PLAYER_ORDER.PLAYER_TWO) {
                Material material = Resources.Load<Material>( "Materials/Cards/" +  _player_data[player_id].card_list[i].name );
                _card_object2[i].GetComponent<Image>().material = material;
			}
		}
	}
    public void setBattle(BATTLE_RESULT player_one, BATTLE_RESULT player_two){
        setCurrentBattle(true);
        if (player_one == BATTLE_RESULT.WIN){
            _player_two.SetActive(false);
        } else if(player_two == BATTLE_RESULT.WIN){
            _player_one.SetActive(false);
        } else if(player_one == BATTLE_RESULT.DRAW){
             _player_one.SetActive(false);
            _player_two.SetActive(false);
        }
    }
    public bool getCurrentBattle( ){
        return _current_battle;
    }
    public void setCurrentBattle( bool current_battle ){
         _current_battle = current_battle;
    }
    public void atherUpdate(){

         if (_card_rend){
            _current_time += Time.deltaTime;
             Debug.Log(_current_time);
            if(_current_time > _end_time){
                for (int i = 0; i < _card_object.Length; i++){
                    _card_object[i].GetComponent<Image>().enabled = false;
                }
                for (int i = 0; i < _card_object2.Length; i++){
                    _card_object2[i].GetComponent<Image>().enabled = false;
                }
                setCurrentBattle(false);
            }
            return;
         }
        
        _current_time += Time.deltaTime;
        if(_current_time > _end_time){
            setCurrentBattle(false);
        }
    }
    public void timeReset(){
        _current_time = 0.0f;
    }
}