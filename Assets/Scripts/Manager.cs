//名前空間
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using TMPro;

//クラス
public class Manager : MonoBehaviour
{
    [SerializeField]
    private Image upImage;
    [SerializeField]
    private Image downImage;
    [SerializeField]
    private Text introductionText;
    [SerializeField]
    private Text outroductionText;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private GameObject nameField;
    [SerializeField]
    private GameObject enterButton;

    //スペースキーが押された時に入る文字列
    private string upData = "";
    //エンターキーが押された時に入る文字列
    private string downData = "";

    // System.IO
    private StreamWriter sw;
    //アプリケーションのパス
    private string path;

    //ステータスが変わった時にtrueになる
    bool stateEnter = true;

    //現在のステータスを表す列挙型
    public enum State
    {
        EnterName,
        Introduction,
        Testing,
        End
    }

    //画像が表示されてからの時間
    private float startTime = 0;
    private float endTime = 0;

    private State state = State.EnterName;
    private string key = "none";
    private string name = "none";

    //画像ファイルの名前を格納する配列(1~8ランダム)
    private int[] image = new int[8];
    //image配列のインデックス
    private int imageIndex = 0;

    //ステータスを変更する関数
    private void ChangeState(State s){
        state = s;
        stateEnter = true;
    }

    //名前を入力して確定ボタンを押した時に呼ばれる関数
    public void OnClickEnterButton(){
        state = State.Introduction;
        name = nameText.text;
        if(name == ""){
            name = "none";
        }

        //name + ".csv"というファイルが存在したら、番号を追加する
        int n = 1;
        string originalName = name;
        while(File.Exists(Path.Combine(Application.streamingAssetsPath, name + ".csv"))){
            name = originalName + n.ToString();
            n++;
        }

        nameField.gameObject.SetActive(false);
        enterButton.gameObject.SetActive(false);
        introductionText.gameObject.SetActive(true);

    }

    //画像をセットする関数
    private void SetImage(){
        Debug.Log(image[imageIndex]);
        int n = image[imageIndex];
        if(Random.value < 0.5f)
        {
            upImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Right/" + n);
            downImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Left/" + n);
            upData = "right";
            downData = "left";
        }
        else
        {
            downImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Right/" + n);
            upImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Left/" + n);
            upData = "left";
            downData = "right";
        }
    }

    //データを保存する関数
    private void SaveData(){
        float time = (endTime - startTime);
        if(key == "space"){
            key = downData;
        }
        else if(key == "enter"){
            key = upData;
        }
        string[] s1 = { time.ToString(), key, image[imageIndex-1].ToString()};

        using (var writer = new StreamWriter(Path.Combine(Application.streamingAssetsPath, name + ".csv"), true))
        {
            writer.WriteLine(string.Join(",", s1));
            Debug.Log("Saved");
        }
    }

    //開始時に呼ばれる関数
    void Start()
    {
        //image配列に1~9の数字を被りなく、ランダムに代入
        for (int i = 0; i < image.Length; i++)
        {
            image[i] = Random.Range(1, 9);
            for (int j = 0; j < i; j++)
            {
                if (image[i] == image[j])
                {
                    i--;
                    break;
                }
            }
        }

        //使わないUIを非表示にする
        upImage.gameObject.SetActive(false);
        downImage.gameObject.SetActive(false);
        introductionText.gameObject.SetActive(false);
        outroductionText.gameObject.SetActive(false);
        nameField.gameObject.SetActive(true);
        enterButton.gameObject.SetActive(true);
    }

    //毎フレーム呼ばれる関数
    void Update()
    {
        //stateパターン
        switch (state)
        {
            //説明が表示されるステータス
            case State.Introduction:
                if(stateEnter){
                    stateEnter = false;
                    startTime = Time.time;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState(State.Testing);
                    upImage.gameObject.SetActive(true);
                    downImage.gameObject.SetActive(true);
                    introductionText.gameObject.SetActive(false);
                }
                break;
            //実験が行われるステータス
            case State.Testing:
                if(stateEnter){
                    stateEnter = false;

                    SetImage();
                    imageIndex++;
                    startTime = Time.time;
                }

                if(Input.GetKeyDown(KeyCode.Space)){
                    key = "space";
                }
                else if(Input.GetKeyDown(KeyCode.Return)){
                    key = "enter";
                }

                //spaceかenterが押されたら画像を更新
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    endTime = Time.time;
                    SaveData();

                    //imageIndexが9になったら終了
                    if (imageIndex >= 8)
                    {
                        ChangeState(State.End);
                        break;
                    }
                    else{
                        SetImage();
                        imageIndex++;
                    }
                    startTime = Time.time;
                }
                break;
            //終了時のステータス
            case State.End:
                if(stateEnter){
                    stateEnter = false;
                }
                upImage.gameObject.SetActive(false);
                downImage.gameObject.SetActive(false);
                introductionText.gameObject.SetActive(false);
                outroductionText.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
