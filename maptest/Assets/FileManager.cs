using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;
using System;

public class FileManager : Manager< FileManager > {
    
    #region ファイルクラス
	[ System.Serializable ]
 	protected class File {
		[ SerializeField ]
		private string _name;		// 名前

		private FILE_DATA _data;	// データ

		/// <summary>
		/// データのセット
		/// </summary>
		/// <param name="data"></param>
		public void setData( FILE_DATA data ) {
			_data = data;
		}

		/// <summary>
		/// ファイル名の取得
		/// </summary>
		/// <returns></returns>
		public string getName( ) {
			return _name;
		}

		/// <summary>
		/// データの存在確認
		/// </summary>
		/// <returns></returns>
        
		public bool isData( ) {
			bool frag = false;
			frag = ( _data.map.ma != null )? true : false;		    // 座標配列の確認
			return frag;
		}

		/// <summary>
		/// データの取得
		/// </summary>
		/// <returns></returns>
		public FILE_DATA getData( ) {
			return _data;
		}
	}
	#endregion

    [ SerializeField ]
	protected File _file = new File( );

    // Awake関数の代わり
	protected override void initialize( ) {

	}

    void FixedUpdate( ) {
		cheackFilesData( );
	}

    /// <summary>
	/// ファイルデータのチェック
	/// </summary>
	void cheackFilesData( ) {
		// データ確認
		if ( !_file.isData( ) ) {
			loadFile( _file );// ロード
		}
	}

    /// <summary>
	/// ファイルのロード
	/// </summary>
	/// <param name="fileName"> ファイルの名前 </param>
	/// <param name="list"> ファイルデータ型のリスト </param>
	private bool loadFile( File file ) {
		try {
			StreamReader sr = new StreamReader( "../Files/" + file.getName( ) + ".csv" );

			FILE_DATA data = new FILE_DATA( );

			// マップデータの取得
			data.map = getLoadFileMapData( ref sr );
				
			sr.Close( );

			// データ上書き
			file.setData( data );

			return true;
		} catch {
			Debug.LogError( "Missing Load File..." );
			return false;
		}
	}

    /// <summary>
	/// ロードしたファイルからマップデータを取得
	/// </summary>
	/// <returns></returns>
	protected FILE_DATA.MAP getLoadFileMapData( ref StreamReader sr ) {
		FILE_DATA.MAP data = new FILE_DATA.MAP( );

		// 個数の取得
		string str = sr.ReadLine( );
		string[ ] values = str.Split( ',' );

        int ma_size = int.Parse( values[ 0 ] );

        // 配列確保
		data.ma = new POSS_DATA[ ma_size ];

        int[ ] array = new int[ ] { ma_size };
        int length = array[ 0 ];

        // 大きいほうを設定
       for ( int i = 1; i < array.Length; i++ ) {
           if ( array[ i ] > length ) {
               length = array[ i ];
           }
       }

		for ( int i = 0; i < length; i++ ) {
			// ファイルから一行読み込む
			str = sr.ReadLine( );

			// 読み込んだ一行をカンマ毎に分けて配列に格納する
			values = str.Split( ',' );

            // 座標データを書き込み
            if ( i < ma_size ) {
				// インデックスの取得
				data.ma[ i ].index = int.Parse( values[ 0 ] );
				// X座標の取得
				data.ma[ i ].x = uint.Parse( values[ 1 ] );
                // Y座標の取得
				data.ma[ i ].y = uint.Parse( values[ 2 ] );
                // Z座標の取得
				data.ma[ i ].z = uint.Parse( values[ 3 ] );
            } 
        }

		return data;

    }

    /// <summary>
	/// ファイルデータの取得
	/// </summary>
	/// <returns></returns>
	private FILE_DATA getFileData( ) {
		FILE_DATA data = new FILE_DATA( );
		if ( _file.isData( ) ) {
			data = _file.getData( );
		}

		return data;
	}

    /// <summary>
	/// マップデータの取得
	/// </summary>
	/// <returns></returns>
	public FILE_DATA.MAP getMapData( ) {
		return getFileData( ).map;
	}

}
