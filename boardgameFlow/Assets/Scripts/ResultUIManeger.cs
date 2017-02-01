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
        public BATTLE_RESULT result;
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
	[SerializeField]
	private GameObject _vs_ui;
	[SerializeField]
	private GameObject _win_ui;
    [SerializeField]
    private GameObject _drow_ui;
    private bool _card_rend = false;
    private bool _result_rend = false;
    private float _wait_time = 3.0f;
    private float _wait_time2 = 3.0f;
    private ApplicationManager _app_manager;
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
    public void setResult(BATTLE_RESULT player_one, BATTLE_RESULT player_two){
        _player_data[0].result = player_one;
        _player_data[1].result = player_two;
        /*if (player_one == BATTLE_RESULT.WIN){
            _player_two.SetActive(false);
        } else if(player_two == BATTLE_RESULT.WIN){
            _player_one.SetActive(false);
        } else if(player_one == BATTLE_RESULT.DRAW){
             _player_one.SetActive(false);
            _player_two.SetActive(false);
        }*/
    }
    public bool getCurrentBattle( ){
        return _current_battle;
    }
    public void setCurrentBattle( bool current_battle ){
         _current_battle = current_battle;
    }
    /// <summary>
    /// id 呼び出すコルーチン（なくすかも）　player_one_result　１Pのリザルトデータ　player_two_result　２Pのリザルトデータ
    /// </summary>
    /// <param name="id"></param>
    /// <param name="player_one_result"></param>
    /// <param name="player_two_result"></param>
	public void setCoroutine(int id, BATTLE_RESULT player_one_result, BATTLE_RESULT player_two_result){
        if (!_card_rend) {
             return;
        }
        _player_data[0].result = player_one_result;
        _player_data[1].result = player_two_result;
        StartCoroutine(battleUIUpdate());
	}
    public void atherUpdate(){
         /*if (_card_rend){
            _current_time += Time.deltaTime;
             Debug.Log(_current_time);
            if(_current_time > _end_time){
                for (int i = 0; i < _card_object.Length; i++){
                    _card_object[i].GetComponent<Image>().enabled = false;
                }
                for (int i = 0; i < _card_object2.Length; i++){
                    _card_object2[i].GetComponent<Image>().enabled = false;
                }
				_card_rend = false;
				_current_time = 0.0f;
                setCurrentBattle(false);
            }
            return;
         }
        
        _current_time += Time.deltaTime;
        if(_current_time > _end_time){
            setCurrentBattle(false);
        }*/
    }
    private IEnumerator battleUIUpdate() {
        Debug.Log("hugo");
        _card_rend = false;
        yield return new WaitForSeconds(_wait_time);
        for (int i = 0; i < _card_object.Length; i++){
            _card_object[i].GetComponent<Image>().enabled = false;
        }
        for (int i = 0; i < _card_object2.Length; i++){
            _card_object2[i].GetComponent<Image>().enabled = false;
        }
        //_result_rend = true;
        StartCoroutine(resultUIUpdate()); 
    }
    private IEnumerator resultUIUpdate() {
        Debug.Log("hugo2");
        //yield return new WaitForSeconds(_wait_time);          //カードアニメーションがないのでコメントアウト　同時に消す
        //_result_rend = false;
        //プレイヤーアイコンを消す　勝利ロゴを出す
        Debug.Log(_player_data[0].result + "プレイヤー１リザルト");
        if (_player_data[0].result == BATTLE_RESULT.WIN){
            Debug.Log("hugo2hugo2hugo2");
            _player_two.SetActive(false);
            _vs_ui.SetActive(false);
            _win_ui.GetComponent<Image>().enabled = true;
        } else if(_player_data[1].result == BATTLE_RESULT.WIN){
            Debug.Log("hugo2hugo2hugo222");
            _player_one.SetActive(false);
            _vs_ui.SetActive(false);
            _win_ui.GetComponent<Image>().enabled = true;
        } else if(_player_data[0].result == BATTLE_RESULT.DRAW){
            Debug.Log("hugo2hugo2hugo222hugo2hugo2hugo222");
             _player_one.SetActive(false);
            _player_two.SetActive(false);
            _vs_ui.SetActive(false);
            _drow_ui.GetComponent<Image>().enabled = true;
        }
        
        if (_app_manager == null){
            _app_manager = GameObject.Find("ApplicationManager").GetComponent<ApplicationManager>();
        }
        yield return new WaitForSeconds(_wait_time);
        //UIを変更したらフェイズ切り替え
        _app_manager.setChangeMainGamePhase();
        //自身を削除
        Destroy(gameObject);
    }
}